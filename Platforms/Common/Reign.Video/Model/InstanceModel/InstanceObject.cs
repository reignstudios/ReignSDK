using Reign.Core;
using System.Runtime.InteropServices;

namespace Reign.Video
{
	public abstract class InstanceObject
	{
		#region Properties
		public Object Object {get; private set;}

		public Vector3 Position, Scale;
		
		private Vector3 rotation;
		public Vector3 Rotation
		{
			get {return rotation;}
			set
			{
				rotation = value;
				Matrix3.FromEuler(ref value, out rotationMatrix);
			}
		}

		private Matrix3 rotationMatrix;
		public Matrix3 RotationMatrix
		{
			get {return rotationMatrix;}
			set
			{
				rotationMatrix = value;
				Matrix3.Euler(ref value, out rotation);
			}
		}

		protected delegate void BindActionDataMethod(float value);
		protected BindActionDataMethod[] bindActionDatas;
		protected int[] boneIndices;
		protected int bindingIndex;

		private Action currentAction;
		public Action CurrentAction
		{
			get {return currentAction;}
			set
			{
				if (currentAction == value) return;
				currentAction = value;
				if (value == null)
				{
					bindActionDatas = null;
					return;
				}

				bindActionDatas = new BindActionDataMethod[value.FCurves.Length];
				if (this.GetType() == typeof(InstanceObjectArmature)) boneIndices = new int[value.FCurves.Length];
				for (int i = 0; i != bindActionDatas.Length; ++i)
				{
					var curve = value.FCurves[i];
					if (curve.Type == FCurveTypes.Object)
					{
						switch (curve.DataPath)
						{
							case "location":
								if (curve.Index == 0) bindActionDatas[i] = bindLocationX;
								else if (curve.Index == 1) bindActionDatas[i] = bindLocationY;
								else if (curve.Index == 2) bindActionDatas[i] = bindLocationZ;
								break;

							case "rotation_euler":
								if (curve.Index == 0) bindActionDatas[i] = bindRotationEulerX;
								else if (curve.Index == 1) bindActionDatas[i] = bindRotationEulerY;
								else if (curve.Index == 2) bindActionDatas[i] = bindRotationEulerZ;
								break;

							case "scale":
								if (curve.Index == 0) bindActionDatas[i] = bindScaleX;
								else if (curve.Index == 1) bindActionDatas[i] = bindScaleY;
								else if (curve.Index == 2) bindActionDatas[i] = bindScaleZ;
								break;

							default: Debug.ThrowError("InstanceObject", "Unsuported FCurve DataPath: " + curve.DataPath); break;
						}
					}
					else if (curve.Type == FCurveTypes.Bone)
					{
						linkObjects(curve.DataPath, curve, i);
					}
					else
					{
						Debug.ThrowError("InstanceObject", "Unsuported FCurve Type: " + curve.Type);
					}
				}
			}
		}
		#endregion

		#region Constructors
		public InstanceObject(Object o)
		{
			Object = o;

			Position = o.Position;
			Scale = o.Scale;
			Rotation = o.Rotation;
		}

		internal void bindObjects(Object o)
		{
			CurrentAction = o.DefaultAction;
		}
		#endregion

		#region Methods
		public virtual void Animate(float frame)
		{
			var action = Object.DefaultAction;
			if (action == null) return;

			if (frame > action.FrameEnd) frame = action.FrameEnd;
			if (frame < action.FrameStart) frame = action.FrameStart;

			// bind curves
			var values = action.calculateValues(frame);
			bindingIndex = 0;
			for (int i = 0; i != values.Length; ++i)
			{
				bindActionDatas[i](values[i]);
				++bindingIndex;
			}

			Matrix3.FromEuler(ref rotation, out rotationMatrix);
		}

		protected virtual void linkObjects(string dataPath, FCurve curve, int i)
		{
			// place holder...
		}

		private void bindLocationX(float value)
		{
			Position.X = value;
		}

		private void bindLocationY(float value)
		{
			Position.Y = value;
		}

		private void bindLocationZ(float value)
		{
			Position.Z = value;
		}

		private void bindRotationEulerX(float value)
		{
			rotation.X = value;
		}

		private void bindRotationEulerY(float value)
		{
			rotation.Y = value;
		}

		private void bindRotationEulerZ(float value)
		{
			rotation.Z = value;
		}

		private void bindScaleX(float value)
		{
			Scale.X = value;
		}

		private void bindScaleY(float value)
		{
			Scale.Y = value;
		}

		private void bindScaleZ(float value)
		{
			Scale.Z = value;
		}

		public virtual void Render()
		{
			// place holder...
		}
		#endregion
	}
}