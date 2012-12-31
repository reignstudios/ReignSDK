#include "pch.h"
#include "ShaderModel.h"
#include <D3DX9Effect.h>

namespace Reign_Video_D3D9_Component
{
	string^ ShaderModelCom::Compile(string^ code, int codeSize, string^ shaderType, [Out] IntPtr% buffer, [Out] int% bufferSize)
	{
		IntPtr codePtr = Marshal::StringToHGlobalAnsi(code);
		IntPtr shaderVersionPtr = Marshal::StringToHGlobalAnsi(shaderType);

		ID3DXConstantTable* variablesTEMP = 0;
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
		}
		//this->code = codeTEMP;
		//variables = variablesTEMP;

		if (shaderVersionPtr != IntPtr::Zero) Marshal::FreeHGlobal(shaderVersionPtr);
		if (codePtr != IntPtr::Zero) Marshal::FreeHGlobal(codePtr);
		buffer = IntPtr(codeTEMP->GetBufferPointer());
		bufferSize = codeTEMP->GetBufferSize();
	}
}