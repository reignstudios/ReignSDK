using System.Collections.Generic;
using Reign.Core;
using System.IO;

namespace Reign.Video
{
	public class ObjectArmature : Object
	{
		#region Properties
		public Armature Armature;
		#endregion

		#region Constructors
		public ObjectArmature(BinaryReader reader, Model model)
		: base(reader, model)
		{
			string armatureName = reader.ReadString();
			foreach (var armature in model.Armatures)
			{
				if (armatureName == armature.Name)
				{
					Armature = armature;
					break;
				}
			}
		}
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareObjectArmature softwareObjectArmature)
		{
			writer.Write(softwareObjectArmature.Armature.Name);
		}
		#endregion
	}
}