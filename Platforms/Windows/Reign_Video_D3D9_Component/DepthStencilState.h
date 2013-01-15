#pragma once
#include "Video.h"

namespace Reign_Video_D3D9_Component
{
	public enum class REIGN_D3DZBUFFERTYPE
	{
		_FALSE = D3DZB_FALSE,
		_TRUE = D3DZB_TRUE,
		_USEW = D3DZB_USEW
	};

	public enum class REIGN_D3DCMPFUNC
	{
		NEVER = D3DCMP_NEVER,
		LESS = D3DCMP_LESS,
		EQUAL = D3DCMP_EQUAL,
		LESSEQUAL = D3DCMP_LESSEQUAL,
		GREATER = D3DCMP_GREATER,
		NOTEQUAL = D3DCMP_NOTEQUAL,
		GREATEREQUAL = D3DCMP_GREATEREQUAL,
		ALWAYS = D3DCMP_ALWAYS
	};

	public enum class REIGN_D3DSTENCILOP
	{
		KEEP = D3DSTENCILOP_KEEP,
		ZERO = D3DSTENCILOP_ZERO,
		REPLACE = D3DSTENCILOP_REPLACE,
		INCRSAT = D3DSTENCILOP_INCRSAT,
		DECRSAT = D3DSTENCILOP_DECRSAT,
		INVERT = D3DSTENCILOP_INVERT,
		INCR = D3DSTENCILOP_INCR,
		DECR = D3DSTENCILOP_DECR
	};

	public ref class DepthStencilStateDescCom sealed
	{
		#pragma region Properties
		internal: D3DZBUFFERTYPE depthReadEnable;
		internal: bool depthWriteEnable;
		internal: D3DCMPFUNC depthFunc;
		internal: bool stencilEnable;
		internal: D3DCMPFUNC stencilFunc;
		internal: D3DSTENCILOP stencilFailOp;
		internal: D3DSTENCILOP stencilDepthFailOp;
		internal: D3DSTENCILOP stencilPassOp;
		#pragma endregion

		#pragma region Constructors
		public: DepthStencilStateDescCom(REIGN_D3DZBUFFERTYPE depthReadEnable, bool depthWriteEnable, REIGN_D3DCMPFUNC depthFunc, bool stencilEnable, REIGN_D3DCMPFUNC stencilFunc, REIGN_D3DSTENCILOP stencilFailOp, REIGN_D3DSTENCILOP stencilDepthFailOp, REIGN_D3DSTENCILOP stencilPassOp);
		#pragma endregion
	};

	public ref class DepthStencilStateCom sealed
	{
		#pragma region Fields
		private: VideoCom^ video;
		private: DepthStencilStateDescCom^ desc;
		#pragma endregion
	
		#pragma region Constructors
		public: DepthStencilStateCom(VideoCom^ video, DepthStencilStateDescCom^ desc);
		#pragma endregion
		
		#pragma region Methods
		public: void Enable();
		#pragma endregion
	};
}