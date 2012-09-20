#pragma once
#include "Texture2D.h"
using namespace System::Collections::Generic;

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class Model : ModelI
	{
		#pragma region Constructors
		public: Model(DisposableI^ parent, SoftwareModel^ softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals, DisposableI^ contentParent, string^ contentDirectory, Dictionary<string^,Type^>^ materialTypes, List<MaterialTextureBinder^>^ materialFieldTypes, Dictionary<string^,string^>^ fileExtOverrides);
		protected: virtual MeshI^ createMesh(ModelI^ model, SoftwareModel^ softwareModel, SoftwareMesh^ softwareMesh, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals) override;
		protected: virtual Texture2DI^ createTexture(DisposableI^ parent, string^ fileName) override;
		#pragma endregion
	};
}
}
}