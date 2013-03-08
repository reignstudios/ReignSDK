using ShaderCompiler.Core;

namespace Shaders
{
	public sealed class VertexDiffuse2Texture : ShaderI
	{
		[VSInput(VSInputTypes.Position, 0)] public Vector3 Position_VS;
		[VSInput(VSInputTypes.Normal, 0)] public Vector3 Normal_VS;
		[VSInput(VSInputTypes.UV, 0)] public Vector2 UV_VS;
		
		[VSOutputPSInput(VSOutputPSInputTypes.Position, 0)] public Vector4 Position_VSPS;
		[VSOutputPSInput(VSOutputPSInputTypes.InOut, 0)] public Vector4 Light_VSPS;
		[VSOutputPSInput(VSOutputPSInputTypes.InOut, 1)] public Vector2 UV_VSPS;
		
		[PSOutput(PSOutputTypes.Color, 0)] public Vector4 Color_PS;
		
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Matrix4 Camera;
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Vector3 LightDirection, LightDirection2;
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Global)] public Vector4 LightColor, LightColor2;
		[FieldUsage(FieldUsageTypes.VS, MaterialUsages.Instance)] public Matrix4 Transform;
		[FieldUsage(FieldUsageTypes.PS, MaterialUsages.Instance)] public Texture2D Diffuse;
		
		[ShaderMethod(ShaderMethodTypes.VS)]
		public void MainVS()
		{
			Vector4 loc = Transform.MultiplyInvert(new Vector4(Position_VS, 1));
			Position_VSPS = Camera.Multiply(loc);
			UV_VSPS = new Vector2(UV_VS.x, 1.0-UV_VS.y);
			
			Vector3 normal = SL.Normalize(Transform.MultiplyInvert(new Vector4(Normal_VS, 0)).xyz);
			double light = SL.Max(SL.Dot(-LightDirection, normal), 0.0);
			double light2 = SL.Max(SL.Dot(-LightDirection2, normal), 0.0);
			Light_VSPS = ((light * LightColor) + (light2 * LightColor2));
		}
		
		[ShaderMethod(ShaderMethodTypes.PS)]
		public void MainPS()
		{
			Color_PS = Diffuse.Sample(UV_VSPS) * Light_VSPS;
		}
	}
}
