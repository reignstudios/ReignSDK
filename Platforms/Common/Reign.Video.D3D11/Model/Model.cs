using System;
using Reign.Core;

namespace Reign.Video.D3D11
{
	public class Model : Disposable, ModelI
	{
		#region Properties
		public MeshI[] Meshes {get; private set;}
		public MaterialI[] Materials {get; private set;}
		#endregion

		#region Constructors
		public Model(DisposableI parent, SoftwareModel model, MeshVertexSizes positionSize, string contentDirectory)
		: base(parent)
		{
			try
			{
				// materials
				Materials = new Material[model.Materials.Count];
				for (int i = 0; i != Materials.Length; ++i)
				{
					Materials[i] = new Material(this, model.Materials[i], contentDirectory);
				}

				// meshes
				Meshes = new Mesh[model.Meshes.Count];
				for (int i = 0; i != Meshes.Length; ++i)
				{
					Meshes[i] = new Mesh(this, model, model.Meshes[i], positionSize);
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}
		#endregion

		#region Methods
		public void Render()
		{
			foreach (var mesh in Meshes)
			{
				mesh.Render();
			}
		}
		#endregion
	}
}