#pragma once
#include "../Audio.h"

namespace Reign_Audio_XAudio_Component
{
	public enum class SoundWAVErrors
	{
		None
	};

	public enum class SoundWAVInstanceErrors
	{
		None,
		SourceVoice
	};

	public ref class SoundWAVCom sealed
	{
		#pragma region Properties
		internal: AudioCom^ audio;
		internal: WAVEFORMATEX* waveFormat;
		internal: byte* buffer;
		internal: int bufferLength;
		#pragma endregion

		#pragma region Constructors
		public: SoundWAVErrors Init(AudioCom^ audio, const array<byte>^ data, short formatCode, short channels, int sampleRate, int formatAvgBytesPerSec, short formatBlockAlign, short bitDepth, short formatExtraSize);
		public: virtual ~SoundWAVCom();
		private: void null();
		#pragma endregion
	};

	public ref class SoundWAVInstanceCom sealed
	{
		#pragma region Properties
		internal: IXAudio2SourceVoice* instance;
		private: XAUDIO2_BUFFER* buffer;
		#pragma endregion
		
		#pragma region Constructors
		public: SoundWAVInstanceErrors Init(SoundWAVCom^ sound, bool looped);
		public: virtual ~SoundWAVInstanceCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void SetVolume(float volume);
		public: bool Update();
		public: void Play();
		public: void Play(float volume);
		public: void Pause();
		public: void Stop();
		#pragma endregion
	};
}