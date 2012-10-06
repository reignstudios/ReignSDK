using Reign.Core;

namespace Reign.Physics
{
	public class Collider
	{
		public float Radius;

		public Collider(float radius)
		{
			Radius = radius;
		}

		public virtual void ApplyRigidRotation(Matrix3 rotationMatrix) {}
	}

	public class CapsuleCollider : Collider
	{
		public Line3 Line, StaticLine;
		public float Diameter;

		public CapsuleCollider(Line3 line, float diameter)
		: base(0)
		{
			Line = line;
			StaticLine = line;
			Diameter = diameter;
			Radius = line.Length() * .5f;
		}

		public override void ApplyRigidRotation(Matrix3 rotationMatrix)
		{
			Line = StaticLine.Transform(rotationMatrix);
		}
	}
}
