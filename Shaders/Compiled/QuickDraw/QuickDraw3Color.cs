
using System;
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;
using A = Reign.Video.API;

namespace ShaderMaterials.Shaders
{
	class QuickDraw3ColorMaterialStreamLoader : StreamLoaderI
	{
		private A.VideoTypes videoType;
		private DisposableI parent;
		private string contentPath;
		private string tag;
		private ShaderVersions shaderVersion;

		public QuickDraw3ColorMaterialStreamLoader(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			this.videoType = videoType;
			this.parent = parent;
			this.contentPath = contentPath;
			this.tag = tag;
			this.shaderVersion = shaderVersion;
		}

		#if METRO
		public override async System.Threading.Tasks.Task<bool> Load() {
		#else
		public override bool Load() {
		#endif
			return QuickDraw3ColorMaterial.load(videoType, parent, contentPath, tag, shaderVersion);
		}
	}

	public class QuickDraw3ColorMaterial : MaterialI
	{
		#region Static Properties
		public static bool Loaded {get; private set;}
		public static ShaderI Shader {get; private set;}
		public static BufferLayoutDescI BufferLayoutDesc {get; private set;}
		public static BufferLayoutI BufferLayout {get; private set;}
		public static ShaderVariableI CameraConstant {get; private set;}
		#endregion

		#region Instance Properties
		public string Name {get; set;}
		public delegate void ApplyCallbackMethod(QuickDraw3ColorMaterial material, MeshI mesh);
		public static ApplyCallbackMethod ApplyGlobalConstantsCallback, ApplyInstanceConstantsCallback, ApplyInstancingConstantsCallback;
		[MaterialField(MaterialFieldUsages.Global)] public static Matrix4 Camera;
		#endregion

		#region Constructors
		public static void Init(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			Shader = A.Shader.Create(videoType, parent, contentPath + tag + "QuickDraw3Color.rs", shaderVersion);
			new QuickDraw3ColorMaterialStreamLoader(videoType, parent, contentPath, tag, shaderVersion);
			var elements = new List<BufferLayoutElement>();
			elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.RGBAx8, BufferLayoutElementUsages.Color, 0, 0, 3));
			BufferLayoutDesc = A.BufferLayoutDesc.Create(videoType, elements);
		}

		public static void Init(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality)
		{
			Shader = A.Shader.Create(videoType, parent, contentPath + tag + "QuickDraw3Color.rs", shaderVersion, vsQuality, psQuality);
			new QuickDraw3ColorMaterialStreamLoader(videoType, parent, contentPath, tag, shaderVersion);
			var elements = new List<BufferLayoutElement>();
			elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.RGBAx8, BufferLayoutElementUsages.Color, 0, 0, 3));
			BufferLayoutDesc = A.BufferLayoutDesc.Create(videoType, elements);
		}

		internal static bool load(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			if (!Shader.Loaded)
			{
				return false;
			}
			CameraConstant = Shader.Variable("Camera");

			BufferLayout = A.BufferLayout.Create(videoType, Shader, Shader, BufferLayoutDesc);

			Loaded = true;
			return true;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			BufferLayout.Enable();
		}

		public void ApplyGlobalContants(MeshI mesh)
		{
			if (ApplyGlobalConstantsCallback != null) ApplyGlobalConstantsCallback(this, mesh);
			CameraConstant.Set(Camera);
		}

		public void ApplyInstanceContants(MeshI mesh)
		{
			if (ApplyInstanceConstantsCallback != null) ApplyInstanceConstantsCallback(this, mesh);
			
		}

		public void ApplyInstancingContants(MeshI mesh)
		{
			if (ApplyInstancingConstantsCallback != null) ApplyInstancingConstantsCallback(this, mesh);
			
		}

		public void Apply(MeshI mesh)
		{
			ApplyGlobalContants(mesh);
			ApplyInstanceContants(mesh);
			ApplyInstancingContants(mesh);
			Shader.Apply();
		}

		public static void ApplyGlobalContants()
		{
			CameraConstant.Set(Camera);
		}

		public void ApplyInstanceContants()
		{
			
		}

		public static void ApplyInstancingContants()
		{
			
		}

		public void Apply()
		{
			ApplyGlobalContants();
			ApplyInstanceContants();
			ApplyInstancingContants();
			Shader.Apply();
		}
		#endregion
	}
}
