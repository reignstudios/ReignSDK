#pragma once
#include "Video.h"
#include <d3d9.h>

using namespace System::Runtime::InteropServices;

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class ShaderVariable : ShaderVariableI
	{
		private:
		[StructLayout(LayoutKind::Explicit)]
		ref struct ValueObject
		{
			public:
			[FieldOffset(0)]
			float X;

			[FieldOffset(4)]
			float Y;

			[FieldOffset(8)]
			float Z;

			[FieldOffset(12)]
			float W;

			[FieldOffset(0)]
			Vector2 Vector2;

			[FieldOffset(0)]
			Vector3 Vector3;

			[FieldOffset(0)]
			Vector4 Vector4;

			[FieldOffset(0)]
			Matrix2 Matrix2;

			[FieldOffset(0)]
			Matrix3 Matrix3;

			[FieldOffset(0)]
			Matrix4 Matrix4;
		};

		#pragma region Properties
		internal: delegate void ApplyFunc();
		internal: ApplyFunc^ Apply;
		private: ValueObject valueObject;
		private: WeakReference^ valueArrayObject;
		private: int valueArrayOffset, valueArrayCount;

		private: Video^ video;
		private: D3DXHANDLE vertexHandle, pixelHandle;
		private: ID3DXConstantTable* vertexVariables, *pixelVariables;

		private: string^ name;
		public: property string^ Name {string^ get();}
		#pragma endregion

		#pragma region Constructors
		public: ShaderVariable(VideoI^ video, D3DXHANDLE vertexHandle, D3DXHANDLE pixelHandle, ID3DXConstantTable* vertexVariables, ID3DXConstantTable* pixelVariables, string^ name);
		#pragma endregion

		#pragma region Methods
		private: void setNothing();
		private: void setFloat();
		private: void setVector2();
		private: void setVector3();
		private: void setVector4();
		private: void setMatrix2();
		private: void setMatrix3();
		private: void setMatrix4();
		private: void setFloatArray();
		private: void setVector4Array();
		private: void setMatrix4Array();

		public: virtual void Set(float value);
		public: virtual void Set(float x, float y);
		public: virtual void Set(float x, float y, float z);
		public: virtual void Set(float x, float y, float z, float w);
		public: virtual void Set(Vector2 value);
		public: virtual void Set(Vector3 value);
		public: virtual void Set(Vector4 value);
		public: virtual void Set(Matrix2 value);
		public: virtual void Set(Matrix3 value);
		public: virtual void Set(Matrix4 value);
		public: virtual void Set(array<float>^ values);
		public: virtual void Set(array<Vector4>^ values);
		public: virtual void Set(array<Matrix4>^ values);
		public: virtual void Set(array<float>^ values, int count);
		public: virtual void Set(array<Vector4>^ values, int count);
		public: virtual void Set(array<Matrix4>^ values, int count);
		public: virtual void Set(array<float>^ values, int offset, int count);
		public: virtual void Set(array<Vector4>^ values, int offset, int count);
		public: virtual void Set(array<Matrix4>^ values, int offset, int count);
		#pragma endregion
	};
}
}
}