#include "pch.h"
#include "SoundWAV.h"
using namespace System::IO;

namespace Reign_Audio_XAudio_Component
{
	#pragma region Constructors
	SoundWAVErrors SoundWAVCom::Init(AudioCom^ audio, array<byte>^ data, WORD formatCode, WORD channels, DWORD sampleRate, DWORD formatAvgBytesPerSec, WORD formatBlockAlign, WORD bitDepth, WORD formatExtraSize)
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

		pin_ptr<void> dataPtr = &data[0];
		buffer = new byte[data->Length];
		memcpy(buffer, dataPtr, data->Length);
		bufferLength = data->Length;
		//data = nullptr;<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Set in C#

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