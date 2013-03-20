using System.Collections.Generic;
using Reign.Core;
using System;

namespace Reign.Video
{
	public enum FCurveTypes
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

	public class SoftwareFCurve
	{
		public FCurveTypes Type;
		public string DataPath;
		public int Index;
		public List<Vector2> Cordinates;
		public List<InterpolationTypes> InterpolationTypes;

		public SoftwareFCurve(RMX_ActionFCurve fcurve)
		{
			switch (fcurve.Type)
			{
				case "OBJECT": Type = FCurveTypes.Object; break;
				case "BONE": Type = FCurveTypes.Bone; break;
				default: Debug.ThrowError("", "Unsuported type: " + fcurve.Type); break;
			}

			DataPath = fcurve.DataPath;
			Index = fcurve.Index;

			var values = fcurve.Coordinates.Values;
			Cordinates = new List<Vector2>();
			int i2 = 0, loop = values.Length / 2;
			for (int i = 0; i != loop; ++i)
			{
				Cordinates.Add(new Vector2(values[i2], values[i2+1]));
				i2 += 2;
			}

			var types = fcurve.InterpolationTypes.Content;
			InterpolationTypes = new List<InterpolationTypes>();
			for (int i = 0; i != types.Length; ++i)
			{
				switch (types[i])
				{
					case 'B': InterpolationTypes.Add(Video.InterpolationTypes.Bezier); break;
					case 'L': InterpolationTypes.Add(Video.InterpolationTypes.Linear); break;
					case 'C': InterpolationTypes.Add(Video.InterpolationTypes.Constant); break;
				}
			}
		}
	}

	public class SoftwareAction
	{
		#region Properties
		public string Name;
		public List<SoftwareFCurve> FCurves;
		#endregion

		#region Constructors
		public SoftwareAction(RMX_Action action)
		{
			Name = action.Name;

			FCurves = new List<SoftwareFCurve>();
			foreach (var fcurve in action.FCurves)
			{
				FCurves.Add(new SoftwareFCurve(fcurve));
			}
		}
		#endregion
	}
}