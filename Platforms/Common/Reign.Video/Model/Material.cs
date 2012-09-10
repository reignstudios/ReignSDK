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
		Texture2DI[] DiffuseTextures {get; set;}
		Texture2DI[] SpecularTextures {get; set;}
		Texture2DI[] EmissionTextures {get; set;}
		Vector4[] DiffuseColors {get; set;}
		Vector4[] SpecularColors {get; set;}
		Vector4[] EmissionColors {get; set;}
		float[] ShininessValues {get; set;}
		float[] IndexOfRefractionValues {get; set;}

		void Enable();
		void ApplyGlobalContants(MeshI mesh);
		void ApplyInstanceContants(MeshI mesh);
		void ApplyInstancingContants(MeshI mesh);
		void Apply(MeshI mesh);
	}
}