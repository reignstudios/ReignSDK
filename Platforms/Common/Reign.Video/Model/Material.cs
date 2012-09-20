using Reign.Core;
using System;
using System.Collections.Generic;

namespace Reign.Video
{
	public enum MaterialFieldUsages
	{
		Global,
		Instance,
		Instancing
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class MaterialField : Attribute
	{
		public MaterialFieldUsages Usage;

		public MaterialField(MaterialFieldUsages usage)
		{
			Usage = usage;
		}
	}

	public class MaterialTextureBinder
	{
		public string MaterialName, TextureID, ShaderMaterialFieldName;

		public MaterialTextureBinder(string materialName, string textureID, string shaderMaterialFieldName)
		{
			MaterialName = materialName;
			TextureID = textureID;
			ShaderMaterialFieldName = shaderMaterialFieldName;
		}
	}

	public interface MaterialI
	{
		void Enable();
		void ApplyGlobalContants(MeshI mesh);
		void ApplyInstanceContants(MeshI mesh);
		void ApplyInstancingContants(MeshI mesh);
		void Apply(MeshI mesh);
	}
}