using ShaderCompiler.Core;

namespace Shaders
{
	public sealed class DiffuseTexture : ShaderI
	{
		[VSInput(VSInputTypes.Position, 0)] public Vector3 Position_VS;
		[VSInput(VSInputTypes.Normal, 0)] public Vector3 Normal_VS;
		[VSInput(VSInputTypes.UV, 0)] public Vector2 UV_VS;

		[VSOutputPSInput(VSOutputPSInputTypes.Position, 0)] public Vector4 Position_VSPS;
		[VSOutputPSInput(VSOutputPSInputTypes.InOut, 0)] public Vector3 Normal_VSPS;
		[VSOutputPSInput(VSOutputPSInputTypes.InOut, 1)] public Vector2 UV_VSPS;

		[PSOutput(PSOutputTypes.Color, 0)] public Vector4 Color_PS;

		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Matrix4 Camera;
		[FieldUsage(FieldUsageTypes.PS, MaterialUsages.Global)] public Vector3 LightDirection;
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Instance)] public Matrix4 Transform;
		[FieldUsage(FieldUsageTypes.PS, MaterialUsages.Instance)] public Texture2D Diffuse;

		[ShaderMethod(ShaderMethodTypes.VS)]
		public void MainVS()
		{
			Vector4 loc = Transform.Multiply(new Vector4(Position_VS, 1));
			Position_VSPS = Camera.Multiply(loc);
			Normal_VSPS = Transform.Multiply(new Vector4(Normal_VS, 0)).xyz;
			UV_VSPS = new Vector2(UV_VS.x, 1.0-UV_VS.y);
		}

		[ShaderMethod(ShaderMethodTypes.PS)]
		public void MainPS()
		{
			double light = SL.Dot(-LightDirection, Normal_VSPS);

			Color_PS = Diffuse.Sample(UV_VSPS) * light;
		}
	}
}
