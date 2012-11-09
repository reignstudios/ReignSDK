using Reign.Core;
using Reign.Video;
using System.Collections.Generic;
using System.IO;

#if METRO
using System.Threading.Tasks;
#endif

namespace Reign.Physics
{
	public class MeshTrianlge
	{
		public int Index1, Index2, Index3;

		public MeshTrianlge(int index1, int index2, int index3)
		{
			Index1 = index1;
			Index2 = index2;
			Index3 = index3;
		}
	}

	public class MeshVertex
	{
		public Vector3 Position;

		public MeshVertex(Vector3 position)
		{
			Position = position;
		}

		public MeshVertex(float x, float y, float z)
		{
			Position = new Vector3(x, y, z);
		}
	}

	class TriangleMeshStreamLoader : StreamLoaderI
	{
		private TriangleMesh triangleMesh;
		private SoftwareMesh mesh;
		private string fileName;

		public TriangleMeshStreamLoader(TriangleMesh triangleMesh, SoftwareMesh mesh, string fileName)
		{
			this.triangleMesh = triangleMesh;
			this.mesh = mesh;
			this.fileName = fileName;
		}

		#if METRO
		public override async Task<bool> Load() {
		#else
		public override bool Load() {
		#endif
			if (mesh != null)
			{
				if (!mesh.Model.Loaded) return false;
				triangleMesh.load(mesh);
				return true;
			}
			else
			{
				#if METRO
				await triangleMesh.load(fileName);
				#else
				triangleMesh.load(fileName);
				#endif
				return true;
			}
		}
	}

	public class TriangleMesh
	{
		public bool Loaded {get; private set;}
		public delegate void FinishedLoadingMethod(TriangleMesh mesh);
		private FinishedLoadingMethod finishedLoading;

		public List<MeshVertex> Verticies;
		public List<MeshTrianlge> Triangles;

		public TriangleMesh(string fileName, FinishedLoadingMethod finishedLoading)
		{
			this.finishedLoading = finishedLoading;
			new TriangleMeshStreamLoader(this, null, fileName);
		}

		public TriangleMesh(SoftwareMesh mesh, FinishedLoadingMethod finishedLoading)
		{
			this.finishedLoading = finishedLoading;
			new TriangleMeshStreamLoader(this, mesh, null);
		}

		#if METRO
		internal async Task load(string fileName) {
			using (var file = await Streams.OpenFile(fileName))
		#else
		internal void load(string fileName) {
			using (var file = Streams.OpenFile(fileName))
		#endif
			using (var reader = new BinaryReader(file))
			{
				// meta data
				int tag = reader.ReadInt32();
				if (tag != Streams.MakeFourCC('R', 'T', 'M', 'M')) Debug.ThrowError("TriangleMesh", "Not a TriangleMesh file");
				float version = reader.ReadSingle();
				if (version != 1.0f) Debug.ThrowError("TriangleMesh", "Unsuported version");
				bool isCompressed = reader.ReadBoolean();

				// data
				int count = reader.ReadInt32();
				Verticies = new List<MeshVertex>();
				for (int i = 0; i != count; ++i)
				{
					Verticies.Add(new MeshVertex(reader.ReadVector3()));
				}

				count = reader.ReadInt32();
				Triangles = new List<MeshTrianlge>();
				for (int i = 0; i != count; ++i)
				{
				    Triangles.Add(new MeshTrianlge(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()));
				}
			}

			Loaded = true;
			if (finishedLoading != null) finishedLoading(this);
		}

		internal void load(SoftwareMesh mesh)
		{
			var hardwareMesh = new HardwareMeshProcessor(mesh, false, false, false);

			Verticies = new List<MeshVertex>();
			foreach (var vertex in hardwareMesh.Verticies)
			{
				Verticies.Add(new MeshVertex(vertex.Positions[0]));
			}

			Triangles = new List<MeshTrianlge>();
		    foreach (var i in hardwareMesh.Triangles)
		    {
		        Triangles.Add(new MeshTrianlge(i.Verticies[1].Index, i.Verticies[0].Index, i.Verticies[2].Index));
		    }

			Loaded = true;
			if (finishedLoading != null) finishedLoading(this);
		}

		public void Scale(float value)
		{
			foreach (var vertex in Verticies)
			{
				vertex.Position *= value;
			}
		}

		public static void Save(SoftwareMesh mesh, string fileName)
		{
			using (var file = Streams.SaveFile(fileName))
			{
				Save(mesh, file);
			}
		}

		public static void Save(SoftwareMesh mesh, Stream stream)
		{
			if (!mesh.Model.Loaded) Debug.ThrowError("TriangleMesh", "SoftwareModel must be loaded in order to save");
			var writer = new BinaryWriter(stream);

			// meta data
			writer.Write(Streams.MakeFourCC('R', 'T', 'M', 'M'));// tag
			writer.Write(1.0f);// version
			writer.Write(false);// is compressed

			// data
			var hardwareMesh = new HardwareMeshProcessor(mesh, false, false, false);

			writer.Write(hardwareMesh.Verticies.Count);
			foreach (var vertex in hardwareMesh.Verticies)
			{
			    writer.WriteVector(vertex.Positions[0]);
			}

			writer.Write(hardwareMesh.Triangles.Count);
			foreach (var triangle in hardwareMesh.Triangles)
			{
			    writer.Write(triangle.Verticies[1].Index);
				writer.Write(triangle.Verticies[0].Index);
				writer.Write(triangle.Verticies[2].Index);
			}
		}
	}
}
