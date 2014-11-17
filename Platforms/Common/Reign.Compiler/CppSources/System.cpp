#define STRING
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
	string::string()
	{
		this->Value = 0;
	}

	string::string(wchar_t* value)
	{
		this->Value = value;
	}

	string& string::operator+(string& value)
	{
		return *this;// TODO
	}

	// ======================================
	// Int32
	// ======================================
	Int32::Int32()
	{
		this->Value = 0;
	}

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
	// Console
	// ======================================
	void Console::Write(object* value)
	{
		
	}

	void Console::WriteLine(object* value)
	{
		
	}

	// ======================================
	// Type
	// ======================================
	Type::Type(string name, bool isValueType)
	{
		this->Name = name;
		this->IsValueType = isValueType;

		this->TypeInfoOffsets = 0;
		this->TypeInfos = 0;
		this->TypeInfosCount = 0;
	}

	// ======================================
	// Type Infos
	// ======================================
	TYPE_string::TYPE_string() : Type(string(L"System.String"), true)
	{
		// ...
	}

	TYPE_Int32::TYPE_Int32() : Type(string(L"System.Int32"), true)
	{
		// ...
	}
}