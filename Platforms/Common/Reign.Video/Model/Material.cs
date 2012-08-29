using Reign.Core;
using System;

namespace Reign.Video
{
	public enum MaterialFieldTypes
	{
		None,
		Diffuse,
		Specular,
		Emission,
		Shininess,
		IndexOfRefraction
	}

	public enum MaterialFieldUsages
	{
		None,
		Global,
		Instance,
		Instancing
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class MaterialField : Attribute
	{
		public MaterialFieldTypes Type;
		public MaterialFieldUsages Usage;

		public MaterialField(MaterialFieldTypes type, MaterialFieldUsages usage)
		{
			Type = type;
			Usage = usage;
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