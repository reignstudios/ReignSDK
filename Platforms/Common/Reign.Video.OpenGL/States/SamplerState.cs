using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class SamplerState : Disposable, SamplerStateI
	{
		#region Properties
		private Video video;
		private SamplerStateDesc desc;
		#endregion

		#region Constructors
		public SamplerState(DisposableI parent, SamplerStateDescI desc)
		: base(parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			this.desc = (SamplerStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable(int index)
		{
			video.currentSamplerStates[index] = this;
		}

		internal void enable(Texture2D texture)
		{
			if (!texture.hasMipmaps) GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, desc.FilterMin);
			else GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, desc.FilterMinMiped);
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, desc.FilterMag);
			
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, desc.AddressU);
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, desc.AddressV);
			#if !iOS && !ANDROID && !NaCl
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_R, desc.AddressW);
			
			unsafe
			{
				int color = desc.BorderColor;
				GL.TexParameteriv(GL.TEXTURE_2D, GL.TEXTURE_BORDER_COLOR, &color);
			}
			#endif

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}