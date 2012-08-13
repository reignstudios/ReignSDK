#include "pch.h"
#include "ShaderModel.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Properites
	ID3DXConstantTable* ShaderModel::Variables::get() {return variables;}
	#pragma endregion

	#pragma region Constructors
	ShaderModel::ShaderModel(ShaderI^ shader, string^ code, ShaderVersions shaderVersion, ShaderTypes shaderType)
	: Disposable(shader)
	{
		null();
		IntPtr codePtr = IntPtr::Zero, shaderVersionPtr = IntPtr::Zero;

		try
		{
			video = shader->FindParentOrSelfWithException<Video^>();

			codePtr = Marshal::StringToHGlobalAnsi(code);
			string^ shaderLvl = L"";
			switch (shaderVersion)
			{
				case (ShaderVersions::HLSL_2_0): shaderLvl = "_2_0"; break;
				case (ShaderVersions::HLSL_2_a): shaderLvl = "_2_a"; break;
				case (ShaderVersions::HLSL_3_0): shaderLvl = "_3_0"; break;
			}
			shaderVersionPtr = Marshal::StringToHGlobalAnsi(shaderType.ToString()->ToLower() + shaderLvl);

			ID3DXConstantTable* variablesTEMP = 0;
			ID3DXBuffer *codeTEMP = 0,  *err = 0;
			if (FAILED(D3DXCompileShader((char*)codePtr.ToPointer(), code->Length, 0, 0, "main", (char*)shaderVersionPtr.ToPointer(), D3DXSHADER_OPTIMIZATION_LEVEL3, &codeTEMP, &err, &variablesTEMP)))
			{
				string^ errString;
				if (err)
				{
					errString = gcnew string(static_cast<char*>(err->GetBufferPointer()));
					err->Release();
				}
				Debug::ThrowError(L"ShaderModel", System::String::Format(L"{0} Shader compile error: {1}", shaderType, errString));
			}
			this->code = codeTEMP;
			variables = variablesTEMP;
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
		finally
		{
			if (codePtr != IntPtr::Zero) Marshal::FreeHGlobal(codePtr);
			if (shaderVersionPtr != IntPtr::Zero) Marshal::FreeHGlobal(shaderVersionPtr);
		}
	}

	ShaderModel::~ShaderModel()
	{
		disposeChilderen();
		if (code) code->Release();
		if (variables) variables->Release();
		null();
	}

	void ShaderModel::null()
	{
		code = 0;
		variables = 0;
	}
	#pragma endregion

	#pragma region Methods
	D3DXHANDLE ShaderModel::Variable(string^ name)
	{
		if (!variables) return 0;

		IntPtr namePtr = Marshal::StringToHGlobalAnsi(name);
		D3DXHANDLE variable = variables->GetConstantByName(0, (char*)namePtr.ToPointer());
		Marshal::FreeHGlobal(namePtr);

		return variable;
	}

	int ShaderModel::Resource(string^ name)
	{
		D3DXHANDLE variable = Variable(name);

		if (variable != 0)
		{
			D3DXCONSTANT_DESC desc;
			ZeroMemory(&desc, sizeof(D3DXCONSTANT_DESC));
			uint count = 1;
			if (!FAILED(variables->GetConstantDesc(variable, &desc, &count)))// USE GetSamplerIndex
			{
				return desc.RegisterIndex;
			}
		}

		return -1;
	}
	#pragma endregion
}
}
}