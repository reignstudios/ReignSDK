using Reign.Core;

namespace Reign.Video
{
	public interface MeshI : DisposableI
	{
		#region Properties
		MaterialI Material {get;}
		BufferLayoutDescI LayoutDesc {get;}
		#endregion

		#region Methods
		void Enable();
		void Draw();
		void Render();
		#endregion
	}
}