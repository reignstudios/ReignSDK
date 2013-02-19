using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video
{
	public class SoftwareMaterial
	{
		#region Properties
		public string Name;
		public Dictionary<string,string> Textures;
		public Dictionary<string,float> Values1;
		public Dictionary<string,Vector2> Values2;
		public Dictionary<string,Vector3> Values3;
		public Dictionary<string,Vector4> Values4;
		#endregion

		#region Constructors
		public SoftwareMaterial(RMX_Material material)
		{
			Name = material.Name;

			// textures
			Textures = new Dictionary<string,string>();
			Values1 = new Dictionary<string,float>();
			Values2 = new Dictionary<string,Vector2>();
			Values3 = new Dictionary<string,Vector3>();
			Values4 = new Dictionary<string,Vector4>();
			foreach (var input in material.Inputs)
			{
				if (input.Type == "Value")
				{
					switch (input.Values.Length)
					{
						case 1: Values1.Add(input.ID, input.Values[0]); break;
						case 2: Values2.Add(input.ID, new Vector2(input.Values[0], input.Values[1])); break;
						case 3: Values3.Add(input.ID, new Vector3(input.Values[0], input.Values[1], input.Values[2])); break;
						case 4: Values4.Add(input.ID, new Vector4(input.Values[0], input.Values[1], input.Values[2], input.Values[3])); break;
					}
				}
				else if (input.Type == "Texture")
				{
					Textures.Add(input.ID, input.Content);
				}
			}
		}
		#endregion
	}
}