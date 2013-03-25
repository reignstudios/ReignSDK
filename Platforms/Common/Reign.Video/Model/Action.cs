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

	public class KeyFrame
	{
		public Vector2 Cordinate {get; private set;}
		public InterpolationTypes InterpolationType {get; private set;}

		public KeyFrame(BinaryReader reader)
		{
			Cordinate = reader.ReadVector2();
			InterpolationType = (InterpolationTypes)reader.ReadInt32();
		}

		public static void Write(BinaryWriter writer, SoftwareKeyFrame softwareKeyFrame)
		{
			writer.WriteVector(softwareKeyFrame.Cordinate);
			writer.Write((int)softwareKeyFrame.InterpolationType);
		}
	}

	public class FCurve
	{
		#region Properties
		public FCurveTypes Type {get; private set;}
		public string DataPath {get; private set;}
		public int Index {get; private set;}
		public Vector2[] Cordinates {get; private set;}
		public InterpolationTypes[] InterpolationTypes {get; private set;}
		public KeyFrame[] KeyFrames {get; private set;}
		#endregion

		#region Constructors
		public FCurve(BinaryReader reader)
		{
			Type = (FCurveTypes)reader.ReadInt32();
			Index = reader.ReadInt32();
			DataPath = reader.ReadString();

			KeyFrames = new KeyFrame[reader.ReadInt32()];
			for (int i = 0; i != KeyFrames.Length; ++i)
			{
				KeyFrames[i] = new KeyFrame(reader);
			}
		}
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareFCurve softwareFCurve)
		{
			writer.Write((int)softwareFCurve.Type);
			writer.Write(softwareFCurve.Index);
			writer.Write(softwareFCurve.DataPath);

			writer.Write(softwareFCurve.KeyFrames.Count);
			foreach (var keyframe in softwareFCurve.KeyFrames)
			{
				KeyFrame.Write(writer, keyframe);
			}
		}

		public void GetKeyFrames(float frame, out KeyFrame start, out KeyFrame end)
		{
			// TODO: this method must be optamized
			for (int i = 0; i != KeyFrames.Length; ++i)
			{
				if (frame >= KeyFrames[i].Cordinate.X && frame <= KeyFrames[i+1].Cordinate.X)
				{
					start = KeyFrames[i];
					end = KeyFrames[i+1];
					return;
				}
			}

			start = null;
			end = null;
		}
		#endregion
	}

	public class Action
	{
		#region Properties
		public string Name {get; private set;}
		public float FrameStart {get; private set;}
		public float FrameEnd {get; private set;}
		public FCurve[] FCurves {get; private set;}
		#endregion

		#region Constructors
		public Action(BinaryReader reader)
		{
			Name = reader.ReadString();
			FrameStart = reader.ReadSingle();
			FrameEnd = reader.ReadSingle();

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
			writer.Write(softwareAction.FrameStart);
			writer.Write(softwareAction.FrameEnd);

			writer.Write(softwareAction.FCurves.Count);
			foreach (var curve in softwareAction.FCurves)
			{
				FCurve.Write(writer, curve);
			}
		}
		#endregion
	}
}