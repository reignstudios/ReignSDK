#include "pch.h"
#include "ShaderModel.h"
//#include <D3DX9Effect.h>// TODO load effects from compiled files

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	ShaderModelErrors ShaderModelCom::Init(VideoCom^ video, IntPtr codePtr, int codeLength, IntPtr shaderVersionTypePtr, [Out] string^% errorText)
	{
		/*null();
		this->video = video;

		ID3DXConstantTable* variablesTEMP = 0;
		ID3DXBuffer *codeTEMP = 0,  *err = 0;
		if (FAILED(D3DXCompileShader((char*)codePtr.ToPointer(), codeLength, 0, 0, "main", (char*)shaderVersionTypePtr.ToPointer(), D3DXSHADER_OPTIMIZATION_LEVEL3, &codeTEMP, &err, &variablesTEMP)))
		{
			if (err)
			{
				errorText = gcnew string(static_cast<char*>(err->GetBufferPointer()));
				err->Release();
			}
			return ShaderModelErrors::Compile;
		}
		code = codeTEMP;
		variables = variablesTEMP;

		errorText = nullptr;*/
		return ShaderModelErrors::None;
	}

	ShaderModelCom::~ShaderModelCom()
	{
		//if (code) code->Release();
		//if (variables) variables->Release();
		null();
	}

	void ShaderModelCom::null()
	{
		//code = 0;
		//variables = 0;
	}
	#pragma endregion

	#pragma region Methods
	/*D3DXHANDLE ShaderModelCom::variableHandle(string^ name)
	{
		if (!variables) return 0;

		IntPtr namePtr = Marshal::StringToHGlobalAnsi(name);
		D3DXHANDLE variable = variables->GetConstantByName(0, (char*)namePtr.ToPointer());
		Marshal::FreeHGlobal(namePtr);

		return variable;
	}*/

	IntPtr ShaderModelCom::Variable(string^ name)
	{
		return IntPtr::Zero;//IntPtr((void*)variableHandle(name));
	}

	int ShaderModelCom::Resource(string^ name)
	{
		//D3DXHANDLE variable = variableHandle(name);
		//if (variable != 0) return variables->GetSamplerIndex(variable);

		return -1;
	}

	string^ ShaderModelCom::Compile(string^ code, int codeSize, string^ shaderType, [Out] IntPtr% buffer, [Out] int% bufferSize)
	{
		IntPtr codePtr = Marshal::StringToHGlobalAnsi(code);
		IntPtr shaderVersionPtr = Marshal::StringToHGlobalAnsi(shaderType);

		/*ID3DXConstantTable* variablesTEMP = 0;
		ID3DXBuffer *codeTEMP = 0,  *err = 0;
		if (FAILED(D3DXCompileShader((char*)codePtr.ToPointer(), code->Length, 0, 0, "main", (char*)shaderVersionPtr.ToPointer(), D3DXSHADER_OPTIMIZATION_LEVEL3, &codeTEMP, &err, &variablesTEMP)))
		{
			string^ errString;
			if (err)
			{
				errString = gcnew string(static_cast<char*>(err->GetBufferPointer()));
				err->Release();
				return errString;
			}

			return L"Failed with unknow error.";
		}*/
		//this->code = codeTEMP;
		//variables = variablesTEMP;

		if (shaderVersionPtr != IntPtr::Zero) Marshal::FreeHGlobal(shaderVersionPtr);
		if (codePtr != IntPtr::Zero) Marshal::FreeHGlobal(codePtr);
		//buffer = IntPtr(codeTEMP->GetBufferPointer());
		//bufferSize = codeTEMP->GetBufferSize();

		return L"";
	}
	#pragma endregion
}