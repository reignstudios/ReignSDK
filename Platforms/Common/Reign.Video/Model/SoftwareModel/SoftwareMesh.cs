using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video
{
	public enum VertexComponentKeyTypes
	{
		Positions
	}

	public enum TriangleComponentKeyTypes
	{
		Normals,
		NormalComponents,
		UVComponents,
		ColorComponents
	}

	public enum EdgeComponentKeyTypes
	{
		
	}

	public class SoftwareMesh
	{
		#region Properties
		public SoftwareModel Model;
		public string Name;
		public Vector3 Position, Rotation, Scale;
		public SoftwareMaterial Material;

		public List<SoftwareVertex> Verticies;
		public List<SoftwareTriangle> Triangles;
		public List<SoftwareEdge> Edges;

		public List<object> VetexComponents, TriangleComponents, EdgeComponents;
		public Dictionary<VertexComponentKeyTypes, int> VertexComponentKeys;
		public Dictionary<TriangleComponentKeyTypes, int> TriangleComponentKeys;
		public Dictionary<EdgeComponentKeyTypes, int> EdgeComponentKeys;
		#endregion

		#region Constructors
		public SoftwareMesh(SoftwareModel model, RMX_Mesh mesh, string name)
		{
			Model = model;
			Name = name;

			Verticies = new List<SoftwareVertex>();
			Triangles = new List<SoftwareTriangle>();
			Edges = new List<SoftwareEdge>();

			VetexComponents = new List<object>();
			TriangleComponents = new List<object>();
			EdgeComponents = new List<object>();
			VertexComponentKeys = new Dictionary<VertexComponentKeyTypes,int>();
			TriangleComponentKeys = new Dictionary<TriangleComponentKeyTypes,int>();
			EdgeComponentKeys = new Dictionary<EdgeComponentKeyTypes,int>();

			// transform
			foreach (var input in mesh.RMXObject.Transform.Inputs)
			{
				switch (input.Type)
				{
					case "EulerRotation": Rotation = new Vector3(input.Values[0], input.Values[1], input.Values[2]); break;
					case "Scale": Scale = new Vector3(input.Values[0], input.Values[1], input.Values[2]); break;
					case "Position": Position = new Vector3(input.Values[0], input.Values[1], input.Values[2]); break;
					default: Debug.ThrowError("SoftwareMesh", "Unsuported Transform Type: " + input.Type); break;
				}
			}

			// material
			if (!string.IsNullOrEmpty(mesh.Material))
			{
				foreach (var material in model.Materials)
				{
					if (material.Name == mesh.Material) this.Material = material;
				}
			}

			// verticies
			var channels = mesh.Verticies.Channels;
			int positionStep = 0;
			for (int i = 0; i != channels.Length; ++i)
			{
				var channel = channels[i];
				if (channel.ID != "Position") continue;

				positionStep = channel.Step;
				if (channel.Step == 3)
				{
					var vertexPostions = new Vector3[channel.Values.Length / 3];
					var positions = channel.Values;
					int i2 = 0;
					for (int i3 = 0; i3 != vertexPostions.Length; ++i3)
					{
						Verticies.Add(new SoftwareVertex(i3));
						vertexPostions[i3] = new Vector3(positions[i2], positions[i2+1], positions[i2+2]);
						i2 += 3;
					}

					VetexComponents.Add(vertexPostions);
				}
				else if (channel.Step == 2)
				{
					var vertexPostions = new Vector2[channel.Values.Length / 2];
					var positions = channel.Values;
					int i2 = 0;
					for (int i3 = 0; i3 != vertexPostions.Length; ++i3)
					{
						Verticies.Add(new SoftwareVertex(i3));
						vertexPostions[i3] = new Vector2(positions[i2], positions[i2+1]);
						i2 += 2;
					}

					VetexComponents.Add(vertexPostions);
				}
				else
				{
					Debug.ThrowError("SoftwareMesh", "Unsupotred position step count");
				}

				VertexComponentKeys.Add(VertexComponentKeyTypes.Positions, i);
			}

			// triangles
			bool hasPositionData = false;
			float[] colors = null, normals = null, uvs = null;
			foreach (var channel in channels)
			{
				switch (channel.ID)
				{
					case ("Position"): hasPositionData = true; break;
					case ("Color"): colors = channel.Values; break;
					case ("Normal"): normals = channel.Values; break;
					case ("UV"): uvs = channel.Values; break;
				}
			}
			if (!hasPositionData) Debug.ThrowError("SoftwareMesh", "Vertices missing position data");

			int[] positionIndices = null, colorIndices = null, normalIndices = null, uvIndices = null;
			foreach (var index in mesh.Faces.Indices)
			{
				switch (index.ID)
				{
					case ("Position"): positionIndices = index.Values; break;
					case ("Color"): colorIndices = index.Values; break;
					case ("Normal"): normalIndices = index.Values; break;
					case ("UV"): uvIndices = index.Values; break;
				}
			}
			if (positionIndices == null) Debug.ThrowError("SoftwareMesh", "Faces missing position data");

			var triangleColors = new List<TriangleColorComponent>();
			var triangleNormals = new List<TriangleNormalComponent>();
			var triangleUVs = new List<TriangleUVComponent>();
			int ti = 0, vi = 0;
			if (positionStep == 3)
			{
				foreach (int step in mesh.Faces.Steps.Values)
				{
					int loop = step - 2;
					int vi2 = vi + 1, vi3 = vi + 2;
					for (int i2 = 0; i2 != loop; ++i2)
					{
						var triangle = new SoftwareTriangle(ti, Verticies[positionIndices[vi]], Verticies[positionIndices[vi2]], Verticies[positionIndices[vi3]]);
						Triangles.Add(triangle);

						if (colorIndices != null)
						{
							int cii = colorIndices[vi] * 3, cii2 = colorIndices[vi2] * 3, cii3 = colorIndices[vi3] * 3;
							triangleColors.Add(new TriangleColorComponent
							(
								new Vector4(colors[cii], colors[cii+1], colors[cii+2], colors[cii+3]),
								new Vector4(colors[cii2], colors[cii2+1], colors[cii2+2], colors[cii2+3]),
								new Vector4(colors[cii3], colors[cii3+1], colors[cii3+2], colors[cii3+3]))
							);
						}

						if (normalIndices != null)
						{
							int nii = normalIndices[vi] * 3, nii2 = normalIndices[vi2] * 3, nii3 = normalIndices[vi3] * 3;
							triangleNormals.Add(new TriangleNormalComponent
							(
								new Vector3(normals[nii], normals[nii+1], normals[nii+2]),
								new Vector3(normals[nii2], normals[nii2+1], normals[nii2+2]),
								new Vector3(normals[nii3], normals[nii3+1], normals[nii3+2]))
							);
						}

						if (uvIndices != null)
						{
							int uii = uvIndices[vi] * 2, uii2 = uvIndices[vi2] * 2, uii3 = uvIndices[vi3] * 2;
							triangleUVs.Add(new TriangleUVComponent
							(
								new Vector2(uvs[uii], uvs[uii+1]),
								new Vector2(uvs[uii2], uvs[uii2+1]),
								new Vector2(uvs[uii3], uvs[uii3+1]))
							);
						}

						++ti;
						++vi2;
						++vi3;
					}

					vi += step;
				}
			}
			else
			{
				Debug.ThrowError("SoftwareMesh", "Position step not implemented yet: Step value = " + positionStep);
			}
			
			int componentIndex = 0;
			if (colors != null)
			{
				TriangleComponents.Add(triangleColors.ToArray());
				TriangleComponentKeys.Add(TriangleComponentKeyTypes.ColorComponents, componentIndex);
				++componentIndex;
			}

			if (normals != null)
			{
				TriangleComponents.Add(triangleNormals.ToArray());
				TriangleComponentKeys.Add(TriangleComponentKeyTypes.NormalComponents, componentIndex);
				++componentIndex;
			}

			if (uvs != null)
			{
				TriangleComponents.Add(triangleUVs.ToArray());
				TriangleComponentKeys.Add(TriangleComponentKeyTypes.UVComponents, componentIndex);
				++componentIndex;
			}
		}
		#endregion

		#region Methods
		public void FlipUVs()
		{
			foreach (var key in TriangleComponentKeys)
			{
				if (key.Key == TriangleComponentKeyTypes.UVComponents)
				{
					var uvComponent = (TriangleUVComponent[])TriangleComponents[key.Value];
					for (int i = 0; i != uvComponent.Length; ++i)
					{
						for (int i2 = 0; i2 != uvComponent[i].UVs.Length; ++i2)
						{
							uvComponent[i].UVs[i2].Y = 1.0f - uvComponent[i].UVs[i2].Y;
						}
					}
				}
			}
		}

		public void Rotate(float x, float y, float z)
		{
			var mat = Matrix3.FromEuler(x, y, z);
			Position = Position.Transform(mat);

			// rotate positions
			foreach (var key in VertexComponentKeys)
			{
				if (key.Key == VertexComponentKeyTypes.Positions)
				{
					var verticies = (Vector3[])VetexComponents[key.Value];
					for (int i = 0; i != verticies.Length; ++i)
					{
						verticies[i] = verticies[i].Transform(mat);
					}
				}
			}

			// rotate normals
			foreach (var key in TriangleComponentKeys)
			{
				if (key.Key == TriangleComponentKeyTypes.NormalComponents)
				{
					var normalComponent = (TriangleNormalComponent[])TriangleComponents[key.Value];
					for (int i = 0; i != normalComponent.Length; ++i)
					{
						for (int i2 = 0; i2 != normalComponent[i].Normals.Length; ++i2)
						{
							normalComponent[i].Normals[i2] = normalComponent[i].Normals[i2].Transform(mat);
						}
					}
				}
			}
		}
		#endregion
	}
}