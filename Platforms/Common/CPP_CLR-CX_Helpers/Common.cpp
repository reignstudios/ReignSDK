#include "pch.h"
#include "Common.h";

uint AsciiLength(char* value)
{
	uint length = 0;
	while (value[length] != '\0') ++length;
	return length;
}

bool AsciiMatch(char* value, char* value2)
{
	int i = 0;
	while (value[i] != '\0' || value2[i] != '\0')
	{
		if (value[i] != value2[i]) return false;
		++i;
	}

	return true;
}

char* StringToAscii(string^ value)
{
	#if WIN32
	char* ascii = new char[value->Length+1];
	for (uint i = 0; i != value->Length; ++i)
	{
		ascii[i] = (char)value[i];
	}
	ascii[value->Length] = '\0';

	return ascii;
	#else
	const wchar_t* data = value->Data();
	char* ascii = new char[value->Length()+1];
	for (uint i = 0; i != value->Length(); ++i)
	{
		ascii[i] = (char)data[i];
	}
	ascii[value->Length()] = '\0';

	return ascii;
	#endif
}

wchar_t* AsciiToUnicode(char* value)
{
	uint length = AsciiLength(value);
	wchar_t* uni = new wchar_t[length+1];
	for (uint i = 0; i != length; ++i)
	{
		uni[i] = (wchar_t)value[i];
	}
	uni[length] = '\0';

	return uni;
}