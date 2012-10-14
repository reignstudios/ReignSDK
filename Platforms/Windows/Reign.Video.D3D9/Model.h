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
		public: Model(DisposableI^ parent, string^ fileName, string^ contentDirectory, Dictionary<string^,Type^>^ materialTypes, List<MaterialFieldBinder^>^ value1BinderTypes, List<MaterialFieldBinder^>^ value2BinderTypes, List<MaterialFieldBinder^>^ value3BinderTypes, List<MaterialFieldBinder^>^ value4BinderTypes, List<MaterialFieldBinder^>^ textureBinderTypes, Dictionary<string^,string^>^ fileExtOverrides, int classicInstanceCount);
		public: Model(DisposableI^ parent, SoftwareModel^ softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals, string^ contentDirectory, Dictionary<string^,Type^>^ materialTypes, List<MaterialFieldBinder^>^ value1BinderTypes, List<MaterialFieldBinder^>^ value2BinderTypes, List<MaterialFieldBinder^>^ value3BinderTypes, List<MaterialFieldBinder^>^ value4BinderTypes, List<MaterialFieldBinder^>^ textureBinderTypes, Dictionary<string^,string^>^ fileExtOverrides, int classicInstanceCount);
		protected: virtual MeshI^ createMesh(BinaryReader^ reader, ModelI^ model, int classicInstanceCount) override;
		protected: virtual Texture2DI^ createTexture(DisposableI^ parent, string^ fileName) override;
		#pragma endregion
	};
}
}
}