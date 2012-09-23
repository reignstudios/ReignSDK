#pragma once

#define ushort unsigned short
#define uint unsigned int
#define ulong unsigned long
#define byte unsigned char
#define sbyte char
#define string String
#define object Object

inline unsigned long FtoDW(float pValue) {return *((unsigned long*)&pValue);}

#if WINDOWS
using namespace System;
using namespace System::Runtime::InteropServices;
#define byte System::Byte
#define sbyte System::SByte
#define OutType(x) [Out] x%
#define PinPtr(x) pin_ptr<x>
#define GetDataPtr(x) &x[0]
#else
using namespace Platform;
#define byte UINT8
#define sbyte INT8
#define gcnew ref new
#define array Array
#define ToPointer() operator void *
#define OutType(x) x*
#define PinPtr(x) x*
#define GetDataPtr(x) x->Data
#endif

uint AsciiLength(char* value);
bool AsciiMatch(char* value, char* value2);
char* StringToAscii(string^ value);
wchar_t* AsciiToUnicode(char* value);