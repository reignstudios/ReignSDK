using System.Collections.Generic;
using Reign.Core;
using System.IO;

namespace Reign.Video
{
	public class ObjectArmature : Object
	{
		#region Properties
		public string Name;
		public Armature Armature;
		#endregion

		#region Constructors
		public ObjectArmature(BinaryReader reader, Model model)
		: base(reader, model)
		{
			
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