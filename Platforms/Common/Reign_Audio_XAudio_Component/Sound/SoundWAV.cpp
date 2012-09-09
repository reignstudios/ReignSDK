#include "pch.h"
#include "SoundWAV.h"

namespace Reign_Audio_XAudio_Component
{
	#pragma region Constructors
	SoundWAVErrors SoundWAVCom::Init(AudioCom^ audio, const array<byte>^ data, short formatCode, short channels, int sampleRate, int formatAvgBytesPerSec, short formatBlockAlign, short bitDepth, short formatExtraSize)
	{
		null();
		this->audio = audio;

		waveFormat = new WAVEFORMATEX();
		ZeroMemory(waveFormat, sizeof(WAVEFORMATEX));
		waveFormat->wFormatTag = formatCode;
		waveFormat->nChannels = channels;
		waveFormat->nSamplesPerSec = sampleRate;
		waveFormat->nAvgBytesPerSec = formatAvgBytesPerSec;
		waveFormat->nBlockAlign = formatBlockAlign;
		waveFormat->wBitsPerSample = bitDepth;
		waveFormat->cbSize = formatExtraSize;

		PinPtr(void) dataPtr = GetDataPtr(data);
		buffer = new byte[data->Length];
		memcpy(buffer, dataPtr, data->Length);
		bufferLength = data->Length;

		return SoundWAVErrors::None;
	}

	SoundWAVCom::~SoundWAVCom()
	{
		if (waveFormat) delete waveFormat;
		if (buffer) delete buffer;
		null();
	}

	void SoundWAVCom::null()
	{
		buffer = 0;
		waveFormat = 0;
	}
	#pragma endregion
}