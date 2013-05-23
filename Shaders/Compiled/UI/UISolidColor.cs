
using System;
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;

namespace ShaderMaterials.Shaders
{
	public class UISolidColorMaterial : MaterialI
	{
		#region Static Properties
		public static bool Loaded {get; private set;}
		public static bool FailedToLoad {get; private set;}
		
		public static ShaderI Shader {get; private set;}
		public static BufferLayoutDescI BufferLayoutDesc {get; private set;}
		public static BufferLayoutI BufferLayout {get; private set;}
		public static ShaderVariableI CameraConstant {get; private set;}public static ShaderVariableI PositionConstant {get; private set;}public static ShaderVariableI SizeConstant {get; private set;}public static ShaderVariableI ColorConstant {get; private set;}
		#endregion

		#region Instance Properties
		public string Name {get; set;}
		
		public delegate void ApplyObjectMeshCallbackMethod(UISolidColorMaterial material, ObjectMesh objectMesh);
		public static ApplyObjectMeshCallbackMethod ApplyObjectMeshCallback;
		
		public delegate void ApplyInstanceObjectMeshCallbackMethod(UISolidColorMaterial material, InstanceObjectMesh intanceObjectMesh);
		public static ApplyInstanceObjectMeshCallbackMethod ApplyInstanceObjectMeshCallback;
		
		[MaterialField(MaterialFieldUsages.Global)] public static Matrix4 Camera;[MaterialField(MaterialFieldUsages.Instance)] public Vector2 Position;[MaterialField(MaterialFieldUsages.Instance)] public Vector2 Size;[MaterialField(MaterialFieldUsages.Instance)] public Vector4 Color;
		#endregion

		#region Constructors
		public static void Init(DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback)
		{
			Shader = ShaderAPI.New(parent, contentPath + tag + "UISolidColor.rs", shaderVersion,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init((ShaderI)sender, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					if (loadedCallback != null) loadedCallback(null, false);
				}
			});
		}

		public static void Init(DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		{
			Shader = ShaderAPI.New(parent, contentPath + tag + "UISolidColor.rs", shaderVersion, vsQuality, psQuality,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init((ShaderI)sender, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					if (loadedCallback != null) loadedCallback(null, false);
				}
			});
		}
		
		private static void init(ShaderI shader, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				CameraConstant = shader.Variable("Camera");PositionConstant = shader.Variable("Position");SizeConstant = shader.Variable("Size");ColorConstant = shader.Variable("Color");
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
				if (loadedCallback != null) loadedCallback(null, false);
				return;
			}
			
			Loaded = true;
			if (loadedCallback != null) loadedCallback(null, true);
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

		public void ApplyInstanceContants(ObjectMesh objectMesh)
		{
			if (ApplyObjectMeshCallback != null) ApplyObjectMeshCallback(this, objectMesh);
			PositionConstant.Set(Position);SizeConstant.Set(Size);ColorConstant.Set(Color);
		}

		public void Apply(ObjectMesh objectMesh)
		{
			CameraConstant.Set(Camera);
			ApplyInstanceContants(objectMesh);
			
			Shader.Apply();
		}
		
		public void ApplyInstanceContants(InstanceObjectMesh instanceObjectMesh)
		{
			if (ApplyInstanceObjectMeshCallback != null) ApplyInstanceObjectMeshCallback(this, instanceObjectMesh);
			PositionConstant.Set(Position);SizeConstant.Set(Size);ColorConstant.Set(Color);
		}

		public void Apply(InstanceObjectMesh instanceObjectMesh)
		{
			CameraConstant.Set(Camera);
			ApplyInstanceContants(instanceObjectMesh);
			
			Shader.Apply();
		}

		public void ApplyGlobalContants()
		{
			CameraConstant.Set(Camera);
		}

		public void ApplyInstanceContants()
		{
			PositionConstant.Set(Position);SizeConstant.Set(Size);ColorConstant.Set(Color);
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
