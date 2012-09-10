using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video
{
	public interface ModelI : DisposableI
	{
		#region Properties
		MeshI[] Meshes {get;}
		MaterialI[] Materials {get;}
		List<Texture2DI> Textures {get;}
		#endregion

		#region Methods
		void Render();
		#endregion
	}
}