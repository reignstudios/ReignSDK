using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.D3D11
{
	public class Model : ModelI
	{
		#region Constructors
		public Model(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals, DisposableI contentParent, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialTextureBinder> materialFieldTypes, Dictionary<string,string> fileExtOverrides)
		: base(parent, softwareModel, positionSize, loadColors, loadUVs, loadNormals, contentParent, contentDirectory, materialTypes, materialFieldTypes, fileExtOverrides)
		{
			
		}

		protected override MeshI createMesh(ModelI model, SoftwareModel softwareModel, SoftwareMesh softwareMesh, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals)
		{
			return new Mesh(model, softwareModel, softwareMesh, positionSize, loadColors, loadUVs, loadNormals);
		}

		protected override Texture2DI createTexture(DisposableI parent, string fileName)
		{
			return Texture2D.New(parent, fileName);
		}
		#endregion
	}
}