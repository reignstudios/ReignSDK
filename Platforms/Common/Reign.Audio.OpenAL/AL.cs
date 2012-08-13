using System;
using System.Runtime.InteropServices;
using System.Security;

//using ALvoid = System.Void;
using ALCdevice = System.IntPtr;
using ALCcontext = System.IntPtr;
using ALCboolean = System.Boolean;
using ALCchar = System.Byte;
using ALCint = System.Int32;
using ALenum = System.Int32;
using ALfloat = System.Single;
using ALsizei = System.Int32;
using ALuint = System.UInt32;
using ALint = System.Int32;

namespace Reign.Audio.OpenAL
{
	public static class AL
	{
		public const string DLL = "libopenal";
		
		public const int FALSE = 0;
		public const int TRUE = 1;
		public const int BUFFER = 0x1009;
		public const int PITCH = 0x1003;
		public const int GAIN = 0x100A;
		public const int LOOPING = 0x1007;
		public const int POSITION = 0x1004;
		public const int VELOCITY = 0x1006;
		public const int ORIENTATION = 0x100F;
		public const int FORMAT_MONO8 = 0x1100;
		public const int FORMAT_MONO16 = 0x1101;
		public const int FORMAT_STEREO8 = 0x1102;
		public const int FORMAT_STEREO16 = 0x1103;
		
		public const int SOURCE_STATE = 0x1010;
		public const int PLAYING = 0x1012;
		public const int PAUSED = 0x1013;
		public const int STOPPED = 0x1014;
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alcOpenDevice", ExactSpelling = true)]
		public unsafe static extern ALCdevice OpenDevice(ALCchar* deviceSpecifier);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alcCreateContext", ExactSpelling = true)]
		public static extern ALCcontext CreateContext(ALCdevice deviceHandle, ALCint[] attrList);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alcDestroyContext", ExactSpelling = true)]
		public static extern void DestroyContext(ALCcontext context);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alcCloseDevice", ExactSpelling = true)]
		public static extern ALCboolean CloseDevice(ALCdevice device);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alcMakeContextCurrent", ExactSpelling = true)]
		public static extern ALCboolean MakeContextCurrent(ALCcontext context);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alListener3f", ExactSpelling = true)]
		public static extern void Listener3f(ALenum param, ALfloat value1, ALfloat value2, ALfloat value3);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alGenBuffers", ExactSpelling = true)]
		public unsafe static extern void GenBuffers(ALsizei n, ALuint* buffers);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alDeleteBuffers", ExactSpelling = true)]
		public unsafe static extern void DeleteBuffers(ALsizei n, ALuint* buffers);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alBufferData", ExactSpelling = true)]
		public unsafe static extern void BufferData(ALuint bid, ALenum format, void* data, ALsizei size, ALsizei freq);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alGenSources", ExactSpelling = true)]
		public unsafe static extern void GenSources(ALsizei n, ALuint* sources);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alDeleteSources", ExactSpelling = true)]
		public unsafe static extern void DeleteSources(ALsizei n, ALuint* sources);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alSourcef", ExactSpelling = true)]
		public static extern void Sourcef(ALuint sid, ALenum param, ALfloat value);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alSource3f", ExactSpelling = true)]
		public static extern void Source3f(ALuint sid, ALenum param, ALfloat value1, ALfloat value2, ALfloat value3);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alSourcei", ExactSpelling = true)]
		public static extern void Sourcei(ALuint sid, ALenum param, ALint value);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alSourcePlay", ExactSpelling = true)]
		public static extern void SourcePlay(ALuint sid);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alSourceStop", ExactSpelling = true)]
		public static extern void SourceStop(ALuint sid);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alSourcePause", ExactSpelling = true)]
		public static extern void SourcePause(ALuint sid);
		
		[SuppressUnmanagedCodeSecurity()]
		[DllImport(DLL, EntryPoint = "alGetSourcei", ExactSpelling = true)]
		public unsafe static extern void GetSourcei(ALuint sid,  ALenum param, ALint* value);
	}
}

