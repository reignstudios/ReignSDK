#include "pch.h"
#include "Model.h"
#include "Mesh.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	Model::Model(DisposableI^ parent, SoftwareModel^ softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals, DisposableI^ contentParent, string^ contentDirectory, Dictionary<string^,Type^>^ materialTypes, List<MaterialTextureBinder^>^ materialFieldTypes, Dictionary<string^,string^>^ fileExtOverrides)
	: ModelI(parent, softwareModel, positionSize, loadColors, loadUVs, loadNormals, contentParent, contentDirectory, materialTypes, materialFieldTypes, fileExtOverrides)
	{
		
	}

	MeshI^ Model::createMesh(ModelI^ model, SoftwareModel^ softwareModel, SoftwareMesh^ softwareMesh, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals)
	{
		return gcnew Mesh(model, softwareModel, softwareMesh, positionSize, loadColors, loadUVs, loadNormals);
	}

	Texture2DI^ Model::createTexture(DisposableI^ parent, string^ fileName)
	{
		return Texture2D::New(parent, fileName);
	}
	#pragma endregion
}
}
}