#include "pch.h"
#include "PixelShader.h"
#include "Shader.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region
	string^ ShaderVariable::Name::get() {return name;}
	#pragma endregion

	#pragma region Constructors
	ShaderVariable::ShaderVariable(VideoI^ video, D3DXHANDLE vertexHandle, D3DXHANDLE pixelHandle, ID3DXConstantTable* vertexVariables, ID3DXConstantTable* pixelVariables, string^ name)
	{
		this->video = (Video^)video;
		this->vertexHandle = vertexHandle;
		this->pixelHandle = pixelHandle;
		this->vertexVariables = vertexVariables;
		this->pixelVariables = pixelVariables;

		this->name = name;
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setNothing);
	}
	#pragma endregion

	#pragma region Methods
	void ShaderVariable::setNothing()
	{
		// Place holder.
	}

	void ShaderVariable::setFloat()
	{
		if (vertexHandle) vertexVariables->SetFloat(video->Device, vertexHandle, valueObject.X);
		if (pixelHandle) pixelVariables->SetFloat(video->Device, pixelHandle, valueObject.X);
	}

	void ShaderVariable::setVector2()
	{
		pin_ptr<void> valuePtr = &valueObject.Vector2;
		D3DXVECTOR4* ptr = (D3DXVECTOR4*)(void*)valuePtr;
		if (vertexHandle) vertexVariables->SetVector(video->Device, vertexHandle, ptr);
		if (pixelHandle) pixelVariables->SetVector(video->Device, pixelHandle, ptr);
	}

	void ShaderVariable::setVector3()
	{
		pin_ptr<void> valuePtr = &valueObject.Vector3;
		D3DXVECTOR4* ptr = (D3DXVECTOR4*)(void*)valuePtr;
		if (vertexHandle) vertexVariables->SetVector(video->Device, vertexHandle, ptr);
		if (pixelHandle) pixelVariables->SetVector(video->Device, pixelHandle, ptr);
	}

	void ShaderVariable::setVector4()
	{
		pin_ptr<void> valuePtr = &valueObject.Vector4;
		D3DXVECTOR4* ptr = (D3DXVECTOR4*)(void*)valuePtr;
		if (vertexHandle) vertexVariables->SetVector(video->Device, vertexHandle, ptr);
		if (pixelHandle) pixelVariables->SetVector(video->Device, pixelHandle, ptr);
	}

	void ShaderVariable::setMatrix2()
	{
		pin_ptr<void> valuePtr = &valueObject.Matrix2;
		D3DXMATRIX* ptr = (D3DXMATRIX*)(void*)valuePtr;
		if (vertexHandle) vertexVariables->SetMatrix(video->Device, vertexHandle, ptr);
		if (pixelHandle) pixelVariables->SetMatrix(video->Device, pixelHandle, ptr);
	}

	void ShaderVariable::setMatrix3()
	{
		pin_ptr<void> valuePtr = &valueObject.Matrix3;
		D3DXMATRIX* ptr = (D3DXMATRIX*)(void*)valuePtr;
		if (vertexHandle) vertexVariables->SetMatrix(video->Device, vertexHandle, ptr);
		if (pixelHandle) pixelVariables->SetMatrix(video->Device, pixelHandle, ptr);
	}

	void ShaderVariable::setMatrix4()
	{
		pin_ptr<void> valuePtr = &valueObject.Matrix4;
		D3DXMATRIX* ptr = (D3DXMATRIX*)(void*)valuePtr;
		if (vertexHandle) vertexVariables->SetMatrix(video->Device, vertexHandle, ptr);
		if (pixelHandle) pixelVariables->SetMatrix(video->Device, pixelHandle, ptr);
	}

	void ShaderVariable::setFloatArray()
	{
		array<float>^ value = (array<float>^)valueArrayObject->Target;
		pin_ptr<void> valuePtr = &value[valueArrayOffset];
		float* ptr = (float*)(void*)valuePtr;
		if (vertexHandle) vertexVariables->SetFloatArray(video->Device, vertexHandle, ptr, value->Length);
		if (pixelHandle) pixelVariables->SetFloatArray(video->Device, pixelHandle, ptr, value->Length);
	}

	void ShaderVariable::setVector4Array()
	{
		array<Vector4>^ value = (array<Vector4>^)valueArrayObject->Target;
		pin_ptr<void> valuePtr = &value[valueArrayOffset];
		D3DXVECTOR4* ptr = (D3DXVECTOR4*)(void*)valuePtr;
		if (vertexHandle) vertexVariables->SetVectorArray(video->Device, vertexHandle, ptr, value->Length);
		if (pixelHandle) pixelVariables->SetVectorArray(video->Device, pixelHandle, ptr, value->Length);
	}

	void ShaderVariable::Set(float value)
	{
		valueObject.X = value;
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setFloat);
	}

	void ShaderVariable::Set(float x, float y)
	{
		valueObject.X = x;
		valueObject.Y = y;
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setVector2);
	}

	void ShaderVariable::Set(float x, float y, float z)
	{
		valueObject.X = x;
		valueObject.Y = y;
		valueObject.Z = z;
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setVector3);
	}

	void ShaderVariable::Set(float x, float y, float z, float w)
	{
		valueObject.X = x;
		valueObject.Y = y;
		valueObject.Z = z;
		valueObject.W = w;
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setVector4);
	}

	void ShaderVariable::Set(Vector2 value)
	{
		valueObject.Vector2 = value;
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setVector2);
	}

	void ShaderVariable::Set(Vector3 value)
	{
		valueObject.Vector3 = value;
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setVector3);
	}

	void ShaderVariable::Set(Vector4 value)
	{
		valueObject.Vector4 = value;
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setVector4);
	}

	void ShaderVariable::Set(Matrix2 value)
	{
		valueObject.Matrix2 = value;
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setMatrix2);
	}

	void ShaderVariable::Set(Matrix3 value)
	{
		valueObject.Matrix3 = value;
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setMatrix3);
	}

	void ShaderVariable::Set(Matrix4 value)
	{
		valueObject.Matrix4 = value;
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setMatrix4);
	}

	void ShaderVariable::Set(array<float>^ values)
	{
		valueArrayOffset = 0;
		valueArrayCount = values->Length;
		valueArrayObject = gcnew WeakReference(values);
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setFloatArray);
	}

	void ShaderVariable::Set(array<Vector4>^ values)
	{
		valueArrayOffset = 0;
		valueArrayCount = values->Length;
		valueArrayObject = gcnew WeakReference(values);
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setVector4Array);
	}

	void ShaderVariable::Set(array<Matrix4>^ values)
	{
		throw gcnew NotImplementedException();
	}

	void ShaderVariable::Set(array<float>^ values, int count)
	{
		valueArrayOffset = 0;
		valueArrayCount = count;
		valueArrayObject = gcnew WeakReference(values);
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setFloatArray);
	}

	void ShaderVariable::Set(array<Vector4>^ values, int count)
	{
		valueArrayOffset = 0;
		valueArrayCount = count;
		valueArrayObject = gcnew WeakReference(values);
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setVector4Array);
	}

	void ShaderVariable::Set(array<Matrix4>^ values, int count)
	{
		throw gcnew NotImplementedException();
	}

	void ShaderVariable::Set(array<float>^ values, int offset, int count)
	{
		valueArrayOffset = offset;
		valueArrayCount = count;
		valueArrayObject = gcnew WeakReference(values);
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setFloatArray);
	}

	void ShaderVariable::Set(array<Vector4>^ values, int offset, int count)
	{
		valueArrayOffset = offset;
		valueArrayCount = count;
		valueArrayObject = gcnew WeakReference(values);
		Apply = gcnew ApplyFunc(this, &ShaderVariable::setVector4Array);
	}

	void ShaderVariable::Set(array<Matrix4>^ values, int offset, int count)
	{
		throw gcnew NotImplementedException();
	}
	#pragma endregion
}
}
}