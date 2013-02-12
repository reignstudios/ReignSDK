struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float2 UV_VSPS : TEXCOORD0;
};

struct VSIn
{
	float2 Position_VS : POSITION0;
};


float4x4 Camera;
float2 Position;
float2 Size;
float2 TexelOffset;

VSOutPSIn mainVS(VSIn In)
{
	VSOutPSIn Out;

	float3 loc = float3((In.Position_VS * Size) + Position, 0);
	Out.Position_VSPS = mul(Camera,  float4(loc, 1.0));
	Out.UV_VSPS = float2(In.Position_VS.x, 1.0-In.Position_VS.y) + TexelOffset;

	return Out;
}


struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

float4 Color;
float Fade;
float Fade2;
texture2D MainTexture;
sampler2D MainTexture_S : register(s0) = sampler_state {Texture = <MainTexture>;};
texture2D MainTexture2;
sampler2D MainTexture2_S : register(s1) = sampler_state {Texture = <MainTexture2>;};
texture2D MainTexture3;
sampler2D MainTexture3_S : register(s2) = sampler_state {Texture = <MainTexture3>;};

PSOut mainPS(VSOutPSIn In)
{
	PSOut Out;

	float4 outColor = tex2D(MainTexture_S, In.UV_VSPS);
	outColor += (tex2D(MainTexture2_S, In.UV_VSPS) - outColor) * Fade;
	outColor += (tex2D(MainTexture3_S, In.UV_VSPS) - outColor) * Fade2;

	Out.Color_PS = outColor * Color;

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
