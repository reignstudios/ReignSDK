#pragma once

namespace System
{
	// ======================================
	// object
	// ======================================
	class object
	{
		public: operator object*();
	};

	// ======================================
	// string
	// ======================================
	class string : public object
	{
		public: wchar_t* Value;

		// constructors
		public: string(wchar_t* value);

		// operators
		public: string& operator+(string& value);
	};

	// ======================================
	// int32
	// ======================================
	struct Int32 : public object
	{
		public: int Value;

		// constructors
		public: Int32(int value);

		// operators
		public: Int32 operator+(int value);
		public: friend Int32 operator+(int p1, Int32 p2);

		// methods
		public: static Int32 Parse(string value);
		public: string ToString();
	};

	// ======================================
	// console
	// ======================================
	class Console : public object
	{
		public: static void Write(object* value);
		public: static void WriteLine(object* value);
	};
}