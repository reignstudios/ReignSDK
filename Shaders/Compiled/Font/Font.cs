
using System;
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;

namespace ShaderMaterials.Shaders
{
	public class FontMaterial : MaterialI
	{
		#region Static Properties
		public static bool Loaded {get; private set;}
		public static bool FailedToLoad {get; private set;}
		
		public static ShaderI Shader {get; private set;}
		public static BufferLayoutDescI BufferLayoutDesc {get; private set;}
		public static BufferLayoutI BufferLayout {get; private set;}
		public static ShaderVariableI CameraConstant {get; private set;}public static ShaderVariableI LocationConstant {get; private set;}public static ShaderVariableI SizeConstant {get; private set;}public static ShaderVariableI LocationUVConstant {get; private set;}public static ShaderVariableI SizeUVConstant {get; private set;}public static ShaderVariableI TexelOffsetConstant {get; private set;}public static ShaderVariableI ColorConstant {get; private set;}public static ShaderResourceI DiffuseTextureConstant {get; private set;}
		#endregion

		#region Instance Properties
		public string Name {get; set;}
		public delegate void ApplyCallbackMethod(FontMaterial material, Mesh mesh);
		public static ApplyCallbackMethod ApplyGlobalConstantsCallback, ApplyInstanceConstantsCallback, ApplyInstancingConstantsCallback;
		[MaterialField(MaterialFieldUsages.Global)] public static Matrix4 Camera;[MaterialField(MaterialFieldUsages.Instance)] public Vector2 Location;[MaterialField(MaterialFieldUsages.Instance)] public Vector2 Size;[MaterialField(MaterialFieldUsages.Instance)] public Vector2 LocationUV;[MaterialField(MaterialFieldUsages.Instance)] public Vector2 SizeUV;[MaterialField(MaterialFieldUsages.Instance)] public Vector2 TexelOffset;[MaterialField(MaterialFieldUsages.Instance)] public Vector4 Color;[MaterialField(MaterialFieldUsages.Instance)] public Texture2DI DiffuseTexture;
		#endregion

		#region Constructors
		public static void Init(DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			Shader = ShaderAPI.New(parent, contentPath + tag + "Font.rs", shaderVersion,
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
			Shader = ShaderAPI.New(parent, contentPath + tag + "Font.rs", shaderVersion, vsQuality, psQuality,
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
				CameraConstant = shader.Variable("Camera");LocationConstant = shader.Variable("Location");SizeConstant = shader.Variable("Size");LocationUVConstant = shader.Variable("LocationUV");SizeUVConstant = shader.Variable("SizeUV");TexelOffsetConstant = shader.Variable("TexelOffset");ColorConstant = shader.Variable("Color");DiffuseTextureConstant = shader.Resource("DiffuseTexture");
				var elements = new List<BufferLayoutElement>();
				elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.Position, 0, 0, 0));
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
			CameraConstant.Set(Camera);
		}

		public void ApplyInstanceContants(Mesh mesh)
		{
			if (ApplyInstanceConstantsCallback != null) ApplyInstanceConstantsCallback(this, mesh);
			LocationConstant.Set(Location);SizeConstant.Set(Size);LocationUVConstant.Set(LocationUV);SizeUVConstant.Set(SizeUV);TexelOffsetConstant.Set(TexelOffset);ColorConstant.Set(Color);DiffuseTextureConstant.Set(DiffuseTexture);
		}

		public void ApplyInstancingContants(Mesh mesh)
		{
			if (ApplyInstancingConstantsCallback != null) ApplyInstancingConstantsCallback(this, mesh);
			
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
			CameraConstant.Set(Camera);
		}

		public void ApplyInstanceContants()
		{
			LocationConstant.Set(Location);SizeConstant.Set(Size);LocationUVConstant.Set(LocationUV);SizeUVConstant.Set(SizeUV);TexelOffsetConstant.Set(TexelOffset);ColorConstant.Set(Color);DiffuseTextureConstant.Set(DiffuseTexture);
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
