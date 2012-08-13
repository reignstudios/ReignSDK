#if OSX || iOS || ANDROID || NaCl
#define IS_NOT_EXT
#endif

using System;
using System.Runtime.InteropServices;
using System.Security;

//using GLvoid = System.Void;
using GLenum = System.UInt32;
using GLuint = System.UInt32;
using GLint = System.Int32;
using GLsizei = System.Int32;
using GLchar = System.Byte;
using GLubyte = System.Byte;
using GLfloat = System.Single;
using GLboolean = System.Boolean;
using GLsizeiptr = System.IntPtr;
using GLintptr = System.IntPtr;
using GLclampf = System.Single;
using GLbitfield = System.UInt32;

namespace Reign.Video.OpenGL
{
	public class GL
	{
		#if WINDOWS
		public const string DLL = "opengl32";
		[DllImport(DLL, EntryPoint = "wglGetProcAddress", ExactSpelling = true)]
		private static extern IntPtr getProcAddress(string procedureName);
		#endif
		
		#if LINUX
		public const string DLL = "libGL.so.1";
		[DllImport(DLL, EntryPoint = "glXGetProcAddress")]
		private static extern IntPtr getProcAddress([MarshalAs(UnmanagedType.LPTStr)] string procedureName);
		#endif
		
		#if OSX
		public const string DLL = "System/Library/Frameworks/OpenGL.framework/OpenGL";

		/*private const string Library = "libdl.dylib";
		[DllImport(Library, EntryPoint = "NSIsSymbolNameDefined", ExactSpelling = true)]
		private static extern bool NSIsSymbolNameDefined(string s);
		[DllImport(Library, EntryPoint = "NSLookupAndBindSymbol", ExactSpelling = true)]
		private static extern IntPtr NSLookupAndBindSymbol(string s);
		[DllImport(Library, EntryPoint = "NSAddressOfSymbol", ExactSpelling = true)]
		private static extern IntPtr NSAddressOfSymbol(IntPtr symbol);

		public IntPtr GetProcAddress(string procedureName)
		{
			string fname = "_" + procedureName;
			if (!NSIsSymbolNameDefined(fname)) return IntPtr.Zero;

			IntPtr symbol = NSLookupAndBindSymbol(fname);
			if (symbol != IntPtr.Zero) symbol = NSAddressOfSymbol(symbol);

			return symbol;
		}*/
		#endif
		
		#if iOS
		public const string DLL = "/System/Library/Frameworks/OpenGLES.framework/OpenGLES";//"libGLESv2.dll"
		#endif
		
		#if ANDROID
		public const string DLL = "libGLESv2";
		#endif
		
		#if NaCl
		public const string DLL = "__Internal";
		#endif

		#if WINDOWS || LINUX
		public static IntPtr GetProcAddress(string procedureName)
		{
			IntPtr ptr = getProcAddress(procedureName);
			if (ptr == IntPtr.Zero) Reign.Core.Debug.ThrowError("GL", string.Format("OpenGL Ext Method '{0}' does not exist", procedureName));
			return ptr;
		}
		#endif

		//Ext Methods
		public static void Init()
		{
			#if WINDOWS || LINUX
			init_Shaders();
			init_Buffers();
			init_SurfaceBuffers();
			#endif
		}

		#region Shaders
		#if WINDOWS || LINUX
		private static void init_Shaders()
		{
			GenerateMipmap = (GenerateMipmapFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glGenerateMipmap"), typeof(GenerateMipmapFunc));
			CreateShader = (CreateShaderFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glCreateShader"), typeof(CreateShaderFunc));
			ShaderSource = (ShaderSourceFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glShaderSource"), typeof(ShaderSourceFunc));
			GetShaderiv = (GetShaderivFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glGetShaderiv"), typeof(GetShaderivFunc));
			CompileShader = (CompileShaderFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glCompileShader"), typeof(CompileShaderFunc));
			DeleteShader = (DeleteShaderFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glDeleteShader"), typeof(DeleteShaderFunc));
			GetShaderInfoLog = (GetShaderInfoLogFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glGetShaderInfoLog"), typeof(GetShaderInfoLogFunc));
			CreateProgram = (CreateProgramFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glCreateProgram"), typeof(CreateProgramFunc));
			DeleteProgram = (DeleteProgramFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glDeleteProgram"), typeof(DeleteProgramFunc));
			AttachShader = (AttachShaderFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glAttachShader"), typeof(AttachShaderFunc));
			LinkProgram = (LinkProgramFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glLinkProgram"), typeof(LinkProgramFunc));
			UseProgram = (UseProgramFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUseProgram"), typeof(UseProgramFunc));
			GetUniformLocation = (GetUniformLocationFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glGetUniformLocation"), typeof(GetUniformLocationFunc));
			Uniform1f = (Uniform1fFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniform1f"), typeof(Uniform1fFunc));
			Uniform2f = (Uniform2fFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniform2f"), typeof(Uniform2fFunc));
			Uniform3f = (Uniform3fFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniform3f"), typeof(Uniform3fFunc));
			Uniform4f = (Uniform4fFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniform4f"), typeof(Uniform4fFunc));
			Uniform1i = (Uniform1iFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniform1i"), typeof(Uniform1iFunc));
			Uniform1fv = (Uniform1fvFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniform1fv"), typeof(Uniform1fvFunc));
			Uniform2fv = (Uniform2fvFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniform2fv"), typeof(Uniform2fvFunc));
			Uniform3fv = (Uniform3fvFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniform3fv"), typeof(Uniform3fvFunc));
			Uniform4fv = (Uniform4fvFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniform4fv"), typeof(Uniform4fvFunc));
			UniformMatrix2fv = (UniformMatrix2fvFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniformMatrix2fv"), typeof(UniformMatrix2fvFunc));
			UniformMatrix3fv = (UniformMatrix3fvFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniformMatrix3fv"), typeof(UniformMatrix3fvFunc));
			UniformMatrix4fv = (UniformMatrix4fvFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glUniformMatrix4fv"), typeof(UniformMatrix4fvFunc));
		}
		#endif

		public const uint SHADING_LANGUAGE_VERSION = 0x8B8C;
		public const uint VERTEX_SHADER = 0x8B31;
		public const uint FRAGMENT_SHADER = 0x8B30;
		public const uint COMPILE_STATUS = 0x8B81;
		public const uint INFO_LOG_LENGTH = 0x8B84;
		public const uint TEXTURE_2D = 0x0de1;
		public const uint MAX_VERTEX_UNIFORM_VECTORS = 0x8DFB;
		public const uint MAX_FRAGMENT_UNIFORM_VECTORS = 0x8DFD;

		public const uint TEXTURE_MAG_FILTER = 0x2800;
		public const uint TEXTURE_MIN_FILTER = 0x2801;
		public const uint TEXTURE_WRAP_S = 0x2802;
		public const uint TEXTURE_WRAP_T = 0x2803;
		public const uint TEXTURE_WRAP_R = 0x8072;
		public const uint TEXTURE_MAX_LEVEL = 0x813D;

		public const int NEAREST = 0x2600;
		public const int LINEAR = 0x2601;
		public const int NEAREST_MIPMAP_NEAREST = 0x2700;
		public const int LINEAR_MIPMAP_NEAREST = 0x2701;
		public const int NEAREST_MIPMAP_LINEAR = 0x2702;
		public const int LINEAR_MIPMAP_LINEAR = 0x2703;

		public const int CLAMP = 0x2900;
		public const int CLAMP_TO_EDGE = 0x812F;
		public const int CLAMP_TO_BORDER = 0x812D;
		public const int REPEAT = 0x2901;
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glTexParameteri", ExactSpelling = true)]
		public static extern void TexParameteri(GLenum target, GLenum pname, GLint param);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glTexParameteriv", ExactSpelling = true)]
		public unsafe static extern void TexParameteriv(GLenum target, GLenum pname, GLint *@params);

		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glGenerateMipmap", ExactSpelling = true)]
			public static extern void GenerateMipmap(GLenum target);
		#else
			public delegate void GenerateMipmapFunc(GLenum target);
			public static GenerateMipmapFunc GenerateMipmap;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glCreateShader", ExactSpelling = true)]
			public static extern GLuint CreateShader(GLenum type);
		#else
			public delegate GLuint CreateShaderFunc(GLenum type);
			public static CreateShaderFunc CreateShader;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glShaderSource", ExactSpelling = true)]
			public unsafe static extern void ShaderSource(GLuint shader, GLsizei count, GLchar** @string, GLint* length);
		#else
			public unsafe delegate void ShaderSourceFunc(GLuint shader, GLsizei count, GLchar** @string, GLint* length);
			public static ShaderSourceFunc ShaderSource;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glGetShaderiv", ExactSpelling = true)]
			public unsafe static extern void GetShaderiv(GLuint shader, GLenum pname, [OutAttribute] GLint* pParams);
		#else
			public unsafe delegate void GetShaderivFunc(GLuint shader, GLenum pname, [OutAttribute] GLint* pParams);
			public static GetShaderivFunc GetShaderiv;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glCompileShader", ExactSpelling = true)]
			public static extern void CompileShader(GLuint shader);
		#else
			public delegate void CompileShaderFunc(GLuint shader);
			public static CompileShaderFunc CompileShader;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glDeleteShader", ExactSpelling = true)]
			public static extern void DeleteShader(GLuint shader);
		#else
			public delegate void DeleteShaderFunc(GLuint shader);
			public static DeleteShaderFunc DeleteShader;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glGetShaderInfoLog")]
			public unsafe static extern void GetShaderInfoLog(GLuint shader, GLsizei bufSize, [OutAttribute] GLsizei* length, [OutAttribute] GLchar* infoLog);
		#else
			public unsafe delegate void GetShaderInfoLogFunc(GLuint shader, GLsizei bufSize, [OutAttribute] GLsizei* length, [OutAttribute] GLchar* infoLog);
			public static GetShaderInfoLogFunc GetShaderInfoLog;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glCreateProgram", ExactSpelling = true)]
			public static extern GLuint CreateProgram();
		#else
			public delegate GLuint CreateProgramFunc();
			public static CreateProgramFunc CreateProgram;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glDeleteProgram", ExactSpelling = true)]
			public static extern void DeleteProgram(GLuint program);
		#else
			public delegate void DeleteProgramFunc(GLuint program);
			public static DeleteProgramFunc DeleteProgram;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glAttachShader", ExactSpelling = true)]
			public static extern void AttachShader(GLuint program, GLuint shader);
		#else
			public delegate void AttachShaderFunc(GLuint program, GLuint shader);
			public static AttachShaderFunc AttachShader;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glLinkProgram", ExactSpelling = true)]
			public static extern void LinkProgram(GLuint program);
		#else
			public delegate void LinkProgramFunc(GLuint program);
			public static LinkProgramFunc LinkProgram;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUseProgram", ExactSpelling = true)]
			public static extern void UseProgram(GLuint program);
		#else
			public delegate void UseProgramFunc(GLuint program);
			public static UseProgramFunc UseProgram;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glGetUniformLocation", ExactSpelling = true)]
			public static extern GLint GetUniformLocation(GLuint program, string name);
		#else
			public delegate GLint GetUniformLocationFunc(GLuint program, string name);
			public static GetUniformLocationFunc GetUniformLocation;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniform1f", ExactSpelling = true)]
			public static extern void Uniform1f(GLint location, GLfloat x);
		#else
			public delegate void Uniform1fFunc(GLint location, GLfloat x);
			public static Uniform1fFunc Uniform1f;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniform2f", ExactSpelling = true)]
			public static extern void Uniform2f(GLint location, GLfloat x, GLfloat y);
		#else
			public delegate void Uniform2fFunc(GLint location, GLfloat x, GLfloat y);
			public static Uniform2fFunc Uniform2f;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniform3f", ExactSpelling = true)]
			public static extern void Uniform3f(GLint location, GLfloat x, GLfloat y, GLfloat z);
		#else
			public delegate void Uniform3fFunc(GLint location, GLfloat x, GLfloat y, GLfloat z);
			public static Uniform3fFunc Uniform3f;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniform4f", ExactSpelling = true)]
			public static extern void Uniform4f(GLint location, GLfloat x, GLfloat y, GLfloat z, GLfloat w);
		#else
			public delegate void Uniform4fFunc(GLint location, GLfloat x, GLfloat y, GLfloat z, GLfloat w);
			public static Uniform4fFunc Uniform4f;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniform1i", ExactSpelling = true)]
			public unsafe static extern void Uniform1i(GLint location, GLint x);
		#else
			public unsafe delegate void Uniform1iFunc(GLint location, GLint x);
			public static Uniform1iFunc Uniform1i;
		#endif

		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniform1fv", ExactSpelling = true)]
			public unsafe static extern void Uniform1fv(GLint location, GLsizei count, GLfloat* v);
		#else
			public unsafe delegate void Uniform1fvFunc(GLint location, GLsizei count, GLfloat* v);
			public static Uniform1fvFunc Uniform1fv;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniform2fv", ExactSpelling = true)]
			public unsafe static extern void Uniform2fv(GLint location, GLsizei count, GLfloat* v);
		#else
			public unsafe delegate void Uniform2fvFunc(GLint location, GLsizei count, GLfloat* v);
			public static Uniform2fvFunc Uniform2fv;
		#endif

		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniform3fv", ExactSpelling = true)]
			public unsafe static extern void Uniform3fv(GLint location, GLsizei count, GLfloat* v);
		#else
			public unsafe delegate void Uniform3fvFunc(GLint location, GLsizei count, GLfloat* v);
			public static Uniform3fvFunc Uniform3fv;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniform4fv", ExactSpelling = true)]
			public unsafe static extern void Uniform4fv(GLint location, GLsizei count, GLfloat* v);
		#else
			public unsafe delegate void Uniform4fvFunc(GLint location, GLsizei count, GLfloat* v);
			public static Uniform4fvFunc Uniform4fv;
		#endif

		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniformMatrix2fv", ExactSpelling = true)]
			public unsafe static extern void UniformMatrix2fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
		#else
			public unsafe delegate void UniformMatrix2fvFunc(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
			public static UniformMatrix2fvFunc UniformMatrix2fv;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniformMatrix3fv", ExactSpelling = true)]
			public unsafe static extern void UniformMatrix3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
		#else
			public unsafe delegate void UniformMatrix3fvFunc(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
			public static UniformMatrix3fvFunc UniformMatrix3fv;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glUniformMatrix4fv", ExactSpelling = true)]
			public unsafe static extern void UniformMatrix4fv(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
		#else
			public unsafe delegate void UniformMatrix4fvFunc(GLint location, GLsizei count, GLboolean transpose, GLfloat* value);
			public static UniformMatrix4fvFunc UniformMatrix4fv;
		#endif
		#endregion

		#region Buffers
		#if WINDOWS || LINUX
		private static void init_Buffers()
		{
			DrawArraysInstanced = (DrawArraysInstancedFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glDrawArraysInstanced"), typeof(DrawArraysInstancedFunc));
			DrawElementsInstanced = (DrawElementsInstancedFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glDrawElementsInstanced"), typeof(DrawElementsInstancedFunc));
			VertexAttribDivisor = (VertexAttribDivisorFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glVertexAttribDivisor"), typeof(VertexAttribDivisorFunc));
			GenBuffers = (GenBuffersFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glGenBuffers"), typeof(GenBuffersFunc));
			BindBuffer = (BindBufferFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glBindBuffer"), typeof(BindBufferFunc));
			DeleteBuffers = (DeleteBuffersFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glDeleteBuffers"), typeof(DeleteBuffersFunc));
			BufferData = (BufferDataFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glBufferData"), typeof(BufferDataFunc));
			BufferSubData = (BufferSubDataFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glBufferSubData"), typeof(BufferSubDataFunc));
			EnableVertexAttribArray = (EnableVertexAttribArrayFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glEnableVertexAttribArray"), typeof(EnableVertexAttribArrayFunc));
			DisableVertexAttribArray = (DisableVertexAttribArrayFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glDisableVertexAttribArray"), typeof(DisableVertexAttribArrayFunc));
			VertexAttribPointer = (VertexAttribPointerFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glVertexAttribPointer"), typeof(VertexAttribPointerFunc));
			BindAttribLocation = (BindAttribLocationFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glBindAttribLocation"), typeof(BindAttribLocationFunc));
			GetAttribLocation = (GetAttribLocationFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glGetAttribLocation"), typeof(GetAttribLocationFunc));
		}
		#endif

		public const uint ARRAY_BUFFER = 0x8892;
		public const uint ELEMENT_ARRAY_BUFFER = 0x8893;
		public const uint STATIC_DRAW = 0x88E4;

		public const uint POINT = 0x1b00;
		public const uint LINE = 0x1b01;
		public const uint FILL = 0x1b02;

		public const uint POINTS = 0x0000;
		public const uint LINES = 0x0001;
		public const uint TRIANGLES = 0x0004;

		public const uint NONE = 0;
		public const uint FRONT_AND_BACK = 0x0408;
		public const uint CULL_FACE = 0x0b44;
		public const uint CW = 0x0900;
		public const uint CCW = 0x0901;

		public const uint FLOAT = 0x1406;
		public const uint UNSIGNED_BYTE = 0x1401;
		public const uint UNSIGNED_SHORT = 0x1403;
		public const uint UNSIGNED_INT = 0x1405;
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glDrawArrays", ExactSpelling = true)]
		public static extern void DrawArrays(GLenum mode, GLint first, GLsizei count);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glDrawElements", ExactSpelling = true)]
		public static extern void DrawElements(GLenum mode, GLsizei count, GLenum type, IntPtr indices);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glCullFace", ExactSpelling = true)]
		public static extern void CullFace(GLenum mode);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glPolygonMode", ExactSpelling = true)]
		public static extern void PolygonMode(GLenum face, GLenum mode);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glFrontFace", ExactSpelling = true)]
		public static extern void FrontFace(GLenum mode);
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glDrawArraysInstanced", ExactSpelling = true)]
			public static extern void DrawArraysInstanced(GLenum mode, GLint first, GLsizei count, GLsizei primcount);
		#else
			public delegate void DrawArraysInstancedFunc(GLenum mode, GLint first, GLsizei count, GLsizei primcount);
			public static DrawArraysInstancedFunc DrawArraysInstanced;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glDrawElementsInstanced", ExactSpelling = true)]
			public static extern void DrawElementsInstanced(GLenum mode, GLsizei count, GLenum type, IntPtr indices, GLsizei primcount);
		#else
			public delegate void DrawElementsInstancedFunc(GLenum mode, GLsizei count, GLenum type, IntPtr indices, GLsizei primcount);
			public static DrawElementsInstancedFunc DrawElementsInstanced;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glVertexAttribDivisorARB", ExactSpelling = false)]
			public static extern void VertexAttribDivisor(GLuint index, GLuint divisor);
		#else
			public delegate void VertexAttribDivisorFunc(GLuint index, GLuint divisor);
			public static VertexAttribDivisorFunc VertexAttribDivisor;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glGenBuffers", ExactSpelling = true)]
			public unsafe static extern void GenBuffers(GLsizei n, [OutAttribute] GLuint* buffers);
		#else
			public unsafe delegate void GenBuffersFunc(GLsizei n, [OutAttribute] GLuint* buffers);
			public static GenBuffersFunc GenBuffers;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glBindBuffer", ExactSpelling = true)]
			public static extern void BindBuffer(GLenum target, GLuint buffer);
		#else
			public delegate void BindBufferFunc(GLenum target, GLuint buffer);
			public static BindBufferFunc BindBuffer;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glBufferData", ExactSpelling = true)]
			public unsafe static extern void BufferData(GLenum target, GLsizeiptr size, void* data, GLenum usage);
		#else
			public unsafe delegate void BufferDataFunc(GLenum target, GLsizeiptr size, void* data, GLenum usage);
			public static BufferDataFunc BufferData;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glBufferSubData", ExactSpelling = true)]
			public unsafe static extern void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, void* data);
		#else
			public unsafe delegate void BufferSubDataFunc(GLenum target, GLintptr offset, GLsizeiptr size, void* data);
			public static BufferSubDataFunc BufferSubData;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glDeleteBuffers", ExactSpelling = true)]
			public unsafe static extern void DeleteBuffers(GLsizei n, GLuint* buffers);
		#else
			public unsafe delegate void DeleteBuffersFunc(GLsizei n, GLuint* buffers);
			public static DeleteBuffersFunc DeleteBuffers;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glEnableVertexAttribArray", ExactSpelling = true)]
			public static extern void EnableVertexAttribArray(GLuint index);
		#else
			public delegate void EnableVertexAttribArrayFunc(GLuint index);
			public static EnableVertexAttribArrayFunc EnableVertexAttribArray;
		#endif

		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glDisableVertexAttribArray", ExactSpelling = true)]
			public static extern void DisableVertexAttribArray(GLuint index);
		#else
			public delegate void DisableVertexAttribArrayFunc(GLuint index);
			public static DisableVertexAttribArrayFunc DisableVertexAttribArray;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glVertexAttribPointer", ExactSpelling = true)]
			public unsafe static extern void VertexAttribPointer(GLuint indx, GLint size, GLenum type, GLboolean normalized, GLsizei stride, void* ptr);
		#else
			public unsafe delegate void VertexAttribPointerFunc(GLuint indx, GLint size, GLenum type, GLboolean normalized, GLsizei stride, void* ptr);
			public static VertexAttribPointerFunc VertexAttribPointer;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glBindAttribLocation", ExactSpelling = true)]
			public unsafe static extern void BindAttribLocation(GLuint program, GLuint index, string name);
		#else
			public unsafe delegate void BindAttribLocationFunc(GLuint program, GLuint index, string name);
			public static BindAttribLocationFunc BindAttribLocation;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glGetAttribLocation", ExactSpelling = true)]
			public unsafe static extern GLint GetAttribLocation(GLuint program, string name);
		#else
			public unsafe delegate GLint GetAttribLocationFunc(GLuint program, string name);
			public static GetAttribLocationFunc GetAttribLocation;
		#endif
		#endregion
		
		#region SurfaceBuffers
		#if WINDOWS || LINUX
		private static void init_SurfaceBuffers()
		{
			BlendEquation = (BlendEquationFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glBlendEquation"), typeof(BlendEquationFunc));
			BlendEquationSeparate = (BlendEquationSeparateFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glBlendEquationSeparate"), typeof(BlendEquationSeparateFunc));
			BlendFuncSeparate = (BlendFuncSeparateFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glBlendFuncSeparate"), typeof(BlendFuncSeparateFunc));
			CompressedTexImage2D = (CompressedTexImage2DFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glCompressedTexImage2D"), typeof(CompressedTexImage2DFunc));
			GenRenderbuffers = (GenRenderbuffersFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glGenRenderbuffers"), typeof(GenRenderbuffersFunc));
			BindRenderbuffer = (BindRenderbufferFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glBindRenderbuffer"), typeof(BindRenderbufferFunc));
			GenFramebuffers = (GenFramebuffersFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glGenFramebuffers"), typeof(GenFramebuffersFunc));
			BindFramebuffer = (BindFramebufferFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glBindFramebuffer"), typeof(BindFramebufferFunc));
			CheckFramebufferStatus = (CheckFramebufferStatusFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glCheckFramebufferStatus"), typeof(CheckFramebufferStatusFunc));
			DeleteFramebuffers = (DeleteFramebuffersFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glDeleteFramebuffers"), typeof(DeleteFramebuffersFunc));
			RenderbufferStorage = (RenderbufferStorageFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glRenderbufferStorage"), typeof(RenderbufferStorageFunc));
			FramebufferRenderbuffer = (FramebufferRenderbufferFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glFramebufferRenderbuffer"), typeof(FramebufferRenderbufferFunc));
			DeleteRenderbuffers = (DeleteRenderbuffersFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glDeleteRenderbuffers"), typeof(DeleteRenderbuffersFunc));
			FramebufferTexture2D = (FramebufferTexture2DFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glFramebufferTexture2D"), typeof(FramebufferTexture2DFunc));
			ActiveTexture = (ActiveTextureFunc)Marshal.GetDelegateForFunctionPointer(GetProcAddress("glActiveTexture"), typeof(ActiveTextureFunc));
		}
		#endif
		
		public const uint RENDERBUFFER = 0x8D41u;
		public const uint FRAMEBUFFER = 0x8D40u;
		public const uint COLOR_ATTACHMENT0 = 0x8CE0u;
		public const uint DEPTH_ATTACHMENT = 0x8D00u;
		public const uint DEPTH_COMPONENT16 = 0x81A5u;
		public const uint FRAMEBUFFER_COMPLETE = 0x8CD5u;
		public const uint TEXTURE0 = 0x84C0u;
		public const uint TEXTURE1 = 0x84C1u;
		public const uint TEXTURE2 = 0x84C2u;
		public const uint TEXTURE3 = 0x84C3u;
		public const uint TEXTURE4 = 0x84C4;
		public const uint TEXTURE5 = 0x84C5;
		public const uint TEXTURE6 = 0x84C6;
		public const uint TEXTURE7 = 0x84C7;
		public const uint MULTISAMPLE = 0x809D;
		public const uint TEXTURE_BORDER_COLOR = 0x1004;

		public const uint BLEND = 0x0BE2;

		public const uint ZERO = 0;
		public const uint ONE = 1;
		public const uint SRC_COLOR = 0x0300;
		public const uint ONE_MINUS_SRC_COLOR = 0x0301;
		public const uint SRC_ALPHA = 0x0302;
		public const uint ONE_MINUS_SRC_ALPHA = 0x0303;
		public const uint DST_ALPHA = 0x0304;
		public const uint ONE_MINUS_DST_ALPHA = 0x0305;
		public const uint DST_COLOR = 0x0306;
		public const uint ONE_MINUS_DST_COLOR = 0x0307;
		public const uint SRC_ALPHA_SATURATE = 0x0308;
		public const uint BLEND_EQUATION_RGB = 0x8009;
		public const uint BLEND_EQUATION_ALPHA = 0x883D;

		public const uint FUNC_ADD = 0x8006;
		public const uint FUNC_SUBTRACT = 0x800A;
		public const uint FUNC_REVERSE_SUBTRACT = 0x800B;

		public const uint BLEND_DST_RGB = 0x80C8;
		public const uint BLEND_SRC_RGB = 0x80C9;
		public const uint BLEND_DST_ALPHA = 0x80CA;
		public const uint BLEND_SRC_ALPHA = 0x80CB;

		public const uint CONSTANT_COLOR = 0x8001;
		public const uint ONE_MINUS_CONSTANT_COLOR = 0x8002;
		public const uint CONSTANT_ALPHA = 0x8003;
		public const uint ONE_MINUS_CONSTANT_ALPHA = 0x8004;
		public const uint BLEND_COLOR = 0x8005;

		public const uint DEPTH_TEST = 0x0b71;
		public const uint DEPTH_WRITEMASK = 0x0b72;
		public const uint STENCIL_TEST = 0x0B90;

		public const uint COLOR = 0x1800;
		public const uint DEPTH = 0x1801;
		public const uint STENCIL = 0x1802;
		public const uint COLOR_BUFFER_BIT = 0x00004000;
		public const uint DEPTH_BUFFER_BIT = 0x00000100;
		public const uint STENCIL_BUFFER_BIT = 0x00000400;

		public const uint RED = 0x1903;
		public const uint GREEN = 0x1904;
		public const uint BLUE = 0x1905;
		public const uint ALPHA = 0x1906;
		public const uint RGB = 0x1907;
		public const uint RGBA = 0x1908;
		public const int RGBA8 = 0x8058;
		public const int RGB10_A2 = 0x8059;
		public const int RGBA16F = 0x881A;
		public const int RGBA32F = 0x8814;

		public const uint KEEP = 0x1e00;
		public const uint REPLACE = 0x1e01;
		public const uint INCR = 0x1e02;
		public const uint DECR = 0x1e03;

		public const uint NEVER = 0x0200;
		public const uint LESS = 0x0201;
		public const uint EQUAL = 0x0202;
		public const uint LEQUAL = 0x0203;
		public const uint GREATER = 0x0204;
		public const uint NOTEQUAL = 0x0205;
		public const uint GEQUAL = 0x0206;
		public const uint ALWAYS = 0x0207;

		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glDepthMask", ExactSpelling = true)]
		public static extern void DepthMask(GLboolean flag);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glDepthFunc", ExactSpelling = true)]
		public static extern void DepthFunc(GLenum func);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glStencilFunc", ExactSpelling = true)]
		public static extern void StencilFunc(GLenum func, GLint refx, GLuint mask);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glStencilOp", ExactSpelling = true)]
		public static extern void StencilOp(GLenum fail, GLenum zfail, GLenum zpass);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glBindTexture", ExactSpelling = true)]
		public static extern void BindTexture(GLenum target, GLuint texture);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glGenTextures", ExactSpelling = true)]
		public unsafe static extern void GenTextures(GLsizei n, [OutAttribute] GLuint* textures);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glTexImage2D", ExactSpelling = true)]
		public unsafe static extern void TexImage2D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, void* data);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glDeleteTextures", ExactSpelling = true)]
		public unsafe static extern void DeleteTextures(GLsizei n, GLuint* textures);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glClearColor", ExactSpelling = true)]
		public static extern void ClearColor(GLclampf red, GLclampf green, GLclampf blue, GLclampf alpha);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glClear", ExactSpelling = true)]
		public static extern void Clear(GLbitfield mask);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glColorMask", ExactSpelling = true)]
		public static extern void ColorMask(GLboolean red, GLboolean green, GLboolean blue, GLboolean alpha);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glBlendFunc", ExactSpelling = true)]
		public static extern void BlendFunc(GLenum sfactor, GLenum dfactor);

		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glBlendEquation", ExactSpelling = true)]
			public unsafe static extern void BlendEquation(GLenum mode);
		#else
			public unsafe delegate void BlendEquationFunc(GLenum mode);
			public static BlendEquationFunc BlendEquation;
		#endif

		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glBlendEquationSeparate", ExactSpelling = true)]
			public unsafe static extern void BlendEquationSeparate(GLenum modeRGB, GLenum modeAlpha);
		#else
			public unsafe delegate void BlendEquationSeparateFunc(GLenum modeRGB, GLenum modeAlpha);
			public static BlendEquationSeparateFunc BlendEquationSeparate;
		#endif

		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glBlendFuncSeparate", ExactSpelling = true)]
			public unsafe static extern void BlendFuncSeparate(GLenum srcRGB, GLenum dstRGB, GLenum srcAlpha, GLenum dstAlpha);
		#else
			public unsafe delegate void BlendFuncSeparateFunc(GLenum srcRGB, GLenum dstRGB, GLenum srcAlpha, GLenum dstAlpha);
			public static BlendFuncSeparateFunc BlendFuncSeparate;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glCompressedTexImage2D", ExactSpelling = true)]
			public unsafe static extern void CompressedTexImage2D(GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLint border, GLsizei imageSize, void* data);
		#else
			public unsafe delegate void CompressedTexImage2DFunc(GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLint border, GLsizei imageSize, void* data);
			public static CompressedTexImage2DFunc CompressedTexImage2D;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glGenRenderbuffers", ExactSpelling = true)]
			public unsafe static extern void GenRenderbuffers(GLsizei n, [OutAttribute] GLuint* renderbuffers);
		#else
			public unsafe delegate void GenRenderbuffersFunc(GLsizei n, [OutAttribute] GLuint* renderbuffers);
			public static GenRenderbuffersFunc GenRenderbuffers;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glBindRenderbuffer", ExactSpelling = true)]
			public static extern void BindRenderbuffer(GLenum target, GLuint renderbuffer);
		#else
			public delegate void BindRenderbufferFunc(GLenum target, GLuint renderbuffer);
			public static BindRenderbufferFunc BindRenderbuffer;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glGenFramebuffers", ExactSpelling = true)]
			public unsafe static extern void GenFramebuffers(GLsizei n, [OutAttribute] GLuint* framebuffers);
		#else
			public unsafe delegate void GenFramebuffersFunc(GLsizei n, [OutAttribute] GLuint* framebuffers);
			public static GenFramebuffersFunc GenFramebuffers;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glBindFramebuffer", ExactSpelling = true)]
			public static extern void BindFramebuffer(GLenum target, GLuint renderbuffer);
		#else
			public delegate void BindFramebufferFunc(GLenum target, GLuint renderbuffer);
			public static BindFramebufferFunc BindFramebuffer;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glCheckFramebufferStatus", ExactSpelling = true)]
			public static extern GLenum CheckFramebufferStatus(GLenum target);
		#else
			public delegate GLenum CheckFramebufferStatusFunc(GLenum target);
			public static CheckFramebufferStatusFunc CheckFramebufferStatus;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glDeleteFramebuffers", ExactSpelling = true)]
			public unsafe static extern void DeleteFramebuffers(GLsizei n, GLuint* framebuffers);
		#else
			public unsafe delegate void DeleteFramebuffersFunc(GLsizei n, GLuint* framebuffers);
			public static DeleteFramebuffersFunc DeleteFramebuffers;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glRenderbufferStorage", ExactSpelling = true)]
			public static extern void RenderbufferStorage(GLenum target, GLenum internalformat, GLsizei width, GLsizei height);
		#else
			public delegate void RenderbufferStorageFunc(GLenum target, GLenum internalformat, GLsizei width, GLsizei height);
			public static RenderbufferStorageFunc RenderbufferStorage;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glFramebufferRenderbuffer", ExactSpelling = true)]
			public static extern void FramebufferRenderbuffer(GLenum target, GLenum attachment, GLenum renderbuffertarget, GLuint renderbuffer);
		#else
			public delegate void FramebufferRenderbufferFunc(GLenum target, GLenum attachment, GLenum renderbuffertarget, GLuint renderbuffer);
			public static FramebufferRenderbufferFunc FramebufferRenderbuffer;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glDeleteRenderbuffers", ExactSpelling = true)]
			public unsafe static extern void DeleteRenderbuffers(GLsizei n, GLuint* renderbuffers);
		#else
			public unsafe delegate void DeleteRenderbuffersFunc(GLsizei n, GLuint* renderbuffers);
			public static DeleteRenderbuffersFunc DeleteRenderbuffers;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glFramebufferTexture2D", ExactSpelling = true)]
			public static extern void FramebufferTexture2D(GLenum target, GLenum attachment, GLenum textarget, GLuint texture, GLint level);
		#else
			public delegate void FramebufferTexture2DFunc(GLenum target, GLenum attachment, GLenum textarget, GLuint texture, GLint level);
			public static FramebufferTexture2DFunc FramebufferTexture2D;
		#endif
		
		[SuppressUnmanagedCodeSecurity]
		#if IS_NOT_EXT
			[DllImport(DLL, EntryPoint = "glActiveTexture", ExactSpelling = true)]
			public static extern void ActiveTexture(GLenum texture);
		#else
			public delegate void ActiveTextureFunc(GLenum texture);
			public static ActiveTextureFunc ActiveTexture;
		#endif
		#endregion

		#region General
		public const uint NO_ERROR = 0;
		public const uint INVALID_ENUM = 0x0500;
		public const uint INVALID_VALUE = 0x0501;
		public const uint INVALID_OPERATION = 0x0502;
		public const uint OUT_OF_MEMORY = 0x0505;
		public const uint STACK_OVERFLOW = 0x0503;
		public const uint STACK_UNDERFLOW = 0x0504;
		public const uint TABLE_TOO_LARGE = 0x8031;

		public const uint EXTENSIONS = 0x1F03;
		public const uint MAX_TEXTURE_SIZE = 0x0D33;
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glGetString", ExactSpelling = true)]
		public unsafe static extern GLubyte* GetString(GLenum name);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glGetIntegerv", ExactSpelling = true)]
		public unsafe static extern void GetIntegerv(GLenum pname, [OutAttribute] GLint* @params);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glGetError", ExactSpelling = true)]
		public static extern GLenum GetError();
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glEnable", ExactSpelling = true)]
		public static extern void Enable(GLenum cap);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glDisable", ExactSpelling = true)]
		public static extern void Disable(GLenum cap);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glViewport", ExactSpelling = true)]
		public static extern void Viewport(GLint x, GLint y, GLsizei width, GLsizei height);
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glFinish", ExactSpelling = true)]
		public static extern void Finish();
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DLL, EntryPoint = "glFlush", ExactSpelling = true)]
		public static extern void Flush();
		#endregion
	}
}