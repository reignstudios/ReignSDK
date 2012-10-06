using System;
using BEPUphysics.Entities;
using Reign.Core;

namespace BEPUphysics.Constraints.TwoEntity.Joints
{
    /// <summary>
    /// Constrains two entities so that one has a point that stays on a line defined by the other.
    /// </summary>
    public class PointOnLineJoint : Joint, I2DImpulseConstraintWithError, I2DJacobianConstraint
    {
        private Vector2 accumulatedImpulse;
        private Vector3 angularA1;
        private Vector3 angularA2;
        private Vector3 angularB1;
        private Vector3 angularB2;
        private Vector2 biasVelocity;
        private Vector3 localRestrictedAxis1; //on a
        private Vector3 localRestrictedAxis2; //on a
        private Vector2 error;
        private Vector3 localAxisAnchor; //on a
        private Vector3 localLineDirection; //on a
        private Vector3 localPoint; //on b
        private Vector3 worldLineAnchor;
        private Vector3 worldLineDirection;
        private Vector3 worldPoint;
        private Matrix2 negativeEffectiveMassMatrix;

        //Jacobians
        //(Linear jacobians are just:
        // axis 1   -axis 1
        // axis 2   -axis 2 )
        private Vector3 rA, rB;
        private Vector3 worldRestrictedAxis1, worldRestrictedAxis2;

        /// <summary>
        /// Constructs a joint which constrains a point of one body to be on a line based on the other body.
        /// To finish the initialization, specify the connections (ConnectionA and ConnectionB),
        /// the LineAnchor, the LineDirection, and the Point (or the entity-local versions).
        /// This constructor sets the constraint's IsActive property to false by default.
        /// </summary>
        public PointOnLineJoint()
        {
            IsActive = false;
        }

        /// <summary>
        /// Constructs a joint which constrains a point of one body to be on a line based on the other body.
        /// </summary>
        /// <param name="connectionA">First connected entity which defines the line.</param>
        /// <param name="connectionB">Second connected entity which has a point.</param>
        /// <param name="lineAnchor">Location off of which the line is based in world space.</param>
        /// <param name="lineDirection">Direction of the line in world space.</param>
        /// <param name="pointLocation">Location of the point anchored to connectionB in world space.</param>
        public PointOnLineJoint(Entity connectionA, Entity connectionB, Vector3 lineAnchor, Vector3 lineDirection, Vector3 pointLocation)
        {
            ConnectionA = connectionA;
            ConnectionB = connectionB;

            LineAnchor = lineAnchor;
            LineDirection = lineDirection;
            Point = pointLocation;
        }

        /// <summary>
        /// Gets or sets the line anchor in world space.
        /// </summary>
        public Vector3 LineAnchor
        {
            get { return worldLineAnchor; }
            set
            {
                localAxisAnchor = value - connectionA.position;
                Vector3.TransformTranspose(ref localAxisAnchor, ref connectionA.orientationMatrix, out localAxisAnchor);
                worldLineAnchor = value;
            }
        }

        /// <summary>
        /// Gets or sets the line direction in world space.
        /// </summary>
        public Vector3 LineDirection
        {
            get { return worldLineDirection; }
            set
            {
                worldLineDirection = value.Normalize();
                Vector3.TransformTranspose(ref worldLineDirection, ref connectionA.orientationMatrix, out localLineDirection);
                UpdateRestrictedAxes();
            }
        }


        /// <summary>
        /// Gets or sets the line anchor in connection A's local space.
        /// </summary>
        public Vector3 LocalLineAnchor
        {
            get { return localAxisAnchor; }
            set
            {
                localAxisAnchor = value;
                Vector3.Transform(ref localAxisAnchor, ref connectionA.orientationMatrix, out worldLineAnchor);
                Vector3.Add(ref worldLineAnchor, ref connectionA.position, out worldLineAnchor);
            }
        }

        /// <summary>
        /// Gets or sets the line direction in connection A's local space.
        /// </summary>
        public Vector3 LocalLineDirection
        {
            get { return localLineDirection; }
            set
            {
                localLineDirection = value.Normalize();
                Vector3.Transform(ref localLineDirection, ref connectionA.orientationMatrix, out worldLineDirection);
                UpdateRestrictedAxes();
            }
        }

        /// <summary>
        /// Gets or sets the point's location in connection B's local space.
        /// The point is the location that is attached to the line.
        /// </summary>
        public Vector3 LocalPoint
        {
            get { return localPoint; }
            set
            {
                localPoint = value;
                Vector3.Transform(ref localPoint, ref connectionB.orientationMatrix, out worldPoint);
                Vector3.Add(ref worldPoint, ref connectionB.position, out worldPoint);
            }
        }

        /// <summary>
        /// Gets the offset from A to the connection point between the entities.
        /// </summary>
        public Vector3 OffsetA
        {
            get { return rA; }
        }

        /// <summary>
        /// Gets the offset from B to the connection point between the entities.
        /// </summary>
        public Vector3 OffsetB
        {
            get { return rB; }
        }

        /// <summary>
        /// Gets or sets the point's location in world space.
        /// The point is the location on connection B that is attached to the line.
        /// </summary>
        public Vector3 Point
        {
            get { return worldPoint; }
            set
            {
                worldPoint = value;
                localPoint = worldPoint - connectionB.position;
                Vector3.TransformTranspose(ref localPoint, ref connectionB.orientationMatrix, out localPoint);
            }
        }

        #region I2DImpulseConstraintWithError Members

        /// <summary>
        /// Gets the current relative velocity between the connected entities with respect to the constraint.
        /// </summary>
        public Vector2 RelativeVelocity
        {
            get
            {
                Vector2 lambda = new Vector2();
                Vector3 dv;
                Vector3 aVel, bVel;
                Vector3.Cross(ref connectionA.angularVelocity, ref rA, out aVel);
                Vector3.Add(ref aVel, ref connectionA.linearVelocity, out aVel);
                Vector3.Cross(ref connectionB.angularVelocity, ref rB, out bVel);
                Vector3.Add(ref bVel, ref connectionB.linearVelocity, out bVel);
                Vector3.Sub(ref aVel, ref bVel, out dv);
                Vector3.Dot(ref dv, ref worldRestrictedAxis1, out lambda.X);
                Vector3.Dot(ref dv, ref worldRestrictedAxis2, out lambda.Y);
                return lambda;
            }
        }


        /// <summary>
        /// Gets the total impulse applied by this constraint.
        /// </summary>
        public Vector2 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        /// <summary>
        /// Gets the current constraint error.
        /// </summary>
        public Vector2 Error
        {
            get { return error; }
        }

        #endregion

        #region I2DJacobianConstraint Members

        /// <summary>
        /// Gets the linear jacobian entry for the first connected entity.
        /// </summary>
        /// <param name="jacobianX">First linear jacobian entry for the first connected entity.</param>
        /// <param name="jacobianY">Second linear jacobian entry for the first connected entity.</param>
        public void GetLinearJacobianA(out Vector3 jacobianX, out Vector3 jacobianY)
        {
            jacobianX = worldRestrictedAxis1;
            jacobianY = worldRestrictedAxis2;
        }

        /// <summary>
        /// Gets the linear jacobian entry for the second connected entity.
        /// </summary>
        /// <param name="jacobianX">First linear jacobian entry for the second connected entity.</param>
        /// <param name="jacobianY">Second linear jacobian entry for the second connected entity.</param>
        public void GetLinearJacobianB(out Vector3 jacobianX, out Vector3 jacobianY)
        {
            jacobianX = -worldRestrictedAxis1;
            jacobianY = -worldRestrictedAxis2;
        }

        /// <summary>
        /// Gets the angular jacobian entry for the first connected entity.
        /// </summary>
        /// <param name="jacobianX">First angular jacobian entry for the first connected entity.</param>
        /// <param name="jacobianY">Second angular jacobian entry for the first connected entity.</param>
        public void GetAngularJacobianA(out Vector3 jacobianX, out Vector3 jacobianY)
        {
            jacobianX = angularA1;
            jacobianY = angularA2;
        }

        /// <summary>
        /// Gets the angular jacobian entry for the second connected entity.
        /// </summary>
        /// <param name="jacobianX">First angular jacobian entry for the second connected entity.</param>
        /// <param name="jacobianY">Second angular jacobian entry for the second connected entity.</param>
        public void GetAngularJacobianB(out Vector3 jacobianX, out Vector3 jacobianY)
        {
            jacobianX = angularB1;
            jacobianY = angularB2;
        }

        /// <summary>
        /// Gets the mass matrix of the constraint.
        /// </summary>
        /// <param name="massMatrix">Constraint's mass matrix.</param>
        public void GetMassMatrix(out Matrix2 massMatrix)
        {
            Matrix2.Neg(ref negativeEffectiveMassMatrix, out massMatrix);
        }

        #endregion

        /// <summary>
        /// Calculates and applies corrective impulses.
        /// Called automatically by space.
        /// </summary>
        public override float SolveIteration()
        {
            #region Theory

            //lambda = -mc * (Jv + b)
            // PraT = [ bx by bz ] * [  0   raz -ray ] = [ (-by * raz + bz * ray) (bx * raz - bz * rax) (-bx * ray + by * rax) ]
            //        [ cx cy cz ]   [ -raz  0   rax ]   [ (-cy * raz + cz * ray) (cx * raz - cz * rax) (-cx * ray + cy * rax) ]
            //                       [ ray -rax   0  ]
            //
            // PrbT = [ bx by bz ] * [  0   rbz -rby ] = [ (-by * rbz + bz * rby) (bx * rbz - bz * rbx) (-bx * rby + by * rbx) ]
            //        [ cx cy cz ]   [ -rbz  0   rbx ]   [ (-cy * rbz + cz * rby) (cx * rbz - cz * rbx) (-cx * rby + cy * rbx) ]
            //                       [ rby -rbx   0  ]
            // Jv = [ bx by bz  PraT  -bx -by -bz  -Prbt ] * [ vax ]
            //      [ cx cy cz        -cx -cy -cz        ]   [ vay ]
            //                                               [ vaz ]
            //                                               [ wax ]
            //                                               [ way ]
            //                                               [ waz ]
            //                                               [ vbx ]
            //                                               [ vby ]
            //                                               [ vbz ]
            //                                               [ wbx ]
            //                                               [ wby ]
            //                                               [ wbz ]
            // va' = [ bx * vax + by * vay + bz * vaz ] = [ b * va ]
            //       [ cx * vax + cy * vay + cz * vaz ]   [ c * va ] 
            // wa' = [ (PraT row 1) * wa ]
            //       [ (PraT row 2) * wa ]
            // vb' = [ -bx * vbx - by * vby - bz * vbz ] = [ -b * vb ]
            //       [ -cx * vbx - cy * vby - cz * vbz ]   [ -c * vb ]
            // wb' = [ -(PrbT row 1) * wb ]
            //       [ -(PrbT row 2) * wb ]
            // Jv = [ b * va + (PraT row 1) * wa - b * vb - (PrbT row 1) * wb ]
            //      [ c * va + (PraT row 2) * wa - c * vb - (PrbT row 2) * wb ]
            // Jv = [ b * (va + wa x ra - vb - wb x rb) ]
            //      [ c * (va + wa x ra - vb - wb x rb) ]
            //P = JT * lambda

            #endregion

            Vector2 lambda = new Vector2();
            //float va1, va2, wa1, wa2, vb1, vb2, wb1, wb2;
            //Vector3.Dot(ref worldAxis1, ref myParentA.myInternalLinearVelocity, out va1);
            //Vector3.Dot(ref worldAxis2, ref myParentA.myInternalLinearVelocity, out va2);
            //wa1 = prAT.X.X * myParentA.myInternalAngularVelocity.X + prAT.X.Y * myParentA.myInternalAngularVelocity.Y + prAT.X.Z * myParentA.myInternalAngularVelocity.Z;
            //wa2 = prAT.Y.X * myParentA.myInternalAngularVelocity.X + prAT.Y.Y * myParentA.myInternalAngularVelocity.Y + prAT.Y.Z * myParentA.myInternalAngularVelocity.Z;

            //Vector3.Dot(ref worldAxis1, ref myParentB.myInternalLinearVelocity, out vb1);
            //Vector3.Dot(ref worldAxis2, ref myParentB.myInternalLinearVelocity, out vb2);
            //wb1 = prBT.X.X * myParentB.myInternalAngularVelocity.X + prBT.X.Y * myParentB.myInternalAngularVelocity.Y + prBT.X.Z * myParentB.myInternalAngularVelocity.Z;
            //wb2 = prBT.Y.X * myParentB.myInternalAngularVelocity.X + prBT.Y.Y * myParentB.myInternalAngularVelocity.Y + prBT.Y.Z * myParentB.myInternalAngularVelocity.Z;

            //lambda.X = va1 + wa1 - vb1 - wb1 + biasVelocity.X + mySoftness * accumulatedImpulse.X;
            //lambda.Y = va2 + wa2 - vb2 - wb2 + biasVelocity.Y + mySoftness * accumulatedImpulse.Y;
            Vector3 dv;
            Vector3 aVel, bVel;
            Vector3.Cross(ref connectionA.angularVelocity, ref rA, out aVel);
            Vector3.Add(ref aVel, ref connectionA.linearVelocity, out aVel);
            Vector3.Cross(ref connectionB.angularVelocity, ref rB, out bVel);
            Vector3.Add(ref bVel, ref connectionB.linearVelocity, out bVel);
            Vector3.Sub(ref aVel, ref bVel, out dv);
            Vector3.Dot(ref dv, ref worldRestrictedAxis1, out lambda.X);
            Vector3.Dot(ref dv, ref worldRestrictedAxis2, out lambda.Y);


            lambda.X += biasVelocity.X + softness * accumulatedImpulse.X;
            lambda.Y += biasVelocity.Y + softness * accumulatedImpulse.Y;

            //Convert to impulse
            Vector2.Transform(ref lambda, ref negativeEffectiveMassMatrix, out lambda);

            Vector2.Add(ref lambda, ref accumulatedImpulse, out accumulatedImpulse);

            float x = lambda.X;
            float y = lambda.Y;
            //Apply impulse
            Vector3 impulse = new Vector3();
            Vector3 torque= new Vector3();
            impulse.X = worldRestrictedAxis1.X * x + worldRestrictedAxis2.X * y;
            impulse.Y = worldRestrictedAxis1.Y * x + worldRestrictedAxis2.Y * y;
            impulse.Z = worldRestrictedAxis1.Z * x + worldRestrictedAxis2.Z * y;
            if (connectionA.isDynamic)
            {
                torque.X = x * angularA1.X + y * angularA2.X;
                torque.Y = x * angularA1.Y + y * angularA2.Y;
                torque.Z = x * angularA1.Z + y * angularA2.Z;

                connectionA.ApplyLinearImpulse(ref impulse);
                connectionA.ApplyAngularImpulse(ref torque);
            }
            if (connectionB.isDynamic)
            {
                impulse.X = -impulse.X;
                impulse.Y = -impulse.Y;
                impulse.Z = -impulse.Z;

                torque.X = x * angularB1.X + y * angularB2.X;
                torque.Y = x * angularB1.Y + y * angularB2.Y;
                torque.Z = x * angularB1.Z + y * angularB2.Z;

                connectionB.ApplyLinearImpulse(ref impulse);
                connectionB.ApplyAngularImpulse(ref torque);
            }
            return (Math.Abs(lambda.X) + Math.Abs(lambda.Y));
        }

        ///<summary>
        /// Performs the frame's configuration step.
        ///</summary>
        ///<param name="dt">Timestep duration.</param>
        public override void Update(float dt)
        {
            //Transform local axes into world space
            Vector3.Transform(ref localRestrictedAxis1, ref connectionA.orientationMatrix, out worldRestrictedAxis1);
            Vector3.Transform(ref localRestrictedAxis2, ref connectionA.orientationMatrix, out worldRestrictedAxis2);
            Vector3.Transform(ref localAxisAnchor, ref connectionA.orientationMatrix, out worldLineAnchor);
            Vector3.Add(ref worldLineAnchor, ref connectionA.position, out worldLineAnchor);
            Vector3.Transform(ref localLineDirection, ref connectionA.orientationMatrix, out worldLineDirection);

            //Transform local 
            Vector3.Transform(ref localPoint, ref connectionB.orientationMatrix, out rB);
            Vector3.Add(ref rB, ref connectionB.position, out worldPoint);

            //Find the closest point worldAxis line to worldPoint on the line.
            Vector3 offset;
            Vector3.Sub(ref worldPoint, ref worldLineAnchor, out offset);
            float distanceAlongAxis;
            Vector3.Dot(ref offset, ref worldLineDirection, out distanceAlongAxis);

            //Find the point on the line closest to the world point.
            Vector3 worldNearPoint;
            Vector3.Mul(ref worldLineDirection, distanceAlongAxis, out offset);
            Vector3.Add(ref worldLineAnchor, ref offset, out worldNearPoint);
            Vector3.Sub(ref worldNearPoint, ref connectionA.position, out rA);

            //Error
            Vector3 error3D;
            Vector3.Sub(ref worldPoint, ref worldNearPoint, out error3D);

            Vector3.Dot(ref error3D, ref worldRestrictedAxis1, out error.X);
            Vector3.Dot(ref error3D, ref worldRestrictedAxis2, out error.Y);

            float errorReduction;
            springSettings.ComputeErrorReductionAndSoftness(dt, out errorReduction, out softness);
            float bias = -errorReduction;


            biasVelocity.X = bias * error.X;
            biasVelocity.Y = bias * error.Y;

            //Ensure that the corrective velocity doesn't exceed the max.
            float length = biasVelocity.LengthSquared();
            if (length > maxCorrectiveVelocitySquared)
            {
                float multiplier = maxCorrectiveVelocity / (float)Math.Sqrt(length);
                biasVelocity.X *= multiplier;
                biasVelocity.Y *= multiplier;
            }

            //Set up the jacobians
            Vector3.Cross(ref rA, ref worldRestrictedAxis1, out angularA1);
            Vector3.Cross(ref worldRestrictedAxis1, ref rB, out angularB1);
            Vector3.Cross(ref rA, ref worldRestrictedAxis2, out angularA2);
            Vector3.Cross(ref worldRestrictedAxis2, ref rB, out angularB2);

            float m11 = 0, m22 = 0, m1221 = 0;
            float inverseMass;
            Vector3 intermediate;
            //Compute the effective mass matrix.
            if (connectionA.isDynamic)
            {
                inverseMass = connectionA.inverseMass;
                Vector3.Transform(ref angularA1, ref connectionA.inertiaTensorInverse, out intermediate);
                Vector3.Dot(ref intermediate, ref angularA1, out m11);
                m11 += inverseMass;
                Vector3.Dot(ref intermediate, ref angularA2, out m1221);
                Vector3.Transform(ref angularA2, ref connectionA.inertiaTensorInverse, out intermediate);
                Vector3.Dot(ref intermediate, ref angularA2, out m22);
                m22 += inverseMass;
            }

            #region Mass Matrix4 B

            if (connectionB.isDynamic)
            {
                float extra;
                inverseMass = connectionB.inverseMass;
                Vector3.Transform(ref angularB1, ref connectionB.inertiaTensorInverse, out intermediate);
                Vector3.Dot(ref intermediate, ref angularB1, out extra);
                m11 += inverseMass + extra;
                Vector3.Dot(ref intermediate, ref angularB2, out extra);
                m1221 += extra;
                Vector3.Transform(ref angularB2, ref connectionB.inertiaTensorInverse, out intermediate);
                Vector3.Dot(ref intermediate, ref angularB2, out extra);
                m22 += inverseMass + extra;
            }

            #endregion

            negativeEffectiveMassMatrix.X.X = m11 + softness;
            negativeEffectiveMassMatrix.X.Y = m1221;
            negativeEffectiveMassMatrix.Y.X = m1221;
            negativeEffectiveMassMatrix.Y.Y = m22 + softness;
            Matrix2.Invert(ref negativeEffectiveMassMatrix, out negativeEffectiveMassMatrix);
            Matrix2.Neg(ref negativeEffectiveMassMatrix, out negativeEffectiveMassMatrix);

        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {

            //Warm starting
            Vector3 impulse = new Vector3();
            Vector3 torque= new Vector3();
            float x = accumulatedImpulse.X;
            float y = accumulatedImpulse.Y;
            impulse.X = worldRestrictedAxis1.X * x + worldRestrictedAxis2.X * y;
            impulse.Y = worldRestrictedAxis1.Y * x + worldRestrictedAxis2.Y * y;
            impulse.Z = worldRestrictedAxis1.Z * x + worldRestrictedAxis2.Z * y;
            if (connectionA.isDynamic)
            {
                torque.X = x * angularA1.X + y * angularA2.X;
                torque.Y = x * angularA1.Y + y * angularA2.Y;
                torque.Z = x * angularA1.Z + y * angularA2.Z;

                connectionA.ApplyLinearImpulse(ref impulse);
                connectionA.ApplyAngularImpulse(ref torque);
            }
            if (connectionB.isDynamic)
            {
                impulse.X = -impulse.X;
                impulse.Y = -impulse.Y;
                impulse.Z = -impulse.Z;

                torque.X = x * angularB1.X + y * angularB2.X;
                torque.Y = x * angularB1.Y + y * angularB2.Y;
                torque.Z = x * angularB1.Z + y * angularB2.Z;


                connectionB.ApplyLinearImpulse(ref impulse);
                connectionB.ApplyAngularImpulse(ref torque);
            }
        }

        private void UpdateRestrictedAxes()
        {
            localRestrictedAxis1 = Vector3.Up.Cross(localLineDirection);
            if (localRestrictedAxis1.LengthSquared() < .001f)
            {
                localRestrictedAxis1 = Vector3.Right.Cross(localLineDirection);
            }
            localRestrictedAxis2 = localLineDirection.Cross(localRestrictedAxis1);
            localRestrictedAxis1 = localRestrictedAxis1.Normalize();
            localRestrictedAxis2 = localRestrictedAxis2.Normalize();
        }
    }
}