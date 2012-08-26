struct VSOutPSIn
{
	float4 Position_VSPS : SV_POSITION0;
	float2 UV_VSPS : TEXCOORD0;
};
float4x4 Camera;
float3 Location;
float2 Size;
float2 LocationUV;
float2 SizeUV;
float2 TexelOffset;
float4 Color;
texture2D DiffuseTexture;
sampler2D DiffuseTexture_S : register(s0) = sampler_state {Texture = <DiffuseTexture>;};

struct VSIn
{
	float2 Position_VS : POSITION0;
};


VSOutPSIn mainVS(VSIn In)
{
	VSOutPSIn Out;

	float3 loc = float3((In.Position_VS * Size) + Location.xy, Location.z);
	Out.Position_VSPS = mul(Camera,  float4(loc, 1.0));

	float2 uv = In.Position_VS + TexelOffset;
	Out.UV_VSPS = ( float2(uv.x, 1.0-uv.y) * SizeUV) + LocationUV;

	return Out;
}


struct PSOut
{
	float4 Color_PS : SV_TARGET0;
};

PSOut mainPS(VSOutPSIn In)
{
	PSOut Out;

	Out.Color_PS = tex2D(DiffuseTexture_S, In.UV_VSPS) * Color;

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
