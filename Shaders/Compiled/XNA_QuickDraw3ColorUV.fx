struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float4 Color_VSPS : TEXCOORD0;
	float2 UV_VSPS : TEXCOORD1;
};

struct VSIn
{
	float3 Position_VS : POSITION0;
	float4 Color_VS : COLOR0;
	float2 UV_VS : TEXCOORD0;
};


float4x4 Camera;

VSOutPSIn mainVS(VSIn In)
{
	VSOutPSIn Out;

	Out.Position_VSPS = mul(Camera,  float4(In.Position_VS, 1));
	Out.Color_VSPS = In.Color_VS;
	Out.UV_VSPS = float2(In.UV_VS.x, 1.0-In.UV_VS.y);

	return Out;
}


struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

texture2D Diffuse;
sampler2D Diffuse_S : register(s0) = sampler_state {Texture = <Diffuse>;};

PSOut mainPS(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = tex2D(Diffuse_S, In.UV_VSPS);

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
