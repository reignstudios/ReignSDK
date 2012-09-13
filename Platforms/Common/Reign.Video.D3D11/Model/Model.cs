using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.D3D11
{
	public class Model : ModelI
	{
		#region Constructors
		public Model(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, DisposableI contentParent, string contentDirectory, Dictionary<string,Type> materialTypes, Dictionary<Type,MaterialFieldBinder> materialFieldTypes)
		: base(parent, softwareModel, positionSize, contentParent, contentDirectory, materialTypes, materialFieldTypes)
		{
			
		}

		protected override MeshI createMesh(ModelI model, SoftwareModel softwareModel, SoftwareMesh softwareMesh, MeshVertexSizes positionSize)
		{
			return new Mesh(model, softwareModel, softwareMesh, positionSize);
		}

		protected override Texture2DI createTexture(DisposableI parent, string fileName)
		{
			return Texture2D.New(parent, fileName);
		}
		#endregion
	}
}