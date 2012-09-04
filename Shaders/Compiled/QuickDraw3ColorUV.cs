
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;
using A = Reign.Video.API;

namespace ShaderMaterials.Shaders
{
	class QuickDraw3ColorUVMaterialStreamLoader : StreamLoaderI
	{
		private A.VideoTypes videoType;
		private DisposableI parent;
		private string contentPath;
		private string tag;
		private ShaderVersions shaderVersion;

		public QuickDraw3ColorUVMaterialStreamLoader(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			this.videoType = videoType;
			this.parent = parent;
			this.contentPath = contentPath;
			this.tag = tag;
			this.shaderVersion = shaderVersion;
		}

		public override bool Load()
		{
			if (!QuickDraw3ColorUVMaterial.load(videoType, parent, contentPath, tag, shaderVersion)) return false;
			return true;
		}
	}

	public class QuickDraw3ColorUVMaterial : MaterialI
	{
		// static properties
		public static bool Loaded {get; private set;}
		public static ShaderI Shader {get; private set;}
		public static BufferLayoutDescI BufferLayoutDesc {get; private set;}
		public static BufferLayoutI BufferLayout {get; private set;}
		public static ShaderVariableI CameraConstant {get; private set;}public static ShaderResourceI DiffuseConstant {get; private set;}

		// instance properties
		public delegate void ApplyCallbackMethod(QuickDraw3ColorUVMaterial material, MeshI mesh);
		public static ApplyCallbackMethod ApplyGlobalConstantsCallback, ApplyInstanceConstantsCallback, ApplyInstancingConstantsCallback;
		[MaterialField(MaterialFieldTypes.None, MaterialFieldUsages.Global)] public static Matrix4 Camera;[MaterialField(MaterialFieldTypes.None, MaterialFieldUsages.Instance)] public Texture2DI Diffuse;

		// constructors
		public static void Init(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			new QuickDraw3ColorUVMaterialStreamLoader(videoType, parent, contentPath, tag, shaderVersion);
			var elements = new List<BufferLayoutElement>();
			elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.RGBAx8, BufferLayoutElementUsages.Color, 0, 0, 3));elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 4));
			BufferLayoutDesc = A.BufferLayoutDesc.Create(videoType, elements);
		}

		internal static bool load(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			if (Shader == null)
			{
				Shader = A.Shader.Create(videoType, parent, contentPath + tag + "QuickDraw3ColorUV.rs", shaderVersion);
				return false;
			}
			if (!Shader.Loaded)
			{
				return false;
			}
			CameraConstant = Shader.Variable("Camera");DiffuseConstant = Shader.Resource("Diffuse");

			BufferLayout = A.BufferLayout.Create(videoType, Shader, Shader, BufferLayoutDesc);

			Loaded = true;
			return true;
		}

		// methods
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
			DiffuseConstant.Set(Diffuse);
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

		public void ApplyGlobalContants()
		{
			CameraConstant.Set(Camera);
		}

		public void ApplyInstanceContants()
		{
			DiffuseConstant.Set(Diffuse);
		}

		public void ApplyInstancingContants()
		{
			
		}

		public void Apply()
		{
			ApplyGlobalContants();
			ApplyInstanceContants();
			ApplyInstancingContants();
			Shader.Apply();
		}
	}
}
