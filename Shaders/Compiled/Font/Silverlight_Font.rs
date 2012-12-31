#GLOBAL
struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float2 UV_VSPS : TEXCOORD0;
};
#END

#VS
struct VSIn
{
	float2 Position_VS : POSITION0;
};


float4x4 Camera : register(c0);
float2 Location : register(c4);
float2 Size : register(c5);
float2 LocationUV : register(c6);
float2 SizeUV : register(c7);
float2 TexelOffset : register(c8);

VSOutPSIn main(VSIn In)
{
	VSOutPSIn Out;

	float3 loc = float3((In.Position_VS * Size) + Location, 0);
	Out.Position_VSPS = mul( float4(loc, 1.0), Camera);

	float2 uv = In.Position_VS + TexelOffset;
	Out.UV_VSPS = ( float2(uv.x, 1.0-uv.y) * SizeUV) + LocationUV;

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

float4 Color : register(c0);
sampler2D DiffuseTexture : register(s0);

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = tex2D(DiffuseTexture, In.UV_VSPS) * Color;

	return Out;
}
#END

