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
		void ApplyInstanceContants(ObjectMesh objectMesh);
		void ApplyInstanceContants(InstanceObjectMesh instanceObjectMesh);
		void Apply(ObjectMesh objectMesh);
		void Apply(InstanceObjectMesh instanceObjectMesh);
	}
}