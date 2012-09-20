struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float3 Normal_VSPS : TEXCOORD0;
	float2 UV_VSPS : TEXCOORD1;
};

struct VSIn
{
	float3 Position_VS : POSITION0;
	float3 Normal_VS : NORMAL0;
	float2 UV_VS : TEXCOORD0;
};


float4x4 Camera;
float4x4 Transform;

VSOutPSIn mainVS(VSIn In)
{
	VSOutPSIn Out;

	float4 loc = mul(Transform,  float4(In.Position_VS, 1));
	Out.Position_VSPS = mul(Camera, loc);
	Out.Normal_VSPS = mul(Transform,  float4(In.Normal_VS, 0)).xyz;
	Out.UV_VSPS = float2(In.UV_VS.x, 1.0-In.UV_VS.y);

	return Out;
}


struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

float3 LightDirection;
texture2D Diffuse;
sampler2D Diffuse_S : register(s0) = sampler_state {Texture = <Diffuse>;};

PSOut mainPS(VSOutPSIn In)
{
	PSOut Out;

	float light = dot(-LightDirection, In.Normal_VSPS);

	Out.Color_PS = tex2D(Diffuse_S, In.UV_VSPS) * light;

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
