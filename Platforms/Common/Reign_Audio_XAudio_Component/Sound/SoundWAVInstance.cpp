#include "pch.h"
#include "SoundWAV.h"

namespace Reign_Audio_XAudio_Component
{
	#pragma region Properties
	/*SoundStates SoundWAVInstance::State::get() {return state;}
	bool SoundWAVInstance::Looped::get() {return looped;}

	float SoundWAVInstance::Volume::get()
	{
		float volume;
		instance->GetVolume(&volume);
		return volume;
	}

	void SoundWAVInstance::Volume::set(float value)
	{
		instance->SetVolume(value, XAUDIO2_COMMIT_NOW);
	}*/
	#pragma endregion

	#pragma region Constructors
	SoundWAVInstanceErrors SoundWAVInstanceCom::Init(SoundWAVCom^ sound, bool looped)
	{
		null();

		//state = SoundStates::Stopped;
			
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
	void SoundWAVInstanceCom::Update()
	{
		XAUDIO2_VOICE_STATE stateDesc;
		instance->GetState(&stateDesc);
		/*if (state != SoundStates::Paused)
		{
			state = (stateDesc.BuffersQueued == 0) ? SoundStates::Stopped : SoundStates::Playing;
		}*/
	}

	void SoundWAVInstanceCom::Play()
	{
		instance->SubmitSourceBuffer(buffer);
		instance->Start();
		//state = SoundStates::Playing;
	}

	void SoundWAVInstanceCom::Play(float volume)
	{
		instance->SubmitSourceBuffer(buffer);
		instance->SetVolume(volume, XAUDIO2_COMMIT_NOW);
		instance->Start();
		//state = SoundStates::Playing;
	}

	void SoundWAVInstanceCom::Pause()
	{
		instance->Stop();
		//state = SoundStates::Paused;
	}

	void SoundWAVInstanceCom::Stop()
	{
		instance->Stop(0);
		//state = SoundStates::Stopped;
	}
	#pragma endregion
}