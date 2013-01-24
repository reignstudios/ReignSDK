using Reign.Core;

namespace Reign.Video
{
	public class Camera
	{
		#region Properties
		public Vector3 Position, LookAtPosition, UpPosition;
		public float Near, Far, Fov, Aspect;

		public Matrix3 BillBoardMatrix, BillBoardMatrixTranspose;
		public Matrix4 ViewMatrix, ProjectionMatrix, TransformMatrix, TransformInverseMatrix;
		public ViewPortI ViewPort;
		#endregion

		#region Constructors
		public Camera(ViewPortI viewPort)
		{
			ViewPort = (ViewPortI)viewPort;
			Position = new Vector3(10, 10, 10);
			LookAtPosition = new Vector3(0, 0, 0);
			UpPosition = new Vector3(10, 11, 10);
			Near = 1;
			Far = 500;
			Fov = MathUtilities.DegToRad(45);
			Aspect = float.NaN;
		}

		public Camera(ViewPortI viewPort, Vector3 position, Vector3 lookAtPosition, Vector3 upPosition)
		{
			ViewPort = (ViewPortI)viewPort;
			Position = position;
			LookAtPosition = lookAtPosition;
			UpPosition = upPosition;
			Near = 1;
			Far = 500;
			Fov = MathUtilities.DegToRad(45);
			Aspect = float.NaN;
		}

		public Camera(ViewPortI viewPort, Vector3 position, Vector3 lookAtPosition, Vector3 upPosition, float near, float far, float fov)
		{
			ViewPort = (ViewPortI)viewPort;
			Position = position;
			LookAtPosition = lookAtPosition;
			UpPosition = upPosition;
			Near = near;
			Far = far;
			Fov = fov;
			Aspect = float.NaN;
		}
		#endregion
	
		#region Methods
		public void Offset(float x, float y, float z)
		{
			Offset(new Vector3(x, y, z));
		}

		public void Offset(Vector3 value)
		{
			Position += value;
			LookAtPosition += value;
			UpPosition += value;
		}

		public void Zoom(float value, float stopRadis)
		{
			var vec = (LookAtPosition - Position);
			float dis = 0;
			vec = vec.Normalize(out dis);
			dis = ((dis-stopRadis) - value);
			if (dis < 0) value += dis;
			vec *= value;
			Position += vec;
			UpPosition += vec;
		}
	
		public void RotateAroundLocation(float radiansX, float radiansY, float radiansZ) {RotateAroundLocation(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundLocation(Vector3 radians)
		{
			var matrix = Matrix3.LookAt((LookAtPosition - Position), (UpPosition - Position));
			var matrixTranspose = matrix.Transpose();
			matrix = matrix.RotateAroundAxisX(radians.X);
			matrix = matrix.RotateAroundAxisY(radians.Y);
			matrix = matrix.RotateAroundAxisZ(radians.Z);
			LookAtPosition -= Position;
			LookAtPosition = LookAtPosition.Transform(matrixTranspose);
			LookAtPosition = LookAtPosition.Transform(matrix);
			LookAtPosition += Position;
			UpPosition -= Position;
			UpPosition = UpPosition.Transform(matrixTranspose);
			UpPosition = UpPosition.Transform(matrix);
			UpPosition += Position;
		}

		public void RotateAroundLookLocation(float radiansX, float radiansY, float radiansZ) {RotateAroundLookLocation(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundLookLocation(Vector3 radians)
		{
			var matrix = Matrix3.LookAt((LookAtPosition - Position), (UpPosition - Position));
			var matrixTranspose = matrix.Transpose();
			matrix = matrix.RotateAroundAxisX(radians.X);
			matrix = matrix.RotateAroundAxisY(radians.Y);
			matrix = matrix.RotateAroundAxisZ(radians.Z);
			Position -= LookAtPosition;
			Position = Position.Transform(matrixTranspose);
			Position = Position.Transform(matrix);
			Position += LookAtPosition;
			UpPosition -= LookAtPosition;
			UpPosition = UpPosition.Transform(matrixTranspose);
			UpPosition = UpPosition.Transform(matrix);
			UpPosition += LookAtPosition;
		}

		public void RotateAroundUpLocation(float radiansX, float radiansY, float radiansZ) {RotateAroundUpLocation(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundUpLocation(Vector3 radians)
		{
			var matrix = Matrix3.LookAt((LookAtPosition - Position), (UpPosition - Position));
			var matrixTranspose = matrix.Transpose();
			matrix = matrix.RotateAroundAxisX(radians.X);
			matrix = matrix.RotateAroundAxisY(radians.Y);
			matrix = matrix.RotateAroundAxisZ(radians.Z);
			LookAtPosition -= UpPosition;
			LookAtPosition = LookAtPosition.Transform(matrixTranspose);
			LookAtPosition = LookAtPosition.Transform(matrix);
			LookAtPosition += UpPosition;
			Position -= UpPosition;
			Position = Position.Transform(matrixTranspose);
			Position = Position.Transform(matrix);
			Position += UpPosition;
		}

		public void RotateAroundLocationWorld(float radiansX, float radiansY, float radiansZ) {RotateAroundLocationWorld(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundLocationWorld(Vector3 radians)
		{
			var matrix = Matrix3.Identity;
			matrix = matrix.RotateAroundWorldAxisX(radians.X);
			matrix = matrix.RotateAroundWorldAxisY(radians.Y);
			matrix = matrix.RotateAroundWorldAxisZ(radians.Z);
			LookAtPosition -= Position;
			LookAtPosition = LookAtPosition.Transform(matrix);
			LookAtPosition += Position;
			UpPosition -= Position;
			UpPosition = UpPosition.Transform(matrix);
			UpPosition += Position;
		}

		public void RotateAroundLookLocationWorld(float radiansX, float radiansY, float radiansZ) {RotateAroundLookLocationWorld(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundLookLocationWorld(Vector3 radians)
		{
			var matrix = Matrix3.Identity;
			matrix = matrix.RotateAroundWorldAxisX(radians.X);
			matrix = matrix.RotateAroundWorldAxisY(radians.Y);
			matrix = matrix.RotateAroundWorldAxisZ(radians.Z);
			Position -= LookAtPosition;
			Position = Position.Transform(matrix);
			Position += LookAtPosition;
			UpPosition -= LookAtPosition;
			UpPosition = UpPosition.Transform(matrix);
			UpPosition += LookAtPosition;
		}

		public void RotateAroundUpLocationWorld(float radiansX, float radiansY, float radiansZ) {RotateAroundUpLocationWorld(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundUpLocationWorld(Vector3 radians)
		{
			var matrix = Matrix3.Identity;
			matrix = matrix.RotateAroundWorldAxisX(radians.X);
			matrix = matrix.RotateAroundWorldAxisY(radians.Y);
			matrix = matrix.RotateAroundWorldAxisZ(radians.Z);  
			LookAtPosition -= UpPosition;
			LookAtPosition = LookAtPosition.Transform(matrix);
			LookAtPosition += UpPosition;
			Position -= UpPosition;
			Position = Position.Transform(matrix);
			Position += UpPosition;
		}

		public void Rotate(Line3 pLine, float radians)
		{
			var vector = (pLine.Point2 - pLine.Point1).NormalizeSafe();
			Position -= pLine.Point1;
			LookAtPosition -= pLine.Point1;
			UpPosition -= pLine.Point1;
			Position = Position.RotateAround(vector, radians);
			LookAtPosition = LookAtPosition.RotateAround(vector, radians);
			UpPosition = UpPosition.RotateAround(vector, radians);
			Position += pLine.Point1;
			LookAtPosition += pLine.Point1;
			UpPosition += pLine.Point1;
		}

		public void ApplyBillBoard()
		{
			BillBoardMatrix = Matrix3.LookAt((Position - LookAtPosition), (UpPosition - Position));
			BillBoardMatrixTranspose = BillBoardMatrix.Transpose();
		}
	
		public void Apply()
		{
			ViewMatrix = Matrix4.LookAt(Position, LookAtPosition, UpPosition-Position);
			ProjectionMatrix = Matrix4.Perspective(Fov, float.IsNaN(Aspect) ? ViewPort.AspectRatio : Aspect, Near, Far);
			TransformMatrix = ViewMatrix.Multiply(ProjectionMatrix);
			TransformInverseMatrix = TransformMatrix.Invert();
		}

		public void ApplyOrthographic()
		{
			ViewMatrix = Matrix4.LookAt(Position, LookAtPosition, UpPosition-Position);
			ProjectionMatrix = Matrix4.Orthographic(ViewPort.Size.Width, ViewPort.Size.Height, Near, Far);
			TransformMatrix = ViewMatrix.Multiply(ProjectionMatrix);
			TransformInverseMatrix = TransformMatrix.Invert();
		}

		public void ApplyOrthographicCentered()
		{
			ViewMatrix = Matrix4.LookAt(Position, LookAtPosition, UpPosition-Position);
			ProjectionMatrix = Matrix4.OrthographicCentered(ViewPort.Size.Width, ViewPort.Size.Height, Near, Far);
			TransformMatrix = ViewMatrix.Multiply(ProjectionMatrix);
			TransformInverseMatrix = TransformMatrix.Invert();
		}

		public Vector2 Project(Vector3 position)
		{
			var pos = new Vector4(position, 1);
			return pos.Project(ProjectionMatrix, ViewMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height).ToVector2();
		}

		public Vector4 Project(Vector4 position)
		{
			return position.Project(ProjectionMatrix, ViewMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height);
		}

		public Vector3 UnProjectNormalized(Vector2 screenPosition)
		{
			var pos = new Vector4(screenPosition, 0, 1);
			var near = pos.UnProject(TransformInverseMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height);
			pos.Z = 1;
			var far = pos.UnProject(TransformInverseMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height);
			return (far - near).ToVector3().Normalize();
		}

		public Vector4 UnProject(Vector2 screenPosition)
		{
			var pos = new Vector4(screenPosition, 0, 1);
			return pos.UnProject(TransformInverseMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height);
		}

		public Vector4 UnProject(Vector4 screenPosition)
		{
			return screenPosition.UnProject(TransformInverseMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height);
		}
		#endregion
	}
}