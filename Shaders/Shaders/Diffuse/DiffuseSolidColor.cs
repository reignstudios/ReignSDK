using ShaderCompiler.Core;

namespace Shaders
{
	public sealed class DiffuseSolidColor : ShaderI
	{
		[VSInput(VSInputTypes.Position, 0)] public Vector3 Position_VS;
		[VSInput(VSInputTypes.Normal, 0)] public Vector3 Normal_VS;
		
		[VSOutputPSInput(VSOutputPSInputTypes.Position, 0)] public Vector4 Position_VSPS;
		[VSOutputPSInput(VSOutputPSInputTypes.InOut, 0)] public Vector3 Normal_VSPS;
		
		[PSOutput(PSOutputTypes.Color, 0)] public Vector4 Color_PS;
		
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Matrix4 Camera;
		[FieldUsage(FieldUsageTypes.PS, MaterialUsages.Global)] public Vector3 LightDirection;
		[FieldUsage(FieldUsageTypes.PS, MaterialUsages.Global)] public Vector4 LightColor;
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Instance)] public Matrix4 Transform;
		[FieldUsage(FieldUsageTypes.PS, MaterialUsages.Instance)] public Vector4 Diffuse;
		
		[ShaderMethod(ShaderMethodTypes.VS)]
		public void MainVS()
		{
			Vector4 loc = Transform.MultiplyInvert(new Vector4(Position_VS, 1));
			Position_VSPS = Camera.Multiply(loc);
			Normal_VSPS = SL.Normalize(Transform.MultiplyInvert(new Vector4(Normal_VS, 0)).xyz);
		}
		
		[ShaderMethod(ShaderMethodTypes.PS)]
		public void MainPS()
		{
			double light = SL.Dot(-LightDirection, Normal_VSPS);
			Color_PS = Diffuse * LightColor * light;
		}
	}
}
