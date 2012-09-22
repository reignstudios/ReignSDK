using System;
using Reign.Core;
using System.Collections.Generic;
using System.IO;

namespace Reign.Video.D3D11
{
	public class Model : ModelI
	{
		#region Constructors
		public Model(DisposableI parent, string fileName, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides)
		: base(parent, fileName, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides)
		{
			
		}

		public Model(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides)
		: base(parent, softwareModel, positionSize, loadColors, loadUVs, loadNormals, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides)
		{
			
		}

		protected override MeshI createMesh(BinaryReader reader, ModelI model)
		{
			return new Mesh(reader, model);
		}

		protected override Texture2DI createTexture(DisposableI parent, string fileName)
		{
			return Texture2D.New(parent, fileName);
		}
		#endregion
	}
}