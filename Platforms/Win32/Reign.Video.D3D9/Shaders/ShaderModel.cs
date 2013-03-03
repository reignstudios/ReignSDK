using System;
using Reign.Core;
using Reign_Video_D3D9_Component;
using System.Runtime.InteropServices;

namespace Reign.Video.D3D9
{
	public abstract class ShaderModel : Disposable
	{
		#region Properties
		protected internal ShaderModelCom com;
		#endregion

		#region Constructors
		public ShaderModel(DisposableI parent, string code, ShaderVersions shaderVersion, ShaderTypes shaderType)
		: base(parent)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();

				string shaderLvl = "";
				switch (shaderVersion)
				{
					case ShaderVersions.HLSL_2_0: shaderLvl = "_2_0"; break;
					case ShaderVersions.HLSL_2_a: shaderLvl = "_2_a"; break;
					case ShaderVersions.HLSL_3_0: shaderLvl = "_3_0"; break;
					default: Debug.ThrowError("ShaderModel", "Unsuported ShaderVersion: " + shaderVersion); break;
				}
				string shaderVersionType = shaderType.ToString().ToLower() + shaderLvl;

				com = new ShaderModelCom();
				var codePtr = Marshal.StringToHGlobalAnsi(code);
				var shaderVersionTypePtr = Marshal.StringToHGlobalAnsi(shaderVersionType);
				string errorText;
				var error = com.Init(video.com, codePtr, code.Length, shaderVersionTypePtr, out errorText);
				if (codePtr != IntPtr.Zero) Marshal.FreeHGlobal(codePtr);
				if (shaderVersionTypePtr != IntPtr.Zero) Marshal.FreeHGlobal(shaderVersionTypePtr);

				switch (error)
				{
					case ShaderModelErrors.Compile: Debug.ThrowError("ShaderModel", string.Format("Failed to compile {0} shader: Errors: {1}", shaderType == ShaderTypes.VS ? "vs" : "ps", errorText)); break;
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public IntPtr Variable(string name)
		{
			return com.Variable(name);
		}

		public int Resource(string name)
		{
			return com.Resource(name);
		}
		#endregion
	}
}
