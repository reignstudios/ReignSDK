using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class RMX_MeshVertexChannel
	{
		[XmlAttribute("ID")] public string ID;
		[XmlAttribute("Index")] public int Index;
		[XmlAttribute("Step")] public int Step;
		[XmlText] public string Content;
		public float[] Values;

		public void Init()
		{
			Values = RMXModel.ConvertToFloatArray(Content);
		}
	}

	public class RMX_MeshVerticies
	{
		[XmlElement("Channel")] public RMX_MeshVertexChannel[] Channels;

		public void Init()
		{
			foreach (var channel in Channels)
			{
				channel.Init();
			}
		}
	}

	public class RMX_MeshFaceIndices
	{
		[XmlAttribute("ID")] public string ID;
		[XmlText] public string Content;
		public int[] Values;

		public void Init()
		{
			Values = RMXModel.ConvertToIntArray(Content);
		}
	}

	public class RMX_MeshFaceSteps
	{
		[XmlText] public string Content;
		public int[] Values;

		public void Init()
		{
			Values = RMXModel.ConvertToIntArray(Content);
		}
	}

	public class RMX_MeshFaces
	{
		[XmlElement("Steps")] public RMX_MeshFaceSteps Steps;
		[XmlElement("Indices")] public RMX_MeshFaceIndices[] Indices;

		public void Init()
		{
			Steps.Init();

			if (Indices != null)
			{
				foreach (var i in Indices)
				{
					i.Init();
				}
			}
		}
	}

	public class RMX_Mesh
	{
		[XmlAttribute("Name")] public string Name;
		[XmlAttribute("Material")] public string Material;
		[XmlElement("Vertices")] public RMX_MeshVerticies Verticies;
		[XmlElement("Faces")] public RMX_MeshFaces Faces;

		public void Init()
		{
			if (Verticies != null) Verticies.Init();
			if (Faces != null) Faces.Init();
		}
	}

	public class RMX_Meshes
	{
		[XmlElement("Mesh")] public RMX_Mesh[] Meshes;

		public void Init()
		{
			if (Meshes != null)
			{
				foreach (var mesh in Meshes)
				{
					mesh.Init();
				}
			}
		}
	}
}