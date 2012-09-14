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
		public: Model(DisposableI^ parent, SoftwareModel^ softwareModel, MeshVertexSizes positionSize, DisposableI^ contentParent, string^ contentDirectory, Dictionary<string^,Type^>^ materialTypes, Dictionary<Type^,MaterialFieldBinder^>^ materialFieldTypes);
		protected: virtual MeshI^ createMesh(ModelI^ model, SoftwareModel^ softwareModel, SoftwareMesh^ softwareMesh, MeshVertexSizes positionSize) override;
		protected: virtual Texture2DI^ createTexture(DisposableI^ parent, string^ fileName) override;
		#pragma endregion
	};
}
}
}