#GLOBAL
struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
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
	Out.Position_VSPS = mul( float4(loc, 1.0), Camera);

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

float4 Color;

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = Color;

	return Out;
}
#END

