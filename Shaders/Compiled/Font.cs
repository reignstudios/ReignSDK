
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;
using A = Reign.Video.API;

namespace ShaderMaterials.Shaders
{
	class FontMaterialStreamLoader : StreamLoaderI
	{
		private A.VideoTypes videoType;
		private DisposableI parent;
		private string contentPath;
		private string tag;
		private ShaderVersions shaderVersion;

		public FontMaterialStreamLoader(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			this.videoType = videoType;
			this.parent = parent;
			this.contentPath = contentPath;
			this.tag = tag;
			this.shaderVersion = shaderVersion;
		}

		public override bool Load()
		{
			if (!FontMaterial.load(videoType, parent, contentPath, tag, shaderVersion)) return false;
			return true;
		}
	}

	public class FontMaterial : MaterialI
	{
		// static properties
		public static bool Loaded {get; private set;}
		public static ShaderI Shader {get; private set;}
		public static BufferLayoutDescI BufferLayoutDesc {get; private set;}
		public static BufferLayoutI BufferLayout {get; private set;}
		public static ShaderVariableI CameraConstant {get; private set;}public static ShaderVariableI LocationConstant {get; private set;}public static ShaderVariableI SizeConstant {get; private set;}public static ShaderVariableI LocationUVConstant {get; private set;}public static ShaderVariableI SizeUVConstant {get; private set;}public static ShaderVariableI TexelOffsetConstant {get; private set;}public static ShaderVariableI ColorConstant {get; private set;}public static ShaderResourceI DiffuseTextureConstant {get; private set;}

		// instance properties
		public delegate void ApplyCallbackMethod(FontMaterial material, MeshI mesh);
		public static ApplyCallbackMethod ApplyGlobalConstantsCallback, ApplyInstanceConstantsCallback, ApplyInstancingConstantsCallback;
		[MaterialField(MaterialFieldTypes.None, MaterialFieldUsages.Global)] public static Matrix4 Camera;[MaterialField(MaterialFieldTypes.None, MaterialFieldUsages.Instance)] public Vector3 Location;[MaterialField(MaterialFieldTypes.None, MaterialFieldUsages.Instance)] public Vector2 Size;[MaterialField(MaterialFieldTypes.None, MaterialFieldUsages.Instance)] public Vector2 LocationUV;[MaterialField(MaterialFieldTypes.None, MaterialFieldUsages.Instance)] public Vector2 SizeUV;[MaterialField(MaterialFieldTypes.None, MaterialFieldUsages.Instance)] public Vector2 TexelOffset;[MaterialField(MaterialFieldTypes.Diffuse, MaterialFieldUsages.Instance)] public Vector4 Color;[MaterialField(MaterialFieldTypes.Diffuse, MaterialFieldUsages.Instance)] public Texture2DI DiffuseTexture;

		// constructors
		public static void Init(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			new FontMaterialStreamLoader(videoType, parent, contentPath, tag, shaderVersion);
			var elements = new List<BufferLayoutElement>();
			elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.Position, 0, 0, 0));
			BufferLayoutDesc = A.BufferLayoutDesc.Create(videoType, elements);
		}

		internal static bool load(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			if (Shader == null)
			{
				Shader = A.Shader.Create(videoType, parent, contentPath + tag + "Font.rs", shaderVersion);
				return false;
			}
			if (!Shader.Loaded)
			{
				return false;
			}
			CameraConstant = Shader.Variable("Camera");LocationConstant = Shader.Variable("Location");SizeConstant = Shader.Variable("Size");LocationUVConstant = Shader.Variable("LocationUV");SizeUVConstant = Shader.Variable("SizeUV");TexelOffsetConstant = Shader.Variable("TexelOffset");ColorConstant = Shader.Variable("Color");DiffuseTextureConstant = Shader.Resource("DiffuseTexture");

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
			LocationConstant.Set(Location);SizeConstant.Set(Size);LocationUVConstant.Set(LocationUV);SizeUVConstant.Set(SizeUV);TexelOffsetConstant.Set(TexelOffset);ColorConstant.Set(Color);DiffuseTextureConstant.Set(DiffuseTexture);
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
			LocationConstant.Set(Location);SizeConstant.Set(Size);LocationUVConstant.Set(LocationUV);SizeUVConstant.Set(SizeUV);TexelOffsetConstant.Set(TexelOffset);ColorConstant.Set(Color);DiffuseTextureConstant.Set(DiffuseTexture);
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
