using Reign.Core;

namespace Reign.Video
{
	public class InstanceModel
	{
		#region Properties
		public Model Model;

		public float CurrentFrame, PlaySpeed;
		public Vector3 Position, Scale;
		public Matrix3 Rotation;

		public InstanceObject[] Objects;
		#endregion

		#region Constructors
		public InstanceModel(Model model)
		{
			Model = model;
			PlaySpeed = 1;

			// orientation
			Position = Vector3.Zero;
			Scale = Vector3.One;
			Rotation = Matrix3.Identity;

			// objects
			Objects = new InstanceObject[model.Objects.Length];
			for (int i = 0; i != Objects.Length; ++i)
			{
				var type = model.Objects.GetType();
				if (type == typeof(ObjectMesh)) Objects[i] = new InstanceObjectMesh((ObjectMesh)model.Objects[i]);
				else if (type == typeof(ObjectArmature)) Objects[i] = new InstanceObjectArmature((ObjectArmature)model.Objects[i]);
				else Debug.ThrowError("InstanceModel", "Unsuported Object type: " + type);
			}
		}
		#endregion

		#region Methods
		public void Play(Time time)
		{
			foreach (var o in Objects)
			{
				o.Animate(CurrentFrame);
			}

			CurrentFrame += Model.FPS * time.Delta * PlaySpeed;
			if (CurrentFrame > Model.FrameEnd) CurrentFrame = Model.FrameStart + (CurrentFrame - Model.FrameEnd);
			if (CurrentFrame < Model.FrameStart) CurrentFrame = Model.FrameEnd + (CurrentFrame - Model.FrameStart);
		}
		#endregion
	}
}