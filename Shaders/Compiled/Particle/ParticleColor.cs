
using System;
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;

namespace ShaderMaterials.Shaders
{
	public class ParticleColorMaterial : MaterialI
	{
		#region Static Properties
		public static bool Loaded {get; private set;}
		public static bool FailedToLoad {get; private set;}
		
		public static ShaderI Shader {get; private set;}
		public static BufferLayoutDescI BufferLayoutDesc {get; private set;}
		public static BufferLayoutI BufferLayout {get; private set;}
		public static ShaderVariableI CameraConstant {get; private set;}public static ShaderVariableI BillboardTransformConstant {get; private set;}public static ShaderResourceI DiffuseConstant {get; private set;}public static ShaderVariableI ColorPalletConstant {get; private set;}public static ShaderVariableI ScalePalletConstant {get; private set;}public static ShaderVariableI TransformsConstant {get; private set;}
		#endregion

		#region Instance Properties
		public string Name {get; set;}
		public delegate void ApplyCallbackMethod(ParticleColorMaterial material, Mesh mesh);
		public static ApplyCallbackMethod ApplyGlobalConstantsCallback, ApplyInstanceConstantsCallback, ApplyInstancingConstantsCallback;
		[MaterialField(MaterialFieldUsages.Global)] public static Matrix4 Camera;[MaterialField(MaterialFieldUsages.Global)] public static Matrix4 BillboardTransform;[MaterialField(MaterialFieldUsages.Global)] public static Texture2DI Diffuse;private static WeakReference colorpallet; [MaterialField(MaterialFieldUsages.Global)] public static Vector4[] ColorPallet { get{return (Vector4[])colorpallet.Target;} set{colorpallet = new WeakReference(value);} }[MaterialField(MaterialFieldUsages.Global)] public static Vector4 ScalePallet;private static WeakReference transforms; [MaterialField(MaterialFieldUsages.Instancing)] public static Vector4[] Transforms { get{return (Vector4[])transforms.Target;} set{transforms = new WeakReference(value);} }
		#endregion

		#region Constructors
		public static void Init(DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			Shader = ShaderAPI.New(parent, contentPath + tag + "ParticleColor.rs", shaderVersion,
			delegate(object sender)
			{
				init((ShaderI)sender, loadedCallback, failedToLoadCallback);
			},
			delegate
			{
				FailedToLoad = true;
				if (failedToLoadCallback != null) failedToLoadCallback();
			});
		}

		public static void Init(DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			Shader = ShaderAPI.New(parent, contentPath + tag + "ParticleColor.rs", shaderVersion, vsQuality, psQuality,
			delegate(object sender)
			{
				init((ShaderI)sender, loadedCallback, failedToLoadCallback);
			},
			delegate
			{
				FailedToLoad = true;
				if (failedToLoadCallback != null) failedToLoadCallback();
			});
		}
		
		private static void init(ShaderI shader, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			try
			{
				CameraConstant = shader.Variable("Camera");BillboardTransformConstant = shader.Variable("BillboardTransform");DiffuseConstant = shader.Resource("Diffuse");ColorPalletConstant = shader.Variable("ColorPallet");ScalePalletConstant = shader.Variable("ScalePallet");TransformsConstant = shader.Variable("Transforms");
				var elements = new List<BufferLayoutElement>();
				elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 3));
				BufferLayoutDesc = BufferLayoutDescAPI.New(elements);
				BufferLayout = BufferLayoutAPI.New(shader, shader, BufferLayoutDesc);
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (failedToLoadCallback != null) failedToLoadCallback();
				return;
			}
			
			Loaded = true;
			if (loadedCallback != null) loadedCallback(null);
		}

		public static void Dispose()
		{
			if (BufferLayout != null) BufferLayout.Dispose();
			if (Shader != null) Shader.Dispose();
		}
		#endregion

		#region Methods
		public void Enable()
		{
			BufferLayout.Enable();
		}

		public void ApplyGlobalContants(Mesh mesh)
		{
			if (ApplyGlobalConstantsCallback != null) ApplyGlobalConstantsCallback(this, mesh);
			CameraConstant.Set(Camera);BillboardTransformConstant.Set(BillboardTransform);DiffuseConstant.Set(Diffuse);ColorPalletConstant.Set(ColorPallet);ScalePalletConstant.Set(ScalePallet);
		}

		public void ApplyInstanceContants(Mesh mesh)
		{
			if (ApplyInstanceConstantsCallback != null) ApplyInstanceConstantsCallback(this, mesh);
			
		}

		public void ApplyInstancingContants(Mesh mesh)
		{
			if (ApplyInstancingConstantsCallback != null) ApplyInstancingConstantsCallback(this, mesh);
			TransformsConstant.Set(Transforms);
		}

		public void Apply(Mesh mesh)
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
