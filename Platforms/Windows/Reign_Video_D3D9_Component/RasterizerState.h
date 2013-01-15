#pragma once
#include "Video.h"

namespace Reign_Video_D3D9_Component
{
	public enum class REIGN_D3DFILLMODE
	{
		POINT = D3DFILL_POINT,
		WIREFRAME = D3DFILL_WIREFRAME,
		SOLID = D3DFILL_SOLID
	};

	public enum class REIGN_D3DCULL
	{
		NONE = D3DCULL_NONE,
		CW = D3DCULL_CW,
		CCW = D3DCULL_CCW
	};

	public ref class RasterizerStateDescCom sealed
	{
		#pragma region Properties
		internal: D3DFILLMODE fillMode;
		internal: D3DCULL cullMode;
		internal: bool multisampleEnable;
		#pragma endregion

		#pragma region Constructors
		public: RasterizerStateDescCom(REIGN_D3DFILLMODE fillMode, REIGN_D3DCULL cullMode, bool multisampleEnable);
		#pragma endregion
	};

	public ref class RasterizerStateCom sealed
	{
		#pragma region Fields
		private: VideoCom^ video;
		private: RasterizerStateDescCom^ desc;
		#pragma endregion

		#pragma region Constructors
		public: RasterizerStateCom(VideoCom^ video, RasterizerStateDescCom^ desc);
		#pragma endregion
		
		#pragma region Methods
		public: void Enable();
		#pragma endregion
	};
}