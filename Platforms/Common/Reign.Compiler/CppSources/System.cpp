#include "System.h"

namespace System
{
	// ======================================
	// object
	// ======================================
	object::operator System::object*()
	{
		return this;
	}

	// ======================================
	// string
	// ======================================
	string::string(wchar_t* value)
	{
		this->Value = value;
	}

	string& string::operator+(string& value)
	{
		return *this;// TODO
	}

	// ======================================
	// int32
	// ======================================
	Int32::Int32(int value)
	{
		this->Value = value;
	}

	Int32 Int32::operator+(int value)
	{
		return Int32(Value + value);
	}

	Int32 operator+(int p1, Int32 p2)
	{
		return Int32(p1 + p2);
	}

	Int32 Int32::Parse(string value)
	{
		return Int32(0);// TODO
	}

	string Int32::ToString()
	{
		return string(L"TODO");// TODO
	}

	// ======================================
	// console
	// ======================================
	void Console::Write(object* value)
	{
		
	}

	void Console::WriteLine(object* value)
	{
		
	}
}