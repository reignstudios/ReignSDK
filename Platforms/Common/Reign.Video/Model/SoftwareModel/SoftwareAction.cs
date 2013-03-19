using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video
{
	public enum ActionTypes
	{
		Object,
		Bone
	}

	public enum InterpolationTypes
	{
		Bezier,
		Linear,
		Constant
	}

	public class SoftwareAction
	{
		#region Properties
		public ActionTypes Type;
		// datapath
		// index

		public float[] FCures;
		public InterpolationTypes[] InterpolationTypes;
		#endregion

		#region Constructors
		public SoftwareAction()
		{
			
		}
		#endregion

		#region Methods
		
		#endregion
	}
}