using ShaderCompiler.Core;

namespace Shaders
{
	public sealed class Font : ShaderI
	{
		[VSInput(VSInputTypes.Position, 0)] public Vector2 Position_VS;

		[VSOutputPSInput(VSOutputPSInputTypes.Position, 0)] public Vector4 Position_VSPS;
		[VSOutputPSInput(VSOutputPSInputTypes.InOut, 0)] public Vector2 UV_VSPS;

		[PSOutput(PSOutputTypes.Color, 0)] public Vector4 Color_PS;

		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Matrix4 Camera;
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Instance)] public Vector3 Location;
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Instance)] public Vector2 Size, LocationUV, SizeUV, TexelOffset;
		[FieldUsage(FieldUsageTypes.PS, MaterialUsages.Instance)] public Vector4 Color;
		[FieldUsage(FieldUsageTypes.PS, MaterialUsages.Instance)] public Texture2D DiffuseTexture;

		[ShaderMethod(ShaderMethodTypes.VS)]
		public void MainVS()
		{
			Vector3 loc = new Vector3((Position_VS * Size) + Location.xy, Location.z);
			Position_VSPS = Camera.Multiply(new Vector4(loc, 1.0));

			Vector2 uv = Position_VS + TexelOffset;
			UV_VSPS = (new Vector2(uv.x, 1.0-uv.y) * SizeUV) + LocationUV;
		}

		[ShaderMethod(ShaderMethodTypes.PS)]
		public void MainPS()
		{
			Color_PS = DiffuseTexture.Sample(UV_VSPS) * Color;
		}
	}
}
