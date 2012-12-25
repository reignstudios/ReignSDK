using Reign.Core;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Content;

namespace Reign.Audio.XNA
{
	public class Music : Disposable, MusicI
	{
		#region Properties
		private Audio audio;
		Song song;

		public MusicStates State {get; private set;}
		#endregion

		#region Constructors
		public static Music New(DisposableI parent, string fileName)
		{
			return new Music(parent, fileName);
		}

		public Music(DisposableI parent, string fileName)
		: base(parent)
		{
			audio = parent.FindParentOrSelfWithException<Audio>();
			audio.UpdateCallback += Update;

			song = parent.FindParentOrSelfWithException<RootDisposable>().Content.Load<Song>(Streams.StripFileExt(fileName));
			MediaPlayer.IsRepeating = true;
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (audio != null) audio.UpdateCallback -= Update;
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Update()
		{
			switch (MediaPlayer.State)
			{
				case (MediaState.Playing):
					State = MusicStates.Playing;
					break;
				case (MediaState.Paused):
					State = MusicStates.Paused;
					break;
				case (MediaState.Stopped):
					State = MusicStates.Stopped;
					break;
			}
		}

		public void Play()
		{
			MediaPlayer.Play(song);
		}

		public void Stop()
		{
			MediaPlayer.Stop();
		}
		#endregion
	}
}
