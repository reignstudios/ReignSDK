using ShaderCompiler.Core;

namespace Shaders
{
	public sealed class UISolidColor : ShaderI
	{
		[VSInput(VSInputTypes.Position, 0)] public Vector2 Position_VS;
		
		[VSOutputPSInput(VSOutputPSInputTypes.Position, 0)] public Vector4 Position_VSPS;
		
		[PSOutput(PSOutputTypes.Color, 0)] public Vector4 Color_PS;
		
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Matrix4 Camera;
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Instance)] public Vector2 Position;
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Instance)] public Vector2 Size;
		[FieldUsage(FieldUsageTypes.PS, MaterialUsages.Instance)] public Vector4 Color;
		
		[ShaderMethod(ShaderMethodTypes.VS)]
		public void MainVS()
		{
			Vector3 loc = new Vector3((Position_VS * Size) + Position, 0);
			Position_VSPS = Camera.Multiply(new Vector4(loc, 1.0));
		}
		
		[ShaderMethod(ShaderMethodTypes.PS)]
		public void MainPS()
		{
			Color_PS = Color;
		}
	}
}
