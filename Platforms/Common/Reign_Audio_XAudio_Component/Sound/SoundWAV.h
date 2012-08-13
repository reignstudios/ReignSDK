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
		public: SoundWAVErrors Init(AudioCom^ audio, array<byte>^ data, WORD formatCode, WORD channels, DWORD sampleRate, DWORD formatAvgBytesPerSec, WORD formatBlockAlign, WORD bitDepth, WORD formatExtraSize);
		public: ~SoundWAVCom();
		private: void null();
		#pragma endregion
	};

	public ref class SoundWAVInstanceCom sealed
	{
		#pragma region Properties
		internal: IXAudio2SourceVoice* instance;
		private: XAUDIO2_BUFFER* buffer;

		//private: SoundStates state;
		//public: property SoundStates State {virtual SoundStates get();}

		//private: bool looped;
		//public: property bool Looped {virtual bool get();}

		//public: property float Volume {virtual float get(); virtual void set(float value);}
		#pragma endregion
		
		#pragma region Constructors
		public: SoundWAVInstanceErrors Init(SoundWAVCom^ sound, bool looped);
		public: ~SoundWAVInstanceCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Update();
		public: void Play();
		public: void Play(float volume);
		public: void Pause();
		public: void Stop();
		#pragma endregion
	};
}