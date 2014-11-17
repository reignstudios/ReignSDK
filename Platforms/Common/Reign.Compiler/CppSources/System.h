#pragma once

namespace System
{
	class string;

	// ======================================
	// object
	// ======================================
	class object
	{
		public: operator object*();
		public: virtual string ToString();
	};

	// ======================================
	// string
	// ======================================
	class string : public object
	{
		// fields
		public: wchar_t* Value;

		// constructors
		public: string();
		public: string(wchar_t* value);

		// operators
		public: string& operator+(string& value);

		// methods
		public: virtual string ToString() override;
	};

	// ======================================
	// Int32
	// ======================================
	struct Int32 : public object
	{
		// fields
		public: int Value;

		// constructors
		public: Int32();
		public: Int32(int value);

		// operators
		public: Int32 operator+(int value);
		public: friend Int32 operator+(int p1, Int32 p2);

		// methods
		public: static Int32 Parse(string value);
		public: string ToString();
	};

	// ======================================
	// Console
	// ======================================
	class Console : public object
	{
		public: static void Write(object* value);
		public: static void WriteLine(object* value);
	};

	// ======================================
	// Type
	// ======================================
	class Type : public object
	{
		// fields
		public: string Name;
		public: bool IsValueType;

		public: int* TypeInfoOffsets;
		public: Type** TypeInfos;
		public: int TypeInfosCount;

		// constructors
		public: Type(string name, bool isValueType);
	};

	// ======================================
	// Type Infos
	// ======================================
	class TYPE_string : public Type
	{
		public: static TYPE_string* Singlton;
		public: TYPE_string();
	};

	class TYPE_Int32 : public Type
	{
		public: static TYPE_Int32* Singlton;
		public: TYPE_Int32();
	};
}