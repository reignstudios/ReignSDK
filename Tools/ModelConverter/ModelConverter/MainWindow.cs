using System;
using Reign.Core;
using Reign.Video;
using Reign.Video.API;
using Reign.Input;
using Reign.Input.API;
using System.Collections.Generic;
using ShaderMaterials.Shaders;

namespace ModelConverter
{
	public class MainWindow : Window
	{
		ToolWindow toolWindow;
		bool loaded;
		RootDisposable root;
		VideoTypes videoType;
		VideoI video;
		RasterizerStateI rasterizerState;
		DepthStencilStateI depthStencilState;
		BlendStateI blendState;
		SamplerStateI samplerState;
		Camera camera;
		ViewPortI viewPort;

		bool loadingSoftwareModelDone = true;
		SoftwareModel softwareModel;
		ModelI model;

		InputI input;
		MouseI mouse;

		public MainWindow()
		: base("Model Converter", 512, 512, WindowStartPositions.CenterCurrentScreen, WindowTypes.FrameSizable)
		{
			
		}

		protected override void shown()
		{
			try
			{
				root = new RootDisposable();
				video = Video.Create(VideoTypes.D3D11 | VideoTypes.D3D9 | VideoTypes.OpenGL, out videoType, root, this, true);
				rasterizerState = RasterizerState.Create(videoType, video, RasterizerStateDesc.Create(videoType, RasterizerStateTypes.Solid_CullNone));
				depthStencilState = DepthStencilState.Create(videoType, video, DepthStencilStateDesc.Create(videoType, DepthStencilStateTypes.ReadWrite_Less));
				blendState = BlendState.Create(videoType, video, BlendStateDesc.Create(videoType, BlendStateTypes.None));
				samplerState = SamplerState.Create(videoType, video, SamplerStateDesc.Create(videoType, SamplerStateTypes.Linear_Wrap));

				var frame = FrameSize;
				viewPort = ViewPort.Create(videoType, video, 0, 0, frame.Width, frame.Height);
				camera = new Camera(viewPort, new Vector3(10, 10, 10), new Vector3(), new Vector3(10, 11, 10));

				DiffuseTextureMaterial.Init(videoType, video, "Materials\\", video.FileTag, ShaderVersions.Max);
				DiffuseTextureMaterial.ApplyGlobalConstantsCallback = diffuseTextureGlobalApply;
				DiffuseTextureMaterial.ApplyInstanceConstantsCallback = diffuseTextureInstanceApply;

				InputTypes inputType;
				input = Input.Create(InputTypes.WinForms, out inputType, root, this);
				mouse = Mouse.Create(inputType, input);

				toolWindow = new ToolWindow(this);
				toolWindow.Show(this);
				loaded = true;
			}
			catch (Exception e)
			{
				Message.Show("Error", e.Message);
				dispose();
			}
		}

		private void dispose()
		{
			loaded = false;
			if (root != null)
			{
				root.Dispose();
				root = null;
			}
		}

		protected override void closing()
		{
			dispose();
		}

		public new void Load(string fileName)
		{
			loadingSoftwareModelDone = false;
			softwareModel = new SoftwareModel(fileName);
		}

		public void Convert(string contentPath, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> materialFieldTypes)
		{
			if (model != null) model.Dispose();
			model = Model.Create(videoType, video, softwareModel, MeshVertexSizes.Float3, false, true, true, contentPath, materialTypes, null, null, null, null, materialFieldTypes, null);
		}

		public void Save(string fileName)
		{
			if (softwareModel == null) return;
			ModelI.Save(fileName, false, softwareModel, MeshVertexSizes.Float3, false, true, true);
		}

		protected override void update(Time time)
		{
			if (!loaded) return;

			input.Update();
			if (mouse.Left.On)
			{
				camera.RotateAroundLookLocation(mouse.Velocity.Y * .05f, mouse.Velocity.X * .05f, 0);
			}
			else
			{
				camera.RotateAroundLookLocationWorld(0, .1f * time.Delta, 0);
			}

			if (mouse.Right.On)
			{
				camera.Zoom(mouse.Velocity.Y * .05f, 1);
			}
		}

		void diffuseTextureGlobalApply(DiffuseTextureMaterial material, MeshI mesh)
		{
			DiffuseTextureMaterial.Camera = camera.TransformMatrix;
			DiffuseTextureMaterial.LightDirection = -camera.Location.Normalize();
		}

		void diffuseTextureInstanceApply(DiffuseTextureMaterial material, MeshI mesh)
		{
			material.Transform = new Matrix4(Matrix3.FromEuler(mesh.Rotation), mesh.Scale, mesh.Location);
		}

		protected override void render(Time time)
		{
			if (!loaded) return;

			video.Update();
			var e = Streams.TryLoad();
			if (e != null)
			{
				Message.Show("Error", e.Message);
				dispose();

				model = null;
				loadingSoftwareModelDone = true;
				Close();
				return;
			}
			if (Streams.ItemsRemainingToLoad != 0) return;

			if (!loadingSoftwareModelDone)
			{
				var materialList = new List<Type>()
				{
					typeof(DiffuseTextureMaterial)
				};
				toolWindow.LoadSoftwareModelData(softwareModel, materialList);
				loadingSoftwareModelDone = true;
			}
			
			video.EnableRenderTarget();
			viewPort.Size = FrameSize;
			viewPort.Apply();
			camera.Apply();
			video.Clear(0, .3f, .3f, 1);
			rasterizerState.Enable();
			depthStencilState.Enable();
			blendState.Enable();
			samplerState.Enable(0);
			if (model != null) model.Render();
			video.Present();
		}
	}
}
