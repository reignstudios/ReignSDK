using Reign.Core;

namespace Reign.Video
{
	public abstract class IQuickDraw : DisposableResource
	{
		#region Properties
		protected int vertexCount, vertexArraySize, vertexNext, vertexNextOffset, primitiveVertexCount;
		protected float[] vertices, vertex;
		protected int[] positionOffset, colorOffset, uvOffset, normalOffset;
		private BufferLayoutElementTypes[] positionTypes;
		#endregion

		#region Constructors
		protected IQuickDraw(IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc)
		: base(parent)
		{
			init(bufferLayoutDesc, 128*2);
		}

		private void init(IBufferLayoutDesc bufferLayoutDesc, int vertexCount)
		{
			vertexArraySize = bufferLayoutDesc.FloatCount;

			var positions = bufferLayoutDesc.ElementsUsages(BufferLayoutElementUsages.Position);
			positionOffset = new int[positions.Count];
			positionTypes = new BufferLayoutElementTypes[positions.Count];
			int i = 0;
			foreach (var position in positions)
			{
				positionOffset[i] = position.FloatOffset;
				positionTypes[i] = position.Type;
				++i;
			}

			var colors = bufferLayoutDesc.ElementsUsages(BufferLayoutElementUsages.Color);
			colorOffset = new int[colors.Count];
			i = 0;
			foreach (var color in colors)
			{
				colorOffset[i] = color.FloatOffset;
				++i;
			}

			var uvs = bufferLayoutDesc.ElementsUsages(BufferLayoutElementUsages.UV);
			uvOffset = new int[uvs.Count];
			i = 0;
			foreach (var uv in uvs)
			{
				uvOffset[i] = uv.FloatOffset;
				++i;
			}

			var normals = bufferLayoutDesc.ElementsUsages(BufferLayoutElementUsages.Normal);
			normalOffset = new int[normals.Count];
			i = 0;
			foreach (var normal in normals)
			{
				normalOffset[i] = normal.FloatOffset;
				++i;
			}

			vertex = new float[vertexArraySize];
			for (i = 0; i != vertex.Length; ++i) vertex[i] = 0;
			this.vertexCount = vertexCount;
			vertices = new float[vertexCount*vertexArraySize];
			for (i = 0; i != vertices.Length; ++i) vertices[i] = 0;
		}
		#endregion

		#region Methods
		public virtual void StartTriangles()
		{
			vertexNext = 0;
			vertexNextOffset = 0;
			primitiveVertexCount = 3;
		}
		public virtual void StartLines()
		{
			vertexNext = 0;
			vertexNextOffset = 0;
			primitiveVertexCount = 2;
		}
		public virtual void StartPoints()
		{
			vertexNext = 0;
			vertexNextOffset = 0;
			primitiveVertexCount = 1;
		}

		public abstract void Draw();
		public void End() {if (vertexNext >= primitiveVertexCount) Draw();}

		public void Pos(Vector3 position) {Pos(position.X, position.Y, position.Z);}
		public void Pos(float x, float y) {Pos(x, y, 0);}
		public void Pos(float x, float y, float z)
		{
			int offset = positionOffset[0];
			vertex[offset] = x;
			vertex[offset+1] = y;
			if (positionTypes[0] != BufferLayoutElementTypes.Vector2) vertex[offset+2] = z;

			for (int i = 0; i != vertex.Length; ++i)
			{
				vertices[vertexNextOffset+i] = vertex[i];
			}

			++vertexNext;
			vertexNextOffset = vertexNext*vertexArraySize;
			if (vertexNext >= vertexCount)
			{
				End();
				vertexNext = 0;
				vertexNextOffset = 0;
			}
		}

		public void UV(Vector2 uv) {UV(uv.X, uv.Y);}
		public void UV(float u, float v)
		{
			if (uvOffset.Length == 0) return;
			vertex[uvOffset[0]] = u;
			vertex[uvOffset[0]+1] = v;
		}

		public virtual void Color(float r, float g, float b, float a)
		{
			if (colorOffset.Length == 0) return;
			vertex[colorOffset[0]] = System.BitConverter.ToSingle(new byte[]{(byte)(r*255.0f), (byte)(g*255.0f), (byte)(b*255.0f), (byte)(a*255.0f)}, 0);
		}
		#endregion
	}
}