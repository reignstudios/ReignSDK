#pragma once

namespace System
{
	class object
	{
		public: operator object*()
		{
			return this;
		}
	};

	class string : object
	{
		public: string(wchar_t* value)
		{
			
		}

		public: string& operator+(string value)
		{
			return value;
		}
	};

	struct Int32 : object
	{
		int Value;

		Int32(int value)
		{
			this->Value = value;
		}

		Int32 operator+(int value)
		{
			return Int32(Value + value);
		}

		friend Int32 operator+(int p1, Int32 p2)
		{
			return Int32(p1 + p2);
		}

		static Int32 Parse(string value)
		{
			return Int32(0);
		}

		string ToString()
		{
			return string(L"TODO");
		}
	};

	class Console
	{
		public: static void Write(object* value)
		{
			
		}
	};
}