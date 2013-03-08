struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float3 Normal_VSPS : TEXCOORD0;
};

struct VSIn
{
	float3 Position_VS : POSITION0;
	float3 Normal_VS : NORMAL0;
};


float4x4 Camera;
float4x4 Transform;

VSOutPSIn mainVS(VSIn In)
{
	VSOutPSIn Out;

	float4 loc = mul( float4(In.Position_VS, 1), Transform);
	Out.Position_VSPS = mul(Camera, loc);
	Out.Normal_VSPS = mul( float4(In.Normal_VS, 0), Transform).xyz;

	return Out;
}


struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

float3 LightDirection;
float4 LightColor;
float4 Diffuse;

PSOut mainPS(VSOutPSIn In)
{
	PSOut Out;

	float3 normal = normalize(In.Normal_VSPS);
	float light = dot(-LightDirection, normal);
	Out.Color_PS = Diffuse * LightColor * light;

	return Out;
}

technique MainTechnique
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 mainVS();
		PixelShader = compile ps_3_0 mainPS();
	}
}
