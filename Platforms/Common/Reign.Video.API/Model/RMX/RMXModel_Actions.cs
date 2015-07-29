using System.Xml.Serialization;
using System;

namespace Reign.Video.Abstraction
{
	public class RMX_ActionFCurveCoordinates
	{
		[XmlText] public string Content;
		public float[] Values;

		public void Init()
		{
			Values = RMXModel.ConvertToFloatArray(Content);
		}
	}

	public class RMX_ActionFCurveInterpolationTypes
	{
		[XmlText] public string Content;
	}

	public class RMX_ActionFCurve
	{
		[XmlAttribute("Type")] public string Type;
		[XmlAttribute("DataPath")] public string DataPath;
		[XmlAttribute("Index")] public int Index;
		[XmlElement("Coordinates")] public RMX_ActionFCurveCoordinates Coordinates;
		[XmlElement("InterpolationType")] public RMX_ActionFCurveInterpolationTypes InterpolationTypes;

		public void Init()
		{
			Coordinates.Init();
		}
	}

	public class RMX_Action
	{
		[XmlAttribute("Name")] public string Name;
		[XmlAttribute("FrameStart")] public float FrameStart;
		[XmlAttribute("FrameEnd")] public float FrameEnd;
		[XmlElement("FCurve")] public RMX_ActionFCurve[] FCurves;

		public void Init()
		{
			if (FCurves != null)
			{
				foreach (var curve in FCurves)
				{
					curve.Init();
				}
			}
		}
	}

	public class RMX_Actions
	{
		[XmlElement("Action")] public RMX_Action[] Actions;

		public void Init()
		{
			if (Actions != null)
			{
				foreach (var action in Actions)
				{
					action.Init();
				}
			}
		}
	}
}