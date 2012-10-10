#GLOBAL
struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float4 Color_VSPS : TEXCOORD0;
	float2 UV_VSPS : TEXCOORD1;
};
#END

#VS
struct VSIn
{
	float3 Position_VS : POSITION0;
	float4 Color_VS : COLOR0;
	float2 UV_VS : TEXCOORD0;
};


float4x4 Camera;

VSOutPSIn main(VSIn In)
{
	VSOutPSIn Out;

	Out.Position_VSPS = mul(Camera,  float4(In.Position_VS, 1));
	Out.Color_VSPS = In.Color_VS;
	Out.UV_VSPS = float2(In.UV_VS.x, 1.0-In.UV_VS.y);

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

sampler2D Diffuse;

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = tex2D(Diffuse, In.UV_VSPS) * In.Color_VSPS;

	return Out;
}
#END

