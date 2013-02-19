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
		public string MaterialName, InputID, ShaderMaterialFieldName;

		public MaterialFieldBinder(string materialName, string inputID, string shaderMaterialFieldName)
		{
			MaterialName = materialName;
			InputID = inputID;
			ShaderMaterialFieldName = shaderMaterialFieldName;
		}
	}

	public interface MaterialI
	{
		string Name {get; set;}

		void Enable();
		void ApplyGlobalContants(Mesh mesh);
		void ApplyInstanceContants(Mesh mesh);
		void ApplyInstancingContants(Mesh mesh);
		void Apply(Mesh mesh);
	}
}