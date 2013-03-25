using System.Collections.Generic;
using Reign.Core;
using System;

namespace Reign.Video
{
	public class SoftwareKeyFrame
	{
		public Vector2 Cordinate {get; private set;}
		public InterpolationTypes InterpolationType {get; private set;}

		public SoftwareKeyFrame(Vector2 cordinate, InterpolationTypes interpolationType)
		{
			Cordinate = cordinate;
			InterpolationType = interpolationType;
		}
	}

	public class SoftwareFCurve
	{
		public FCurveTypes Type;
		public string DataPath;
		public int Index;
		public List<SoftwareKeyFrame> KeyFrames;

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
			var types = fcurve.InterpolationTypes.Content;
			KeyFrames = new List<SoftwareKeyFrame>();
			int i2 = 0, loop = values.Length / 2;
			for (int i = 0; i != loop; ++i)
			{
				InterpolationTypes interpolationType = InterpolationTypes.Bezier;
				switch (types[i])
				{
					case 'B': interpolationType = InterpolationTypes.Bezier; break;
					case 'L': interpolationType = InterpolationTypes.Linear; break;
					case 'C': interpolationType = InterpolationTypes.Constant; break;
				}

				KeyFrames.Add(new SoftwareKeyFrame(new Vector2(values[i2], values[i2+1]), interpolationType));
				i2 += 2;
			}
		}
	}

	public class SoftwareAction
	{
		#region Properties
		public string Name;
		public float FrameStart, FrameEnd;
		public List<SoftwareFCurve> FCurves;
		#endregion

		#region Constructors
		public SoftwareAction(RMX_Action action)
		{
			Name = action.Name;
			FrameStart = action.FrameStart;
			FrameEnd = action.FrameEnd;

			FCurves = new List<SoftwareFCurve>();
			foreach (var fcurve in action.FCurves)
			{
				FCurves.Add(new SoftwareFCurve(fcurve));
			}
		}
		#endregion
	}
}