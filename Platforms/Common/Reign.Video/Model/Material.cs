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

	public class MaterialFieldBinder
	{
		public string ID, MaterialFieldName;

		public MaterialFieldBinder(string id, string materialFieldName)
		{
			ID = id;
			MaterialFieldName = materialFieldName;
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