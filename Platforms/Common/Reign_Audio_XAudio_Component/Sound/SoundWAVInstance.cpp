#include "pch.h"
#include "SoundWAV.h"

namespace Reign_Audio_XAudio_Component
{
	#pragma region Constructors
	SoundWAVInstanceErrors SoundWAVInstanceCom::Init(SoundWAVCom^ sound, bool looped)
	{
		null();
			
		buffer = new XAUDIO2_BUFFER();
		ZeroMemory(buffer, sizeof(XAUDIO2_BUFFER));
		buffer->pAudioData = sound->buffer;
		buffer->AudioBytes = sound->bufferLength;
		buffer->Flags = XAUDIO2_END_OF_STREAM;
		if (looped) buffer->LoopCount = XAUDIO2_LOOP_INFINITE;

		IXAudio2SourceVoice* instanceTEMP;
		if (FAILED(sound->audio->xAudio->CreateSourceVoice(&instanceTEMP, sound->waveFormat)))
		{
			return SoundWAVInstanceErrors::SourceVoice;
		}
		instance = instanceTEMP;

		return SoundWAVInstanceErrors::None;
	}

	SoundWAVInstanceCom::~SoundWAVInstanceCom()
	{
		if (instance)
		{
			instance->Stop(0);
			instance->FlushSourceBuffers();
			instance->DestroyVoice();
		}

		if (buffer) delete buffer;
		null();
	}

	void SoundWAVInstanceCom::null()
	{
		instance = 0;
		buffer = 0;
	}
	#pragma endregion

	#pragma region Methods
	void SoundWAVInstanceCom::SetVolume(float volume)
	{
		instance->SetVolume(volume, XAUDIO2_COMMIT_NOW);
	}

	bool SoundWAVInstanceCom::Update()
	{
		XAUDIO2_VOICE_STATE stateDesc;
		instance->GetState(&stateDesc);
		return stateDesc.BuffersQueued != 0;
	}

	void SoundWAVInstanceCom::Play()
	{
		instance->SubmitSourceBuffer(buffer);
		instance->Start();
	}

	void SoundWAVInstanceCom::Play(float volume)
	{
		instance->SubmitSourceBuffer(buffer);
		instance->SetVolume(volume, XAUDIO2_COMMIT_NOW);
		instance->Start();
	}

	void SoundWAVInstanceCom::Pause()
	{
		instance->Stop();
	}

	void SoundWAVInstanceCom::Stop()
	{
		instance->Stop(0);
		instance->FlushSourceBuffers();
	}
	#pragma endregion
}