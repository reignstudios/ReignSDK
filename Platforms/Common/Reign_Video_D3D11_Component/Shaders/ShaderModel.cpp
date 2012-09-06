#include "pch.h"
#include <D3DCompiler.h>
#include "ShaderModel.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	#if WINDOWS
	ShaderModelErrors ShaderModelCom::Init(VideoCom^ video, string^ code, int codeSize, string^ shaderType, OutType(string^) errorText)
	#else
	ShaderModelErrors ShaderModelCom::Init(VideoCom^ video, const array<byte>^ code, int codeSize, int variableBufferSize, int resourceCount)
	#endif
	{
		null();
		this->video = video;
		
		// Code
		#if WINDOWS
		char* shaderTypeName = StringToAscii(shaderType);
		char* codeAscii = StringToAscii(code);
		ID3DBlob *codeTEMP, *err = 0;
		if (FAILED(D3DCompile(codeAscii, codeSize, NULL, NULL, NULL, "main", shaderTypeName, D3DCOMPILE_OPTIMIZATION_LEVEL3, NULL, &codeTEMP, &err)))
		{
			if (shaderTypeName) delete shaderTypeName;
			if (codeAscii) delete codeAscii;
			if (err)
			{
				string^ error = gcnew string(AsciiToUnicode((char*)err->GetBufferPointer()));
				#if WINDOWS
				errorText = error;
				#else
				*errorText = error;
				#endif
				err->Release();
			}
			return ShaderModelErrors::CompileCode;
		}
		if (shaderTypeName) delete shaderTypeName;
		if (codeAscii) delete codeAscii;
		this->code = codeTEMP;

		// Reflection
		void* reflectionTEMP = 0;
		if (FAILED(D3DReflect(this->code->GetBufferPointer(), this->code->GetBufferSize(), IID_ID3D11ShaderReflection, &reflectionTEMP)))
		{
			return ShaderModelErrors::Reflect;
		}
		reflection = (ID3D11ShaderReflection*)reflectionTEMP;

		// Variable Buffers
		uint constantBufferIndex = 0;
		variables = reflection->GetConstantBufferByIndex(constantBufferIndex);

		D3D11_SHADER_BUFFER_DESC constDesc;
		ZeroMemory(&constDesc, sizeof(D3D11_SHADER_BUFFER_DESC));
		variables->GetDesc(&constDesc);

		if (constDesc.Variables != 0)
		{
			variableBufferBytesCount = constDesc.Size;
			variableBufferBytes = new char[constDesc.Size];
			ZeroMemory(variableBufferBytes, sizeof(char) * constDesc.Size);

			ID3D11Buffer* variableBufferTEMP = 0;
			D3D11_BUFFER_DESC cbDesc;
			cbDesc.ByteWidth = constDesc.Size;
			cbDesc.Usage = D3D11_USAGE_DYNAMIC;
			cbDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
			cbDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
			cbDesc.MiscFlags = 0;
			video->device->CreateBuffer(&cbDesc, 0, &variableBufferTEMP);
			if (variableBufferTEMP == 0)
			{
				return ShaderModelErrors::VariableBuffer;
			}
			variableBuffer = variableBufferTEMP;
		}

		// Resources
		D3D11_SHADER_DESC shaderDesc;
		ZeroMemory(&shaderDesc, sizeof(D3D11_SHADER_DESC));
		reflection->GetDesc(&shaderDesc);
		if (shaderDesc.BoundResources != 0)
		{
			resourceKnownCount = 0;
			resourceCount = shaderDesc.BoundResources;
			resources = new ID3D11ShaderResourceView*[resourceCount];
			for (uint i = 0; i != shaderDesc.BoundResources; ++i)
			{
				D3D11_SHADER_INPUT_BIND_DESC inputDesc;
				ZeroMemory(&inputDesc, sizeof(D3D11_SHADER_INPUT_BIND_DESC));
				reflection->GetResourceBindingDesc(i, &inputDesc);
				if (inputDesc.Dimension != D3D11_SRV_DIMENSION_UNKNOWN)
				{
					++resourceKnownCount;
				}
			}
		}
		#else
		this->code = &code->get(0);
		this->codeSize = codeSize;

		// Variable Buffers
		if (variableBufferSize != 0)
		{
			variableBufferBytesCount = variableBufferSize;
			variableBufferBytes = new char[variableBufferSize];
			ZeroMemory(variableBufferBytes, sizeof(char) * variableBufferSize);

			ID3D11Buffer* variableBufferTEMP = 0;
			D3D11_BUFFER_DESC cbDesc;
			cbDesc.ByteWidth = variableBufferSize;
			cbDesc.Usage = D3D11_USAGE_DYNAMIC;
			cbDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
			cbDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
			cbDesc.MiscFlags = 0;
			video->device->CreateBuffer(&cbDesc, 0, &variableBufferTEMP);
			if (variableBufferTEMP == 0)
			{
				return ShaderModelErrors::VariableBuffer;
			}
			variableBuffer = variableBufferTEMP;
		}

		// Resources
		if (resourceCount != 0)
		{
			this->resourceCount = resourceCount;
			this->resourceKnownCount = resourceCount;
			resources = new ID3D11ShaderResourceView*[resourceCount];
		}
		#endif

		return ShaderModelErrors::None;
	}

	ShaderModelCom::~ShaderModelCom()
	{
		if (resources) delete resources;
		if (variableBufferBytes) delete variableBufferBytes;
		if (variableBuffer) variableBuffer->Release();
		#if WINDOWS
		if (code) code->Release();
		if (reflection) reflection->Release();
		#else
		delete code;
		#endif
		null();
	}

	void ShaderModelCom::null()
	{
		code = 0;
		resources = 0;
		resourceCount = 0;
		resourceKnownCount = 0;
		variableBufferBytes = 0;
		variableBuffer = 0;
		#if WINDOWS
		reflection = 0;
		#endif
	}
	#pragma endregion

	#pragma region Methods
	void ShaderModelCom::Apply()
	{
		if (variableBuffer)
		{
			D3D11_MAPPED_SUBRESOURCE source;
			video->deviceContext->Map(variableBuffer, 0, D3D11_MAP_WRITE_DISCARD, NULL, &source);
			memcpy(source.pData, variableBufferBytes, variableBufferBytesCount);
			video->deviceContext->Unmap(variableBuffer, 0);
		}
	}

	#if WINDOWS
	int ShaderModelCom::Variable(string^ name)
	{
		char* namePtr = StringToAscii(name);
		ID3D11ShaderReflectionVariable* variable = variables->GetVariableByName(namePtr);
		delete namePtr;

		D3D11_SHADER_VARIABLE_DESC desc;
		ZeroMemory(&desc, sizeof(D3D11_SHADER_VARIABLE_DESC));
		variable->GetDesc(&desc);
		if (desc.Size != 0)
		{
			return desc.StartOffset;
		}

		return -1;
	}

	int ShaderModelCom::Resource(string^ name)
	{
		char* nameAscii = StringToAscii(name);
		for (uint i = 0; i != resourceCount; ++i)
		{
			D3D11_SHADER_INPUT_BIND_DESC desc;
			ZeroMemory(&desc, sizeof(D3D11_SHADER_INPUT_BIND_DESC));
			reflection->GetResourceBindingDesc(i, &desc);
			if (desc.Dimension != D3D11_SRV_DIMENSION_UNKNOWN && AsciiMatch((char*)desc.Name, nameAscii))
			{
				delete nameAscii;
				return desc.BindPoint;
			}
		}
		delete nameAscii;

		return -1;
	}

	string^ ShaderModelCom::Compile(string^ code, int codeSize, string^ shaderType, [Out] IntPtr% buffer, [Out] int% bufferSize)
	{
		char* shaderTypeName = StringToAscii(shaderType);
		char* codeAscii = StringToAscii(code);
		ID3DBlob *codeTEMP, *err = 0;
		if (FAILED(D3DCompile(codeAscii, codeSize, NULL, NULL, NULL, "main", shaderTypeName, D3DCOMPILE_OPTIMIZATION_LEVEL3, NULL, &codeTEMP, &err)))
		{
			if (shaderTypeName) delete shaderTypeName;
			if (codeAscii) delete codeAscii;
			if (err)
			{
				string^ error = gcnew string(AsciiToUnicode((char*)err->GetBufferPointer()));
				err->Release();
				return error;
			}

			return L"Failed with unknow error.";
		}
		if (shaderTypeName) delete shaderTypeName;
		if (codeAscii) delete codeAscii;
		buffer = IntPtr(codeTEMP->GetBufferPointer());
		bufferSize = codeTEMP->GetBufferSize();

		return nullptr;
	}
	#endif
	#pragma endregion
}