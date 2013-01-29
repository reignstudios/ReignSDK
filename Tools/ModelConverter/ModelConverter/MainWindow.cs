using System;
using Reign.Core;
using Reign.Video;
using Reign.Video.API;
using Reign.Input;
using Reign.Input.API;
using System.Collections.Generic;
using ShaderMaterials.Shaders;
using Reign.Physics;

namespace ModelConverter
{
	public class MainWindow : WinFormApplication
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
		Model model;

		InputI input;
		MouseI mouse;

		public MainWindow()
		{
			var desc = new ApplicationDesc()
			{
				FrameSize = new Size2(512, 512),
				Name = "Model Converter"
			};
			Init(desc);
		}

		public override void Shown()
		{
			try
			{
				root = new RootDisposable();
				video = Video.Init(VideoTypes.D3D11 | VideoTypes.D3D9 | VideoTypes.OpenGL, out videoType, root, this, true);
				rasterizerState = RasterizerStateAPI.New(video, RasterizerStateDescAPI.New(RasterizerStateTypes.Solid_CullNone));
				depthStencilState = DepthStencilStateAPI.New(video, DepthStencilStateDescAPI.New(DepthStencilStateTypes.ReadWrite_Less));
				blendState = BlendStateAPI.New(video, BlendStateDescAPI.New(BlendStateTypes.None));
				samplerState = SamplerStateAPI.New(video, SamplerStateDescAPI.New(SamplerStateTypes.Linear_Wrap));

				var frame = FrameSize;
				viewPort = ViewPortAPI.New(video, 0, 0, frame.Width, frame.Height);
				camera = new Camera(viewPort, new Vector3(10, 10, 10), new Vector3(), new Vector3(10, 11, 10));

				DiffuseTextureMaterial.Init(video, "Materials\\", video.FileTag, ShaderVersions.Max, null);
				DiffuseTextureMaterial.ApplyGlobalConstantsCallback = diffuseTextureGlobalApply;
				DiffuseTextureMaterial.ApplyInstanceConstantsCallback = diffuseTextureInstanceApply;

				InputTypes inputType;
				input = Input.Init(InputTypes.WinForms, out inputType, root, this);
				mouse = MouseAPI.New(input);

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

		public override void Closing()
		{
			if (toolWindow != null)
			{
				toolWindow.Close();
				toolWindow = null;
			}
			dispose();
		}

		public new void Load(string fileName)
		{
			loadingSoftwareModelDone = false;
			softwareModel = new SoftwareModel(fileName, null);
		}

		public void Convert(string contentPath, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> materialFieldTypes)
		{
			if (model != null) model.Dispose();
			model = new Model(video, softwareModel, MeshVertexSizes.Float3, false, true, true, contentPath, materialTypes, null, null, null, null, materialFieldTypes, null, 0, null);
		}

		public void Save(string fileName, bool saveColors, bool saveUVs, bool saveNormals)
		{
			if (softwareModel == null) return;
			Model.Save(fileName, false, softwareModel, MeshVertexSizes.Float3, saveColors, saveUVs, saveNormals);
		}

		public void SaveTriangleMesh(string fileName)
		{
			if (softwareModel == null) return;
			TriangleMesh.Save(softwareModel.Meshes[0], fileName);
		}

		public override void Update(Time time)
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

		void diffuseTextureGlobalApply(DiffuseTextureMaterial material, Mesh mesh)
		{
			DiffuseTextureMaterial.Camera = camera.TransformMatrix;
			DiffuseTextureMaterial.LightDirection = -camera.Position.Normalize();
			DiffuseTextureMaterial.LightColor = new Vector4(1);
		}

		void diffuseTextureInstanceApply(DiffuseTextureMaterial material, Mesh mesh)
		{
			material.Transform = Matrix4.FromAffineTransform(Matrix3.FromEuler(mesh.Rotation), mesh.Scale, mesh.Position);
		}

		public override void Render(Time time)
		{
			if (!loaded) return;

			video.Update();
			var e = Loader.UpdateLoad();
			if (e != null)
			{
				Message.Show("Error", e.Message);
				dispose();

				model = null;
				loadingSoftwareModelDone = true;
				Close();
				return;
			}
			if (Loader.ItemsRemainingToLoad != 0) return;

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
			video.ClearColorDepth(0, .3f, .3f, 1);
			rasterizerState.Enable();
			depthStencilState.Enable();
			blendState.Enable();
			samplerState.Enable(0);
			if (model != null) model.Render();
			video.Present();
		}
	}
}
