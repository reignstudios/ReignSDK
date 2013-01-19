using Reign.Core;
using System;

#if OSX
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreVideo;
using MonoMac.CoreGraphics;
using MonoMac.OpenGL;
#endif

#if iOS
using MonoTouch.GLKit;
#endif

#if RPI
using System.Runtime.InteropServices;
#endif

namespace Reign.Video.OpenGL
{
	public enum Versions
	{
		GL1,
		GL2,
		GL3
	}

	public class Caps
	{
		public Versions Version {get; internal set;}
		public int MaxTextureSize {get; internal set;}
		public int MaxVertexConstants {get; internal set;}
		public int MaxPixelConstants {get; internal set;}
		public ShaderVersions MaxShaderVersion {get; internal set;}
		public bool HardwareInstancing {get; internal set;}

		public bool TextureCompression_S3TC {get; internal set;}
		public bool TextureCompression_ATC {get; internal set;}
		public bool TextureCompression_PVR {get; internal set;}
	}

	public class Video : Disposable, VideoI
	{
		#region Properties
		public string FileTag {get; private set;}
		public Size2 BackBufferSize {get; private set;}

		#if WINDOWS || OSX || LINUX || NaCl
		private Window window;
		#else
		private Application application;
		#endif

		public Caps Caps;
		private bool disposed;
		internal Texture2D[] currentPixelTextures, currentVertexTextures;
		internal SamplerState[] currentSamplerStates;
		internal BufferLayout currentBufferLayout;
		internal VertexBuffer currentVertexBuffer;

		private IntPtr ctx, dc;
		uint frameBuffer;

		#if WINDOWS || LINUX
		private IntPtr handle;
		#endif
		
		#if RPI
		private IntPtr surface;
		private RaspberryPi.DISPMANX_WINDOW_T nativeWindow;
		private GCHandle windowHandle;
		#endif

		#if OSX
		public NSOpenGLContext NSContext {get; private set;}
		#endif
		
		#if NaCl
		private int context;
		private IntPtr graphics;
		#endif
		#endregion

		#region Constructors
		#if WINDOWS || OSX || LINUX || NaCl
		public Video(DisposableI parent, Window window, bool vSync)
		#elif iOS || ANDROID
		public Video(DisposableI parent, Application application)
		#else
		public Video(DisposableI parent, Application application, bool vSync)
		#endif
		: base(parent)
		{
			try
			{
				currentPixelTextures = new Texture2D[8];
				currentVertexTextures = new Texture2D[8];
				currentSamplerStates = new SamplerState[8];

				#if WINDOWS || OSX || LINUX || NaCl
				BackBufferSize = window.FrameSize;
				this.window = window;
				#else
				BackBufferSize = application.FrameSize;
				this.application = application;
				#endif
				
				#if WINDOWS
				//Get DC
				handle = window.Handle;
				dc = WGL.GetDC(handle);
				WGL.SwapBuffers(dc);

				//Set BackBuffer format
				WGL.PIXELFORMATDESCRIPTOR pfd = new WGL.PIXELFORMATDESCRIPTOR();
				WGL.ZeroPixelDescriptor(ref pfd);
				pfd.nVersion        = 1;
				pfd.dwFlags         = WGL.PFD_DRAW_TO_WINDOW | WGL.PFD_SUPPORT_OPENGL | WGL.PFD_DOUBLEBUFFER;
				pfd.iPixelType      = (byte)WGL.PFD_TYPE_RGBA;
				pfd.cColorBits      = 24;
				pfd.cAlphaBits      = 8;
				pfd.cDepthBits      = 16;
				pfd.iLayerType      = (byte)WGL.PFD_MAIN_PLANE;
				unsafe{pfd.nSize = (ushort)sizeof(WGL.PIXELFORMATDESCRIPTOR);}

				int pixelFormatIndex = WGL.ChoosePixelFormat(dc, ref pfd);
				if (pixelFormatIndex == 0) Debug.ThrowError("Video", "ChoosePixelFormat failed");
				if (WGL.SetPixelFormat(dc, pixelFormatIndex, ref pfd) == 0) Debug.ThrowError("Video", "Failed to set PixelFormat");
				    
				ctx = WGL.CreateContext(dc);
				if (ctx == IntPtr.Zero) Debug.ThrowError("Video", "Failed to create GL context");
				if (WGL.MakeCurrent(dc, ctx) == 0) Debug.ThrowError("Video", "Failed to make GL context current");

				WGL.Init();
				WGL.SwapInterval(vSync ? 1 : 0);
				#endif
				
				#if LINUX
				#if RPI
				unsafe
				{
					// Init Pi video
					RaspberryPi.bcm_host_init();
					
					const int piDisplay = 0;
					uint piWidth = 0, piHeight = 0;
					if (RaspberryPi.graphics_get_display_size(piDisplay, &piWidth, &piHeight) < 0) Debug.ThrowError("Video", "Failed to get display size");
					Console.WriteLine("piWidth - " + piWidth);
					Console.WriteLine("piHeight - " + piHeight);
					BackBufferSize = new Size2((int)piWidth, (int)piHeight);
					
					IntPtr dispman_display = RaspberryPi.vc_dispmanx_display_open(piDisplay);
					if (dispman_display == IntPtr.Zero) Debug.ThrowError("Video", "Failed: vc_dispmanx_display_open");
					
					IntPtr dispman_update = RaspberryPi.vc_dispmanx_update_start(0);
					if (dispman_update == IntPtr.Zero) Debug.ThrowError("Video", "Failed: vc_dispmanx_update_start");
					
					var dstRect = new RaspberryPi.VC_RECT_T()
					{
						x = 0,
						y = 0,
						width = (int)piWidth,
						height = (int)piHeight
					};
					var srcRect = new RaspberryPi.VC_RECT_T()
					{
						x = 0,
						y = 0,
						width = ((int)piWidth) << 16,
						height = ((int)piHeight) << 16
					};
					IntPtr dispman_element = RaspberryPi.vc_dispmanx_element_add(dispman_update, dispman_display, 0, &dstRect, IntPtr.Zero, &srcRect, RaspberryPi.DISPMANX_PROTECTION_NONE, IntPtr.Zero, IntPtr.Zero, 0);
					if (dispman_element == IntPtr.Zero) Debug.ThrowError("Video", "Failed: vc_dispmanx_element_add");
					
					nativeWindow = new RaspberryPi.DISPMANX_WINDOW_T()
					{
						element = dispman_element,
						width = (int)piWidth,
						height = (int)piHeight
					};
					windowHandle = GCHandle.Alloc(nativeWindow, GCHandleType.Pinned);
					RaspberryPi.vc_dispmanx_update_submit_sync(dispman_update);
					
					// Init EGL
					handle = window.Handle;
					GL.GetError();//NOTE: THIS MUST BE HERE SO THAT libGLES LOADS BEFORE libEGL
					dc = EGL.GetDisplay(IntPtr.Zero);
					
					int major, minor;
					if (!EGL.Initialize(dc, out major, out minor))
					{
						Debug.ThrowError("Video", string.Format("Failed to initialize display connection, Error {0}", EGL.GetError()));
					}
					
					int[] pixelFormat = new int[] 
					{ 
						//EGL.RENDERABLE_TYPE, EGL.OPENGL_ES2_BIT,
						
						//EGL.RED_SIZE, 5,
						//EGL.GREEN_SIZE, 6,
						//EGL.BLUE_SIZE, 5,
						//EGL.ALPHA_SIZE, 0,//EGL.DONT_CARE,
						EGL.SURFACE_TYPE, EGL.WINDOW_BIT,
						
						EGL.DEPTH_SIZE, 16,
						//EGL.STENCIL_SIZE, EGL.DONT_CARE,
						
						//EGL.SAMPLE_BUFFERS, 0,
						//EGL.SAMPLES, 0,
						
						//EGL.MIN_SWAP_INTERVAL, 0,
						//EGL.MAX_SWAP_INTERVAL, 1,
						
						EGL.NONE,
					};
					
					int num_configs;
					var configs = new IntPtr();
					if (EGL.ChooseConfig(dc, pixelFormat, &configs, 1, &num_configs) == 0 || num_configs == 0)
					{
						Debug.ThrowError("Video", string.Format("Failed to retrieve GraphicsMode, error {0}", EGL.GetError()));
					}
					
					if (EGL.BindAPI(EGL.OPENGL_ES_API) == 0) Debug.ThrowError("Video", "Failed to bind GLES API");;
					
					int[] attrib_list = new int[]
					{
						EGL.CONTEXT_CLIENT_VERSION, 2,
						EGL.NONE
					};
					
					ctx = EGL.CreateContext(dc, configs, IntPtr.Zero, attrib_list);
					if (ctx == IntPtr.Zero) Debug.ThrowError("Video", "Failed to create context");
					
					surface = EGL.CreateWindowSurface(dc, configs, windowHandle.AddrOfPinnedObject(), null);
					//surface = EGL.CreateWindowSurface(dc, configs[0], handle, null);// <<<<<<<<<<<<<<<<<<<<<<<<<<<< used in x11
					if (surface == IntPtr.Zero) Debug.ThrowError("Video", "Failed to create window surface");
					
					if (EGL.MakeCurrent(dc, surface, surface, ctx) == 0) Debug.ThrowError("Video", "Failed to make EGL context current");
					//if (EGL.SwapInterval(dc, vSync ? 1 : 0) == 0) Debug.ThrowError("Video", "Failed to set vSync");
					checkForEGLError();
				}
				#else
				//Get DC
				handle = window.Handle;
				dc = X11.XOpenDisplay(IntPtr.Zero);
				int screen = X11.XDefaultScreen(dc);
		
				//Set Pixel format
				int[] attrbs =
				{
					GLX.RGBA,
					GLX.DOUBLEBUFFER, 1,
					//GLX.RED_SIZE, 8,
					//GLX.GREEN_SIZE, 8,
					//GLX.BLUE_SIZE, 8,
					//GLX.ALPHA_SIZE, 8,
					GLX.DEPTH_SIZE, 16,
					GLX.NONE
				};
			
				IntPtr visual = GLX.ChooseVisual(dc, screen, attrbs);
				if (visual == IntPtr.Zero) Debug.ThrowError("Video", "Failed to get visual");
				
				ctx = GLX.CreateContext(dc, visual, new IntPtr(0), true);
				GLX.MakeCurrent(dc, handle, ctx);

				bool failed = false;
				try {GLX.SwapIntervalSGI(vSync ? 1 : 0);}
				catch {failed = true;}
				
				if (failed)
				{
					GLX.Init();
					GLX.SwapIntervalMesa(vSync ? 1 : 0);
				}
				#endif
				#endif
				
				#if OSX
				var attribs = new object[]
				{
					NSOpenGLPixelFormatAttribute.Window,
					NSOpenGLPixelFormatAttribute.Accelerated,
					NSOpenGLPixelFormatAttribute.NoRecovery,
					NSOpenGLPixelFormatAttribute.DoubleBuffer,
					NSOpenGLPixelFormatAttribute.ColorSize, 24,
					NSOpenGLPixelFormatAttribute.AlphaSize, 8,
					NSOpenGLPixelFormatAttribute.DepthSize, 16
				};
	
				var pixelFormat = new NSOpenGLPixelFormat(attribs);
				if (pixelFormat == null) Debug.ThrowError("Video", "NSOpenGLPixelFormat failed");
	
				NSContext = new NSOpenGLContext(pixelFormat, null);
				if (NSContext == null) Debug.ThrowError("Video", "Failed to create GL context");
				NSContext.MakeCurrentContext();
				NSContext.SwapInterval = vSync;
				NSContext.View = window.View;
				ctx = NSContext.CGLContext.Handle;
				OS.NSContext = NSContext;
				
				/*//Get DC
				dc = CGL.DisplayIDToOpenGLDisplayMask(CGL.MainDisplayID());
				if (dc == IntPtr.Zero) Debug.ThrowError("Video", "Failed to get DC");
				
				//Set BackBuffer format
				int[] attributes =
				{
					//(int)CGL.PixelFormatAttribute.kCGLPFADoubleBuffer,
					//(int)CGL.PixelFormatAttribute.kCGLPFADisplayMask, dc.ToInt32(),
					//(int)CGL.PixelFormatAttribute.kCGLPFAWindow,
					(int)CGL.PixelFormatAttribute.kCGLPFAAccelerated,
					(int)CGL.PixelFormatAttribute.kCGLPFAColorSize, 32,
					(int)CGL.PixelFormatAttribute.kCGLPFADepthSize, 32,
					0
				};
			
				IntPtr pixelFormat = IntPtr.Zero;
				int pixelFormatCount = 0;
				int error = CGL.ChoosePixelFormat(attributes, ref pixelFormat, ref pixelFormatCount);
				if (error != 0 || pixelFormat == IntPtr.Zero)
				{
					Debug.ThrowError("Video", "ChoosePixelFormat failed");
				}
				
				error = CGL.CreateContext(pixelFormat, IntPtr.Zero, ref ctx);
				if (error != 0 || ctx == IntPtr.Zero)
				{
					Debug.ThrowError("Video", "Failed to create GL fullscreen context");
				}

				CGL.SetCurrentContext(ctx);*/
				#endif
				
				#if iOS
				// GL will be created by the Application
				#endif
				
				#if ANDROID
				// GL will be created by the Application
				#endif
				
				#if NaCl
				var frame = window.FrameSize;
				int[] attribs =
				{
			        (int)PPAPI.Graphics3DAttrib.PP_GRAPHICS3DATTRIB_ALPHA_SIZE, 8,
			        (int)PPAPI.Graphics3DAttrib.PP_GRAPHICS3DATTRIB_DEPTH_SIZE, 24,
			        (int)PPAPI.Graphics3DAttrib.PP_GRAPHICS3DATTRIB_STENCIL_SIZE, 8,
			        (int)PPAPI.Graphics3DAttrib.PP_GRAPHICS3DATTRIB_SAMPLES, 0,
			        (int)PPAPI.Graphics3DAttrib.PP_GRAPHICS3DATTRIB_SAMPLE_BUFFERS, 0,
			        (int)PPAPI.Graphics3DAttrib.PP_GRAPHICS3DATTRIB_WIDTH, frame.Width,
			        (int)PPAPI.Graphics3DAttrib.PP_GRAPHICS3DATTRIB_HEIGHT, frame.Height,
			        (int)PPAPI.Graphics3DAttrib.PP_GRAPHICS3DATTRIB_NONE
			    };
			    
			   	graphics = OS.GetGraphics();
			   	if (graphics == IntPtr.Zero) Debug.ThrowError("Video", "Failed to get Graphics object");
			   	
			   	IntPtr pbbInstance = OS.GetPBBInstance();
			   	if (graphics == IntPtr.Zero) Debug.ThrowError("Video", "Failed to get PBBInstance object");
			   	
			   	IntPtr gles = OS.GetGlES();
			   	if (gles == IntPtr.Zero) Debug.ThrowError("Video", "Failed to get GLES object");
			   	
			   	context = PPAPI.InitOpenGL(OS.GetInstance(), graphics, pbbInstance, attribs, gles);
			   	if (context == 0) Debug.ThrowError("Video", "Failed to create GL context");
			   	
			   	PPAPI.SetCurrentContextPPAPI(context);
			   	PPAPI.StartSwapBufferLoop(graphics, context);
				#endif
				
				checkForError();
		
				//Setup defualt OpenGL characteristics
				GL.FrontFace(GL.CCW);
				
				// Get Caps
				Caps = new Caps();
				Caps.MaxShaderVersion = ShaderVersions.Unknown;
				unsafe
				{
					int max = 0;
					GL.GetIntegerv(GL.MAX_TEXTURE_SIZE, &max);
					Caps.MaxTextureSize = max;
					
					// SEEMS TO MESS UP ON OSX
					//GL.GetIntegerv(GL.MAX_VERTEX_UNIFORM_VECTORS, &max);
					//Caps.MaxVertexConstants = max;

					//GL.GetIntegerv(GL.MAX_FRAGMENT_UNIFORM_VECTORS, &max);
					//Caps.MaxPixelConstants = max;

					#if RPI
					byte* shaderVersionPtr = null;
					#else
					byte* shaderVersionPtr = GL.GetString(GL.SHADING_LANGUAGE_VERSION);
					#endif
					string shaderVersion = "";
					while (shaderVersionPtr != null && shaderVersionPtr[0] != 0)
					{
						shaderVersion += (char)shaderVersionPtr[0];
						shaderVersionPtr++;
					}
					
					#if iOS || RPI
					shaderVersion = "1.0";
					#elif ANDROID
					shaderVersion = shaderVersion.Substring(shaderVersion.Length-4, 4);
					#elif NaCl
					shaderVersion = shaderVersion.Substring(18, 3);
					#else
					shaderVersion = shaderVersion.Substring(0, 4);
					#endif
					float shaderValue = Convert.ToSingle(shaderVersion);
					
					Caps.Version = Versions.GL1;
					FileTag = "";
					#if iOS || ANDROID || NaCl || RPI
					if (shaderValue >= 1.0f)
					{
						Caps.Version = Versions.GL2;
						FileTag = "GLES2_";
						Caps.MaxShaderVersion = ShaderVersions.GLSL_1_00;
					}
					#else
					if (shaderValue >= 1.1f)
					{
						Caps.Version = Versions.GL2;
						FileTag = "GL2_";
						Caps.MaxShaderVersion = ShaderVersions.GLSL_1_10;
					}
					if (shaderValue >= 1.2f)
					{
						Caps.Version = Versions.GL2;
						FileTag = "GL2_";
						Caps.MaxShaderVersion = ShaderVersions.GLSL_1_20;
					}
					// GL3 not supported in GLSL 1.30 becuase 'gl_InstanceID' only exists in GLSL 1.40
					/*if (shaderValue >= 1.3f)
					{
						Caps.Version = Versions.GL3;
						ShaderI.FileTag = "GL3_";
						Caps.MaxShaderVersion = ShaderVersions.GLSL_1_30;
					}*/
					if (shaderValue >= 1.4f)
					{
						Caps.Version = Versions.GL3;
						FileTag = "GL3_";
						Caps.MaxShaderVersion = ShaderVersions.GLSL_1_40;
					}
					if (shaderValue >= 1.5f)
					{
						Caps.Version = Versions.GL3;
						FileTag = "GL3_";
						Caps.MaxShaderVersion = ShaderVersions.GLSL_1_50;
					}
					if (shaderValue >= 3.3f)
					{
						Caps.Version = Versions.GL3;
						FileTag = "GL3_";
						Caps.MaxShaderVersion = ShaderVersions.GLSL_3_30;
					}
					//if (shaderValue >= 4.0f) Caps.MaxShaderVersion = ShaderVersions.GLSL_4_00;
					//if (shaderValue >= 4.1f) Caps.MaxShaderVersion = ShaderVersions.GLSL_4_10;
					//if (shaderValue >= 4.2f) Caps.MaxShaderVersion = ShaderVersions.GLSL_4_20;
					#endif

					if (Caps.Version == Versions.GL1) Debug.ThrowError("Video", "OpenGL 2 or higher is required.\nAre your video card drivers up to date?");
					
					byte* extensionsPtr = GL.GetString(GL.EXTENSIONS);
					string extensionValues = "";
					while (extensionsPtr != null && extensionsPtr[0] != 0)
					{
						extensionValues += (char)extensionsPtr[0];
						extensionsPtr++;
					}
					
					var extensions = extensionValues.Split(' ');
					foreach (var ext in extensions)
					{
						switch (ext)
						{
							case ("GL_instanced_arrays"): Caps.HardwareInstancing = true; break;
							case ("GL_ARB_instanced_arrays"): Caps.HardwareInstancing = true; break;
							case ("GL_EXT_texture_compression_s3tc"): Caps.TextureCompression_S3TC = true; break;
							case ("GL_AMD_compressed_ATC_texture"): Caps.TextureCompression_ATC = true; break;
							case ("GL_ATI_texture_compression_atitc"): Caps.TextureCompression_ATC = true; break;
							case ("GL_IMG_texture_compression_pvrtc"): Caps.TextureCompression_PVR = true; break;
						}
						//Console.WriteLine(ext);
					}
				}
				
				checkForError();
				
				// Init Ext
				GL.Init();
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public unsafe override void Dispose()
		{
			if (disposed) return;
			disposed = true;
			disposeChilderen();
			
			#if NaCl
			PPAPI.StopSwapBufferLoop();
			#endif

			if (frameBuffer != 0)
			{
				uint frameBufferTEMP = frameBuffer;
				GL.BindFramebuffer(GL.FRAMEBUFFER, 0);
				GL.DeleteFramebuffers(1, &frameBufferTEMP);
				frameBuffer = 0;

				#if DEBUG
				Video.checkForError();
				#endif
			}
			
			if (dc != IntPtr.Zero)
			{
				DisableContext();
			}

			#if WINDOWS
			if (dc != IntPtr.Zero)
			{
				if (ctx != IntPtr.Zero)
				{
					WGL.DeleteContext(ctx);
					ctx = IntPtr.Zero;
				}

				if (handle != IntPtr.Zero)
				{
					WGL.ReleaseDC(handle, dc);
					handle = IntPtr.Zero;
				}

				dc = IntPtr.Zero;
			}
			#endif
			
			#if LINUX
			#if RPI
			if (dc != IntPtr.Zero)
			{
				if (surface != IntPtr.Zero) EGL.DestroySurface(dc, surface);
				if (ctx != IntPtr.Zero) EGL.DestroyContext(dc, ctx);
				EGL.Terminate(dc);
				dc = IntPtr.Zero;
			}
			
			RaspberryPi.bcm_host_deinit();
			windowHandle.Free();
			#else
			if (dc != IntPtr.Zero)
			{
				if (ctx != IntPtr.Zero)
				{
					GLX.DestroyContext(dc, ctx);
					ctx = IntPtr.Zero;
				}

				X11.XCloseDisplay(dc);
				dc = IntPtr.Zero;
			}
			#endif
			#endif
			
			#if OSX
			if (dc != IntPtr.Zero)
			{
				if (ctx != IntPtr.Zero)
				{
					CGL.DestroyContext(ctx);
					ctx = IntPtr.Zero;
				}

				CGL.ReleaseAllDisplays();
				dc = IntPtr.Zero;
			}
			#endif
			base.Dispose();
		}
		#endregion

		#region Methods
		public void DisableContext()
		{
			#if WINDOWS
			WGL.MakeCurrent(dc, IntPtr.Zero);
			#endif
			
			#if LINUX
			#if RPI
			EGL.MakeCurrent(dc, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			#else
			GLX.MakeCurrent(dc, IntPtr.Zero, IntPtr.Zero);
			#endif
			#endif
			
			#if OSX
			CGL.SetCurrentContext(IntPtr.Zero);
			#endif
		}

		public void Update()
		{
			#if !RPI
			#if WINDOWS || OSX || LINUX || NaCl
			var frame = window.FrameSize;
			#else
			var frame = application.FrameSize;
			#endif
			if (frame.Width != 0 && frame.Height != 0) BackBufferSize = frame;
			#endif

			#if WINDOWS
			WGL.MakeCurrent(dc, ctx);
			#endif
			
			#if LINUX
			#if RPI
			EGL.MakeCurrent(dc, surface, surface, ctx);
			#else
			GLX.MakeCurrent(dc, handle, ctx);
			#endif
			#endif
			
			#if OSX
			CGL.SetCurrentContext(ctx);
			#endif
			
			#if NaCl
			PPAPI.SetCurrentContextPPAPI(context);
			#endif

			#if DEBUG
			#if RPI
			checkForEGLError();
			#endif
			checkForError();
			#endif
		}

		internal static void checkForError()
		{
			uint error;
			string errorName;
			if (checkForError(out error, out errorName)) Debug.ThrowError("Video", string.Format("GL ERROR {0}: {1}", error, errorName));
		}

		internal static bool checkForError(out uint error, out string errorName)
		{
			error = GL.GetError();
			if (error != GL.NO_ERROR)
			{
				switch (error)
				{
					case (GL.INVALID_ENUM): errorName = "INVALID_ENUM"; break;
					case (GL.INVALID_VALUE): errorName = "INVALID_VALUE"; break;
					case (GL.INVALID_OPERATION): errorName = "INVALID_OPERATION"; break;
					case (GL.OUT_OF_MEMORY): errorName = "OUT_OF_MEMORY"; break;
					case (GL.STACK_OVERFLOW): errorName = "STACK_OVERFLOW"; break;
					case (GL.STACK_UNDERFLOW): errorName = "STACK_UNDERFLOW"; break;
					case (GL.TABLE_TOO_LARGE): errorName = "TABLE_TOO_LARGE"; break;
					default: errorName = "Unknown Error"; break;
				}

				return true;
			}

			errorName = null;
			return false;
		}
		
		#if RPI
		internal static bool checkForEGLError(out int error, out string errorName)
		{
			error = EGL.GetError();
			if (error != EGL.SUCCESS)
			{
				switch (error)
				{
					case (EGL.NOT_INITIALIZED): errorName = "NOT_INITIALIZED"; break;
					case (EGL.BAD_ACCESS): errorName = "BAD_ACCESS"; break;
					case (EGL.BAD_ALLOC): errorName = "BAD_ALLOC"; break;
					case (EGL.BAD_ATTRIBUTE): errorName = "BAD_ATTRIBUTE"; break;
					case (EGL.BAD_CONFIG): errorName = "BAD_CONFIG"; break;
					case (EGL.BAD_CONTEXT): errorName = "BAD_CONTEXT"; break;
					case (EGL.BAD_CURRENT_SURFACE): errorName = "BAD_CURRENT_SURFACE"; break;
					case (EGL.BAD_DISPLAY): errorName = "BAD_DISPLAY"; break;
					case (EGL.BAD_MATCH): errorName = "BAD_MATCH"; break;
					case (EGL.BAD_NATIVE_PIXMAP): errorName = "BAD_NATIVE_PIXMAP"; break;
					case (EGL.BAD_NATIVE_WINDOW): errorName = "BAD_NATIVE_WINDOW"; break;
					case (EGL.BAD_PARAMETER): errorName = "BAD_PARAMETER"; break;
					case (EGL.BAD_SURFACE): errorName = "BAD_SURFACE"; break;
					case (EGL.CONTEXT_LOST): errorName = "CONTEXT_LOST"; break;
					default: errorName = "Unknown Error"; break;
				}
				
				return true;
			}
			
			errorName = null;
			return false;
		}
		
		public void checkForEGLError()
		{
			int error;
			string errorName;
			if (checkForEGLError(out error, out errorName)) Debug.ThrowError("Video", string.Format("EGL ERROR {0}: {1}", error, errorName));
		}
		#endif

		public void EnableRenderTarget()
		{
			#if iOS
			((GLKView)application.View).BindDrawable();
			#else
		    GL.BindFramebuffer(GL.FRAMEBUFFER, frameBuffer);
		    #endif
		}

		public void EnableRenderTarget(DepthStencilI depthStencil)
		{
			#if iOS
			((GLKView)application.View).BindDrawable();
			#else
		    GL.BindFramebuffer(GL.FRAMEBUFFER, frameBuffer);
		    #endif
			
			if (depthStencil != null)
			{
				uint surface = ((DepthStencil)depthStencil).surface;
				GL.BindRenderbuffer(GL.RENDERBUFFER, surface);
				GL.FramebufferRenderbuffer(GL.FRAMEBUFFER, GL.DEPTH_ATTACHMENT, GL.RENDERBUFFER, surface);
			}
			else
			{
				GL.BindRenderbuffer(GL.RENDERBUFFER, 0);
			}
		}

		public void ClearAll(float r, float g, float b, float a)
		{
			GL.ClearColor(r, g, b, a);
			GL.Clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT | GL.STENCIL_BUFFER_BIT);
		}

		public void ClearColor(float r, float g, float b, float a)
		{
			GL.ClearColor(r, g, b, a);
			GL.Clear(GL.COLOR_BUFFER_BIT);
		}

		public void ClearColorDepth(float r, float g, float b, float a)
		{
			GL.ClearColor(r, g, b, a);
			GL.Clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);
		}

		public void ClearDepthStencil()
		{
			GL.Clear(GL.DEPTH_BUFFER_BIT | GL.STENCIL_BUFFER_BIT);
		}

		public void Present()
		{
			#if WINDOWS
			WGL.SwapBuffers(dc);
			#endif
			
			#if LINUX
			#if RPI
			EGL.SwapBuffers(dc, surface);
			#else
			/*unsafe
			{
				if (vSync)
				{
					uint retraceCount = 0;
					GLX.GetVideoSyncSGI(&retraceCount);
					GLX.WaitVideoSyncSGI(2, ((int)retraceCount+1)%2, &retraceCount);
				}
			}*/
			GLX.SwapBuffers(dc, handle);
			#endif
			#endif
			
			#if OSX
			CGL.FlushDrawable(ctx);
			#endif
			
			#if NaCl
			GL.ColorMask(false, false, false, true);
			ClearColor(0, 0, 0, 1);
			GL.ColorMask(true, true, true, true);
			#endif
		}

		internal void disableActiveTextures(Texture2D texture)
		{
			var textures = currentVertexTextures;
			for (int i = 0; i != textures.Length; ++i)
			{
				if (textures[i] == texture)
				{
					throw new NotImplementedException();
					//textures[i] = null;
				}
			}

			textures = currentPixelTextures;
			for (int i = 0; i != textures.Length; ++i)
			{
				if (textures[i] == texture)
				{
					GL.ActiveTexture(ShaderResource.textureMaps[i]);
					GL.BindTexture(GL.TEXTURE_2D, 0);
					textures[i] = null;
				}
			}
		}

		internal static int surfaceFormat(SurfaceFormats surfaceFormat)
		{
			switch (surfaceFormat)
			{
				case (SurfaceFormats.RGBAx8):
					#if iOS || ANDROID || NaCl || RPI
					return (int)GL.RGBA;
					#else
					return GL.RGBA8;
					#endif
				case (SurfaceFormats.RGBx10_Ax2): return GL.RGB10_A2;
				case (SurfaceFormats.RGBAx16f): return GL.RGBA16F;
				case (SurfaceFormats.RGBAx32f): return GL.RGBA32F;
				default: 
					Debug.ThrowError("Video", "Unsuported SurfaceFormat");
					return GL.RGBA8;
			}
		}
		#endregion
	}
}
