
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;
using A = Reign.Video.API;

namespace ShaderMaterials.Shaders
{
	class DiffuseTextureMaterialStreamLoader : StreamLoaderI
	{
		private A.VideoTypes videoType;
		private DisposableI parent;
		private string contentPath;
		private string tag;
		private ShaderVersions shaderVersion;

		public DiffuseTextureMaterialStreamLoader(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			this.videoType = videoType;
			this.parent = parent;
			this.contentPath = contentPath;
			this.tag = tag;
			this.shaderVersion = shaderVersion;
		}

		public override bool Load()
		{
			if (!DiffuseTextureMaterial.load(videoType, parent, contentPath, tag, shaderVersion)) return false;
			return true;
		}
	}

	public class DiffuseTextureMaterial : MaterialI
	{
		#region Static Properties
		public static bool Loaded {get; private set;}
		public static ShaderI Shader {get; private set;}
		public static BufferLayoutDescI BufferLayoutDesc {get; private set;}
		public static BufferLayoutI BufferLayout {get; private set;}
		public static ShaderVariableI CameraConstant {get; private set;}public static ShaderVariableI LightDirectionConstant {get; private set;}public static ShaderVariableI TransformConstant {get; private set;}public static ShaderResourceI DiffuseConstant {get; private set;}
		#endregion

		#region Instance Properties
		public string Name {get; set;}
		public delegate void ApplyCallbackMethod(DiffuseTextureMaterial material, MeshI mesh);
		public static ApplyCallbackMethod ApplyGlobalConstantsCallback, ApplyInstanceConstantsCallback, ApplyInstancingConstantsCallback;
		[MaterialField(MaterialFieldUsages.Global)] public static Matrix4 Camera;[MaterialField(MaterialFieldUsages.Global)] public static Vector3 LightDirection;[MaterialField(MaterialFieldUsages.Instance)] public Matrix4 Transform;[MaterialField(MaterialFieldUsages.Instance)] public Texture2DI Diffuse;
		#endregion

		#region Constructors
		public static void Init(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			Shader = A.Shader.Create(videoType, parent, contentPath + tag + "DiffuseTexture.rs", shaderVersion);
			new DiffuseTextureMaterialStreamLoader(videoType, parent, contentPath, tag, shaderVersion);
			var elements = new List<BufferLayoutElement>();
			elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Normal, 0, 0, 3));elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 6));
			BufferLayoutDesc = A.BufferLayoutDesc.Create(videoType, elements);
		}

		internal static bool load(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			if (!Shader.Loaded)
			{
				return false;
			}
			CameraConstant = Shader.Variable("Camera");LightDirectionConstant = Shader.Variable("LightDirection");TransformConstant = Shader.Variable("Transform");DiffuseConstant = Shader.Resource("Diffuse");

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
			CameraConstant.Set(Camera);LightDirectionConstant.Set(LightDirection);
		}

		public void ApplyInstanceContants(MeshI mesh)
		{
			if (ApplyInstanceConstantsCallback != null) ApplyInstanceConstantsCallback(this, mesh);
			TransformConstant.Set(Transform);DiffuseConstant.Set(Diffuse);
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
			CameraConstant.Set(Camera);LightDirectionConstant.Set(LightDirection);
		}

		public void ApplyInstanceContants()
		{
			TransformConstant.Set(Transform);DiffuseConstant.Set(Diffuse);
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
		#endregion
	}
}
