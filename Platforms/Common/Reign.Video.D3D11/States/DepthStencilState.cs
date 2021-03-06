﻿using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class DepthStencilState : DisposableResource, IDepthStencilState
	{
		#region Properties
		private DepthStencilStateCom com;
		#endregion

		#region Constructors
		public DepthStencilState(IDisposableResource parent, IDepthStencilStateDesc desc)
		: base(parent)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();

				com = new DepthStencilStateCom();
				var error = com.Init(video.com, ((DepthStencilStateDesc)desc).com);

				if (error == DepthStencilStateError.DepthStencil) Debug.ThrowError("DepthStencil", "Failed to create DepthStencil");
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