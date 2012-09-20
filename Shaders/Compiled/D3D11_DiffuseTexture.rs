#GLOBAL
struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float3 Normal_VSPS : TEXCOORD0;
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


float4x4 Camera;
float4x4 Transform;

VSOutPSIn main(VSIn In)
{
	VSOutPSIn Out;

	float4 loc = mul( float4(In.Position_VS, 1), Transform);
	Out.Position_VSPS = mul(loc, Camera);
	Out.Normal_VSPS = mul( float4(In.Normal_VS, 0), Transform).xyz;
	Out.UV_VSPS = float2(In.UV_VS.x, 1.0-In.UV_VS.y);

	return Out;
}
#END

#PS

struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

sampler Samplers[1];
float3 LightDirection;
texture2D Diffuse;

PSOut main(VSOutPSIn In)
{
	PSOut Out;

	float light = dot(-LightDirection, In.Normal_VSPS);

	Out.Color_PS = Diffuse.Sample(Samplers[0], In.UV_VSPS) * light;

	return Out;
}
#END

