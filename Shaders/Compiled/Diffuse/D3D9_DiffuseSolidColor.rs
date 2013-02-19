#GLOBAL
struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float3 Normal_VSPS : TEXCOORD0;
};
#END

#VS
struct VSIn
{
	float3 Position_VS : POSITION0;
	float3 Normal_VS : NORMAL0;
};


float4x4 Camera;
float4x4 Transform;

VSOutPSIn main(VSIn In)
{
	VSOutPSIn Out;

	float4 loc = mul( float4(In.Position_VS, 1), Transform);
	Out.Position_VSPS = mul(Camera, loc);
	Out.Normal_VSPS = normalize(mul( float4(In.Normal_VS, 0), Transform).xyz);

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

float3 LightDirection;
float4 LightColor;
float4 Diffuse;

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	float light = dot(-LightDirection, In.Normal_VSPS);
	Out.Color_PS = Diffuse * LightColor * light;

	return Out;
}
#END

