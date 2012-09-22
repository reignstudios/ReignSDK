#include "pch.h"
#include "Model.h"
#include "Mesh.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	Model::Model(DisposableI^ parent, string^ fileName, string^ contentDirectory, Dictionary<string^,Type^>^ materialTypes, List<MaterialFieldBinder^>^ value1BinderTypes, List<MaterialFieldBinder^>^ value2BinderTypes, List<MaterialFieldBinder^>^ value3BinderTypes, List<MaterialFieldBinder^>^ value4BinderTypes, List<MaterialFieldBinder^>^ textureBinderTypes, Dictionary<string^,string^>^ fileExtOverrides)
	: ModelI(parent, fileName, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides)
	{
		
	}

	Model::Model(DisposableI^ parent, SoftwareModel^ softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals, string^ contentDirectory, Dictionary<string^,Type^>^ materialTypes, List<MaterialFieldBinder^>^ value1BinderTypes, List<MaterialFieldBinder^>^ value2BinderTypes, List<MaterialFieldBinder^>^ value3BinderTypes, List<MaterialFieldBinder^>^ value4BinderTypes, List<MaterialFieldBinder^>^ textureBinderTypes, Dictionary<string^,string^>^ fileExtOverrides)
	: ModelI(parent, softwareModel, positionSize, loadColors, loadUVs, loadNormals, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides)
	{
		
	}

	MeshI^ Model::createMesh(BinaryReader^ reader, ModelI^ model)
	{
		return gcnew Mesh(reader, model);
	}

	Texture2DI^ Model::createTexture(DisposableI^ parent, string^ fileName)
	{
		return Texture2D::New(parent, fileName);
	}
	#pragma endregion
}
}
}