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
		public SoftwareMesh(SoftwareModel model, ColladaModel_Geometry geometry)
		{
			Model = model;
			Name = geometry.Name;

			Verticies = new List<SoftwareVertex>();
			Triangles = new List<SoftwareTriangle>();
			Edges = new List<SoftwareEdge>();

			VetexComponents = new List<object>();
			TriangleComponents = new List<object>();
			EdgeComponents = new List<object>();
			VertexComponentKeys = new Dictionary<VertexComponentKeyTypes,int>();
			TriangleComponentKeys = new Dictionary<TriangleComponentKeyTypes,int>();
			EdgeComponentKeys = new Dictionary<EdgeComponentKeyTypes,int>();

			// verticies
			var vertInputs = geometry.Mesh.Vertices.Inputs;
			for (int i = 0; i != vertInputs.Length; ++i)
			{
				var source = geometry.Mesh.FindSource(vertInputs[i].Source);
				if (source == null) Debug.ThrowError("SoftwareMesh", "Failed to find vertex source position data: " + vertInputs[i].Source);

				var vertexPostions = new Vector3[source.FloatArray.Count / 3];
				var positions = source.FloatArray.Values;
				int i2 = 0;
				for (int i3 = 0; i3 != vertexPostions.Length; ++i3)
				{
					Verticies.Add(new SoftwareVertex(i3));
					vertexPostions[i3] = new Vector3(positions[i2], positions[i2+1], positions[i2+2]);
					i2 += 3;
				}
				VetexComponents.Add(vertexPostions);
				VertexComponentKeys.Add(VertexComponentKeyTypes.Positions, i);
			}

			// triangles
			var indexData = geometry.Mesh.Polylist.IndexData;
			int posIndex = -1, colorIndex = -1, normalIndex = -1, uvIndex = -1;
			var inputs = geometry.Mesh.Polylist.Inputs;
			for (int i = 0; i != inputs.Length; ++i)
			{
				switch (inputs[i].Semantic)
				{
					case ("VERTEX"): posIndex = inputs[i].Offset; break;
					case ("COLOR"): colorIndex = inputs[i].Offset; break;
					case ("NORMAL"): normalIndex = inputs[i].Offset; break;
					case ("TEXCOORD"): uvIndex = inputs[i].Offset; break;
				}
			}
			if (posIndex == -1) Debug.ThrowError("SoftwareMesh", "Polylist missing vertex data");

			float[] colors = null;
			if (colorIndex != -1)
			{
				var input = geometry.Mesh.Polylist.Inputs[colorIndex];
				var source = geometry.Mesh.FindSource(input.Source);
				if (source == null) Debug.ThrowError("SoftwareMesh", "Failed to find color source data: " + input.Source);

				colors = source.FloatArray.Values;
			}

			float[] normals = null;
			if (normalIndex != -1)
			{
				var input = geometry.Mesh.Polylist.Inputs[normalIndex];
				var source = geometry.Mesh.FindSource(input.Source);
				if (source == null) Debug.ThrowError("SoftwareMesh", "Failed to find normal source data: " + input.Source);

				normals = source.FloatArray.Values;
			}

			float[] uvs = null;
			if (uvIndex != -1)
			{
				var input = geometry.Mesh.Polylist.Inputs[uvIndex];
				var source = geometry.Mesh.FindSource(input.Source);
				if (source == null) Debug.ThrowError("SoftwareMesh", "Failed to find uv source data: " + input.Source);

				uvs = source.FloatArray.Values;
			}

			int vertOffset = geometry.Mesh.Polylist.Inputs.Length;
			int polyOffset = 0;
			var indices = geometry.Mesh.Polylist.IndexData;
			var triangleColors = new List<TriangleColorComponent>();
			var triangleNormals = new List<TriangleNormalComponent>();
			var triangleUVs = new List<TriangleUVComponent>();
			int ti = 0;
			for (int i = 0; i != geometry.Mesh.Polylist.Count; ++i)
			{
				int vi = posIndex + polyOffset;
				int vi2 = vi + vertOffset;
				int vi3 = vi + (vertOffset * 2);

				int ci = colorIndex + polyOffset;
				int ci2 = ci + vertOffset;
				int ci3 = ci + (vertOffset * 2);

				int ni = normalIndex + polyOffset;
				int ni2 = ni + vertOffset;
				int ni3 = ni + (vertOffset * 2);

				int ui = uvIndex + polyOffset;
				int ui2 = ui + vertOffset;
				int ui3 = ui + (vertOffset * 2);

				int loop = geometry.Mesh.Polylist.VertexCounts[i] - 2;
				for (int i2 = 0; i2 != loop; ++i2)
				{
					var triangle = new SoftwareTriangle(ti, Verticies[indices[vi]], Verticies[indices[vi2]], Verticies[indices[vi3]]);
					++ti;
					Triangles.Add(triangle);
					vi2 += vertOffset;
				    vi3 += vertOffset;

					if (colorIndex != -1)
					{
						int cii = indices[ci] * 3, cii2 = indices[ci2] * 3, cii3 = indices[ci3] * 3;
						triangleColors.Add(new TriangleColorComponent
						(
							new Vector4(colors[cii], colors[cii+1], colors[cii+2], colors[cii+3]),
							new Vector4(colors[cii2], colors[cii2+1], colors[cii2+2], colors[cii2+3]),
							new Vector4(colors[cii3], colors[cii3+1], colors[cii3+2], colors[cii3+3]))
						);
						ci2 += vertOffset;
						ci3 += vertOffset;
					}

					if (normalIndex != -1)
					{
						int nii = indices[ni] * 3, nii2 = indices[ni2] * 3, nii3 = indices[ni3] * 3;
						triangleNormals.Add(new TriangleNormalComponent
						(
							new Vector3(normals[nii], normals[nii+1], normals[nii+2]),
							new Vector3(normals[nii2], normals[nii2+1], normals[nii2+2]),
							new Vector3(normals[nii3], normals[nii3+1], normals[nii3+2]))
						);
						ni2 += vertOffset;
						ni3 += vertOffset;
					}

					if (uvIndex != -1)
					{
						int uii = indices[ui] * 2, uii2 = indices[ui2] * 2, uii3 = indices[ui3] * 2;
						triangleUVs.Add(new TriangleUVComponent
						(
							new Vector2(uvs[uii], uvs[uii+1]),
							new Vector2(uvs[uii2], uvs[uii2+1]),
							new Vector2(uvs[uii3], uvs[uii3+1]))
						);
						ui2 += vertOffset;
						ui3 += vertOffset;
					}
				}

				polyOffset += geometry.Mesh.Polylist.VertexCounts[i] * vertOffset;
			}

			int componentIndex = 0;
			if (colorIndex != -1)
			{
				TriangleComponents.Add(triangleColors.ToArray());
				TriangleComponentKeys.Add(TriangleComponentKeyTypes.ColorComponents, componentIndex);
				++componentIndex;
			}

			if (normalIndex != -1)
			{
				TriangleComponents.Add(triangleNormals.ToArray());
				TriangleComponentKeys.Add(TriangleComponentKeyTypes.NormalComponents, componentIndex);
				++componentIndex;
			}

			if (uvIndex != -1)
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

			//var rotMat = Matrix3.FromEuler(Rotation.X, Rotation.Y, Rotation.Z);
			//rotMat = rotMat.Multiply(mat);
			//Rotation = rotMat.Euler();

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