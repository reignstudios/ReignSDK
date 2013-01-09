using Reign.Core;
using Reign_Video_D3D9_Component;
using System;

namespace Reign.Video.D3D9
{
	public class Texture2D : Disposable, LoadableI
	{
		#region Properties
		public bool Loaded {get; protected set;}
		public bool FailedToLoad {get; protected set;}

		public Size2 Size {get; protected set;}
		public Vector2 SizeF {get; protected set;}
		public Vector2 TexelOffset {get; protected set;}
		public int PixelByteSize {get; protected set;}

		protected Video video;
		internal Texture2DCom com;
		#endregion

		#region Constructors
		public Texture2D(DisposableI parent)
		: base(parent)
		{
			
		}

		public bool UpdateLoad()
		{
			return Loaded;
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
		public void Copy(Texture2DI texture)
		{
			//com.Copy(((Texture2D)texture).com);
		}

		public void Update(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void WritePixels(byte[] data)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
