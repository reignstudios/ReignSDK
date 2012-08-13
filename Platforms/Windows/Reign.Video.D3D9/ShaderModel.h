#pragma once
#include "Video.h"

using namespace System::Runtime::InteropServices;

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class ShaderModel abstract : Disposable
	{
		#pragma region Properties
		protected: Video^ video;
		protected: ID3DXBuffer* code;

		private: ID3DXConstantTable* variables;
		public: property ID3DXConstantTable* Variables {ID3DXConstantTable* get();}
		#pragma endregion

		#pragma region Constructors
		public: ShaderModel(ShaderI^ shader, string^ code, ShaderVersions shaderVersion, ShaderTypes shaderType);
		public: ~ShaderModel();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: D3DXHANDLE Variable(string^ name);
		public: int Resource(string^ name);
		#pragma endregion
	};
}
}
}