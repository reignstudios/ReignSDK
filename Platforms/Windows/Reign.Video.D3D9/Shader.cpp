#include "pch.h"
#include "Shader.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	ref class ShaderStreamLoader : StreamLoaderI
	{
		private: Shader^ shader;
		private: string^ fileName;
		private: ShaderVersions shaderVersion;

		public: ShaderStreamLoader(Shader^ shader, string^ fileName, ShaderVersions shaderVersion)
		{
			this->shader = shader;
			this->fileName = fileName;
			this->shaderVersion = shaderVersion;
		}

		public: virtual bool Load() override
		{
			shader->load(fileName, shaderVersion);
			return true;
		}
	};

	#pragma region Properties
	VertexShader^ Shader::Vertex::get() {return vertex;}
	PixelShader^ Shader::Pixel::get() {return pixel;}
	#pragma endregion

	#pragma region Constructors
	Shader^ Shader::New(DisposableI^ parent, string^ fileName, ShaderVersions shaderVersion)
	{
		Shader^ shader = parent->FindChild<Shader^>(gcnew string(L"New"),
			gcnew ConstructorParam(DisposableI::typeid, parent),
			gcnew ConstructorParam(string::typeid, fileName),
			gcnew ConstructorParam(ShaderVersions::typeid, shaderVersion));

		if (shader)
		{
			++shader->referenceCount;
			return shader;
		}

		return gcnew Shader(parent, fileName, shaderVersion);
	}

	Shader::Shader(DisposableI^ parent, string^ fileName, ShaderVersions shaderVersion)
	: ShaderI(parent)
	{
		video = parent->FindParentOrSelfWithException<Video^>();
		gcnew ShaderStreamLoader(this, fileName, shaderVersion);
	}

	void Shader::load(string^ fileName, ShaderVersions shaderVersion)
	{
		try
		{
			array<string^>^ code = getShaders(fileName);
			vertex = gcnew VertexShader(this, code[0], (shaderVersion == ShaderVersions::Max) ? video->Caps->MaxVertexShaderVersion : shaderVersion);
			pixel = gcnew PixelShader(this, code[1], (shaderVersion == ShaderVersions::Max) ? video->Caps->MaxPixelShaderVersion : shaderVersion);

			variables = gcnew List<ShaderVariable^>();
			resources = gcnew List<ShaderResource^>();
			Loaded = true;
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
	}
	#pragma endregion

	#pragma region Methods
	void Shader::Apply()
	{
		vertex->Apply();
		pixel->Apply();
		
		for each (ShaderVariable^ variable in variables)
		{
			variable->Apply();
		}

		for each (ShaderResource^ resource in resources)
		{
			resource->Apply();
		}
	}

	ShaderVariableI^ Shader::Variable(string^ name)
	{
		// Try to find existing variable
		for each (ShaderVariable^ variable in variables)
		{
			if (variable->Name == name) return variable;
		}
		
		// Otherwise add a variable instance
		D3DXHANDLE vertexVariable = vertex->Variable(name);
		D3DXHANDLE pixelVariable = pixel->Variable(name);

		if (vertexVariable == 0 && pixelVariable == 0)
		{
			Debug::ThrowError(L"Shader", System::String::Format(L"Shader variable '{0}' does not exist", name));
		}

		ShaderVariable^ variable = gcnew ShaderVariable(video, vertexVariable, pixelVariable, vertex->Variables, pixel->Variables, name);
		variables->Add(variable);
		return variable;
	}

	ShaderResourceI^ Shader::Resource(string^ name)
	{
		// Try to find existing variable
		for each (ShaderResource^ resource in resources)
		{
			if (resource->Name == name) return resource;
		}

		// Otherwise add a variable instance
		int vertexIndex = vertex->Resource(name);
		int pixelIndex = pixel->Resource(name);

		if (vertexIndex == -1 && pixelIndex == -1)
		{
			Debug::ThrowError(L"Shader", System::String::Format(L"Shader resource '{0}' does not exist.", name));
		}

		ShaderResource^ resource = gcnew ShaderResource(video, vertexIndex, pixelIndex, name);
		resources->Add(resource);
		return resource;
	}
	#pragma endregion
}
}
}