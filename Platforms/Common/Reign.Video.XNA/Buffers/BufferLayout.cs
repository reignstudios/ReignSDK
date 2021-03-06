﻿using Microsoft.Xna.Framework.Graphics;
using Reign.Core;
using System;

namespace Reign.Video.XNA
{
	public class BufferLayout : Disposable, BufferLayoutI
	{
		#region Properties
		private Video video;
		internal VertexDeclaration layout;
		#endregion

		#region Constructors
		public static BufferLayout New(DisposableI parent, ShaderI shader, BufferLayoutDescI desc)
		{
			return new BufferLayout(parent, shader, desc);
		}

		public BufferLayout(DisposableI parent, ShaderI shader, BufferLayoutDescI inputLayoutDesc)
		: base(parent)
		{
			init(parent, shader, inputLayoutDesc, false);
		}

		internal BufferLayout(DisposableI parent, ShaderI shader, BufferLayoutDescI inputLayoutDesc, bool construct)
		: base(parent)
		{
			init(parent, shader, inputLayoutDesc, construct);
		}

		private void init(DisposableI parent, ShaderI shader, BufferLayoutDescI inputLayoutDesc, bool construct)
		{
			if (!construct) return;

			try
			{
				video = parent.FindParentOrSelfWithException<Video>();

				var inputLayoutDescTEMP = (BufferLayoutDesc)inputLayoutDesc;
				layout = new VertexDeclaration(inputLayoutDescTEMP.desc);
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
			if (layout != null)
			{
				layout.Dispose();
				layout = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Enable()
		{
			// Not used in XNA
		}
		#endregion
	}
}