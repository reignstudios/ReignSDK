
using System;
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;
using A = Reign.Video.API;

namespace ShaderMaterials.Shaders
{
	class ParticleColorMaterialStreamLoader : StreamLoaderI
	{
		private A.VideoTypes videoType;
		private DisposableI parent;
		private string contentPath;
		private string tag;
		private ShaderVersions shaderVersion;

		public ParticleColorMaterialStreamLoader(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
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
			return ParticleColorMaterial.load(videoType, parent, contentPath, tag, shaderVersion);
		}
	}

	public class ParticleColorMaterial : MaterialI
	{
		#region Static Properties
		public static bool Loaded {get; private set;}
		public static ShaderI Shader {get; private set;}
		public static BufferLayoutDescI BufferLayoutDesc {get; private set;}
		public static BufferLayoutI BufferLayout {get; private set;}
		public static ShaderVariableI CameraConstant {get; private set;}public static ShaderVariableI BillboardTransformConstant {get; private set;}public static ShaderResourceI DiffuseConstant {get; private set;}public static ShaderVariableI ColorPalletConstant {get; private set;}public static ShaderVariableI ScalePalletConstant {get; private set;}public static ShaderVariableI TransformsConstant {get; private set;}
		#endregion

		#region Instance Properties
		public string Name {get; set;}
		public delegate void ApplyCallbackMethod(ParticleColorMaterial material, MeshI mesh);
		public static ApplyCallbackMethod ApplyGlobalConstantsCallback, ApplyInstanceConstantsCallback, ApplyInstancingConstantsCallback;
		[MaterialField(MaterialFieldUsages.Global)] public static Matrix4 Camera;[MaterialField(MaterialFieldUsages.Global)] public static Matrix4 BillboardTransform;[MaterialField(MaterialFieldUsages.Global)] public static Texture2DI Diffuse;private static WeakReference colorpallet; [MaterialField(MaterialFieldUsages.Global)] public static Vector4[] ColorPallet { get{return (Vector4[])colorpallet.Target;} set{colorpallet = new WeakReference(value);} }[MaterialField(MaterialFieldUsages.Global)] public static Vector4 ScalePallet;private static WeakReference transforms; [MaterialField(MaterialFieldUsages.Instancing)] public static Vector4[] Transforms { get{return (Vector4[])transforms.Target;} set{transforms = new WeakReference(value);} }
		#endregion

		#region Constructors
		public static void Init(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			Shader = A.Shader.Create(videoType, parent, contentPath + tag + "ParticleColor.rs", shaderVersion);
			new ParticleColorMaterialStreamLoader(videoType, parent, contentPath, tag, shaderVersion);
			var elements = new List<BufferLayoutElement>();
			elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 3));
			BufferLayoutDesc = A.BufferLayoutDesc.Create(videoType, elements);
		}

		public static void Init(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality)
		{
			Shader = A.Shader.Create(videoType, parent, contentPath + tag + "ParticleColor.rs", shaderVersion, vsQuality, psQuality);
			new ParticleColorMaterialStreamLoader(videoType, parent, contentPath, tag, shaderVersion);
			var elements = new List<BufferLayoutElement>();
			elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 3));
			BufferLayoutDesc = A.BufferLayoutDesc.Create(videoType, elements);
		}

		internal static bool load(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{
			if (!Shader.Loaded)
			{
				return false;
			}
			CameraConstant = Shader.Variable("Camera");BillboardTransformConstant = Shader.Variable("BillboardTransform");DiffuseConstant = Shader.Resource("Diffuse");ColorPalletConstant = Shader.Variable("ColorPallet");ScalePalletConstant = Shader.Variable("ScalePallet");TransformsConstant = Shader.Variable("Transforms");

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
			CameraConstant.Set(Camera);BillboardTransformConstant.Set(BillboardTransform);DiffuseConstant.Set(Diffuse);ColorPalletConstant.Set(ColorPallet);ScalePalletConstant.Set(ScalePallet);
		}

		public void ApplyInstanceContants(MeshI mesh)
		{
			if (ApplyInstanceConstantsCallback != null) ApplyInstanceConstantsCallback(this, mesh);
			
		}

		public void ApplyInstancingContants(MeshI mesh)
		{
			if (ApplyInstancingConstantsCallback != null) ApplyInstancingConstantsCallback(this, mesh);
			TransformsConstant.Set(Transforms);
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
			CameraConstant.Set(Camera);BillboardTransformConstant.Set(BillboardTransform);DiffuseConstant.Set(Diffuse);ColorPalletConstant.Set(ColorPallet);ScalePalletConstant.Set(ScalePallet);
		}

		public void ApplyInstanceContants()
		{
			
		}

		public static void ApplyInstancingContants()
		{
			TransformsConstant.Set(Transforms);
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
