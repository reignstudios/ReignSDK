using System;
using Reign.Core;
using Reign.Audio;
#if XNA
using AX = Reign.Audio.XNA;
#endif

namespace Reign.Audio.API
{
	public static class Music
	{
		public static MusicI Create(DisposableI parent, AudioTypes apiType, string fileName)
		{
			#if XNA
			if (apiType == AudioTypes.XNA)
			{
				return new AX.Music(parent, fileName);
			}
			#endif

			return null;
		}
	}
}
