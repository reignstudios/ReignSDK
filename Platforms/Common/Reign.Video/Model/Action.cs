using System.Collections.Generic;
using Reign.Core;
using System;
using System.IO;

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

	public class FCurve
	{
		#region Properties
		public FCurveTypes Type;
		public string DataPath;
		public int Index;
		public Vector2[] Cordinates;
		public InterpolationTypes[] InterpolationTypes;
		#endregion

		#region Constructors
		public FCurve()
		{
			
		}
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareFCurve softwareFCurve)
		{
			writer.Write((int)softwareFCurve.Type);
			writer.Write(softwareFCurve.Index);
			writer.Write(softwareFCurve.DataPath);

			writer.Write(softwareFCurve.Cordinates.Count);
			foreach (var cord in softwareFCurve.Cordinates)
			{
				writer.WriteVector(cord);
			}

			writer.Write(softwareFCurve.InterpolationTypes.Count);
			foreach (var type in softwareFCurve.InterpolationTypes)
			{
				writer.Write((int)type);
			}
		}
		#endregion
	}

	public class Action
	{
		#region Properties
		public string Name;
		public FCurve[] FCurves;
		#endregion

		#region Constructors
		public Action()
		{
			
		}
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareAction softwareAction)
		{
			writer.Write(softwareAction.Name);

			writer.Write(softwareAction.FCurves.Count);
			foreach (var curve in softwareAction.FCurves)
			{
				FCurve.Write(writer, curve);
			}
		}
		#endregion
	}
}