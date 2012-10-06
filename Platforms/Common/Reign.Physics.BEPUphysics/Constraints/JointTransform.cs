using System;
using Reign.Core;

namespace BEPUphysics.Constraints
{
    /// <summary>
    /// Defines a three dimensional orthonormal basis used by a constraint.
    /// </summary>
    public class JointBasis3D
    {
        internal Vector3 localPrimaryAxis = Vector3.Backward;
        internal Vector3 localXAxis = Vector3.Right;
        internal Vector3 localYAxis = Vector3.Up;
        internal Vector3 primaryAxis = Vector3.Backward;
        internal Matrix3 rotationMatrix = Matrix3.Identity;
        internal Vector3 xAxis = Vector3.Right;
        internal Vector3 yAxis = Vector3.Up;

        /// <summary>
        /// Gets the primary axis of the transform in local space.
        /// </summary>
        public Vector3 LocalPrimaryAxis
        {
            get { return localPrimaryAxis; }
        }

        /// <summary>
        /// Gets or sets the local transform of the basis.
        /// </summary>
        public Matrix3 LocalTransform
        {
            get
            {
                var toReturn = new Matrix3(localXAxis, localYAxis, localPrimaryAxis);
                return toReturn;
            }
            set { SetLocalAxes(value); }
        }

        /// <summary>
        /// Gets the X axis of the transform in local space.
        /// </summary>
        public Vector3 LocalXAxis
        {
            get { return localXAxis; }
        }

        /// <summary>
        /// Gets the Y axis of the transform in local space.
        /// </summary>
        public Vector3 LocalYAxis
        {
            get { return localYAxis; }
        }

        /// <summary>
        /// Gets the primary axis of the transform.
        /// </summary>
        public Vector3 PrimaryAxis
        {
            get { return primaryAxis; }
        }

        /// <summary>
        /// Gets or sets the rotation matrix used by the joint transform to convert local space axes to world space.
        /// </summary>
        public Matrix3 RotationMatrix
        {
            get { return rotationMatrix; }
            set
            {
                rotationMatrix = value;
                ComputeWorldSpaceAxes();
            }
        }

        /// <summary>
        /// Gets or sets the world transform of the basis.
        /// </summary>
        public Matrix3 WorldTransform
        {
            get
            {
                var toReturn = new Matrix3(xAxis, yAxis, primaryAxis);
                return toReturn;
            }
            set { SetWorldAxes(value); }
        }

        /// <summary>
        /// Gets the X axis of the transform.
        /// </summary>
        public Vector3 XAxis
        {
            get { return xAxis; }
        }

        /// <summary>
        /// Gets the Y axis of the transform.
        /// </summary>
        public Vector3 YAxis
        {
            get { return yAxis; }
        }


        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="yAxis">Third axis in the transform.</param>
        /// <param name="rotationMatrix">Matrix4 to use to transform the local axes into world space.</param>
        public void SetLocalAxes(Vector3 primaryAxis, Vector3 xAxis, Vector3 yAxis, Matrix3 rotationMatrix)
        {
            this.rotationMatrix = rotationMatrix;
            SetLocalAxes(primaryAxis, xAxis, yAxis);
        }


        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="yAxis">Third axis in the transform.</param>
        public void SetLocalAxes(Vector3 primaryAxis, Vector3 xAxis, Vector3 yAxis)
        {
            if (Math.Abs(primaryAxis.Dot(xAxis)) > Toolbox.BigEpsilon ||
                Math.Abs(primaryAxis.Dot(yAxis)) > Toolbox.BigEpsilon ||
                Math.Abs(xAxis.Dot(yAxis)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");

            localPrimaryAxis = primaryAxis.Normalize();
            localXAxis = xAxis.Normalize();
            localYAxis = yAxis.Normalize();
            ComputeWorldSpaceAxes();
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="matrix">Rotation matrix representing the three axes.
        /// The matrix's backward vector is used as the primary axis.  
        /// The matrix's right vector is used as the x axis.
        /// The matrix's up vector is used as the y axis.</param>
        public void SetLocalAxes(Matrix3 matrix)
        {
            if (Math.Abs(matrix.Back.Dot(matrix.Right)) > Toolbox.BigEpsilon ||
                Math.Abs(matrix.Back.Dot(matrix.Up)) > Toolbox.BigEpsilon ||
                Math.Abs(matrix.Right.Dot(matrix.Up)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");

            localPrimaryAxis = matrix.Back.Normalize();
            localXAxis = matrix.Right.Normalize();
            localYAxis = matrix.Up.Normalize();
            ComputeWorldSpaceAxes();
        }


        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="yAxis">Third axis in the transform.</param>
        /// <param name="rotationMatrix">Matrix4 to use to transform the local axes into world space.</param>
        public void SetWorldAxes(Vector3 primaryAxis, Vector3 xAxis, Vector3 yAxis, Matrix3 rotationMatrix)
        {
            this.rotationMatrix = rotationMatrix;
            SetWorldAxes(primaryAxis, xAxis, yAxis);
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="yAxis">Third axis in the transform.</param>
        public void SetWorldAxes(Vector3 primaryAxis, Vector3 xAxis, Vector3 yAxis)
        {
            if (Math.Abs(primaryAxis.Dot(xAxis)) > Toolbox.BigEpsilon ||
                Math.Abs(primaryAxis.Dot(yAxis)) > Toolbox.BigEpsilon ||
                Math.Abs(xAxis.Dot(yAxis)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");

            this.primaryAxis = primaryAxis.Normalize();
            this.xAxis = xAxis.Normalize();
            this.yAxis = yAxis.Normalize();
            Vector3.TransformTranspose(ref this.primaryAxis, ref rotationMatrix, out localPrimaryAxis);
            Vector3.TransformTranspose(ref this.xAxis, ref rotationMatrix, out localXAxis);
            Vector3.TransformTranspose(ref this.yAxis, ref rotationMatrix, out localYAxis);
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="matrix">Rotation matrix representing the three axes.
        /// The matrix's backward vector is used as the primary axis.  
        /// The matrix's right vector is used as the x axis.
        /// The matrix's up vector is used as the y axis.</param>
        public void SetWorldAxes(Matrix3 matrix)
        {
            if (Math.Abs(matrix.Back.Dot(matrix.Right)) > Toolbox.BigEpsilon ||
                Math.Abs(matrix.Back.Dot(matrix.Up)) > Toolbox.BigEpsilon ||
                Math.Abs(matrix.Right.Dot(matrix.Up)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");

            primaryAxis = matrix.Back.Normalize();
            xAxis = matrix.Right.Normalize();
            yAxis = matrix.Up.Normalize();
            Vector3.TransformTranspose(ref this.primaryAxis, ref rotationMatrix, out localPrimaryAxis);
            Vector3.TransformTranspose(ref this.xAxis, ref rotationMatrix, out localXAxis);
            Vector3.TransformTranspose(ref this.yAxis, ref rotationMatrix, out localYAxis);
        }

        internal void ComputeWorldSpaceAxes()
        {
            Vector3.Transform(ref localPrimaryAxis, ref rotationMatrix, out primaryAxis);
            Vector3.Transform(ref localXAxis, ref rotationMatrix, out xAxis);
            Vector3.Transform(ref localYAxis, ref rotationMatrix, out yAxis);
        }
    }

    /// <summary>
    /// Defines a two axes which are perpendicular to each other used by a constraint.
    /// </summary>
    public class JointBasis2D
    {
        internal Vector3 localPrimaryAxis = Vector3.Backward;
        internal Vector3 localXAxis = Vector3.Right;
        internal Vector3 primaryAxis = Vector3.Backward;
        internal Matrix3 rotationMatrix = Matrix3.Identity;
        internal Vector3 xAxis = Vector3.Right;

        /// <summary>
        /// Gets the primary axis of the transform in local space.
        /// </summary>
        public Vector3 LocalPrimaryAxis
        {
            get { return localPrimaryAxis; }
        }

        /// <summary>
        /// Gets the X axis of the transform in local space.
        /// </summary>
        public Vector3 LocalXAxis
        {
            get { return localXAxis; }
        }

        /// <summary>
        /// Gets the primary axis of the transform.
        /// </summary>
        public Vector3 PrimaryAxis
        {
            get { return primaryAxis; }
        }

        /// <summary>
        /// Gets or sets the rotation matrix used by the joint transform to convert local space axes to world space.
        /// </summary>
        public Matrix3 RotationMatrix
        {
            get { return rotationMatrix; }
            set
            {
                rotationMatrix = value;
                ComputeWorldSpaceAxes();
            }
        }

        /// <summary>
        /// Gets the X axis of the transform.
        /// </summary>
        public Vector3 XAxis
        {
            get { return xAxis; }
        }


        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="rotationMatrix">Matrix4 to use to transform the local axes into world space.</param>
        public void SetLocalAxes(Vector3 primaryAxis, Vector3 xAxis, Matrix3 rotationMatrix)
        {
            this.rotationMatrix = rotationMatrix;
            SetLocalAxes(primaryAxis, xAxis);
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        public void SetLocalAxes(Vector3 primaryAxis, Vector3 xAxis)
        {
            if (Math.Abs(primaryAxis.Dot(xAxis)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform are not perpendicular.  Ensure that the specified axes form a valid constraint.");

            localPrimaryAxis = primaryAxis.Normalize();
            localXAxis = xAxis.Normalize();
            ComputeWorldSpaceAxes();
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="matrix">Rotation matrix representing the three axes.
        /// The matrix's backward vector is used as the primary axis.  
        /// The matrix's right vector is used as the x axis.</param>
        public void SetLocalAxes(Matrix3 matrix)
        {
            if (Math.Abs(matrix.Back.Dot(matrix.Right)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform are not perpendicular.  Ensure that the specified axes form a valid constraint.");
            localPrimaryAxis = matrix.Back.Normalize();
            localXAxis = matrix.Right.Normalize();
            ComputeWorldSpaceAxes();
        }


        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        /// <param name="rotationMatrix">Matrix4 to use to transform the local axes into world space.</param>
        public void SetWorldAxes(Vector3 primaryAxis, Vector3 xAxis, Matrix3 rotationMatrix)
        {
            this.rotationMatrix = rotationMatrix;
            SetWorldAxes(primaryAxis, xAxis);
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
        /// <param name="xAxis">Second axis in the transform.</param>
        public void SetWorldAxes(Vector3 primaryAxis, Vector3 xAxis)
        {
            if (Math.Abs(primaryAxis.Dot(xAxis)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform are not perpendicular.  Ensure that the specified axes form a valid constraint.");
            this.primaryAxis = primaryAxis.Normalize();
            this.xAxis = xAxis.Normalize();
            Vector3.TransformTranspose(ref this.primaryAxis, ref rotationMatrix, out localPrimaryAxis);
            Vector3.TransformTranspose(ref this.xAxis, ref rotationMatrix, out localXAxis);
        }

        /// <summary>
        /// Sets up the axes of the transform and ensures that it is an orthonormal basis.
        /// </summary>
        /// <param name="matrix">Rotation matrix representing the three axes.
        /// The matrix's backward vector is used as the primary axis.  
        /// The matrix's right vector is used as the x axis.</param>
        public void SetWorldAxes(Matrix3 matrix)
        {
            if (Math.Abs(matrix.Back.Dot(matrix.Right)) > Toolbox.BigEpsilon)
                throw new ArgumentException("The axes provided to the joint transform are not perpendicular.  Ensure that the specified axes form a valid constraint.");
            primaryAxis = matrix.Back.Normalize();
            xAxis = matrix.Right.Normalize();
            Vector3.TransformTranspose(ref this.primaryAxis, ref rotationMatrix, out localPrimaryAxis);
            Vector3.TransformTranspose(ref this.xAxis, ref rotationMatrix, out localXAxis);
        }

        internal void ComputeWorldSpaceAxes()
        {
            Vector3.Transform(ref localPrimaryAxis, ref rotationMatrix, out primaryAxis);
            Vector3.Transform(ref localXAxis, ref rotationMatrix, out xAxis);
        }
    }
}