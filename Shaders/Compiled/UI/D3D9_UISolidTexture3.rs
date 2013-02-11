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


float4x4 Camera;
float2 Position;
float2 Size;

VSOutPSIn main(VSIn In)
{
	VSOutPSIn Out;

	float3 loc = float3((In.Position_VS * Size) + Position, 0);
	Out.Position_VSPS = mul(Camera,  float4(loc, 1.0));
	Out.UV_VSPS = float2(In.Position_VS.x, 1.0-In.Position_VS.y);

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

float4 Color;
sampler2D MainTexture;
sampler2D MainTexture2;
sampler2D MainTexture3;

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = tex2D(MainTexture, In.UV_VSPS) * tex2D(MainTexture2, In.UV_VSPS) * tex2D(MainTexture3, In.UV_VSPS) * Color;

	return Out;
}
#END

