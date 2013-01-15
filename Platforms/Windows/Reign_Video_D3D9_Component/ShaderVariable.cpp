#include "pch.h"
#include "ShaderVariable.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	ShaderVariableCom::ShaderVariableCom(VideoCom^ video, IntPtr vertexHandle, IntPtr pixelHandle, ShaderModelCom^ vertexVariables, ShaderModelCom^ pixelVariables)
	{
		this->video = video;
		this->vertexHandle = (D3DXHANDLE)vertexHandle.ToPointer();
		this->pixelHandle = (D3DXHANDLE)pixelHandle.ToPointer();
		this->vertexVariables = vertexVariables->variables;
		this->pixelVariables = pixelVariables->variables;
	}
	#pragma endregion

	#pragma region Methods
	void ShaderVariableCom::SetFloat(float value)
	{
		if (vertexHandle) vertexVariables->SetFloat(video->device, vertexHandle, value);
		if (pixelHandle) pixelVariables->SetFloat(video->device, pixelHandle, value);
	}

	/*void ShaderVariableCom::SetVector2(float x, float y)
	{
		D3DXVECTOR4 vector;
		vector.x = x;
		vector.y = y;
		vector.z = 0;
		vector.w = 0;
		if (vertexHandle) vertexVariables->SetVector(video->device, vertexHandle, &vector);
		if (pixelHandle) pixelVariables->SetVector(video->device, pixelHandle, &vector);
	}

	void ShaderVariableCom::SetVector3(float x, float y, float z)
	{
		D3DXVECTOR4 vector;
		vector.x = x;
		vector.y = y;
		vector.z = z;
		vector.w = 0;
		if (vertexHandle) vertexVariables->SetVector(video->device, vertexHandle, &vector);
		if (pixelHandle) pixelVariables->SetVector(video->device, pixelHandle, &vector);
	}*/

	void ShaderVariableCom::SetVector4(void* vector)
	{
		D3DXVECTOR4* ptr = (D3DXVECTOR4*)vector;
		if (vertexHandle) vertexVariables->SetVector(video->device, vertexHandle, ptr);
		if (pixelHandle) pixelVariables->SetVector(video->device, pixelHandle, ptr);
	}

	/*void ShaderVariableCom::SetMatrix2()
	{
		// use set raw value
		throw gcnew NotImplementedException();
	}

	void ShaderVariableCom::setMatrix3()
	{
		// use set raw value
		throw gcnew NotImplementedException();
	}*/

	void ShaderVariableCom::SetMatrix4(void* matrix)
	{
		D3DXMATRIX* ptr = (D3DXMATRIX*)matrix;
		if (vertexHandle) vertexVariables->SetMatrix(video->device, vertexHandle, ptr);
		if (pixelHandle) pixelVariables->SetMatrix(video->device, pixelHandle, ptr);
	}

	void ShaderVariableCom::SetFloatArray(array<float>^ values, int valueArrayOffset, int valueArrayCount)
	{
		pin_ptr<void> valuePtr = &values[valueArrayOffset];
		float* ptr = ((float*)(void*)valuePtr);
		if (vertexHandle) vertexVariables->SetFloatArray(video->device, vertexHandle, ptr, valueArrayCount);
		if (pixelHandle) pixelVariables->SetFloatArray(video->device, pixelHandle, ptr, valueArrayCount);
	}

	void ShaderVariableCom::SetVector4Array(void* values, int valueArrayOffset, int valueArrayCount)
	{
		D3DXVECTOR4* ptr = ((D3DXVECTOR4*)values) + valueArrayOffset;
		if (vertexHandle) vertexVariables->SetVectorArray(video->device, vertexHandle, ptr, valueArrayCount);
		if (pixelHandle) pixelVariables->SetVectorArray(video->device, pixelHandle, ptr, valueArrayCount);
	}

	void ShaderVariableCom::SetMatrix4Array(void* values, int valueArrayOffset, int valueArrayCount)
	{
		D3DXMATRIX* ptr = ((D3DXMATRIX*)values) + valueArrayOffset;
		if (vertexHandle) vertexVariables->SetMatrixArray(video->device, vertexHandle, ptr, valueArrayCount);
		if (pixelHandle) pixelVariables->SetMatrixArray(video->device, pixelHandle, ptr, valueArrayCount);
	}
	#pragma endregion
}