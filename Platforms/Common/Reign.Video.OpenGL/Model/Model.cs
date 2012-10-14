using System;
using Reign.Core;
using System.Collections.Generic;
using System.IO;

namespace Reign.Video.OpenGL
{
	public class Model : ModelI
	{
		#region Constructors
		public Model(DisposableI parent, string fileName, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides, int classicInstanceCount)
		: base(parent, fileName, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides, classicInstanceCount)
		{
			
		}

		public Model(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides, int classicInstanceCount)
		: base(parent, softwareModel, positionSize, loadColors, loadUVs, loadNormals, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides, classicInstanceCount)
		{
			
		}

		protected override MeshI createMesh(BinaryReader reader, ModelI model, int classicInstanceCount)
		{
			return new Mesh(reader, model, classicInstanceCount);
		}

		protected override Texture2DI createTexture(DisposableI parent, string fileName)
		{
			return Texture2D.New(parent, fileName);
		}
		#endregion
	}
}