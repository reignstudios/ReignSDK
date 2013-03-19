using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video
{
	public class SoftwareBone
	{
		public string Name;
		public SoftwareBone Parent;
		public bool InheritScale, InheritRotation;
		public Vector3 Position;
		public Matrix3 Orientation;
	}

	public class SoftwareArmature
	{
		#region Properties
		public SoftwareBone[] Bones;
		public SoftwareAction DefaultAction;
		#endregion

		#region Constructors
		public SoftwareArmature()
		{
			
		}
		#endregion

		#region Methods
		
		#endregion
	}
}