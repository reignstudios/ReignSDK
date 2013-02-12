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
float2 Position : register(c4);
float2 Size : register(c5);
float2 TexelOffset : register(c6);

VSOutPSIn main(VSIn In)
{
	VSOutPSIn Out;

	float3 loc = float3((In.Position_VS * Size) + Position, 0);
	Out.Position_VSPS = mul( float4(loc, 1.0), Camera);
	Out.UV_VSPS = float2(In.Position_VS.x, 1.0-In.Position_VS.y) + TexelOffset;

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

float4 Color : register(c0);
float Fade : register(c1);
sampler2D MainTexture : register(s0);
sampler2D MainTexture2 : register(s1);

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	float4 outColor = tex2D(MainTexture, In.UV_VSPS);
	outColor += (tex2D(MainTexture2, In.UV_VSPS) - outColor) * Fade;

	Out.Color_PS = outColor * Color;

	return Out;
}
#END

