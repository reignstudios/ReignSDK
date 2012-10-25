struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float4 Color_VSPS : TEXCOORD0;
};

struct VSIn
{
	float3 Position_VS : POSITION0;
	float4 Color_VS : COLOR0;
};


float4x4 Camera;

VSOutPSIn mainVS(VSIn In)
{
	VSOutPSIn Out;

	Out.Position_VSPS = mul(Camera,  float4(In.Position_VS, 1.0));
	Out.Color_VSPS = In.Color_VS;

	return Out;
}


struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};


PSOut mainPS(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = In.Color_VSPS;

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
