#pragma once

namespace Reign_Video_D3D11_Component
{
	public delegate void RenderDelegate();

	static ref class RenderDelegateObject sealed
	{
		internal: static event RenderDelegate^ render;

		public: static void Render()
		{
			render();
		}
	};
}