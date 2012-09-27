using ShaderCompiler.Core;

namespace Shaders
{
	public sealed class QuickDraw3ColorUV : ShaderI
	{
		[VSInput(VSInputTypes.Position, 0)] public Vector3 Position_VS;
		[VSInput(VSInputTypes.Color, 0)] public Vector4 Color_VS;
		[VSInput(VSInputTypes.UV, 0)] public Vector2 UV_VS;

		[VSOutputPSInput(VSOutputPSInputTypes.Position, 0)] public Vector4 Position_VSPS;
		[VSOutputPSInput(VSOutputPSInputTypes.InOut, 0)] public Vector4 Color_VSPS;
		[VSOutputPSInput(VSOutputPSInputTypes.InOut, 1)] public Vector2 UV_VSPS;

		[PSOutput(PSOutputTypes.Color, 0)] public Vector4 Color_PS;

		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Matrix4 Camera;
		[FieldUsage(FieldUsageTypes.PS, MaterialUsages.Instance)] public Texture2D Diffuse;

		[ShaderMethod(ShaderMethodTypes.VS)]
		public void MainVS()
		{
			Position_VSPS = Camera.Multiply(new Vector4(Position_VS, 1));
			Color_VSPS = Color_VS;
			UV_VSPS = new Vector2(UV_VS.x, 1.0-UV_VS.y);
		}

		[ShaderMethod(ShaderMethodTypes.PS)]
		public void MainPS()
		{
			Color_PS = Diffuse.Sample(UV_VSPS) * Color_VSPS;
		}
	}
}
