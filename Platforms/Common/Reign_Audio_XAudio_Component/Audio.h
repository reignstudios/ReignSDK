#pragma once
#include <xaudio2.h>
#include "../CPP_CLR-CX_Helpers/Common.h"

namespace Reign_Audio_XAudio_Component
{
	public enum class AudioErrors
	{
		None,
		XAudio2,
		MasteringVoice
	};

	public ref class AudioCom sealed
	{
		#pragma region Properties
		internal: IXAudio2* xAudio;
		private: IXAudio2MasteringVoice* masteringVoice;
		#pragma endregion

		#pragma region Constructors
		public: AudioErrors Init();
		public: virtual virtual ~AudioCom();
		private: void null();
		#pragma endregion
	};
}