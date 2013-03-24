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
		public FCurveTypes Type {get; private set;}
		public string DataPath {get; private set;}
		public int Index {get; private set;}
		public Vector2[] Cordinates {get; private set;}
		public InterpolationTypes[] InterpolationTypes {get; private set;}
		#endregion

		#region Constructors
		public FCurve(BinaryReader reader)
		{
			Type = (FCurveTypes)reader.ReadInt32();
			Index = reader.ReadInt32();
			DataPath = reader.ReadString();

			Cordinates = new Vector2[reader.ReadInt32()];
			for (int i = 0; i != Cordinates.Length; ++i)
			{
				Cordinates[i] = reader.ReadVector2();
			}

			InterpolationTypes = new InterpolationTypes[reader.ReadInt32()];
			for (int i = 0; i != Cordinates.Length; ++i)
			{
				InterpolationTypes[i] = (InterpolationTypes)reader.ReadInt32();
			}
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
		public string Name {get; private set;}
		public FCurve[] FCurves {get; private set;}
		#endregion

		#region Constructors
		public Action(BinaryReader reader)
		{
			Name = reader.ReadString();

			FCurves = new FCurve[reader.ReadInt32()];
			for (int i = 0; i != FCurves.Length; ++i)
			{
				FCurves[i] = new FCurve(reader);
			}
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