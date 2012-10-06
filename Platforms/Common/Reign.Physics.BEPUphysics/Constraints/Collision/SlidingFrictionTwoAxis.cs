using System;
using BEPUphysics.Entities;
using BEPUphysics.Settings;
using Reign.Core;

namespace BEPUphysics.Constraints.Collision
{
    /// <summary>
    /// Computes the forces to slow down and stop sliding motion between two entities when centralized friction is active.
    /// </summary>
    public class SlidingFrictionTwoAxis : EntitySolverUpdateable
    {
        private ConvexContactManifoldConstraint contactManifoldConstraint;
        ///<summary>
        /// Gets the contact manifold constraint that owns this constraint.
        ///</summary>
        public ConvexContactManifoldConstraint ContactManifoldConstraint
        {
            get
            {
                return contactManifoldConstraint;
            }
        }
        internal Vector2 accumulatedImpulse;
        internal Matrix2x3 angularA, angularB;
        private int contactCount;
        private float friction;
        internal Matrix2x3 linearA;
        private Entity entityA, entityB;
        private bool entityADynamic, entityBDynamic;
        private Vector3 ra, rb;
        private Matrix2 velocityToImpulse;


        /// <summary>
        /// Gets the first direction in which the friction force acts.
        /// This is one of two directions that are perpendicular to each other and the normal of a collision between two entities.
        /// </summary>
        public Vector3 FrictionDirectionX
        {
            get { return new Vector3(linearA.X.X, linearA.X.Y, linearA.X.Z); }
        }

        /// <summary>
        /// Gets the second direction in which the friction force acts.
        /// This is one of two directions that are perpendicular to each other and the normal of a collision between two entities.
        /// </summary>
        public Vector3 FrictionDirectionY
        {
            get { return new Vector3(linearA.Y.X, linearA.Y.Y, linearA.Y.Z); }
        }

        /// <summary>
        /// Gets the total impulse applied by sliding friction in the last time step.
        /// The X component of this vector is the force applied along the frictionDirectionX,
        /// while the Y component is the force applied along the frictionDirectionY.
        /// </summary>
        public Vector2 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        ///<summary>
        /// Gets the tangential relative velocity between the associated entities at the contact point.
        ///</summary>
        public Vector2 RelativeVelocity
        {
            get
            {
                //Compute relative velocity
                //Explicit version:
                //Vector2 dot;
                //Matrix2x3.Transform(ref parentA.myInternalLinearVelocity, ref linearA, out lambda);
                //Matrix2x3.Transform(ref parentB.myInternalLinearVelocity, ref linearA, out dot);
                //lambda.X -= dot.X; lambda.Y -= dot.Y;
                //Matrix2x3.Transform(ref parentA.myInternalAngularVelocity, ref angularA, out dot);
                //lambda.X += dot.X; lambda.Y += dot.Y;
                //Matrix2x3.Transform(ref parentB.myInternalAngularVelocity, ref angularB, out dot);
                //lambda.X += dot.X; lambda.Y += dot.Y;

                //Inline version:
                //lambda.X = linearA.X.X * parentA.myInternalLinearVelocity.X + linearA.X.Y * parentA.myInternalLinearVelocity.Y + linearA.X.Z * parentA.myInternalLinearVelocity.Z -
                //           linearA.X.X * parentB.myInternalLinearVelocity.X - linearA.X.Y * parentB.myInternalLinearVelocity.Y - linearA.X.Z * parentB.myInternalLinearVelocity.Z +
                //           angularA.X.X * parentA.myInternalAngularVelocity.X + angularA.X.Y * parentA.myInternalAngularVelocity.Y + angularA.X.Z * parentA.myInternalAngularVelocity.Z +
                //           angularB.X.X * parentB.myInternalAngularVelocity.X + angularB.X.Y * parentB.myInternalAngularVelocity.Y + angularB.X.Z * parentB.myInternalAngularVelocity.Z;
                //lambda.Y = linearA.Y.X * parentA.myInternalLinearVelocity.X + linearA.Y.Y * parentA.myInternalLinearVelocity.Y + linearA.Y.Z * parentA.myInternalLinearVelocity.Z -
                //           linearA.Y.X * parentB.myInternalLinearVelocity.X - linearA.Y.Y * parentB.myInternalLinearVelocity.Y - linearA.Y.Z * parentB.myInternalLinearVelocity.Z +
                //           angularA.Y.X * parentA.myInternalAngularVelocity.X + angularA.Y.Y * parentA.myInternalAngularVelocity.Y + angularA.Y.Z * parentA.myInternalAngularVelocity.Z +
                //           angularB.Y.X * parentB.myInternalAngularVelocity.X + angularB.Y.Y * parentB.myInternalAngularVelocity.Y + angularB.Y.Z * parentB.myInternalAngularVelocity.Z;

                //Re-using information version:
                //TODO: va + wa x ra - vb - wb x rb, dotted against each axis, is it faster?
                float dvx = 0, dvy = 0, dvz = 0;
                if (entityA != null)
                {
                    dvx = entityA.linearVelocity.X + (entityA.angularVelocity.Y * ra.Z) - (entityA.angularVelocity.Z * ra.Y);
                    dvy = entityA.linearVelocity.Y + (entityA.angularVelocity.Z * ra.X) - (entityA.angularVelocity.X * ra.Z);
                    dvz = entityA.linearVelocity.Z + (entityA.angularVelocity.X * ra.Y) - (entityA.angularVelocity.Y * ra.X);
                }
                if (entityB != null)
                {
                    dvx += -entityB.linearVelocity.X - (entityB.angularVelocity.Y * rb.Z) + (entityB.angularVelocity.Z * rb.Y);
                    dvy += -entityB.linearVelocity.Y - (entityB.angularVelocity.Z * rb.X) + (entityB.angularVelocity.X * rb.Z);
                    dvz += -entityB.linearVelocity.Z - (entityB.angularVelocity.X * rb.Y) + (entityB.angularVelocity.Y * rb.X);
                }

                //float dvx = entityA.linearVelocity.X + (entityA.angularVelocity.Y * ra.Z) - (entityA.angularVelocity.Z * ra.Y)
                //            - entityB.linearVelocity.X - (entityB.angularVelocity.Y * rb.Z) + (entityB.angularVelocity.Z * rb.Y);

                //float dvy = entityA.linearVelocity.Y + (entityA.angularVelocity.Z * ra.X) - (entityA.angularVelocity.X * ra.Z)
                //            - entityB.linearVelocity.Y - (entityB.angularVelocity.Z * rb.X) + (entityB.angularVelocity.X * rb.Z);

                //float dvz = entityA.linearVelocity.Z + (entityA.angularVelocity.X * ra.Y) - (entityA.angularVelocity.Y * ra.X)
                //            - entityB.linearVelocity.Z - (entityB.angularVelocity.X * rb.Y) + (entityB.angularVelocity.Y * rb.X);

                Vector2 lambda = new Vector2();
                lambda.X = dvx * linearA.X.X + dvy * linearA.X.Y + dvz * linearA.X.Z;
                lambda.Y = dvx * linearA.Y.X + dvy * linearA.Y.Y + dvz * linearA.Y.Z;
                return lambda;

                //Using XNA Cross product instead of inline
                //Vector3 wara, wbrb;
                //Vector3.Cross(ref parentA.myInternalAngularVelocity, ref Ra, out wara);
                //Vector3.Cross(ref parentB.myInternalAngularVelocity, ref Rb, out wbrb);

                //float dvx, dvy, dvz;
                //dvx = wara.X + parentA.myInternalLinearVelocity.X - wbrb.X - parentB.myInternalLinearVelocity.X;
                //dvy = wara.Y + parentA.myInternalLinearVelocity.Y - wbrb.Y - parentB.myInternalLinearVelocity.Y;
                //dvz = wara.Z + parentA.myInternalLinearVelocity.Z - wbrb.Z - parentB.myInternalLinearVelocity.Z;

                //lambda.X = dvx * linearA.X.X + dvy * linearA.X.Y + dvz * linearA.X.Z;
                //lambda.Y = dvx * linearA.Y.X + dvy * linearA.Y.Y + dvz * linearA.Y.Z;
            }
        }


        ///<summary>
        /// Constructs a new sliding friction constraint.
        ///</summary>
        public SlidingFrictionTwoAxis()
        {
            isActive = false;
        }

        /// <summary>
        /// Computes one iteration of the constraint to meet the solver updateable's goal.
        /// </summary>
        /// <returns>The rough applied impulse magnitude.</returns>
        public override float SolveIteration()
        {

            Vector2 lambda = RelativeVelocity;

            //Convert to impulse
            //Matrix2x2.Transform(ref lambda, ref velocityToImpulse, out lambda);
            float x = lambda.X;
            lambda.X = x * velocityToImpulse.X.X + lambda.Y * velocityToImpulse.Y.X;
            lambda.Y = x * velocityToImpulse.X.Y + lambda.Y * velocityToImpulse.Y.Y;

            //Accumulate and clamp
            Vector2 previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse.X += lambda.X;
            accumulatedImpulse.Y += lambda.Y;
            float length = accumulatedImpulse.LengthSquared();
            float maximumFrictionForce = 0;
            for (int i = 0; i < contactCount; i++)
            {
                maximumFrictionForce += contactManifoldConstraint.penetrationConstraints.Elements[i].accumulatedImpulse;
            }
            maximumFrictionForce *= friction;
            if (length > maximumFrictionForce * maximumFrictionForce)
            {
                length = maximumFrictionForce / (float)Math.Sqrt(length);
                accumulatedImpulse.X *= length;
                accumulatedImpulse.Y *= length;
            }
            lambda.X = accumulatedImpulse.X - previousAccumulatedImpulse.X;
            lambda.Y = accumulatedImpulse.Y - previousAccumulatedImpulse.Y;
            //Single Axis clamp
            //float maximumFrictionForce = 0;
            //for (int i = 0; i < contactCount; i++)
            //{
            //    maximumFrictionForce += pair.contacts[i].penetrationConstraint.accumulatedImpulse;
            //}
            //maximumFrictionForce *= friction;
            //float previousAccumulatedImpulse = accumulatedImpulse.X;
            //accumulatedImpulse.X = MathUtilities.Clamp(accumulatedImpulse.X + lambda.X, -maximumFrictionForce, maximumFrictionForce);
            //lambda.X = accumulatedImpulse.X - previousAccumulatedImpulse;
            //previousAccumulatedImpulse = accumulatedImpulse.Y;
            //accumulatedImpulse.Y = MathUtilities.Clamp(accumulatedImpulse.Y + lambda.Y, -maximumFrictionForce, maximumFrictionForce);
            //lambda.Y = accumulatedImpulse.Y - previousAccumulatedImpulse;

            //Apply impulse
            Vector3 linear = new Vector3();
            Vector3 angular = new Vector3();

            //Matrix2x3.Transform(ref lambda, ref linearA, out linear);
            linear.X = lambda.X * linearA.X.X + lambda.Y * linearA.Y.X;
            linear.Y = lambda.X * linearA.X.Y + lambda.Y * linearA.Y.Y;
            linear.Z = lambda.X * linearA.X.Z + lambda.Y * linearA.Y.Z;
            if (entityADynamic)
            {
                //Matrix2x3.Transform(ref lambda, ref angularA, out angular);
                angular.X = lambda.X * angularA.X.X + lambda.Y * angularA.Y.X;
                angular.Y = lambda.X * angularA.X.Y + lambda.Y * angularA.Y.Y;
                angular.Z = lambda.X * angularA.X.Z + lambda.Y * angularA.Y.Z;
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                linear.X = -linear.X;
                linear.Y = -linear.Y;
                linear.Z = -linear.Z;
                //Matrix2x3.Transform(ref lambda, ref angularB, out angular);
                angular.X = lambda.X * angularB.X.X + lambda.Y * angularB.Y.X;
                angular.Y = lambda.X * angularB.X.Y + lambda.Y * angularB.Y.Y;
                angular.Z = lambda.X * angularB.X.Z + lambda.Y * angularB.Y.Z;
                entityB.ApplyLinearImpulse(ref linear);
                entityB.ApplyAngularImpulse(ref angular);
            }


            return Math.Abs(lambda.X) + Math.Abs(lambda.Y);
        }

        internal Vector3 manifoldCenter, relativeVelocity;

        ///<summary>
        /// Performs the frame's configuration step.
        ///</summary>
        ///<param name="dt">Timestep duration.</param>
        public override void Update(float dt)
        {

            entityADynamic = entityA != null && entityA.isDynamic;
            entityBDynamic = entityB != null && entityB.isDynamic;

            contactCount = contactManifoldConstraint.penetrationConstraints.count;
            switch (contactCount)
            {
                case 1:
                    manifoldCenter = contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position;
                    break;
                case 2:
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position,
                                ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position,
                                out manifoldCenter);
                    manifoldCenter.X *= .5f;
                    manifoldCenter.Y *= .5f;
                    manifoldCenter.Z *= .5f;
                    break;
                case 3:
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position,
                                ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position,
                                out manifoldCenter);
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[2].contact.Position,
                                ref manifoldCenter,
                                out manifoldCenter);
                    manifoldCenter.X *= .333333333f;
                    manifoldCenter.Y *= .333333333f;
                    manifoldCenter.Z *= .333333333f;
                    break;
                case 4:
                    //This isn't actually the center of the manifold.  Is it good enough?  Sure seems like it.
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Position,
                                ref contactManifoldConstraint.penetrationConstraints.Elements[1].contact.Position,
                                out manifoldCenter);
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[2].contact.Position,
                                ref manifoldCenter,
                                out manifoldCenter);
                    Vector3.Add(ref contactManifoldConstraint.penetrationConstraints.Elements[3].contact.Position,
                                ref manifoldCenter,
                                out manifoldCenter);
                    manifoldCenter.X *= .25f;
                    manifoldCenter.Y *= .25f;
                    manifoldCenter.Z *= .25f;
                    break;
                default:
                    manifoldCenter = Toolbox.NoVector;
                    break;
            }

            //Compute the three dimensional relative velocity at the point.


            Vector3 velocityA, velocityB;
            if (entityA != null)
            {
                Vector3.Sub(ref manifoldCenter, ref entityA.position, out ra);
                Vector3.Cross(ref entityA.angularVelocity, ref ra, out velocityA);
                Vector3.Add(ref velocityA, ref entityA.linearVelocity, out velocityA);
            }
            else
                velocityA = new Vector3();
            if (entityB != null)
            {
                Vector3.Sub(ref manifoldCenter, ref entityB.position, out rb);
                Vector3.Cross(ref entityB.angularVelocity, ref rb, out velocityB);
                Vector3.Add(ref velocityB, ref entityB.linearVelocity, out velocityB);
            }
            else
                velocityB = new Vector3();
            Vector3.Sub(ref velocityA, ref velocityB, out relativeVelocity);

            //Get rid of the normal velocity.
            Vector3 normal = contactManifoldConstraint.penetrationConstraints.Elements[0].contact.Normal;
            float normalVelocityScalar = normal.X * relativeVelocity.X + normal.Y * relativeVelocity.Y + normal.Z * relativeVelocity.Z;
            relativeVelocity.X -= normalVelocityScalar * normal.X;
            relativeVelocity.Y -= normalVelocityScalar * normal.Y;
            relativeVelocity.Z -= normalVelocityScalar * normal.Z;

            //Create the jacobian entry and decide the friction coefficient.
            float length = relativeVelocity.LengthSquared();
            if (length > Toolbox.Epsilon)
            {
                length = (float)Math.Sqrt(length);
                float inverseLength = 1 / length;
                linearA.X.X = relativeVelocity.X * inverseLength;
                linearA.X.Y = relativeVelocity.Y * inverseLength;
                linearA.X.Z = relativeVelocity.Z * inverseLength;


                friction = length > CollisionResponseSettings.StaticFrictionVelocityThreshold ?
                           contactManifoldConstraint.materialInteraction.KineticFriction :
                           contactManifoldConstraint.materialInteraction.StaticFriction;
            }
            else
            {
                friction = contactManifoldConstraint.materialInteraction.StaticFriction;

                //If there was no velocity, try using the previous frame's jacobian... if it exists.
                //Reusing an old one is okay since jacobians are cleared when a contact is initialized.
                if (!(linearA.X.X != 0 || linearA.X.Y != 0 || linearA.X.Z != 0))
                {
                    //Otherwise, just redo it all.
                    //Create arbitrary axes.
                    Vector3 axis1;
                    Vector3.Cross(ref normal, ref Toolbox.RightVector, out axis1);
                    length = axis1.LengthSquared();
                    if (length > Toolbox.Epsilon)
                    {
                        length = (float)Math.Sqrt(length);
                        float inverseLength = 1 / length;
                        linearA.X.X = axis1.X * inverseLength;
                        linearA.X.Y = axis1.Y * inverseLength;
                        linearA.X.Z = axis1.Z * inverseLength;
                    }
                    else
                    {
                        Vector3.Cross(ref normal, ref Toolbox.UpVector, out axis1);
                        axis1 = axis1.Normalize();
                        linearA.X.X = axis1.X;
                        linearA.X.Y = axis1.Y;
                        linearA.X.Z = axis1.Z;
                    }
                }
            }

            //Second axis is first axis x normal
            linearA.Y.X = (linearA.X.Y * normal.Z) - (linearA.X.Z * normal.Y);
            linearA.Y.Y = (linearA.X.Z * normal.X) - (linearA.X.X * normal.Z);
            linearA.Y.Z = (linearA.X.X * normal.Y) - (linearA.X.Y * normal.X);


            //Compute angular jacobians
            if (entityA != null)
            {
                //angularA 1 =  ra x linear axis 1
                angularA.X.X = (ra.Y * linearA.X.Z) - (ra.Z * linearA.X.Y);
                angularA.X.Y = (ra.Z * linearA.X.X) - (ra.X * linearA.X.Z);
                angularA.X.Z = (ra.X * linearA.X.Y) - (ra.Y * linearA.X.X);

                //angularA 2 =  ra x linear axis 2
                angularA.Y.X = (ra.Y * linearA.Y.Z) - (ra.Z * linearA.Y.Y);
                angularA.Y.Y = (ra.Z * linearA.Y.X) - (ra.X * linearA.Y.Z);
                angularA.Y.Z = (ra.X * linearA.Y.Y) - (ra.Y * linearA.Y.X);
            }

            //angularB 1 =  linear axis 1 x rb
            if (entityB != null)
            {
                angularB.X.X = (linearA.X.Y * rb.Z) - (linearA.X.Z * rb.Y);
                angularB.X.Y = (linearA.X.Z * rb.X) - (linearA.X.X * rb.Z);
                angularB.X.Z = (linearA.X.X * rb.Y) - (linearA.X.Y * rb.X);

                //angularB 2 =  linear axis 2 x rb
                angularB.Y.X = (linearA.Y.Y * rb.Z) - (linearA.Y.Z * rb.Y);
                angularB.Y.Y = (linearA.Y.Z * rb.X) - (linearA.Y.X * rb.Z);
                angularB.Y.Z = (linearA.Y.X * rb.Y) - (linearA.Y.Y * rb.X);
            }
            //Compute inverse effective mass matrix
            Matrix2 entryA, entryB;

            //these are the transformed coordinates
            Matrix2x3 transform;
            Matrix3x2 transpose;
            if (entityADynamic)
            {
                Matrix2x3.Multiply(ref angularA, ref entityA.inertiaTensorInverse, out transform);
                Matrix2x3.Transpose(ref angularA, out transpose);
                Matrix2x3.Multiply(ref transform, ref transpose, out entryA);
                entryA.X.X += entityA.inverseMass;
                entryA.Y.Y += entityA.inverseMass;
            }
            else
            {
                entryA = new Matrix2();
            }

            if (entityBDynamic)
            {
                Matrix2x3.Multiply(ref angularB, ref entityB.inertiaTensorInverse, out transform);
                Matrix2x3.Transpose(ref angularB, out transpose);
                Matrix2x3.Multiply(ref transform, ref transpose, out entryB);
                entryB.X.X += entityB.inverseMass;
                entryB.Y.Y += entityB.inverseMass;
            }
            else
            {
                entryB = new Matrix2();
            }

            velocityToImpulse.X.X = -entryA.X.X - entryB.X.X;
            velocityToImpulse.X.Y = -entryA.X.Y - entryB.X.Y;
            velocityToImpulse.Y.X = -entryA.Y.X - entryB.Y.X;
            velocityToImpulse.Y.Y = -entryA.Y.Y - entryB.Y.Y;
            Matrix2.Invert(ref velocityToImpulse, out velocityToImpulse);


        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {

            //Warm starting
            Vector3 linear = new Vector3();
            Vector3 angular = new Vector3();

            //Matrix2x3.Transform(ref lambda, ref linearA, out linear);
            linear.X = accumulatedImpulse.X * linearA.X.X + accumulatedImpulse.Y * linearA.Y.X;
            linear.Y = accumulatedImpulse.X * linearA.X.Y + accumulatedImpulse.Y * linearA.Y.Y;
            linear.Z = accumulatedImpulse.X * linearA.X.Z + accumulatedImpulse.Y * linearA.Y.Z;
            if (entityADynamic)
            {
                //Matrix2x3.Transform(ref lambda, ref angularA, out angular);
                angular.X = accumulatedImpulse.X * angularA.X.X + accumulatedImpulse.Y * angularA.Y.X;
                angular.Y = accumulatedImpulse.X * angularA.X.Y + accumulatedImpulse.Y * angularA.Y.Y;
                angular.Z = accumulatedImpulse.X * angularA.X.Z + accumulatedImpulse.Y * angularA.Y.Z;
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                linear.X = -linear.X;
                linear.Y = -linear.Y;
                linear.Z = -linear.Z;
                //Matrix2x3.Transform(ref lambda, ref angularB, out angular);
                angular.X = accumulatedImpulse.X * angularB.X.X + accumulatedImpulse.Y * angularB.Y.X;
                angular.Y = accumulatedImpulse.X * angularB.X.Y + accumulatedImpulse.Y * angularB.Y.Y;
                angular.Z = accumulatedImpulse.X * angularB.X.Z + accumulatedImpulse.Y * angularB.Y.Z;
                entityB.ApplyLinearImpulse(ref linear);
                entityB.ApplyAngularImpulse(ref angular);
            }
        }

        internal void Setup(ConvexContactManifoldConstraint contactManifoldConstraint)
        {
            this.contactManifoldConstraint = contactManifoldConstraint;
            isActive = true;

            linearA = new Matrix2x3();

            entityA = contactManifoldConstraint.EntityA;
            entityB = contactManifoldConstraint.EntityB;
        }

        internal void CleanUp()
        {
            accumulatedImpulse = new Vector2();
            contactManifoldConstraint = null;
            entityA = null;
            entityB = null;
            isActive = false;
        }

        protected internal override void CollectInvolvedEntities(DataStructures.RawList<Entity> outputInvolvedEntities)
        {
            //This should never really have to be called.
            if (entityA != null)
                outputInvolvedEntities.Add(entityA);
            if (entityB != null)
                outputInvolvedEntities.Add(entityB);
        }
    }
}