using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video.Abstraction
{
	public class SoftwareObjectArmature : SoftwareObject
	{
		#region Properties
		public SoftwareArmature Armature;
		#endregion

		#region Constructors
		public SoftwareObjectArmature(SoftwareModel model, RMX_Object o)
		: base(model, o)
		{
			// find armature
			foreach (var armature in model.Armatures)
			{
				if (o.Armature.Name == armature.Name)
				{
					Armature = armature;
					break;
				}
			}
			if (Armature == null) Debug.ThrowError("SoftwareObjectArmature", "Failed to find Armature: " + o.Armature.Name);
		}
		#endregion
	}
}