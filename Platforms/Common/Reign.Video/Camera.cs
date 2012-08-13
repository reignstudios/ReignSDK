using Reign.Core;

namespace Reign.Video
{
	public class Camera
	{
		#region Properties
		public Vector3 Location, LookLocation, UpLocation;
		public float Near, Far, Fov, Aspect;

		public Matrix3 BillBoardMatrix, BillBoardMatrixTranspose;
		public Matrix4 ViewMatrix, ProjectionMatrix, TransformMatrix, TransformInverseMatrix;
		public ViewPortI ViewPort;
		#endregion

		#region Constructors
		public Camera(ViewPortI viewPort)
		{
			ViewPort = (ViewPortI)viewPort;
			Location = new Vector3(10, 10, 10);
			LookLocation = new Vector3(0, 0, 0);
			UpLocation = new Vector3(10, 11, 10);
			Near = 1;
			Far = 500;
			Fov = Math.DegToRad(45);
			Aspect = float.NaN;
		}

		public Camera(ViewPortI viewPort, Vector3 location, Vector3 lookLocation, Vector3 upLocation)
		{
			ViewPort = (ViewPortI)viewPort;
			Location = location;
			LookLocation = lookLocation;
			UpLocation = upLocation;
			Near = 1;
			Far = 500;
			Fov = Math.DegToRad(45);
			Aspect = float.NaN;
		}

		public Camera(ViewPortI viewPort, Vector3 location, Vector3 lookLocation, Vector3 upLocation, float near, float far, float fov)
		{
			ViewPort = (ViewPortI)viewPort;
			Location = location;
			LookLocation = lookLocation;
			UpLocation = upLocation;
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
			Location += value;
			LookLocation += value;
			UpLocation += value;
		}

		public void Zoom(float value, float stopRadis)
		{
			var vec = (LookLocation - Location);
			float dis = 0;
			vec = vec.Normalize(out dis);
			dis = ((dis-stopRadis) - value);
			if (dis < 0) value += dis;
			vec *= value;
			Location += vec;
			UpLocation += vec;
		}
	
		public void RotateAroundLocation(float radiansX, float radiansY, float radiansZ) {RotateAroundLocation(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundLocation(Vector3 radians)
		{
			var matrix = Matrix3.Cross((LookLocation - Location), (UpLocation - Location));
			var matrixTranspose = matrix.Transpose();
			matrix = matrix.RotateAroundAxisX(radians.X);
			matrix = matrix.RotateAroundAxisY(radians.Y);
			matrix = matrix.RotateAroundAxisZ(radians.Z);
			LookLocation -= Location;
			LookLocation = LookLocation.Transform(matrixTranspose);
			LookLocation = LookLocation.Transform(matrix);
			LookLocation += Location;
			UpLocation -= Location;
			UpLocation = UpLocation.Transform(matrixTranspose);
			UpLocation = UpLocation.Transform(matrix);
			UpLocation += Location;
		}

		public void RotateAroundLookLocation(float radiansX, float radiansY, float radiansZ) {RotateAroundLookLocation(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundLookLocation(Vector3 radians)
		{
			var matrix = Matrix3.Cross((LookLocation - Location), (UpLocation - Location));
			var matrixTranspose = matrix.Transpose();
			matrix = matrix.RotateAroundAxisX(radians.X);
			matrix = matrix.RotateAroundAxisY(radians.Y);
			matrix = matrix.RotateAroundAxisZ(radians.Z);
			Location -= LookLocation;
			Location = Location.Transform(matrixTranspose);
			Location = Location.Transform(matrix);
			Location += LookLocation;
			UpLocation -= LookLocation;
			UpLocation = UpLocation.Transform(matrixTranspose);
			UpLocation = UpLocation.Transform(matrix);
			UpLocation += LookLocation;
		}

		public void RotateAroundUpLocation(float radiansX, float radiansY, float radiansZ) {RotateAroundUpLocation(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundUpLocation(Vector3 radians)
		{
			var matrix = Matrix3.Cross((LookLocation - Location), (UpLocation - Location));
			var matrixTranspose = matrix.Transpose();
			matrix = matrix.RotateAroundAxisX(radians.X);
			matrix = matrix.RotateAroundAxisY(radians.Y);
			matrix = matrix.RotateAroundAxisZ(radians.Z);
			LookLocation -= UpLocation;
			LookLocation = LookLocation.Transform(matrixTranspose);
			LookLocation = LookLocation.Transform(matrix);
			LookLocation += UpLocation;
			Location -= UpLocation;
			Location = Location.Transform(matrixTranspose);
			Location = Location.Transform(matrix);
			Location += UpLocation;
		}

		public void RotateAroundLocationWorld(float radiansX, float radiansY, float radiansZ) {RotateAroundLocationWorld(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundLocationWorld(Vector3 radians)
		{
			var matrix = Matrix3.Identity;
			matrix = matrix.RotateAroundWorldAxisX(radians.X);
			matrix = matrix.RotateAroundWorldAxisY(radians.Y);
			matrix = matrix.RotateAroundWorldAxisZ(radians.Z);
			LookLocation -= Location;
			LookLocation = LookLocation.Transform(matrix);
			LookLocation += Location;
			UpLocation -= Location;
			UpLocation = UpLocation.Transform(matrix);
			UpLocation += Location;
		}

		public void RotateAroundLookLocationWorld(float radiansX, float radiansY, float radiansZ) {RotateAroundLookLocationWorld(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundLookLocationWorld(Vector3 radians)
		{
			var matrix = Matrix3.Identity;
			matrix = matrix.RotateAroundWorldAxisX(radians.X);
			matrix = matrix.RotateAroundWorldAxisY(radians.Y);
			matrix = matrix.RotateAroundWorldAxisZ(radians.Z);
			Location -= LookLocation;
			Location = Location.Transform(matrix);
			Location += LookLocation;
			UpLocation -= LookLocation;
			UpLocation = UpLocation.Transform(matrix);
			UpLocation += LookLocation;
		}

		public void RotateAroundUpLocationWorld(float radiansX, float radiansY, float radiansZ) {RotateAroundUpLocationWorld(new Vector3(radiansX, radiansY, radiansZ));}
		public void RotateAroundUpLocationWorld(Vector3 radians)
		{
			var matrix = Matrix3.Identity;
			matrix = matrix.RotateAroundWorldAxisX(radians.X);
			matrix = matrix.RotateAroundWorldAxisY(radians.Y);
			matrix = matrix.RotateAroundWorldAxisZ(radians.Z);  
			LookLocation -= UpLocation;
			LookLocation = LookLocation.Transform(matrix);
			LookLocation += UpLocation;
			Location -= UpLocation;
			Location = Location.Transform(matrix);
			Location += UpLocation;
		}

		public void Rotate(Line3 pLine, float radians)
		{
			var vector = (pLine.P2 - pLine.P1).NormalizeSafe();
			Location -= pLine.P1;
			LookLocation -= pLine.P1;
			UpLocation -= pLine.P1;
			Location = Location.Rotate(vector, radians);
			LookLocation = LookLocation.Rotate(vector, radians);
			UpLocation = UpLocation.Rotate(vector, radians);
			Location += pLine.P1;
			LookLocation += pLine.P1;
			UpLocation += pLine.P1;
		}

		public void ApplyBillBoard()
		{
			BillBoardMatrix = Matrix3.Cross((Location - LookLocation), (UpLocation - Location));
			BillBoardMatrixTranspose = BillBoardMatrix.Transpose();
		}
	
		public virtual void Apply()
		{
			ViewMatrix = Matrix4.LookAt(Location, LookLocation, UpLocation-Location);
			ProjectionMatrix = Matrix4.Perspective(Fov, float.IsNaN(Aspect) ? ViewPort.AspectRatio : Aspect, Near, Far);
			TransformMatrix = ViewMatrix.Multiply(ProjectionMatrix);
			TransformInverseMatrix = TransformMatrix.Invert();
		}

		public void ApplyOrthographic()
		{
			ViewMatrix = Matrix4.LookAt(Location, LookLocation, UpLocation-Location);
			ProjectionMatrix = Matrix4.Orthographic(ViewPort.Size.Width, ViewPort.Size.Height, Near, Far);
			TransformMatrix = ViewMatrix.Multiply(ProjectionMatrix);
			TransformInverseMatrix = TransformMatrix.Invert();
		}

		public void ApplyOrthographicCentered()
		{
			ViewMatrix = Matrix4.LookAt(Location, LookLocation, UpLocation-Location);
			ProjectionMatrix = Matrix4.OrthographicCentered(ViewPort.Size.Width, ViewPort.Size.Height, Near, Far);
			TransformMatrix = ViewMatrix.Multiply(ProjectionMatrix);
			TransformInverseMatrix = TransformMatrix.Invert();
		}

		public Vector3 Project(Vector3 position)
		{
			var pos = new Vector4(position, 1);
			return pos.Project(ProjectionMatrix, ViewMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height).ToVector3();
		}

		public Vector4 Project(Vector4 position)
		{
			return position.Project(ProjectionMatrix, ViewMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height);
		}

		public Vector3 UnProjectNormalized(Vector3 screenPosition)
		{
			var pos = new Vector4(screenPosition, 0);
			var near = pos.UnProject(TransformInverseMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height);
			pos.W = 1;
			var far = pos.UnProject(TransformInverseMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height);
			return (far - near).ToVector3().Normalize();
		}

		public Vector3 UnProject(Vector3 screenPosition)
		{
			var pos = new Vector4(screenPosition, 1);
			return pos.UnProject(TransformInverseMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height).ToVector3();
		}

		public Vector4 UnProject(Vector4 screenPosition)
		{
			return screenPosition.UnProject(TransformInverseMatrix, ViewPort.Location.X, ViewPort.Location.Y, ViewPort.Size.Width, ViewPort.Size.Height);
		}
		#endregion
	}
}