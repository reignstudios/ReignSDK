using Reign.Core;

namespace Reign.Video
{
	public abstract class InstanceObject
	{
		#region Properties
		public Object Object;

		public Vector3 Position, Scale;
		public Matrix3 Rotation;
		#endregion

		#region Constructors
		public InstanceObject(Object o)
		{
			Object = o;

			Position = o.Position;
			Scale = o.Scale;
			Rotation = o.RotationMatrix;
		}
		#endregion

		#region Methods
		public virtual void Animate(float frame)
		{
			var action = Object.Action;
			if (action == null) return;

			if (frame > action.FrameEnd) frame = action.FrameEnd;
			if (frame < action.FrameStart) frame = action.FrameStart;


		}
		#endregion
	}
}