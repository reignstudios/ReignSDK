#include "pch.h"
#include "Audio.h"

namespace Reign_Audio_XAudio_Component
{
	#pragma region Constructors
	AudioErrors AudioCom::Init()
	{
		null();
		
		IXAudio2* xAudioTEMP;
		if(FAILED(XAudio2Create(&xAudioTEMP, 0)))
		{
			return AudioErrors::XAudio2;
		}
		xAudio = xAudioTEMP;

		IXAudio2MasteringVoice* masteringVoiceTEMP;
		if (FAILED(xAudio->CreateMasteringVoice(&masteringVoiceTEMP)))
		{
			return AudioErrors::MasteringVoice;
		}
		masteringVoice = masteringVoiceTEMP;

		return AudioErrors::None;
	}

	AudioCom::~AudioCom()
	{
		if (masteringVoice) masteringVoice->DestroyVoice();
		if (xAudio) xAudio->Release();
		null();
	}

	void AudioCom::null()
	{
		masteringVoice = 0;
		xAudio = 0;
	}
	#pragma endregion
}