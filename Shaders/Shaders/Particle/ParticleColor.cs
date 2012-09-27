using ShaderCompiler.Core;

namespace Shaders
{
	public sealed class ParticleColor : ShaderI
	{
		[VSInput(VSInputTypes.Position, 0)] public Vector3 Position_VS;
		[VSInput(VSInputTypes.UV, 0)] public Vector2 UV_VS;

		[VSOutputPSInput(VSOutputPSInputTypes.Position, 0)] public Vector4 Position_VSPS;
		[VSOutputPSInput(VSOutputPSInputTypes.InOut, 0)] public Vector2 UV_VSPS;

		[PSOutput(PSOutputTypes.Color, 0)] public Vector4 Color_PS;

		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Matrix4 Camera;
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Matrix4 BillboardTransform;
		[FieldUsage(FieldUsageTypes.PS, MaterialUsages.Global)] public Texture2D Diffuse;

		[ArrayType(4)]
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Vector4[] ColorPallet;
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Vector4 ScalePallet;

		[ArrayType(10)]
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Instancing)] public Vector4[] Transforms;

		[ShaderMethod(ShaderMethodTypes.VS)]
		public void MainVS()
		{
			Vector4 loc = BillboardTransform.Multiply(new Vector4(Position_VS, 1));
			Position_VSPS = Camera.Multiply(loc);
			UV_VSPS = new Vector2(UV_VS.x, 1.0-UV_VS.y);
		}

		[ShaderMethod(ShaderMethodTypes.PS)]
		public void MainPS()
		{
			Color_PS = Diffuse.Sample(UV_VSPS);
		}
	}
}
