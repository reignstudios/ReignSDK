#include "pch.h"
#include "Model.h"
#include "Mesh.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	Model::Model(DisposableI^ parent, SoftwareModel^ softwareModel, MeshVertexSizes positionSize, DisposableI^ contentParent, string^ contentDirectory, Dictionary<string^,Type^>^ materialTypes, Dictionary<Type^,MaterialFieldBinder^>^ materialFieldTypes)
	: ModelI(parent, softwareModel, positionSize, contentParent, contentDirectory, materialTypes, materialFieldTypes)
	{
		
	}

	MeshI^ Model::createMesh(ModelI^ model, SoftwareModel^ softwareModel, SoftwareMesh^ softwareMesh, MeshVertexSizes positionSize)
	{
		return gcnew Mesh(model, softwareModel, softwareMesh, positionSize);
	}

	Texture2DI^ Model::createTexture(DisposableI^ parent, string^ fileName)
	{
		return Texture2D::New(parent, fileName);
	}
	#pragma endregion
}
}
}