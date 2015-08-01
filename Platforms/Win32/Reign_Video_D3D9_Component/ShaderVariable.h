#pragma once
#include "ShaderModel.h"

namespace Reign_Video_D3D9_Component
{
	public ref class ShaderVariableCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		//private: D3DXHANDLE vertexHandle, pixelHandle;
		//private: ID3DXConstantTable* vertexVariables, *pixelVariables;
		#pragma endregion

		#pragma region Constructors
		public: ShaderVariableCom(VideoCom^ video, IntPtr vertexHandle, IntPtr pixelHandle, ShaderModelCom^ vertexVariables, ShaderModelCom^ pixelVariables);
		#pragma endregion

		#pragma region Methods
		public: void SetFloat(float value);
		//public: void SetVector2();
		//public: void SetVector3();
		public: void SetVector4(void* vector);
		//public: void SetMatrix2();
		//public: void SetMatrix3();
		public: void SetMatrix4(void* matrix);
		public: void SetFloatArray(array<float>^ values, int valueArrayOffset, int valueArrayCount);
		public: void SetVector4Array(void* values, int valueArrayOffset, int valueArrayCount);
		public: void SetMatrix4Array(void* values, int valueArrayOffset, int valueArrayCount);
		#pragma endregion
	};
}