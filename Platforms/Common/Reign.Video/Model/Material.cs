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

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class MaterialField : Attribute
	{
		public MaterialFieldUsages Usage;

		public MaterialField(MaterialFieldUsages usage)
		{
			Usage = usage;
		}
	}

	public class MaterialFieldBinder
	{
		public string MaterialName, ID, ShaderMaterialFieldName;

		public MaterialFieldBinder(string materialName, string id, string shaderMaterialFieldName)
		{
			MaterialName = materialName;
			ID = id;
			ShaderMaterialFieldName = shaderMaterialFieldName;
		}
	}

	public interface MaterialI
	{
		string Name {get; set;}

		void Enable();
		void ApplyGlobalContants(MeshI mesh);
		void ApplyInstanceContants(MeshI mesh);
		void ApplyInstancingContants(MeshI mesh);
		void Apply(MeshI mesh);
	}
}