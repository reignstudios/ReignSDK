using Reign.Core;

namespace Reign.Video
{
	public class InstanceObjectMesh : InstanceObject
	{
		#region Properties
		public Mesh Mesh {get; private set;}
		#endregion

		#region Constructors
		public InstanceObjectMesh(ObjectMesh o)
		: base(o)
		{
			Mesh = o.Mesh;
		}
		#endregion

		#region Methods
		public override void Render()
		{
			Mesh.Material.Enable();
			Mesh.Material.Apply(this);

			Mesh.VertexBuffer.Enable(Mesh.IndexBuffer);
			Mesh.VertexBuffer.Draw();
		}
		#endregion
	}
}