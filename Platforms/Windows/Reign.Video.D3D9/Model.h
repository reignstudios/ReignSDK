#pragma once
#include "Texture2D.h"

using namespace System::Collections::Generic;
using namespace System::IO;

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class Model : ModelI
	{
		#pragma region Constructors
		public: Model(DisposableI^ parent, string^ fileName, string^ contentDirectory, Dictionary<string^,Type^>^ materialTypes, List<MaterialFieldBinder^>^ textureBinderTypes, Dictionary<string^,string^>^ fileExtOverrides);
		public: Model(DisposableI^ parent, SoftwareModel^ softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals, string^ contentDirectory, Dictionary<string^,Type^>^ materialTypes, List<MaterialFieldBinder^>^ textureBinderTypes, Dictionary<string^,string^>^ fileExtOverrides);
		protected: virtual MeshI^ createMesh(BinaryReader^ reader, ModelI^ model) override;
		protected: virtual Texture2DI^ createTexture(DisposableI^ parent, string^ fileName) override;
		#pragma endregion
	};
}
}
}