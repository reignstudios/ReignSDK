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
		public static SamplerState New(DisposableI parent, SamplerStateDescI desc)
		{
			return new SamplerState(parent, desc);
		}

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
			if (!texture.hasMipmaps) GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, desc.filterMin);
			else GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, desc.filterMinMiped);
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, desc.filterMag);
			
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, desc.addressU);
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, desc.addressV);
			#if !iOS && !ANDROID && !NaCl && !(LINUX && ARM)
			GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_R, desc.addressW);
			
			unsafe
			{
				int color = desc.borderColor;
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