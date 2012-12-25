using Reign.Core;
using Reign_Video_D3D11_Component;
using System;

namespace Reign.Video.D3D11
{
	public class BufferLayout : Disposable, BufferLayoutI
	{
		#region Properties
		private BufferLayoutCom com;
		#endregion

		#region Constructors
		public static BufferLayout New(DisposableI parent, ShaderI shader, BufferLayoutDescI desc)
		{
			return new BufferLayout(parent, shader, desc);
		}

		public BufferLayout(DisposableI parent, ShaderI shader, BufferLayoutDescI desc)
		: base(parent)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();
				com = new BufferLayoutCom();
				var error = com.Init(video.com, ((Shader)shader).vertex.com, ((BufferLayoutDesc)desc).com);

				if (error == BufferLayoutErrors.InputLayout) Debug.ThrowError("BufferLayout", "Failed to create InputLayout");
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
		public void Enable()
		{
			com.Enable();
		}
		#endregion
	}
}