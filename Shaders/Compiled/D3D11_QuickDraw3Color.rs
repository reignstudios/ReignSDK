#GLOBAL
struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float4 Color_VSPS : TEXCOORD0;
};
float4x4 Camera;
#END

#VS
struct VSIn
{
	float3 Position_VS : POSITION0;
	float4 Color_VS : COLOR0;
};


VSOutPSIn main(VSIn In)
{
	VSOutPSIn Out;

	Out.Position_VSPS = mul( float4(In.Position_VS, 1.0), Camera);
	Out.Color_VSPS = In.Color_VS;

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = In.Color_VSPS;

	return Out;
}
#END

