using ShaderCompiler.Core;

namespace Shaders
{
	public sealed class QuickDraw3Color : ShaderI
	{
		[VSInput(VSInputTypes.Position, 0)] public Vector3 Position_VS;
		[VSInput(VSInputTypes.Color, 0)] public Vector4 Color_VS;

		[VSOutputPSInput(VSOutputPSInputTypes.Position, 0)] public Vector4 Position_VSPS;
		[VSOutputPSInput(VSOutputPSInputTypes.InOut, 0)] public Vector4 Color_VSPS;

		[PSOutput(PSOutputTypes.Color, 0)] public Vector4 Color_PS;

		[FieldUsage(FieldUsageTypes.VS)] public Matrix4 Camera;

		[ShaderMethod(ShaderMethodTypes.VS)]
		public void MainVS()
		{
			Position_VSPS = Camera.Multiply(new Vector4(Position_VS, 1.0));
			Color_VSPS = Color_VS;
		}

		[ShaderMethod(ShaderMethodTypes.PS)]
		public void MainPS()
		{
			Color_PS = Color_VSPS;
		}
	}
}
