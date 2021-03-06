#GLOBAL
struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float4 Light_VSPS : TEXCOORD0;
	float2 UV_VSPS : TEXCOORD1;
};
#END

#VS
struct VSIn
{
	float3 Position_VS : POSITION0;
	float3 Normal_VS : NORMAL0;
	float2 UV_VS : TEXCOORD0;
};


float4x4 Camera : register(c0);
float3 LightDirection : register(c4);
float3 LightDirection2 : register(c5);
float4 LightColor : register(c6);
float4 LightColor2 : register(c7);
float4x4 Transform : register(c8);

VSOutPSIn main(VSIn In)
{
	VSOutPSIn Out;

	float4 loc = mul( float4(In.Position_VS, 1), Transform);
	Out.Position_VSPS = mul(loc, Camera);
	Out.UV_VSPS = float2(In.UV_VS.x, 1.0-In.UV_VS.y);

	float3 normal = normalize(mul(Transform,  float4(In.Normal_VS, 0)).xyz);
	float light = max(dot(-LightDirection, normal), 0.0);
	float light2 = max(dot(-LightDirection2, normal), 0.0);
	Out.Light_VSPS = ((light * LightColor) + (light2 * LightColor2));

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

sampler2D Diffuse : register(s0);

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = tex2D(Diffuse, In.UV_VSPS) * In.Light_VSPS;

	return Out;
}
#END

