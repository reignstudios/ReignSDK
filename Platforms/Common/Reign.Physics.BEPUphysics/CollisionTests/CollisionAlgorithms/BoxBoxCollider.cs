using System;
using System.Runtime.InteropServices;
using BEPUphysics.DataStructures;
using BEPUphysics.CollisionShapes.ConvexShapes;
using System.Diagnostics;
using BEPUphysics.Settings;
using Reign.Core;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms
{
    /// <summary>
    /// Stores basic data used by some collision systems.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BoxContactData : IEquatable<BoxContactData>
    {
        /// <summary>
        /// Position of the candidate contact.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Depth of the candidate contact.
        /// </summary>
        public float Depth;

        /// <summary>
        /// Id of the candidate contact.
        /// </summary>
        public int Id;

        #region IEquatable<BoxContactData> Members

        /// <summary>
        /// Returns true if the other data has the same id.
        /// </summary>
        /// <param name="other">Data to compare.</param>
        /// <returns>True if the other data has the same id, false otherwise.</returns>
        public bool Equals(BoxContactData other)
        {
            return Id == other.Id;
        }

        #endregion
    }


	#if XBOX360
    [StructLayout(LayoutKind.Sequential)]
	#else
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	#endif
    /// <summary>
    /// Basic storage structure for contact data.
    /// Designed for performance critical code and pointer access.
    /// </summary>
    public struct BoxContactDataCache
    {
        public BoxContactData D1;
        public BoxContactData D2;
        public BoxContactData D3;
        public BoxContactData D4;

        public BoxContactData D5;
        public BoxContactData D6;
        public BoxContactData D7;
        public BoxContactData D8;

        //internal BoxContactData d9;
        //internal BoxContactData d10;
        //internal BoxContactData d11;
        //internal BoxContactData d12;

        //internal BoxContactData d13;
        //internal BoxContactData d14;
        //internal BoxContactData d15;
        //internal BoxContactData d16;

        /// <summary>
        /// Number of elements in the cache.
        /// </summary>
        public byte Count;

#if ALLOWUNSAFE
        /// <summary>
        /// Removes an item at the given index.
        /// </summary>
        /// <param name="index">Index to remove.</param>
        public unsafe void RemoveAt(int index)
        {
            BoxContactDataCache copy = this;
            BoxContactData* pointer = &copy.D1;
            pointer[index] = pointer[Count - 1];
            this = copy;
            Count--;
        }
#endif
    }


    /// <summary>
    /// Contains helper methods for testing collisions between boxes.
    /// </summary>
    public static class BoxBoxCollider
    {
        /// <summary>
        /// Determines if the two boxes are colliding.
        /// </summary>
        /// <param name="a">First box to collide.</param>
        /// <param name="b">Second box to collide.</param>
        /// <param name="transformA">Transform to apply to shape a.</param>
        /// <param name="transformB">Transform to apply to shape b.</param>
        /// <returns>Whether or not the boxes collide.</returns>
        public static bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform3 transformA, ref RigidTransform3 transformB)
        {
            float aX = a.HalfWidth;
            float aY = a.HalfHeight;
            float aZ = a.HalfLength;

            float bX = b.HalfWidth;
            float bY = b.HalfHeight;
            float bZ = b.HalfLength;

            //Relative rotation from A to B.
            Matrix3 bR;

            Matrix3 aO;
            Matrix3.FromQuaternion(ref transformA.Orientation, out aO);
            Matrix3 bO;
            Matrix3.FromQuaternion(ref transformB.Orientation, out bO);

            //Relative translation rotated into A's configuration space.
            Vector3 t;
            Vector3.Sub(ref transformB.Position, ref transformA.Position, out t);

            bR.X.X = aO.X.X * bO.X.X + aO.X.Y * bO.X.Y + aO.X.Z * bO.X.Z;
            bR.X.Y = aO.X.X * bO.Y.X + aO.X.Y * bO.Y.Y + aO.X.Z * bO.Y.Z;
            bR.X.Z = aO.X.X * bO.Z.X + aO.X.Y * bO.Z.Y + aO.X.Z * bO.Z.Z;
            Matrix3 absBR;
            //Epsilons are added to deal with near-parallel edges.
            absBR.X.X = Math.Abs(bR.X.X) + Toolbox.Epsilon;
            absBR.X.Y = Math.Abs(bR.X.Y) + Toolbox.Epsilon;
            absBR.X.Z = Math.Abs(bR.X.Z) + Toolbox.Epsilon;
            float tX = t.X;
            t.X = t.X * aO.X.X + t.Y * aO.X.Y + t.Z * aO.X.Z;

            //Test the axes defines by entity A's rotation matrix.
            //A.X
            float rb = bX * absBR.X.X + bY * absBR.X.Y + bZ * absBR.X.Z;
            if (Math.Abs(t.X) > aX + rb)
                return false;
            bR.Y.X = aO.Y.X * bO.X.X + aO.Y.Y * bO.X.Y + aO.Y.Z * bO.X.Z;
            bR.Y.Y = aO.Y.X * bO.Y.X + aO.Y.Y * bO.Y.Y + aO.Y.Z * bO.Y.Z;
            bR.Y.Z = aO.Y.X * bO.Z.X + aO.Y.Y * bO.Z.Y + aO.Y.Z * bO.Z.Z;
            absBR.Y.X = Math.Abs(bR.Y.X) + Toolbox.Epsilon;
            absBR.Y.Y = Math.Abs(bR.Y.Y) + Toolbox.Epsilon;
            absBR.Y.Z = Math.Abs(bR.Y.Z) + Toolbox.Epsilon;
            float tY = t.Y;
            t.Y = tX * aO.Y.X + t.Y * aO.Y.Y + t.Z * aO.Y.Z;

            //A.Y
            rb = bX * absBR.Y.X + bY * absBR.Y.Y + bZ * absBR.Y.Z;
            if (Math.Abs(t.Y) > aY + rb)
                return false;

            bR.Z.X = aO.Z.X * bO.X.X + aO.Z.Y * bO.X.Y + aO.Z.Z * bO.X.Z;
            bR.Z.Y = aO.Z.X * bO.Y.X + aO.Z.Y * bO.Y.Y + aO.Z.Z * bO.Y.Z;
            bR.Z.Z = aO.Z.X * bO.Z.X + aO.Z.Y * bO.Z.Y + aO.Z.Z * bO.Z.Z;
            absBR.Z.X = Math.Abs(bR.Z.X) + Toolbox.Epsilon;
            absBR.Z.Y = Math.Abs(bR.Z.Y) + Toolbox.Epsilon;
            absBR.Z.Z = Math.Abs(bR.Z.Z) + Toolbox.Epsilon;
            t.Z = tX * aO.Z.X + tY * aO.Z.Y + t.Z * aO.Z.Z;

            //A.Z
            rb = bX * absBR.Z.X + bY * absBR.Z.Y + bZ * absBR.Z.Z;
            if (Math.Abs(t.Z) > aZ + rb)
                return false;

            //Test the axes defines by entity B's rotation matrix.
            //B.X
            float ra = aX * absBR.X.X + aY * absBR.Y.X + aZ * absBR.Z.X;
            if (Math.Abs(t.X * bR.X.X + t.Y * bR.Y.X + t.Z * bR.Z.X) > ra + bX)
                return false;

            //B.Y
            ra = aX * absBR.X.Y + aY * absBR.Y.Y + aZ * absBR.Z.Y;
            if (Math.Abs(t.X * bR.X.Y + t.Y * bR.Y.Y + t.Z * bR.Z.Y) > ra + bY)
                return false;

            //B.Z
            ra = aX * absBR.X.Z + aY * absBR.Y.Z + aZ * absBR.Z.Z;
            if (Math.Abs(t.X * bR.X.Z + t.Y * bR.Y.Z + t.Z * bR.Z.Z) > ra + bZ)
                return false;

            //Now for the edge-edge cases.
            //A.X x B.X
            ra = aY * absBR.Z.X + aZ * absBR.Y.X;
            rb = bY * absBR.X.Z + bZ * absBR.X.Y;
            if (Math.Abs(t.Z * bR.Y.X - t.Y * bR.Z.X) > ra + rb)
                return false;

            //A.X x B.Y
            ra = aY * absBR.Z.Y + aZ * absBR.Y.Y;
            rb = bX * absBR.X.Z + bZ * absBR.X.X;
            if (Math.Abs(t.Z * bR.Y.Y - t.Y * bR.Z.Y) > ra + rb)
                return false;

            //A.X x B.Z
            ra = aY * absBR.Z.Z + aZ * absBR.Y.Z;
            rb = bX * absBR.X.Y + bY * absBR.X.X;
            if (Math.Abs(t.Z * bR.Y.Z - t.Y * bR.Z.Z) > ra + rb)
                return false;


            //A.Y x B.X
            ra = aX * absBR.Z.X + aZ * absBR.X.X;
            rb = bY * absBR.Y.Z + bZ * absBR.Y.Y;
            if (Math.Abs(t.X * bR.Z.X - t.Z * bR.X.X) > ra + rb)
                return false;

            //A.Y x B.Y
            ra = aX * absBR.Z.Y + aZ * absBR.X.Y;
            rb = bX * absBR.Y.Z + bZ * absBR.Y.X;
            if (Math.Abs(t.X * bR.Z.Y - t.Z * bR.X.Y) > ra + rb)
                return false;

            //A.Y x B.Z
            ra = aX * absBR.Z.Z + aZ * absBR.X.Z;
            rb = bX * absBR.Y.Y + bY * absBR.Y.X;
            if (Math.Abs(t.X * bR.Z.Z - t.Z * bR.X.Z) > ra + rb)
                return false;

            //A.Z x B.X
            ra = aX * absBR.Y.X + aY * absBR.X.X;
            rb = bY * absBR.Z.Z + bZ * absBR.Z.Y;
            if (Math.Abs(t.Y * bR.X.X - t.X * bR.Y.X) > ra + rb)
                return false;

            //A.Z x B.Y
            ra = aX * absBR.Y.Y + aY * absBR.X.Y;
            rb = bX * absBR.Z.Z + bZ * absBR.Z.X;
            if (Math.Abs(t.Y * bR.X.Y - t.X * bR.Y.Y) > ra + rb)
                return false;

            //A.Z x B.Z
            ra = aX * absBR.Y.Z + aY * absBR.X.Z;
            rb = bX * absBR.Z.Y + bY * absBR.Z.X;
            if (Math.Abs(t.Y * bR.X.Z - t.X * bR.Y.Z) > ra + rb)
                return false;

            return true;
        }

        /// <summary>
        /// Determines if the two boxes are colliding.
        /// </summary>
        /// <param name="a">First box to collide.</param>
        /// <param name="b">Second box to collide.</param>
        /// <param name="separationDistance">Distance of separation.</param>
        /// <param name="separatingAxis">Axis of separation.</param>
        /// <param name="transformA">Transform to apply to shape A.</param>
        /// <param name="transformB">Transform to apply to shape B.</param>
        /// <returns>Whether or not the boxes collide.</returns>
        public static bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform3 transformA, ref RigidTransform3 transformB, out float separationDistance, out Vector3 separatingAxis)
        {
            float aX = a.HalfWidth;
            float aY = a.HalfHeight;
            float aZ = a.HalfLength;

            float bX = b.HalfWidth;
            float bY = b.HalfHeight;
            float bZ = b.HalfLength;

            //Relative rotation from A to B.
            Matrix3 bR;

            Matrix3 aO;
            Matrix3.FromQuaternion(ref transformA.Orientation, out aO);
            Matrix3 bO;
            Matrix3.FromQuaternion(ref transformB.Orientation, out bO);

            //Relative translation rotated into A's configuration space.
            Vector3 t;
            Vector3.Sub(ref transformB.Position, ref transformA.Position, out t);

            #region A Face Normals

            bR.X.X = aO.X.X * bO.X.X + aO.X.Y * bO.X.Y + aO.X.Z * bO.X.Z;
            bR.X.Y = aO.X.X * bO.Y.X + aO.X.Y * bO.Y.Y + aO.X.Z * bO.Y.Z;
            bR.X.Z = aO.X.X * bO.Z.X + aO.X.Y * bO.Z.Y + aO.X.Z * bO.Z.Z;
            Matrix3 absBR;
            //Epsilons are added to deal with near-parallel edges.
            absBR.X.X = Math.Abs(bR.X.X) + Toolbox.Epsilon;
            absBR.X.Y = Math.Abs(bR.X.Y) + Toolbox.Epsilon;
            absBR.X.Z = Math.Abs(bR.X.Z) + Toolbox.Epsilon;
            float tX = t.X;
            t.X = t.X * aO.X.X + t.Y * aO.X.Y + t.Z * aO.X.Z;

            //Test the axes defines by entity A's rotation matrix.
            //A.X
            float rarb = aX + bX * absBR.X.X + bY * absBR.X.Y + bZ * absBR.X.Z;
            if (t.X > rarb)
            {
                separationDistance = t.X - rarb;
                separatingAxis = new Vector3(aO.X.X, aO.X.Y, aO.X.Z);
                return false;
            }
            if (t.X < -rarb)
            {
                separationDistance = -t.X - rarb;
                separatingAxis = new Vector3(-aO.X.X, -aO.X.Y, -aO.X.Z);
                return false;
            }


            bR.Y.X = aO.Y.X * bO.X.X + aO.Y.Y * bO.X.Y + aO.Y.Z * bO.X.Z;
            bR.Y.Y = aO.Y.X * bO.Y.X + aO.Y.Y * bO.Y.Y + aO.Y.Z * bO.Y.Z;
            bR.Y.Z = aO.Y.X * bO.Z.X + aO.Y.Y * bO.Z.Y + aO.Y.Z * bO.Z.Z;
            absBR.Y.X = Math.Abs(bR.Y.X) + Toolbox.Epsilon;
            absBR.Y.Y = Math.Abs(bR.Y.Y) + Toolbox.Epsilon;
            absBR.Y.Z = Math.Abs(bR.Y.Z) + Toolbox.Epsilon;
            float tY = t.Y;
            t.Y = tX * aO.Y.X + t.Y * aO.Y.Y + t.Z * aO.Y.Z;

            //A.Y
            rarb = aY + bX * absBR.Y.X + bY * absBR.Y.Y + bZ * absBR.Y.Z;
            if (t.Y > rarb)
            {
                separationDistance = t.Y - rarb;
                separatingAxis = new Vector3(aO.Y.X, aO.Y.Y, aO.Y.Z);
                return false;
            }
            if (t.Y < -rarb)
            {
                separationDistance = -t.Y - rarb;
                separatingAxis = new Vector3(-aO.Y.X, -aO.Y.Y, -aO.Y.Z);
                return false;
            }

            bR.Z.X = aO.Z.X * bO.X.X + aO.Z.Y * bO.X.Y + aO.Z.Z * bO.X.Z;
            bR.Z.Y = aO.Z.X * bO.Y.X + aO.Z.Y * bO.Y.Y + aO.Z.Z * bO.Y.Z;
            bR.Z.Z = aO.Z.X * bO.Z.X + aO.Z.Y * bO.Z.Y + aO.Z.Z * bO.Z.Z;
            absBR.Z.X = Math.Abs(bR.Z.X) + Toolbox.Epsilon;
            absBR.Z.Y = Math.Abs(bR.Z.Y) + Toolbox.Epsilon;
            absBR.Z.Z = Math.Abs(bR.Z.Z) + Toolbox.Epsilon;
            t.Z = tX * aO.Z.X + tY * aO.Z.Y + t.Z * aO.Z.Z;

            //A.Z
            rarb = aZ + bX * absBR.Z.X + bY * absBR.Z.Y + bZ * absBR.Z.Z;
            if (t.Z > rarb)
            {
                separationDistance = t.Z - rarb;
                separatingAxis = new Vector3(aO.Z.X, aO.Z.Y, aO.Z.Z);
                return false;
            }
            if (t.Z < -rarb)
            {
                separationDistance = -t.Z - rarb;
                separatingAxis = new Vector3(-aO.Z.X, -aO.Z.Y, -aO.Z.Z);
                return false;
            }

            #endregion

            #region B Face Normals

            //Test the axes defines by entity B's rotation matrix.
            //B.X
            rarb = bX + aX * absBR.X.X + aY * absBR.Y.X + aZ * absBR.Z.X;
            float tl = t.X * bR.X.X + t.Y * bR.Y.X + t.Z * bR.Z.X;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(bO.X.X, bO.X.Y, bO.X.Z);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(-bO.X.X, -bO.X.Y, -bO.X.Z);
                return false;
            }

            //B.Y
            rarb = bY + aX * absBR.X.Y + aY * absBR.Y.Y + aZ * absBR.Z.Y;
            tl = t.X * bR.X.Y + t.Y * bR.Y.Y + t.Z * bR.Z.Y;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(bO.Y.X, bO.Y.Y, bO.Y.Z);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(-bO.Y.X, -bO.Y.Y, -bO.Y.Z);
                return false;
            }


            //B.Z
            rarb = bZ + aX * absBR.X.Z + aY * absBR.Y.Z + aZ * absBR.Z.Z;
            tl = t.X * bR.X.Z + t.Y * bR.Y.Z + t.Z * bR.Z.Z;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(bO.Z.X, bO.Z.Y, bO.Z.Z);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(-bO.Z.X, -bO.Z.Y, -bO.Z.Z);
                return false;
            }

            #endregion

            #region A.X x B.()

            //Now for the edge-edge cases.
            //A.X x B.X
            rarb = aY * absBR.Z.X + aZ * absBR.Y.X +
                   bY * absBR.X.Z + bZ * absBR.X.Y;
            tl = t.Z * bR.Y.X - t.Y * bR.Z.X;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(aO.X.Y * bO.X.Z - aO.X.Z * bO.X.Y,
                                             aO.X.Z * bO.X.X - aO.X.X * bO.X.Z,
                                             aO.X.X * bO.X.Y - aO.X.Y * bO.X.X);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(bO.X.Y * aO.X.Z - bO.X.Z * aO.X.Y,
                                             bO.X.Z * aO.X.X - bO.X.X * aO.X.Z,
                                             bO.X.X * aO.X.Y - bO.X.Y * aO.X.X);
                return false;
            }

            //A.X x B.Y
            rarb = aY * absBR.Z.Y + aZ * absBR.Y.Y +
                   bX * absBR.X.Z + bZ * absBR.X.X;
            tl = t.Z * bR.Y.Y - t.Y * bR.Z.Y;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(aO.X.Y * bO.Y.Z - aO.X.Z * bO.Y.Y,
                                             aO.X.Z * bO.Y.X - aO.X.X * bO.Y.Z,
                                             aO.X.X * bO.Y.Y - aO.X.Y * bO.Y.X);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(bO.Y.Y * aO.X.Z - bO.Y.Z * aO.X.Y,
                                             bO.Y.Z * aO.X.X - bO.Y.X * aO.X.Z,
                                             bO.Y.X * aO.X.Y - bO.Y.Y * aO.X.X);
                return false;
            }

            //A.X x B.Z
            rarb = aY * absBR.Z.Z + aZ * absBR.Y.Z +
                   bX * absBR.X.Y + bY * absBR.X.X;
            tl = t.Z * bR.Y.Z - t.Y * bR.Z.Z;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(aO.X.Y * bO.Z.Z - aO.X.Z * bO.Z.Y,
                                             aO.X.Z * bO.Z.X - aO.X.X * bO.Z.Z,
                                             aO.X.X * bO.Z.Y - aO.X.Y * bO.Z.X);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(bO.Z.Y * aO.X.Z - bO.Z.Z * aO.X.Y,
                                             bO.Z.Z * aO.X.X - bO.Z.X * aO.X.Z,
                                             bO.Z.X * aO.X.Y - bO.Z.Y * aO.X.X);
                return false;
            }

            #endregion

            #region A.Y x B.()

            //A.Y x B.X
            rarb = aX * absBR.Z.X + aZ * absBR.X.X +
                   bY * absBR.Y.Z + bZ * absBR.Y.Y;
            tl = t.X * bR.Z.X - t.Z * bR.X.X;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(aO.Y.Y * bO.X.Z - aO.Y.Z * bO.X.Y,
                                             aO.Y.Z * bO.X.X - aO.Y.X * bO.X.Z,
                                             aO.Y.X * bO.X.Y - aO.Y.Y * bO.X.X);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(bO.X.Y * aO.Y.Z - bO.X.Z * aO.Y.Y,
                                             bO.X.Z * aO.Y.X - bO.X.X * aO.Y.Z,
                                             bO.X.X * aO.Y.Y - bO.X.Y * aO.Y.X);
                return false;
            }

            //A.Y x B.Y
            rarb = aX * absBR.Z.Y + aZ * absBR.X.Y +
                   bX * absBR.Y.Z + bZ * absBR.Y.X;
            tl = t.X * bR.Z.Y - t.Z * bR.X.Y;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(aO.Y.Y * bO.Y.Z - aO.Y.Z * bO.Y.Y,
                                             aO.Y.Z * bO.Y.X - aO.Y.X * bO.Y.Z,
                                             aO.Y.X * bO.Y.Y - aO.Y.Y * bO.Y.X);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(bO.Y.Y * aO.Y.Z - bO.Y.Z * aO.Y.Y,
                                             bO.Y.Z * aO.Y.X - bO.Y.X * aO.Y.Z,
                                             bO.Y.X * aO.Y.Y - bO.Y.Y * aO.Y.X);
                return false;
            }

            //A.Y x B.Z
            rarb = aX * absBR.Z.Z + aZ * absBR.X.Z +
                   bX * absBR.Y.Y + bY * absBR.Y.X;
            tl = t.X * bR.Z.Z - t.Z * bR.X.Z;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(aO.Y.Y * bO.Z.Z - aO.Y.Z * bO.Z.Y,
                                             aO.Y.Z * bO.Z.X - aO.Y.X * bO.Z.Z,
                                             aO.Y.X * bO.Z.Y - aO.Y.Y * bO.Z.X);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(bO.Z.Y * aO.Y.Z - bO.Z.Z * aO.Y.Y,
                                             bO.Z.Z * aO.Y.X - bO.Z.X * aO.Y.Z,
                                             bO.Z.X * aO.Y.Y - bO.Z.Y * aO.Y.X);
                return false;
            }

            #endregion

            #region A.Z x B.()

            //A.Z x B.X
            rarb = aX * absBR.Y.X + aY * absBR.X.X +
                   bY * absBR.Z.Z + bZ * absBR.Z.Y;
            tl = t.Y * bR.X.X - t.X * bR.Y.X;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(aO.Z.Y * bO.X.Z - aO.Z.Z * bO.X.Y,
                                             aO.Z.Z * bO.X.X - aO.Z.X * bO.X.Z,
                                             aO.Z.X * bO.X.Y - aO.Z.Y * bO.X.X);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(bO.X.Y * aO.Z.Z - bO.X.Z * aO.Z.Y,
                                             bO.X.Z * aO.Z.X - bO.X.X * aO.Z.Z,
                                             bO.X.X * aO.Z.Y - bO.X.Y * aO.Z.X);
                return false;
            }

            //A.Z x B.Y
            rarb = aX * absBR.Y.Y + aY * absBR.X.Y +
                   bX * absBR.Z.Z + bZ * absBR.Z.X;
            tl = t.Y * bR.X.Y - t.X * bR.Y.Y;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(aO.Z.Y * bO.Y.Z - aO.Z.Z * bO.Y.Y,
                                             aO.Z.Z * bO.Y.X - aO.Z.X * bO.Y.Z,
                                             aO.Z.X * bO.Y.Y - aO.Z.Y * bO.Y.X);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(bO.Y.Y * aO.Z.Z - bO.Y.Z * aO.Z.Y,
                                             bO.Y.Z * aO.Z.X - bO.Y.X * aO.Z.Z,
                                             bO.Y.X * aO.Z.Y - bO.Y.Y * aO.Z.X);
                return false;
            }

            //A.Z x B.Z
            rarb = aX * absBR.Y.Z + aY * absBR.X.Z +
                   bX * absBR.Z.Y + bY * absBR.Z.X;
            tl = t.Y * bR.X.Z - t.X * bR.Y.Z;
            if (tl > rarb)
            {
                separationDistance = tl - rarb;
                separatingAxis = new Vector3(aO.Z.Y * bO.Z.Z - aO.Z.Z * bO.Z.Y,
                                             aO.Z.Z * bO.Z.X - aO.Z.X * bO.Z.Z,
                                             aO.Z.X * bO.Z.Y - aO.Z.Y * bO.Z.X);
                return false;
            }
            if (tl < -rarb)
            {
                separationDistance = -tl - rarb;
                separatingAxis = new Vector3(bO.Z.Y * aO.Z.Z - bO.Z.Z * aO.Z.Y,
                                             bO.Z.Z * aO.Z.X - bO.Z.X * aO.Z.Z,
                                             bO.Z.X * aO.Z.Y - bO.Z.Y * aO.Z.X);
                return false;
            }

            #endregion

            separationDistance = 0;
            separatingAxis = Vector3.Zero;
            return true;
        }

        /// <summary>
        /// Determines if the two boxes are colliding, including penetration depth data.
        /// </summary>
        /// <param name="a">First box to collide.</param>
        /// <param name="b">Second box to collide.</param>
        /// <param name="distance">Distance of separation or penetration.</param>
        /// <param name="axis">Axis of separation or penetration.</param>
        /// <param name="transformA">Transform to apply to shape A.</param>
        /// <param name="transformB">Transform to apply to shape B.</param>
        /// <returns>Whether or not the boxes collide.</returns>
        public static bool AreBoxesCollidingWithPenetration(BoxShape a, BoxShape b, ref RigidTransform3 transformA, ref RigidTransform3 transformB, out float distance, out Vector3 axis)
        {
            float aX = a.HalfWidth;
            float aY = a.HalfHeight;
            float aZ = a.HalfLength;

            float bX = b.HalfWidth;
            float bY = b.HalfHeight;
            float bZ = b.HalfLength;

            //Relative rotation from A to B.
            Matrix3 bR;

            Matrix3 aO;
            Matrix3.FromQuaternion(ref transformA.Orientation, out aO);
            Matrix3 bO;
            Matrix3.FromQuaternion(ref transformB.Orientation, out bO);

            //Relative translation rotated into A's configuration space.
            Vector3 t;
            Vector3.Sub(ref transformB.Position, ref transformA.Position, out t);

            float tempDistance;
            float minimumDistance = -float.MaxValue;
            var minimumAxis = new Vector3();

            #region A Face Normals

            bR.X.X = aO.X.X * bO.X.X + aO.X.Y * bO.X.Y + aO.X.Z * bO.X.Z;
            bR.X.Y = aO.X.X * bO.Y.X + aO.X.Y * bO.Y.Y + aO.X.Z * bO.Y.Z;
            bR.X.Z = aO.X.X * bO.Z.X + aO.X.Y * bO.Z.Y + aO.X.Z * bO.Z.Z;
            Matrix3 absBR;
            //Epsilons are added to deal with near-parallel edges.
            absBR.X.X = Math.Abs(bR.X.X) + Toolbox.Epsilon;
            absBR.X.Y = Math.Abs(bR.X.Y) + Toolbox.Epsilon;
            absBR.X.Z = Math.Abs(bR.X.Z) + Toolbox.Epsilon;
            float tX = t.X;
            t.X = t.X * aO.X.X + t.Y * aO.X.Y + t.Z * aO.X.Z;

            //Test the axes defines by entity A's rotation matrix.
            //A.X
            float rarb = aX + bX * absBR.X.X + bY * absBR.X.Y + bZ * absBR.X.Z;
            if (t.X > rarb)
            {
                distance = t.X - rarb;
                axis = new Vector3(aO.X.X, aO.X.Y, aO.X.Z);
                return false;
            }
            if (t.X < -rarb)
            {
                distance = -t.X - rarb;
                axis = new Vector3(-aO.X.X, -aO.X.Y, -aO.X.Z);
                return false;
            }
            //Inside
            if (t.X > 0)
            {
                tempDistance = t.X - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(aO.X.X, aO.X.Y, aO.X.Z);
                }
            }
            else
            {
                tempDistance = -t.X - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-aO.X.X, -aO.X.Y, -aO.X.Z);
                }
            }


            bR.Y.X = aO.Y.X * bO.X.X + aO.Y.Y * bO.X.Y + aO.Y.Z * bO.X.Z;
            bR.Y.Y = aO.Y.X * bO.Y.X + aO.Y.Y * bO.Y.Y + aO.Y.Z * bO.Y.Z;
            bR.Y.Z = aO.Y.X * bO.Z.X + aO.Y.Y * bO.Z.Y + aO.Y.Z * bO.Z.Z;
            absBR.Y.X = Math.Abs(bR.Y.X) + Toolbox.Epsilon;
            absBR.Y.Y = Math.Abs(bR.Y.Y) + Toolbox.Epsilon;
            absBR.Y.Z = Math.Abs(bR.Y.Z) + Toolbox.Epsilon;
            float tY = t.Y;
            t.Y = tX * aO.Y.X + t.Y * aO.Y.Y + t.Z * aO.Y.Z;

            //A.Y
            rarb = aY + bX * absBR.Y.X + bY * absBR.Y.Y + bZ * absBR.Y.Z;
            if (t.Y > rarb)
            {
                distance = t.Y - rarb;
                axis = new Vector3(aO.Y.X, aO.Y.Y, aO.Y.Z);
                return false;
            }
            if (t.Y < -rarb)
            {
                distance = -t.Y - rarb;
                axis = new Vector3(-aO.Y.X, -aO.Y.Y, -aO.Y.Z);
                return false;
            }
            //Inside
            if (t.Y > 0)
            {
                tempDistance = t.Y - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(aO.Y.X, aO.Y.Y, aO.Y.Z);
                }
            }
            else
            {
                tempDistance = -t.Y - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-aO.Y.X, -aO.Y.Y, -aO.Y.Z);
                }
            }

            bR.Z.X = aO.Z.X * bO.X.X + aO.Z.Y * bO.X.Y + aO.Z.Z * bO.X.Z;
            bR.Z.Y = aO.Z.X * bO.Y.X + aO.Z.Y * bO.Y.Y + aO.Z.Z * bO.Y.Z;
            bR.Z.Z = aO.Z.X * bO.Z.X + aO.Z.Y * bO.Z.Y + aO.Z.Z * bO.Z.Z;
            absBR.Z.X = Math.Abs(bR.Z.X) + Toolbox.Epsilon;
            absBR.Z.Y = Math.Abs(bR.Z.Y) + Toolbox.Epsilon;
            absBR.Z.Z = Math.Abs(bR.Z.Z) + Toolbox.Epsilon;
            t.Z = tX * aO.Z.X + tY * aO.Z.Y + t.Z * aO.Z.Z;

            //A.Z
            rarb = aZ + bX * absBR.Z.X + bY * absBR.Z.Y + bZ * absBR.Z.Z;
            if (t.Z > rarb)
            {
                distance = t.Z - rarb;
                axis = new Vector3(aO.Z.X, aO.Z.Y, aO.Z.Z);
                return false;
            }
            if (t.Z < -rarb)
            {
                distance = -t.Z - rarb;
                axis = new Vector3(-aO.Z.X, -aO.Z.Y, -aO.Z.Z);
                return false;
            }
            //Inside
            if (t.Z > 0)
            {
                tempDistance = t.Z - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(aO.Z.X, aO.Z.Y, aO.Z.Z);
                }
            }
            else
            {
                tempDistance = -t.Z - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-aO.Z.X, -aO.Z.Y, -aO.Z.Z);
                }
            }

            #endregion

            #region B Face Normals

            //Test the axes defines by entity B's rotation matrix.
            //B.X
            rarb = bX + aX * absBR.X.X + aY * absBR.Y.X + aZ * absBR.Z.X;
            float tl = t.X * bR.X.X + t.Y * bR.Y.X + t.Z * bR.Z.X;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.X.X, bO.X.Y, bO.X.Z);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(-bO.X.X, -bO.X.Y, -bO.X.Z);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempDistance = tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(bO.X.X, bO.X.Y, bO.X.Z);
                }
            }
            else
            {
                tempDistance = -tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-bO.X.X, -bO.X.Y, -bO.X.Z);
                }
            }

            //B.Y
            rarb = bY + aX * absBR.X.Y + aY * absBR.Y.Y + aZ * absBR.Z.Y;
            tl = t.X * bR.X.Y + t.Y * bR.Y.Y + t.Z * bR.Z.Y;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.Y.X, bO.Y.Y, bO.Y.Z);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(-bO.Y.X, -bO.Y.Y, -bO.Y.Z);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempDistance = tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(bO.Y.X, bO.Y.Y, bO.Y.Z);
                }
            }
            else
            {
                tempDistance = -tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-bO.Y.X, -bO.Y.Y, -bO.Y.Z);
                }
            }

            //B.Z
            rarb = bZ + aX * absBR.X.Z + aY * absBR.Y.Z + aZ * absBR.Z.Z;
            tl = t.X * bR.X.Z + t.Y * bR.Y.Z + t.Z * bR.Z.Z;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.Z.X, bO.Z.Y, bO.Z.Z);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(-bO.Z.X, -bO.Z.Y, -bO.Z.Z);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempDistance = tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(bO.Z.X, bO.Z.Y, bO.Z.Z);
                }
            }
            else
            {
                tempDistance = -tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-bO.Z.X, -bO.Z.Y, -bO.Z.Z);
                }
            }

            #endregion

            float axisLengthInverse;
            Vector3 tempAxis;

            #region A.X x B.()

            //Now for the edge-edge cases.
            //A.X x B.X
            rarb = aY * absBR.Z.X + aZ * absBR.Y.X +
                   bY * absBR.X.Z + bZ * absBR.X.Y;
            tl = t.Z * bR.Y.X - t.Y * bR.Z.X;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(aO.X.Y * bO.X.Z - aO.X.Z * bO.X.Y,
                                   aO.X.Z * bO.X.X - aO.X.X * bO.X.Z,
                                   aO.X.X * bO.X.Y - aO.X.Y * bO.X.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.X.Y * aO.X.Z - bO.X.Z * aO.X.Y,
                                   bO.X.Z * aO.X.X - bO.X.X * aO.X.Z,
                                   bO.X.X * aO.X.Y - bO.X.Y * aO.X.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(aO.X.Y * bO.X.Z - aO.X.Z * bO.X.Y,
                                       aO.X.Z * bO.X.X - aO.X.X * bO.X.Z,
                                       aO.X.X * bO.X.Y - aO.X.Y * bO.X.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(bO.X.Y * aO.X.Z - bO.X.Z * aO.X.Y,
                                       bO.X.Z * aO.X.X - bO.X.X * aO.X.Z,
                                       bO.X.X * aO.X.Y - bO.X.Y * aO.X.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.X x B.Y
            rarb = aY * absBR.Z.Y + aZ * absBR.Y.Y +
                   bX * absBR.X.Z + bZ * absBR.X.X;
            tl = t.Z * bR.Y.Y - t.Y * bR.Z.Y;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(aO.X.Y * bO.Y.Z - aO.X.Z * bO.Y.Y,
                                   aO.X.Z * bO.Y.X - aO.X.X * bO.Y.Z,
                                   aO.X.X * bO.Y.Y - aO.X.Y * bO.Y.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.Y.Y * aO.X.Z - bO.Y.Z * aO.X.Y,
                                   bO.Y.Z * aO.X.X - bO.Y.X * aO.X.Z,
                                   bO.Y.X * aO.X.Y - bO.Y.Y * aO.X.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(aO.X.Y * bO.Y.Z - aO.X.Z * bO.Y.Y,
                                       aO.X.Z * bO.Y.X - aO.X.X * bO.Y.Z,
                                       aO.X.X * bO.Y.Y - aO.X.Y * bO.Y.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(bO.Y.Y * aO.X.Z - bO.Y.Z * aO.X.Y,
                                       bO.Y.Z * aO.X.X - bO.Y.X * aO.X.Z,
                                       bO.Y.X * aO.X.Y - bO.Y.Y * aO.X.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.X x B.Z
            rarb = aY * absBR.Z.Z + aZ * absBR.Y.Z +
                   bX * absBR.X.Y + bY * absBR.X.X;
            tl = t.Z * bR.Y.Z - t.Y * bR.Z.Z;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(aO.X.Y * bO.Z.Z - aO.X.Z * bO.Z.Y,
                                   aO.X.Z * bO.Z.X - aO.X.X * bO.Z.Z,
                                   aO.X.X * bO.Z.Y - aO.X.Y * bO.Z.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.Z.Y * aO.X.Z - bO.Z.Z * aO.X.Y,
                                   bO.Z.Z * aO.X.X - bO.Z.X * aO.X.Z,
                                   bO.Z.X * aO.X.Y - bO.Z.Y * aO.X.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(aO.X.Y * bO.Z.Z - aO.X.Z * bO.Z.Y,
                                       aO.X.Z * bO.Z.X - aO.X.X * bO.Z.Z,
                                       aO.X.X * bO.Z.Y - aO.X.Y * bO.Z.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(bO.Z.Y * aO.X.Z - bO.Z.Z * aO.X.Y,
                                       bO.Z.Z * aO.X.X - bO.Z.X * aO.X.Z,
                                       bO.Z.X * aO.X.Y - bO.Z.Y * aO.X.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            #endregion

            #region A.Y x B.()

            //A.Y x B.X
            rarb = aX * absBR.Z.X + aZ * absBR.X.X +
                   bY * absBR.Y.Z + bZ * absBR.Y.Y;
            tl = t.X * bR.Z.X - t.Z * bR.X.X;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(aO.Y.Y * bO.X.Z - aO.Y.Z * bO.X.Y,
                                   aO.Y.Z * bO.X.X - aO.Y.X * bO.X.Z,
                                   aO.Y.X * bO.X.Y - aO.Y.Y * bO.X.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.X.Y * aO.Y.Z - bO.X.Z * aO.Y.Y,
                                   bO.X.Z * aO.Y.X - bO.X.X * aO.Y.Z,
                                   bO.X.X * aO.Y.Y - bO.X.Y * aO.Y.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(aO.Y.Y * bO.X.Z - aO.Y.Z * bO.X.Y,
                                       aO.Y.Z * bO.X.X - aO.Y.X * bO.X.Z,
                                       aO.Y.X * bO.X.Y - aO.Y.Y * bO.X.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(bO.X.Y * aO.Y.Z - bO.X.Z * aO.Y.Y,
                                       bO.X.Z * aO.Y.X - bO.X.X * aO.Y.Z,
                                       bO.X.X * aO.Y.Y - bO.X.Y * aO.Y.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.Y x B.Y
            rarb = aX * absBR.Z.Y + aZ * absBR.X.Y +
                   bX * absBR.Y.Z + bZ * absBR.Y.X;
            tl = t.X * bR.Z.Y - t.Z * bR.X.Y;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(aO.Y.Y * bO.Y.Z - aO.Y.Z * bO.Y.Y,
                                   aO.Y.Z * bO.Y.X - aO.Y.X * bO.Y.Z,
                                   aO.Y.X * bO.Y.Y - aO.Y.Y * bO.Y.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.Y.Y * aO.Y.Z - bO.Y.Z * aO.Y.Y,
                                   bO.Y.Z * aO.Y.X - bO.Y.X * aO.Y.Z,
                                   bO.Y.X * aO.Y.Y - bO.Y.Y * aO.Y.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(aO.Y.Y * bO.Y.Z - aO.Y.Z * bO.Y.Y,
                                       aO.Y.Z * bO.Y.X - aO.Y.X * bO.Y.Z,
                                       aO.Y.X * bO.Y.Y - aO.Y.Y * bO.Y.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(bO.Y.Y * aO.Y.Z - bO.Y.Z * aO.Y.Y,
                                       bO.Y.Z * aO.Y.X - bO.Y.X * aO.Y.Z,
                                       bO.Y.X * aO.Y.Y - bO.Y.Y * aO.Y.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.Y x B.Z
            rarb = aX * absBR.Z.Z + aZ * absBR.X.Z +
                   bX * absBR.Y.Y + bY * absBR.Y.X;
            tl = t.X * bR.Z.Z - t.Z * bR.X.Z;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(aO.Y.Y * bO.Z.Z - aO.Y.Z * bO.Z.Y,
                                   aO.Y.Z * bO.Z.X - aO.Y.X * bO.Z.Z,
                                   aO.Y.X * bO.Z.Y - aO.Y.Y * bO.Z.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.Z.Y * aO.Y.Z - bO.Z.Z * aO.Y.Y,
                                   bO.Z.Z * aO.Y.X - bO.Z.X * aO.Y.Z,
                                   bO.Z.X * aO.Y.Y - bO.Z.Y * aO.Y.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(aO.Y.Y * bO.Z.Z - aO.Y.Z * bO.Z.Y,
                                       aO.Y.Z * bO.Z.X - aO.Y.X * bO.Z.Z,
                                       aO.Y.X * bO.Z.Y - aO.Y.Y * bO.Z.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(bO.Z.Y * aO.Y.Z - bO.Z.Z * aO.Y.Y,
                                       bO.Z.Z * aO.Y.X - bO.Z.X * aO.Y.Z,
                                       bO.Z.X * aO.Y.Y - bO.Z.Y * aO.Y.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            #endregion

            #region A.Z x B.()

            //A.Z x B.X
            rarb = aX * absBR.Y.X + aY * absBR.X.X +
                   bY * absBR.Z.Z + bZ * absBR.Z.Y;
            tl = t.Y * bR.X.X - t.X * bR.Y.X;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(aO.Z.Y * bO.X.Z - aO.Z.Z * bO.X.Y,
                                   aO.Z.Z * bO.X.X - aO.Z.X * bO.X.Z,
                                   aO.Z.X * bO.X.Y - aO.Z.Y * bO.X.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.X.Y * aO.Z.Z - bO.X.Z * aO.Z.Y,
                                   bO.X.Z * aO.Z.X - bO.X.X * aO.Z.Z,
                                   bO.X.X * aO.Z.Y - bO.X.Y * aO.Z.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(aO.Z.Y * bO.X.Z - aO.Z.Z * bO.X.Y,
                                       aO.Z.Z * bO.X.X - aO.Z.X * bO.X.Z,
                                       aO.Z.X * bO.X.Y - aO.Z.Y * bO.X.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(bO.X.Y * aO.Z.Z - bO.X.Z * aO.Z.Y,
                                       bO.X.Z * aO.Z.X - bO.X.X * aO.Z.Z,
                                       bO.X.X * aO.Z.Y - bO.X.Y * aO.Z.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.Z x B.Y
            rarb = aX * absBR.Y.Y + aY * absBR.X.Y +
                   bX * absBR.Z.Z + bZ * absBR.Z.X;
            tl = t.Y * bR.X.Y - t.X * bR.Y.Y;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(aO.Z.Y * bO.Y.Z - aO.Z.Z * bO.Y.Y,
                                   aO.Z.Z * bO.Y.X - aO.Z.X * bO.Y.Z,
                                   aO.Z.X * bO.Y.Y - aO.Z.Y * bO.Y.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.Y.Y * aO.Z.Z - bO.Y.Z * aO.Z.Y,
                                   bO.Y.Z * aO.Z.X - bO.Y.X * aO.Z.Z,
                                   bO.Y.X * aO.Z.Y - bO.Y.Y * aO.Z.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(aO.Z.Y * bO.Y.Z - aO.Z.Z * bO.Y.Y,
                                       aO.Z.Z * bO.Y.X - aO.Z.X * bO.Y.Z,
                                       aO.Z.X * bO.Y.Y - aO.Z.Y * bO.Y.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(bO.Y.Y * aO.Z.Z - bO.Y.Z * aO.Z.Y,
                                       bO.Y.Z * aO.Z.X - bO.Y.X * aO.Z.Z,
                                       bO.Y.X * aO.Z.Y - bO.Y.Y * aO.Z.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.Z x B.Z
            rarb = aX * absBR.Y.Z + aY * absBR.X.Z +
                   bX * absBR.Z.Y + bY * absBR.Z.X;
            tl = t.Y * bR.X.Z - t.X * bR.Y.Z;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(aO.Z.Y * bO.Z.Z - aO.Z.Z * bO.Z.Y,
                                   aO.Z.Z * bO.Z.X - aO.Z.X * bO.Z.Z,
                                   aO.Z.X * bO.Z.Y - aO.Z.Y * bO.Z.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.Z.Y * aO.Z.Z - bO.Z.Z * aO.Z.Y,
                                   bO.Z.Z * aO.Z.X - bO.Z.X * aO.Z.Z,
                                   bO.Z.X * aO.Z.Y - bO.Z.Y * aO.Z.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(aO.Z.Y * bO.Z.Z - aO.Z.Z * bO.Z.Y,
                                       aO.Z.Z * bO.Z.X - aO.Z.X * bO.Z.Z,
                                       aO.Z.X * bO.Z.Y - aO.Z.Y * bO.Z.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(bO.Z.Y * aO.Z.Z - bO.Z.Z * aO.Z.Y,
                                       bO.Z.Z * aO.Z.X - bO.Z.X * aO.Z.Z,
                                       bO.Z.X * aO.Z.Y - bO.Z.Y * aO.Z.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            #endregion

            distance = minimumDistance;
            axis = minimumAxis;
            return true;
        }

#if ALLOWUNSAFE
        /// <summary>
        /// Determines if the two boxes are colliding and computes contact data.
        /// </summary>
        /// <param name="a">First box to collide.</param>
        /// <param name="b">Second box to collide.</param>
        /// <param name="distance">Distance of separation or penetration.</param>
        /// <param name="axis">Axis of separation or penetration.</param>
        /// <param name="contactData">Computed contact data.</param>
        /// <param name="transformA">Transform to apply to shape A.</param>
        /// <param name="transformB">Transform to apply to shape B.</param>
        /// <returns>Whether or not the boxes collide.</returns>
        public static unsafe bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform3 transformA, ref RigidTransform3 transformB, out float distance, out Vector3 axis, out TinyStructList<BoxContactData> contactData)
        {
            BoxContactDataCache tempData;
            bool toReturn = AreBoxesColliding(a, b, ref transformA, ref transformB, out distance, out axis, out tempData);
            BoxContactData* dataPointer = &tempData.D1;
            contactData = new TinyStructList<BoxContactData>();
            for (int i = 0; i < tempData.Count; i++)
            {
                contactData.Add(ref dataPointer[i]);
            }
            return toReturn;
        }
#endif

        /// <summary>
        /// Determines if the two boxes are colliding and computes contact data.
        /// </summary>
        /// <param name="a">First box to collide.</param>
        /// <param name="b">Second box to collide.</param>
        /// <param name="distance">Distance of separation or penetration.</param>
        /// <param name="axis">Axis of separation or penetration.</param>
        /// <param name="contactData">Contact positions, depths, and ids.</param>
        /// <param name="transformA">Transform to apply to shape A.</param>
        /// <param name="transformB">Transform to apply to shape B.</param>
        /// <returns>Whether or not the boxes collide.</returns>
#if ALLOWUNSAFE
        public static bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform3 transformA, ref RigidTransform3 transformB, out float distance, out Vector3 axis, out BoxContactDataCache contactData)
#else
        public static bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform3 transformA, ref RigidTransform3 transformB, out float distance, out Vector3 axis, out TinyStructList<BoxContactData> contactData)
#endif
        {
            float aX = a.HalfWidth;
            float aY = a.HalfHeight;
            float aZ = a.HalfLength;

            float bX = b.HalfWidth;
            float bY = b.HalfHeight;
            float bZ = b.HalfLength;

#if ALLOWUNSAFE
            contactData = new BoxContactDataCache();
#else
            contactData = new TinyStructList<BoxContactData>();
#endif
            //Relative rotation from A to B.
            Matrix3 bR;

            Matrix3 aO;
            Matrix3.FromQuaternion(ref transformA.Orientation, out aO);
            Matrix3 bO;
            Matrix3.FromQuaternion(ref transformB.Orientation, out bO);

            //Relative translation rotated into A's configuration space.
            Vector3 t;
            Vector3.Sub(ref transformB.Position, ref transformA.Position, out t);

            float tempDistance;
            float minimumDistance = -float.MaxValue;
            var minimumAxis = new Vector3();
            byte minimumFeature = 2; //2 means edge.  0-> A face, 1 -> B face.

            #region A Face Normals

            bR.X.X = aO.X.X * bO.X.X + aO.X.Y * bO.X.Y + aO.X.Z * bO.X.Z;
            bR.X.Y = aO.X.X * bO.Y.X + aO.X.Y * bO.Y.Y + aO.X.Z * bO.Y.Z;
            bR.X.Z = aO.X.X * bO.Z.X + aO.X.Y * bO.Z.Y + aO.X.Z * bO.Z.Z;
            Matrix3 absBR;
            //Epsilons are added to deal with near-parallel edges.
            absBR.X.X = Math.Abs(bR.X.X) + Toolbox.Epsilon;
            absBR.X.Y = Math.Abs(bR.X.Y) + Toolbox.Epsilon;
            absBR.X.Z = Math.Abs(bR.X.Z) + Toolbox.Epsilon;
            float tX = t.X;
            t.X = t.X * aO.X.X + t.Y * aO.X.Y + t.Z * aO.X.Z;

            //Test the axes defines by entity A's rotation matrix.
            //A.X
            float rarb = aX + bX * absBR.X.X + bY * absBR.X.Y + bZ * absBR.X.Z;
            if (t.X > rarb)
            {
                distance = t.X - rarb;
                axis = new Vector3(-aO.X.X, -aO.X.Y, -aO.X.Z);
                return false;
            }
            if (t.X < -rarb)
            {
                distance = -t.X - rarb;
                axis = new Vector3(aO.X.X, aO.X.Y, aO.X.Z);
                return false;
            }
            //Inside
            if (t.X > 0)
            {
                tempDistance = t.X - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-aO.X.X, -aO.X.Y, -aO.X.Z);
                    minimumFeature = 0;
                }
            }
            else
            {
                tempDistance = -t.X - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(aO.X.X, aO.X.Y, aO.X.Z);
                    minimumFeature = 0;
                }
            }


            bR.Y.X = aO.Y.X * bO.X.X + aO.Y.Y * bO.X.Y + aO.Y.Z * bO.X.Z;
            bR.Y.Y = aO.Y.X * bO.Y.X + aO.Y.Y * bO.Y.Y + aO.Y.Z * bO.Y.Z;
            bR.Y.Z = aO.Y.X * bO.Z.X + aO.Y.Y * bO.Z.Y + aO.Y.Z * bO.Z.Z;
            absBR.Y.X = Math.Abs(bR.Y.X) + Toolbox.Epsilon;
            absBR.Y.Y = Math.Abs(bR.Y.Y) + Toolbox.Epsilon;
            absBR.Y.Z = Math.Abs(bR.Y.Z) + Toolbox.Epsilon;
            float tY = t.Y;
            t.Y = tX * aO.Y.X + t.Y * aO.Y.Y + t.Z * aO.Y.Z;

            //A.Y
            rarb = aY + bX * absBR.Y.X + bY * absBR.Y.Y + bZ * absBR.Y.Z;
            if (t.Y > rarb)
            {
                distance = t.Y - rarb;
                axis = new Vector3(-aO.Y.X, -aO.Y.Y, -aO.Y.Z);
                return false;
            }
            if (t.Y < -rarb)
            {
                distance = -t.Y - rarb;
                axis = new Vector3(aO.Y.X, aO.Y.Y, aO.Y.Z);
                return false;
            }
            //Inside
            if (t.Y > 0)
            {
                tempDistance = t.Y - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-aO.Y.X, -aO.Y.Y, -aO.Y.Z);
                    minimumFeature = 0;
                }
            }
            else
            {
                tempDistance = -t.Y - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(aO.Y.X, aO.Y.Y, aO.Y.Z);
                    minimumFeature = 0;
                }
            }

            bR.Z.X = aO.Z.X * bO.X.X + aO.Z.Y * bO.X.Y + aO.Z.Z * bO.X.Z;
            bR.Z.Y = aO.Z.X * bO.Y.X + aO.Z.Y * bO.Y.Y + aO.Z.Z * bO.Y.Z;
            bR.Z.Z = aO.Z.X * bO.Z.X + aO.Z.Y * bO.Z.Y + aO.Z.Z * bO.Z.Z;
            absBR.Z.X = Math.Abs(bR.Z.X) + Toolbox.Epsilon;
            absBR.Z.Y = Math.Abs(bR.Z.Y) + Toolbox.Epsilon;
            absBR.Z.Z = Math.Abs(bR.Z.Z) + Toolbox.Epsilon;
            t.Z = tX * aO.Z.X + tY * aO.Z.Y + t.Z * aO.Z.Z;

            //A.Z
            rarb = aZ + bX * absBR.Z.X + bY * absBR.Z.Y + bZ * absBR.Z.Z;
            if (t.Z > rarb)
            {
                distance = t.Z - rarb;
                axis = new Vector3(-aO.Z.X, -aO.Z.Y, -aO.Z.Z);
                return false;
            }
            if (t.Z < -rarb)
            {
                distance = -t.Z - rarb;
                axis = new Vector3(aO.Z.X, aO.Z.Y, aO.Z.Z);
                return false;
            }
            //Inside
            if (t.Z > 0)
            {
                tempDistance = t.Z - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-aO.Z.X, -aO.Z.Y, -aO.Z.Z);
                    minimumFeature = 0;
                }
            }
            else
            {
                tempDistance = -t.Z - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(aO.Z.X, aO.Z.Y, aO.Z.Z);
                    minimumFeature = 0;
                }
            }

            #endregion

            const float antiBBias = .01f;
            minimumDistance += antiBBias;

            #region B Face Normals

            //Test the axes defines by entity B's rotation matrix.
            //B.X
            rarb = bX + aX * absBR.X.X + aY * absBR.Y.X + aZ * absBR.Z.X;
            float tl = t.X * bR.X.X + t.Y * bR.Y.X + t.Z * bR.Z.X;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(-bO.X.X, -bO.X.Y, -bO.X.Z);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.X.X, bO.X.Y, bO.X.Z);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempDistance = tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-bO.X.X, -bO.X.Y, -bO.X.Z);
                    minimumFeature = 1;
                }
            }
            else
            {
                tempDistance = -tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(bO.X.X, bO.X.Y, bO.X.Z);
                    minimumFeature = 1;
                }
            }

            //B.Y
            rarb = bY + aX * absBR.X.Y + aY * absBR.Y.Y + aZ * absBR.Z.Y;
            tl = t.X * bR.X.Y + t.Y * bR.Y.Y + t.Z * bR.Z.Y;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(-bO.Y.X, -bO.Y.Y, -bO.Y.Z);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.Y.X, bO.Y.Y, bO.Y.Z);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempDistance = tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-bO.Y.X, -bO.Y.Y, -bO.Y.Z);
                    minimumFeature = 1;
                }
            }
            else
            {
                tempDistance = -tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(bO.Y.X, bO.Y.Y, bO.Y.Z);
                    minimumFeature = 1;
                }
            }

            //B.Z
            rarb = bZ + aX * absBR.X.Z + aY * absBR.Y.Z + aZ * absBR.Z.Z;
            tl = t.X * bR.X.Z + t.Y * bR.Y.Z + t.Z * bR.Z.Z;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(-bO.Z.X, -bO.Z.Y, -bO.Z.Z);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(bO.Z.X, bO.Z.Y, bO.Z.Z);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempDistance = tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(-bO.Z.X, -bO.Z.Y, -bO.Z.Z);
                    minimumFeature = 1;
                }
            }
            else
            {
                tempDistance = -tl - rarb;
                if (tempDistance > minimumDistance)
                {
                    minimumDistance = tempDistance;
                    minimumAxis = new Vector3(bO.Z.X, bO.Z.Y, bO.Z.Z);
                    minimumFeature = 1;
                }
            }

            #endregion

            if (minimumFeature != 1)
                minimumDistance -= antiBBias;

            float antiEdgeBias = .01f;
            minimumDistance += antiEdgeBias;
            float axisLengthInverse;
            Vector3 tempAxis;

            #region A.X x B.()

            //Now for the edge-edge cases.
            //A.X x B.X
            rarb = aY * absBR.Z.X + aZ * absBR.Y.X +
                   bY * absBR.X.Z + bZ * absBR.X.Y;
            tl = t.Z * bR.Y.X - t.Y * bR.Z.X;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.X.Y * aO.X.Z - bO.X.Z * aO.X.Y,
                                   bO.X.Z * aO.X.X - bO.X.X * aO.X.Z,
                                   bO.X.X * aO.X.Y - bO.X.Y * aO.X.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(aO.X.Y * bO.X.Z - aO.X.Z * bO.X.Y,
                                   aO.X.Z * bO.X.X - aO.X.X * bO.X.Z,
                                   aO.X.X * bO.X.Y - aO.X.Y * bO.X.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(bO.X.Y * aO.X.Z - bO.X.Z * aO.X.Y,
                                       bO.X.Z * aO.X.X - bO.X.X * aO.X.Z,
                                       bO.X.X * aO.X.Y - bO.X.Y * aO.X.X);

                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(aO.X.Y * bO.X.Z - aO.X.Z * bO.X.Y,
                                       aO.X.Z * bO.X.X - aO.X.X * bO.X.Z,
                                       aO.X.X * bO.X.Y - aO.X.Y * bO.X.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.X x B.Y
            rarb = aY * absBR.Z.Y + aZ * absBR.Y.Y +
                   bX * absBR.X.Z + bZ * absBR.X.X;
            tl = t.Z * bR.Y.Y - t.Y * bR.Z.Y;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.Y.Y * aO.X.Z - bO.Y.Z * aO.X.Y,
                                   bO.Y.Z * aO.X.X - bO.Y.X * aO.X.Z,
                                   bO.Y.X * aO.X.Y - bO.Y.Y * aO.X.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(aO.X.Y * bO.Y.Z - aO.X.Z * bO.Y.Y,
                                   aO.X.Z * bO.Y.X - aO.X.X * bO.Y.Z,
                                   aO.X.X * bO.Y.Y - aO.X.Y * bO.Y.X);

                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(bO.Y.Y * aO.X.Z - bO.Y.Z * aO.X.Y,
                                       bO.Y.Z * aO.X.X - bO.Y.X * aO.X.Z,
                                       bO.Y.X * aO.X.Y - bO.Y.Y * aO.X.X);

                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(aO.X.Y * bO.Y.Z - aO.X.Z * bO.Y.Y,
                                       aO.X.Z * bO.Y.X - aO.X.X * bO.Y.Z,
                                       aO.X.X * bO.Y.Y - aO.X.Y * bO.Y.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.X x B.Z
            rarb = aY * absBR.Z.Z + aZ * absBR.Y.Z +
                   bX * absBR.X.Y + bY * absBR.X.X;
            tl = t.Z * bR.Y.Z - t.Y * bR.Z.Z;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.Z.Y * aO.X.Z - bO.Z.Z * aO.X.Y,
                                   bO.Z.Z * aO.X.X - bO.Z.X * aO.X.Z,
                                   bO.Z.X * aO.X.Y - bO.Z.Y * aO.X.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;

                axis = new Vector3(aO.X.Y * bO.Z.Z - aO.X.Z * bO.Z.Y,
                                   aO.X.Z * bO.Z.X - aO.X.X * bO.Z.Z,
                                   aO.X.X * bO.Z.Y - aO.X.Y * bO.Z.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(bO.Z.Y * aO.X.Z - bO.Z.Z * aO.X.Y,
                                       bO.Z.Z * aO.X.X - bO.Z.X * aO.X.Z,
                                       bO.Z.X * aO.X.Y - bO.Z.Y * aO.X.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(aO.X.Y * bO.Z.Z - aO.X.Z * bO.Z.Y,
                                       aO.X.Z * bO.Z.X - aO.X.X * bO.Z.Z,
                                       aO.X.X * bO.Z.Y - aO.X.Y * bO.Z.X);

                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            #endregion

            #region A.Y x B.()

            //A.Y x B.X
            rarb = aX * absBR.Z.X + aZ * absBR.X.X +
                   bY * absBR.Y.Z + bZ * absBR.Y.Y;
            tl = t.X * bR.Z.X - t.Z * bR.X.X;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.X.Y * aO.Y.Z - bO.X.Z * aO.Y.Y,
                                   bO.X.Z * aO.Y.X - bO.X.X * aO.Y.Z,
                                   bO.X.X * aO.Y.Y - bO.X.Y * aO.Y.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(aO.Y.Y * bO.X.Z - aO.Y.Z * bO.X.Y,
                                   aO.Y.Z * bO.X.X - aO.Y.X * bO.X.Z,
                                   aO.Y.X * bO.X.Y - aO.Y.Y * bO.X.X);

                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(bO.X.Y * aO.Y.Z - bO.X.Z * aO.Y.Y,
                                       bO.X.Z * aO.Y.X - bO.X.X * aO.Y.Z,
                                       bO.X.X * aO.Y.Y - bO.X.Y * aO.Y.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(aO.Y.Y * bO.X.Z - aO.Y.Z * bO.X.Y,
                                       aO.Y.Z * bO.X.X - aO.Y.X * bO.X.Z,
                                       aO.Y.X * bO.X.Y - aO.Y.Y * bO.X.X);

                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.Y x B.Y
            rarb = aX * absBR.Z.Y + aZ * absBR.X.Y +
                   bX * absBR.Y.Z + bZ * absBR.Y.X;
            tl = t.X * bR.Z.Y - t.Z * bR.X.Y;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.Y.Y * aO.Y.Z - bO.Y.Z * aO.Y.Y,
                                   bO.Y.Z * aO.Y.X - bO.Y.X * aO.Y.Z,
                                   bO.Y.X * aO.Y.Y - bO.Y.Y * aO.Y.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;

                axis = new Vector3(aO.Y.Y * bO.Y.Z - aO.Y.Z * bO.Y.Y,
                                   aO.Y.Z * bO.Y.X - aO.Y.X * bO.Y.Z,
                                   aO.Y.X * bO.Y.Y - aO.Y.Y * bO.Y.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(bO.Y.Y * aO.Y.Z - bO.Y.Z * aO.Y.Y,
                                       bO.Y.Z * aO.Y.X - bO.Y.X * aO.Y.Z,
                                       bO.Y.X * aO.Y.Y - bO.Y.Y * aO.Y.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(aO.Y.Y * bO.Y.Z - aO.Y.Z * bO.Y.Y,
                                       aO.Y.Z * bO.Y.X - aO.Y.X * bO.Y.Z,
                                       aO.Y.X * bO.Y.Y - aO.Y.Y * bO.Y.X);

                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.Y x B.Z
            rarb = aX * absBR.Z.Z + aZ * absBR.X.Z +
                   bX * absBR.Y.Y + bY * absBR.Y.X;
            tl = t.X * bR.Z.Z - t.Z * bR.X.Z;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.Z.Y * aO.Y.Z - bO.Z.Z * aO.Y.Y,
                                   bO.Z.Z * aO.Y.X - bO.Z.X * aO.Y.Z,
                                   bO.Z.X * aO.Y.Y - bO.Z.Y * aO.Y.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;

                axis = new Vector3(aO.Y.Y * bO.Z.Z - aO.Y.Z * bO.Z.Y,
                                   aO.Y.Z * bO.Z.X - aO.Y.X * bO.Z.Z,
                                   aO.Y.X * bO.Z.Y - aO.Y.Y * bO.Z.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(bO.Z.Y * aO.Y.Z - bO.Z.Z * aO.Y.Y,
                                       bO.Z.Z * aO.Y.X - bO.Z.X * aO.Y.Z,
                                       bO.Z.X * aO.Y.Y - bO.Z.Y * aO.Y.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(aO.Y.Y * bO.Z.Z - aO.Y.Z * bO.Z.Y,
                                       aO.Y.Z * bO.Z.X - aO.Y.X * bO.Z.Z,
                                       aO.Y.X * bO.Z.Y - aO.Y.Y * bO.Z.X);

                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            #endregion

            #region A.Z x B.()

            //A.Z x B.X
            rarb = aX * absBR.Y.X + aY * absBR.X.X +
                   bY * absBR.Z.Z + bZ * absBR.Z.Y;
            tl = t.Y * bR.X.X - t.X * bR.Y.X;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.X.Y * aO.Z.Z - bO.X.Z * aO.Z.Y,
                                   bO.X.Z * aO.Z.X - bO.X.X * aO.Z.Z,
                                   bO.X.X * aO.Z.Y - bO.X.Y * aO.Z.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;

                axis = new Vector3(aO.Z.Y * bO.X.Z - aO.Z.Z * bO.X.Y,
                                   aO.Z.Z * bO.X.X - aO.Z.X * bO.X.Z,
                                   aO.Z.X * bO.X.Y - aO.Z.Y * bO.X.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(bO.X.Y * aO.Z.Z - bO.X.Z * aO.Z.Y,
                                       bO.X.Z * aO.Z.X - bO.X.X * aO.Z.Z,
                                       bO.X.X * aO.Z.Y - bO.X.Y * aO.Z.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(aO.Z.Y * bO.X.Z - aO.Z.Z * bO.X.Y,
                                       aO.Z.Z * bO.X.X - aO.Z.X * bO.X.Z,
                                       aO.Z.X * bO.X.Y - aO.Z.Y * bO.X.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.Z x B.Y
            rarb = aX * absBR.Y.Y + aY * absBR.X.Y +
                   bX * absBR.Z.Z + bZ * absBR.Z.X;
            tl = t.Y * bR.X.Y - t.X * bR.Y.Y;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.Y.Y * aO.Z.Z - bO.Y.Z * aO.Z.Y,
                                   bO.Y.Z * aO.Z.X - bO.Y.X * aO.Z.Z,
                                   bO.Y.X * aO.Z.Y - bO.Y.Y * aO.Z.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;

                axis = new Vector3(aO.Z.Y * bO.Y.Z - aO.Z.Z * bO.Y.Y,
                                   aO.Z.Z * bO.Y.X - aO.Z.X * bO.Y.Z,
                                   aO.Z.X * bO.Y.Y - aO.Z.Y * bO.Y.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(bO.Y.Y * aO.Z.Z - bO.Y.Z * aO.Z.Y,
                                       bO.Y.Z * aO.Z.X - bO.Y.X * aO.Z.Z,
                                       bO.Y.X * aO.Z.Y - bO.Y.Y * aO.Z.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(aO.Z.Y * bO.Y.Z - aO.Z.Z * bO.Y.Y,
                                       aO.Z.Z * bO.Y.X - aO.Z.X * bO.Y.Z,
                                       aO.Z.X * bO.Y.Y - aO.Z.Y * bO.Y.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            //A.Z x B.Z
            rarb = aX * absBR.Y.Z + aY * absBR.X.Z +
                   bX * absBR.Z.Y + bY * absBR.Z.X;
            tl = t.Y * bR.X.Z - t.X * bR.Y.Z;
            if (tl > rarb)
            {
                distance = tl - rarb;
                axis = new Vector3(bO.Z.Y * aO.Z.Z - bO.Z.Z * aO.Z.Y,
                                   bO.Z.Z * aO.Z.X - bO.Z.X * aO.Z.Z,
                                   bO.Z.X * aO.Z.Y - bO.Z.Y * aO.Z.X);
                return false;
            }
            if (tl < -rarb)
            {
                distance = -tl - rarb;
                axis = new Vector3(aO.Z.Y * bO.Z.Z - aO.Z.Z * bO.Z.Y,
                                   aO.Z.Z * bO.Z.X - aO.Z.X * bO.Z.Z,
                                   aO.Z.X * bO.Z.Y - aO.Z.Y * bO.Z.X);
                return false;
            }
            //Inside
            if (tl > 0)
            {
                tempAxis = new Vector3(bO.Z.Y * aO.Z.Z - bO.Z.Z * aO.Z.Y,
                                       bO.Z.Z * aO.Z.X - bO.Z.X * aO.Z.Z,
                                       bO.Z.X * aO.Z.Y - bO.Z.Y * aO.Z.X);
                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }
            else
            {
                tempAxis = new Vector3(aO.Z.Y * bO.Z.Z - aO.Z.Z * bO.Z.Y,
                                       aO.Z.Z * bO.Z.X - aO.Z.X * bO.Z.Z,
                                       aO.Z.X * bO.Z.Y - aO.Z.Y * bO.Z.X);

                axisLengthInverse = 1 / tempAxis.Length();
                tempDistance = (-tl - rarb) * axisLengthInverse;
                if (tempDistance > minimumDistance)
                {
                    minimumFeature = 2;
                    minimumDistance = tempDistance;
                    tempAxis.X *= axisLengthInverse;
                    tempAxis.Y *= axisLengthInverse;
                    tempAxis.Z *= axisLengthInverse;
                    minimumAxis = tempAxis;
                }
            }

            #endregion

            if (minimumFeature == 2)
            {

                //Edge-edge contact conceptually only has one contact, but allowing it to create multiple due to penetration is more robust.
                GetEdgeEdgeContact(a, b, ref transformA.Position, ref aO, ref transformB.Position, ref bO, minimumDistance, ref minimumAxis, out contactData);

                //Vector3 position;
                //float depth;
                //int id;
                //                GetEdgeEdgeContact(a, b, ref transformA.Position, ref aO, ref transformB.Position, ref bO, ref minimumAxis, out position, out id);
                //#if ALLOWUNSAFE
                //                contactData.D1.Position = position;
                //                contactData.D1.Depth = minimumDistance; 
                //                contactData.D1.Id = id;
                //                contactData.Count = 1;
                //#else
                //                var toAdd = new BoxContactData();
                //                toAdd.Position = position;
                //                toAdd.Depth = minimumDistance;
                //                toAdd.Id = id;
                //                contactData.Add(ref toAdd);
                //#endif
            }
            else
            {
                minimumDistance -= antiEdgeBias;
                GetFaceContacts(a, b, ref transformA.Position, ref aO, ref transformB.Position, ref bO, minimumFeature == 0, ref minimumAxis, out contactData);

            }

            distance = minimumDistance;
            axis = minimumAxis;
            return true;
        }

#if ALLOWUNSAFE
        internal static void GetEdgeEdgeContact(BoxShape a, BoxShape b, ref Vector3 positionA, ref Matrix3 orientationA, ref Vector3 positionB, ref Matrix3 orientationB, float depth, ref Vector3 mtd, out BoxContactDataCache contactData)
#else
        internal static void GetEdgeEdgeContact(BoxShape a, BoxShape b, ref Vector3 positionA, ref Matrix3 orientationA, ref Vector3 positionB, ref Matrix3 orientationB, float depth, ref Vector3 mtd, out TinyStructList<BoxContactData> contactData)
#endif
        {
            //Edge-edge contacts conceptually can only create one contact in perfectly rigid collisions.
            //However, this is a discrete approximation of rigidity; things can penetrate each other.
            //If edge-edge only returns a single contact, there's a good chance that the box will get into
            //an oscillating state when under pressure.

            //To avoid the oscillation, we may sometimes need two edge contacts.
            //To determine which edges to use, compute 8 dot products.
            //One for each edge parallel to the contributing axis on each of the two shapes.
            //The resulting cases are:
            //One edge on A touching one edge on B.
            //Two edges on A touching one edge on B.
            //One edge on A touching two edges on B.
            //Two edges on A touching two edges on B.

            //The three latter cases SHOULD be covered by the face-contact system, but in practice,
            //they are not sufficiently covered because the system decides that the single edge-edge pair
            //should be used and drops the other contacts, producting the aforementioned oscillation.

            //All edge cross products result in the MTD, so no recalculation is necessary.

            //Of the four edges which are aligned with the local edge axis, pick the two
            //who have vertices which, when dotted with the local mtd, are greatest.

            //Compute the closest points between each edge pair.  For two edges each,
            //this comes out to four total closest point tests.
            //This is not a traditional closest point between segments test.
            //Completely ignore the pair if the closest points turn out to be beyond the intervals of the segments.

            //Use the offsets found from each test.
            //Test the A to B offset against the MTD, which is also known to be oriented in a certain way.
            //That known directionality allows easy computation of depth using MTD dot offset.
            //Do not use any contacts which have negative depth/positive distance.


            //Put the minimum translation direction into the local space of each object.
            Vector3 mtdA, mtdB;
            Vector3 negatedMtd;
            Vector3.Neg(ref mtd, out negatedMtd);
            Vector3.TransformTranspose(ref negatedMtd, ref orientationA, out mtdA);
            Vector3.TransformTranspose(ref mtd, ref orientationB, out mtdB);


            Vector3 edgeAStart1, edgeAEnd1, edgeAStart2, edgeAEnd2;
            Vector3 edgeBStart1, edgeBEnd1, edgeBStart2, edgeBEnd2;
            float aHalfWidth = a.halfWidth;
            float aHalfHeight = a.halfHeight;
            float aHalfLength = a.halfLength;

            float bHalfWidth = b.halfWidth;
            float bHalfHeight = b.halfHeight;
            float bHalfLength = b.halfLength;

            //Letter stands for owner.  Number stands for edge (1 or 2).
            int edgeAStart1Id, edgeAEnd1Id, edgeAStart2Id, edgeAEnd2Id;
            int edgeBStart1Id, edgeBEnd1Id, edgeBStart2Id, edgeBEnd2Id;

            //This is an edge-edge collision, so one (AND ONLY ONE) of the components in the 
            //local direction must be very close to zero.  We can use an arbitrary fixed 
            //epsilon because the mtd is always unit length.

            #region Edge A

            if (Math.Abs(mtdA.X) < Toolbox.Epsilon)
            {
                //mtd is in the Y-Z plane.
                //Perform an implicit dot with the edge location relative to the center.
                //Find the two edges furthest in the direction of the mtdA.
                var dots = new TinyList<float>();
                dots.Add(-aHalfHeight * mtdA.Y - aHalfLength * mtdA.Z);
                dots.Add(-aHalfHeight * mtdA.Y + aHalfLength * mtdA.Z);
                dots.Add(aHalfHeight * mtdA.Y - aHalfLength * mtdA.Z);
                dots.Add(aHalfHeight * mtdA.Y + aHalfLength * mtdA.Z);

                //Find the first and second highest indices.
                int highestIndex, secondHighestIndex;
                FindHighestIndices(ref dots, out highestIndex, out secondHighestIndex);
                //Use the indices to compute the edges.
                GetEdgeData(highestIndex, 0, aHalfWidth, aHalfHeight, aHalfLength, out edgeAStart1, out edgeAEnd1, out edgeAStart1Id, out edgeAEnd1Id);
                GetEdgeData(secondHighestIndex, 0, aHalfWidth, aHalfHeight, aHalfLength, out edgeAStart2, out edgeAEnd2, out edgeAStart2Id, out edgeAEnd2Id);


            }
            else if (Math.Abs(mtdA.Y) < Toolbox.Epsilon)
            {
                //mtd is in the X-Z plane
                //Perform an implicit dot with the edge location relative to the center.
                //Find the two edges furthest in the direction of the mtdA.
                var dots = new TinyList<float>();
                dots.Add(-aHalfWidth * mtdA.X - aHalfLength * mtdA.Z);
                dots.Add(-aHalfWidth * mtdA.X + aHalfLength * mtdA.Z);
                dots.Add(aHalfWidth * mtdA.X - aHalfLength * mtdA.Z);
                dots.Add(aHalfWidth * mtdA.X + aHalfLength * mtdA.Z);

                //Find the first and second highest indices.
                int highestIndex, secondHighestIndex;
                FindHighestIndices(ref dots, out highestIndex, out secondHighestIndex);
                //Use the indices to compute the edges.
                GetEdgeData(highestIndex, 1, aHalfWidth, aHalfHeight, aHalfLength, out edgeAStart1, out edgeAEnd1, out edgeAStart1Id, out edgeAEnd1Id);
                GetEdgeData(secondHighestIndex, 1, aHalfWidth, aHalfHeight, aHalfLength, out edgeAStart2, out edgeAEnd2, out edgeAStart2Id, out edgeAEnd2Id);
            }
            else
            {
                //mtd is in the X-Y plane
                //Perform an implicit dot with the edge location relative to the center.
                //Find the two edges furthest in the direction of the mtdA.
                var dots = new TinyList<float>();
                dots.Add(-aHalfWidth * mtdA.X - aHalfHeight * mtdA.Y);
                dots.Add(-aHalfWidth * mtdA.X + aHalfHeight * mtdA.Y);
                dots.Add(aHalfWidth * mtdA.X - aHalfHeight * mtdA.Y);
                dots.Add(aHalfWidth * mtdA.X + aHalfHeight * mtdA.Y);

                //Find the first and second highest indices.
                int highestIndex, secondHighestIndex;
                FindHighestIndices(ref dots, out highestIndex, out secondHighestIndex);
                //Use the indices to compute the edges.
                GetEdgeData(highestIndex, 2, aHalfWidth, aHalfHeight, aHalfLength, out edgeAStart1, out edgeAEnd1, out edgeAStart1Id, out edgeAEnd1Id);
                GetEdgeData(secondHighestIndex, 2, aHalfWidth, aHalfHeight, aHalfLength, out edgeAStart2, out edgeAEnd2, out edgeAStart2Id, out edgeAEnd2Id);
            }

            #endregion

            #region Edge B

            if (Math.Abs(mtdB.X) < Toolbox.Epsilon)
            {
                //mtd is in the Y-Z plane.
                //Perform an implicit dot with the edge location relative to the center.
                //Find the two edges furthest in the direction of the mtdB.
                var dots = new TinyList<float>();
                dots.Add(-bHalfHeight * mtdB.Y - bHalfLength * mtdB.Z);
                dots.Add(-bHalfHeight * mtdB.Y + bHalfLength * mtdB.Z);
                dots.Add(bHalfHeight * mtdB.Y - bHalfLength * mtdB.Z);
                dots.Add(bHalfHeight * mtdB.Y + bHalfLength * mtdB.Z);

                //Find the first and second highest indices.
                int highestIndex, secondHighestIndex;
                FindHighestIndices(ref dots, out highestIndex, out secondHighestIndex);
                //Use the indices to compute the edges.
                GetEdgeData(highestIndex, 0, bHalfWidth, bHalfHeight, bHalfLength, out edgeBStart1, out edgeBEnd1, out edgeBStart1Id, out edgeBEnd1Id);
                GetEdgeData(secondHighestIndex, 0, bHalfWidth, bHalfHeight, bHalfLength, out edgeBStart2, out edgeBEnd2, out edgeBStart2Id, out edgeBEnd2Id);


            }
            else if (Math.Abs(mtdB.Y) < Toolbox.Epsilon)
            {
                //mtd is in the X-Z plane
                //Perform an implicit dot with the edge location relative to the center.
                //Find the two edges furthest in the direction of the mtdB.
                var dots = new TinyList<float>();
                dots.Add(-bHalfWidth * mtdB.X - bHalfLength * mtdB.Z);
                dots.Add(-bHalfWidth * mtdB.X + bHalfLength * mtdB.Z);
                dots.Add(bHalfWidth * mtdB.X - bHalfLength * mtdB.Z);
                dots.Add(bHalfWidth * mtdB.X + bHalfLength * mtdB.Z);

                //Find the first and second highest indices.
                int highestIndex, secondHighestIndex;
                FindHighestIndices(ref dots, out highestIndex, out secondHighestIndex);
                //Use the indices to compute the edges.
                GetEdgeData(highestIndex, 1, bHalfWidth, bHalfHeight, bHalfLength, out edgeBStart1, out edgeBEnd1, out edgeBStart1Id, out edgeBEnd1Id);
                GetEdgeData(secondHighestIndex, 1, bHalfWidth, bHalfHeight, bHalfLength, out edgeBStart2, out edgeBEnd2, out edgeBStart2Id, out edgeBEnd2Id);
            }
            else
            {
                //mtd is in the X-Y plane
                //Perform an implicit dot with the edge location relative to the center.
                //Find the two edges furthest in the direction of the mtdB.
                var dots = new TinyList<float>();
                dots.Add(-bHalfWidth * mtdB.X - bHalfHeight * mtdB.Y);
                dots.Add(-bHalfWidth * mtdB.X + bHalfHeight * mtdB.Y);
                dots.Add(bHalfWidth * mtdB.X - bHalfHeight * mtdB.Y);
                dots.Add(bHalfWidth * mtdB.X + bHalfHeight * mtdB.Y);

                //Find the first and second highest indices.
                int highestIndex, secondHighestIndex;
                FindHighestIndices(ref dots, out highestIndex, out secondHighestIndex);
                //Use the indices to compute the edges.
                GetEdgeData(highestIndex, 2, bHalfWidth, bHalfHeight, bHalfLength, out edgeBStart1, out edgeBEnd1, out edgeBStart1Id, out edgeBEnd1Id);
                GetEdgeData(secondHighestIndex, 2, bHalfWidth, bHalfHeight, bHalfLength, out edgeBStart2, out edgeBEnd2, out edgeBStart2Id, out edgeBEnd2Id);
            }

            #endregion


            Vector3.Transform(ref edgeAStart1, ref orientationA, out edgeAStart1);
            Vector3.Transform(ref edgeAEnd1, ref orientationA, out edgeAEnd1);
            Vector3.Transform(ref edgeBStart1, ref orientationB, out edgeBStart1);
            Vector3.Transform(ref edgeBEnd1, ref orientationB, out edgeBEnd1);

            Vector3.Transform(ref edgeAStart2, ref orientationA, out edgeAStart2);
            Vector3.Transform(ref edgeAEnd2, ref orientationA, out edgeAEnd2);
            Vector3.Transform(ref edgeBStart2, ref orientationB, out edgeBStart2);
            Vector3.Transform(ref edgeBEnd2, ref orientationB, out edgeBEnd2);

            Vector3.Add(ref edgeAStart1, ref positionA, out edgeAStart1);
            Vector3.Add(ref edgeAEnd1, ref positionA, out edgeAEnd1);
            Vector3.Add(ref edgeBStart1, ref positionB, out edgeBStart1);
            Vector3.Add(ref edgeBEnd1, ref positionB, out edgeBEnd1);

            Vector3.Add(ref edgeAStart2, ref positionA, out edgeAStart2);
            Vector3.Add(ref edgeAEnd2, ref positionA, out edgeAEnd2);
            Vector3.Add(ref edgeBStart2, ref positionB, out edgeBStart2);
            Vector3.Add(ref edgeBEnd2, ref positionB, out edgeBEnd2);

            Vector3 onA, onB;
            Vector3 offset;
            float dot;
#if ALLOWUNSAFE
            var tempContactData = new BoxContactDataCache();
            unsafe
            {
                var contactDataPointer = &tempContactData.D1;
#else
            contactData = new TinyStructList<BoxContactData>();
#endif

            //Go through the pairs and add any contacts with positive depth that are within the segments' intervals.

            if (GetClosestPointsBetweenSegments(ref edgeAStart1, ref edgeAEnd1, ref edgeBStart1, ref edgeBEnd1, out onA, out onB))
            {
                Vector3.Sub(ref onA, ref onB, out offset);
                Vector3.Dot(ref offset, ref mtd, out dot);
                if (dot < 0) //Distance must be negative.
                {
                    BoxContactData data;
                    data.Position = onA;
                    data.Depth = dot;
                    data.Id = GetContactId(edgeAStart1Id, edgeAEnd1Id, edgeBStart1Id, edgeBEnd1Id);
#if ALLOWUNSAFE
                        contactDataPointer[tempContactData.Count] = data;
                        tempContactData.Count++;
#else
                    contactData.Add(ref data);
#endif
                }

            }
            if (GetClosestPointsBetweenSegments(ref edgeAStart1, ref edgeAEnd1, ref edgeBStart2, ref edgeBEnd2, out onA, out onB))
            {
                Vector3.Sub(ref onA, ref onB, out offset);
                Vector3.Dot(ref offset, ref mtd, out dot);
                if (dot < 0) //Distance must be negative.
                {
                    BoxContactData data;
                    data.Position = onA;
                    data.Depth = dot;
                    data.Id = GetContactId(edgeAStart1Id, edgeAEnd1Id, edgeBStart2Id, edgeBEnd2Id);
#if ALLOWUNSAFE
                        contactDataPointer[tempContactData.Count] = data;
                        tempContactData.Count++;
#else
                    contactData.Add(ref data);
#endif
                }

            }
            if (GetClosestPointsBetweenSegments(ref edgeAStart2, ref edgeAEnd2, ref edgeBStart1, ref edgeBEnd1, out onA, out onB))
            {
                Vector3.Sub(ref onA, ref onB, out offset);
                Vector3.Dot(ref offset, ref mtd, out dot);
                if (dot < 0) //Distance must be negative.
                {
                    BoxContactData data;
                    data.Position = onA;
                    data.Depth = dot;
                    data.Id = GetContactId(edgeAStart2Id, edgeAEnd2Id, edgeBStart1Id, edgeBEnd1Id);
#if ALLOWUNSAFE
                        contactDataPointer[tempContactData.Count] = data;
                        tempContactData.Count++;
#else
                    contactData.Add(ref data);
#endif
                }

            }
            if (GetClosestPointsBetweenSegments(ref edgeAStart2, ref edgeAEnd2, ref edgeBStart2, ref edgeBEnd2, out onA, out onB))
            {
                Vector3.Sub(ref onA, ref onB, out offset);
                Vector3.Dot(ref offset, ref mtd, out dot);
                if (dot < 0) //Distance must be negative.
                {
                    BoxContactData data;
                    data.Position = onA;
                    data.Depth = dot;
                    data.Id = GetContactId(edgeAStart2Id, edgeAEnd2Id, edgeBStart2Id, edgeBEnd2Id);
#if ALLOWUNSAFE
                        contactDataPointer[tempContactData.Count] = data;
                        tempContactData.Count++;
#else
                    contactData.Add(ref data);
#endif
                }

            }
#if ALLOWUNSAFE
            }
            contactData = tempContactData;
#endif

        }

        private static void GetEdgeData(int index, int axis, float x, float y, float z, out Vector3 edgeStart, out Vector3 edgeEnd, out int edgeStartId, out int edgeEndId)
        {
            //Index defines which edge to use.
            //They follow this pattern:
            //0: --
            //1: -+
            //2: +-
            //3: ++

            //The axis index determines the dimensions to use.
            //0: plane with normal X
            //1: plane with normal Y
            //2: plane with normal Z

            edgeStart = new Vector3();
            edgeEnd = new Vector3();

            switch (index + axis * 4)
            {
                case 0:
                    //X--
                    edgeStart.X = -x;
                    edgeStart.Y = -y;
                    edgeStart.Z = -z;
                    edgeStartId = 0; //000

                    edgeEnd.X = x;
                    edgeEnd.Y = -y;
                    edgeEnd.Z = -z;
                    edgeEndId = 4; //100
                    break;
                case 1:
                    //X-+
                    edgeStart.X = -x;
                    edgeStart.Y = -y;
                    edgeStart.Z = z;
                    edgeStartId = 1; //001

                    edgeEnd.X = x;
                    edgeEnd.Y = -y;
                    edgeEnd.Z = z;
                    edgeEndId = 5; //101
                    break;
                case 2:
                    //X+-
                    edgeStart.X = -x;
                    edgeStart.Y = y;
                    edgeStart.Z = -z;
                    edgeStartId = 2; //010

                    edgeEnd.X = x;
                    edgeEnd.Y = y;
                    edgeEnd.Z = -z;
                    edgeEndId = 6; //110
                    break;
                case 3:
                    //X++
                    edgeStart.X = -x;
                    edgeStart.Y = y;
                    edgeStart.Z = z;
                    edgeStartId = 3; //011

                    edgeEnd.X = x;
                    edgeEnd.Y = y;
                    edgeEnd.Z = z;
                    edgeEndId = 7; //111
                    break;
                case 4:
                    //-Y-
                    edgeStart.X = -x;
                    edgeStart.Y = -y;
                    edgeStart.Z = -z;
                    edgeStartId = 0; //000

                    edgeEnd.X = -x;
                    edgeEnd.Y = y;
                    edgeEnd.Z = -z;
                    edgeEndId = 2; //010
                    break;
                case 5:
                    //-Y+
                    edgeStart.X = -x;
                    edgeStart.Y = -y;
                    edgeStart.Z = z;
                    edgeStartId = 1; //001

                    edgeEnd.X = -x;
                    edgeEnd.Y = y;
                    edgeEnd.Z = z;
                    edgeEndId = 3; //011
                    break;
                case 6:
                    //+Y-
                    edgeStart.X = x;
                    edgeStart.Y = -y;
                    edgeStart.Z = -z;
                    edgeStartId = 4; //100

                    edgeEnd.X = x;
                    edgeEnd.Y = y;
                    edgeEnd.Z = -z;
                    edgeEndId = 6; //110
                    break;
                case 7:
                    //+Y+
                    edgeStart.X = x;
                    edgeStart.Y = -y;
                    edgeStart.Z = z;
                    edgeStartId = 5; //101

                    edgeEnd.X = x;
                    edgeEnd.Y = y;
                    edgeEnd.Z = z;
                    edgeEndId = 7; //111
                    break;
                case 8:
                    //--Z
                    edgeStart.X = -x;
                    edgeStart.Y = -y;
                    edgeStart.Z = -z;
                    edgeStartId = 0; //000

                    edgeEnd.X = -x;
                    edgeEnd.Y = -y;
                    edgeEnd.Z = z;
                    edgeEndId = 1; //001
                    break;
                case 9:
                    //-+Z
                    edgeStart.X = -x;
                    edgeStart.Y = y;
                    edgeStart.Z = -z;
                    edgeStartId = 2; //010

                    edgeEnd.X = -x;
                    edgeEnd.Y = y;
                    edgeEnd.Z = z;
                    edgeEndId = 3; //011
                    break;
                case 10:
                    //+-Z
                    edgeStart.X = x;
                    edgeStart.Y = -y;
                    edgeStart.Z = -z;
                    edgeStartId = 4; //100

                    edgeEnd.X = x;
                    edgeEnd.Y = -y;
                    edgeEnd.Z = z;
                    edgeEndId = 5; //101
                    break;
                case 11:
                    //++Z
                    edgeStart.X = x;
                    edgeStart.Y = y;
                    edgeStart.Z = -z;
                    edgeStartId = 6; //110

                    edgeEnd.X = x;
                    edgeEnd.Y = y;
                    edgeEnd.Z = z;
                    edgeEndId = 7; //111
                    break;
                default:
                    throw new Exception("Invalid index or axis.");
            }
        }

        static void FindHighestIndices(ref TinyList<float> dots, out int highestIndex, out int secondHighestIndex)
        {
            highestIndex = 0;
            float highestValue = dots[0];
            for (int i = 1; i < 4; i++)
            {
                float dot = dots[i];
                if (dot > highestValue)
                {
                    highestIndex = i;
                    highestValue = dot;
                }
            }
            secondHighestIndex = 0;
            float secondHighestValue = -float.MaxValue;
            for (int i = 0; i < 4; i++)
            {
                float dot = dots[i];
                if (i != highestIndex && dot > secondHighestValue)
                {
                    secondHighestIndex = i;
                    secondHighestValue = dot;
                }
            }
        }

        /// <summary>
        /// Computes closest points c1 and c2 betwen segments p1q1 and p2q2.
        /// </summary>
        /// <param name="p1">First point of first segment.</param>
        /// <param name="q1">Second point of first segment.</param>
        /// <param name="p2">First point of second segment.</param>
        /// <param name="q2">Second point of second segment.</param>
        /// <param name="c1">Closest point on first segment.</param>
        /// <param name="c2">Closest point on second segment.</param>
        static bool GetClosestPointsBetweenSegments(ref Vector3 p1, ref Vector3 q1, ref Vector3 p2, ref Vector3 q2,
                                                           out Vector3 c1, out Vector3 c2)
        {
            //Segment direction vectors
            Vector3 d1;
            Vector3.Sub(ref q1, ref p1, out d1);
            Vector3 d2;
            Vector3.Sub(ref q2, ref p2, out d2);
            Vector3 r;
            Vector3.Sub(ref p1, ref p2, out r);
            //distance
            float a = d1.LengthSquared();
            float e = d2.LengthSquared();
            float f;
            Vector3.Dot(ref d2, ref r, out f);

            float s, t;

            if (a <= Toolbox.Epsilon && e <= Toolbox.Epsilon)
            {
                //These segments are more like points.
                c1 = p1;
                c2 = p2;
                return false;
            }
            if (a <= Toolbox.Epsilon)
            {
                // First segment is basically a point.
                s = 0.0f;
                t = f / e;
                if (t < 0 || t > 1)
                {
                    c1 = new Vector3();
                    c2 = new Vector3();
                    return false;
                }
            }
            else
            {
                float c = d1.Dot(r);
                if (e <= Toolbox.Epsilon)
                {
                    // Second segment is basically a point.
                    t = 0.0f;
                    s = MathUtilities.Clamp(-c / a, 0.0f, 1.0f);
                }
                else
                {
                    float b = d1.Dot(d2);
                    float denom = a * e - b * b;

                    // If segments not parallel, compute closest point on L1 to L2, and
                    // clamp to segment S1. Else pick some s (here .5f)
                    if (denom != 0.0f)
                    {
                        s = (b * f - c * e) / denom;
                        if (s < 0 || s > 1)
                        {
                            //Closest point would be outside of the segment.
                            c1 = new Vector3();
                            c2 = new Vector3();
                            return false;
                        }
                    }
                    else //Parallel, just use .5f
                        s = .5f;


                    t = (b * s + f) / e;

                    if (t < 0 || t > 1)
                    {
                        //Closest point would be outside of the segment.
                        c1 = new Vector3();
                        c2 = new Vector3();
                        return false;
                    }
                }
            }

            Vector3.Mul(ref d1, s, out c1);
            Vector3.Add(ref c1, ref p1, out c1);
            Vector3.Mul(ref d2, t, out c2);
            Vector3.Add(ref c2, ref p2, out c2);
            return true;
        }

        //        internal static void GetEdgeEdgeContact(BoxShape a, BoxShape b, ref Vector3 positionA, ref Matrix3 orientationA, ref Vector3 positionB, ref Matrix3 orientationB, float depth, ref Vector3 mtd, out TinyStructList<BoxContactData> contactData)
        //        {
        //            //Put the minimum translation direction into the local space of each object.
        //            Vector3 mtdA, mtdB;
        //            Vector3 negatedMtd;
        //            Vector3.Neg(ref mtd, out negatedMtd);
        //            Vector3.TransformTranspose(ref negatedMtd, ref orientationA, out mtdA);
        //            Vector3.TransformTranspose(ref mtd, ref orientationB, out mtdB);


        //#if !WINDOWS
        //            Vector3 edgeA1 = new Vector3(), edgeA2 = new Vector3();
        //            Vector3 edgeB1 = new Vector3(), edgeB2 = new Vector3();
        //#else
        //            Vector3 edgeA1, edgeA2;
        //            Vector3 edgeB1, edgeB2;
        //#endif
        //            float aHalfWidth = a.halfWidth;
        //            float aHalfHeight = a.halfHeight;
        //            float aHalfLength = a.halfLength;

        //            float bHalfWidth = b.halfWidth;
        //            float bHalfHeight = b.halfHeight;
        //            float bHalfLength = b.halfLength;

        //            int edgeA1Id, edgeA2Id;
        //            int edgeB1Id, edgeB2Id;

        //            //This is an edge-edge collision, so one (AND ONLY ONE) of the components in the 
        //            //local direction must be very close to zero.  We can use an arbitrary fixed 
        //            //epsilon because the mtd is always unit length.

        //            #region Edge A

        //            if (Math.Abs(mtdA.X) < Toolbox.Epsilon)
        //            {
        //                //mtd is in the Y-Z plane.
        //                if (mtdA.Y > 0)
        //                {
        //                    if (mtdA.Z > 0)
        //                    {
        //                        //++
        //                        edgeA1.X = -aHalfWidth;
        //                        edgeA1.Y = aHalfHeight;
        //                        edgeA1.Z = aHalfLength;

        //                        edgeA2.X = aHalfWidth;
        //                        edgeA2.Y = aHalfHeight;
        //                        edgeA2.Z = aHalfLength;

        //                        edgeA1Id = 6;
        //                        edgeA2Id = 7;
        //                    }
        //                    else
        //                    {
        //                        //+-
        //                        edgeA1.X = -aHalfWidth;
        //                        edgeA1.Y = aHalfHeight;
        //                        edgeA1.Z = -aHalfLength;

        //                        edgeA2.X = aHalfWidth;
        //                        edgeA2.Y = aHalfHeight;
        //                        edgeA2.Z = -aHalfLength;

        //                        edgeA1Id = 2;
        //                        edgeA2Id = 3;
        //                    }
        //                }
        //                else
        //                {
        //                    if (mtdA.Z > 0)
        //                    {
        //                        //-+
        //                        edgeA1.X = -aHalfWidth;
        //                        edgeA1.Y = -aHalfHeight;
        //                        edgeA1.Z = aHalfLength;

        //                        edgeA2.X = aHalfWidth;
        //                        edgeA2.Y = -aHalfHeight;
        //                        edgeA2.Z = aHalfLength;

        //                        edgeA1Id = 4;
        //                        edgeA2Id = 5;
        //                    }
        //                    else
        //                    {
        //                        //--
        //                        edgeA1.X = -aHalfWidth;
        //                        edgeA1.Y = -aHalfHeight;
        //                        edgeA1.Z = -aHalfLength;

        //                        edgeA2.X = aHalfWidth;
        //                        edgeA2.Y = -aHalfHeight;
        //                        edgeA2.Z = -aHalfLength;

        //                        edgeA1Id = 0;
        //                        edgeA2Id = 1;
        //                    }
        //                }
        //            }
        //            else if (Math.Abs(mtdA.Y) < Toolbox.Epsilon)
        //            {
        //                //mtd is in the X-Z plane
        //                if (mtdA.X > 0)
        //                {
        //                    if (mtdA.Z > 0)
        //                    {
        //                        //++
        //                        edgeA1.X = aHalfWidth;
        //                        edgeA1.Y = -aHalfHeight;
        //                        edgeA1.Z = aHalfLength;

        //                        edgeA2.X = aHalfWidth;
        //                        edgeA2.Y = aHalfHeight;
        //                        edgeA2.Z = aHalfLength;

        //                        edgeA1Id = 5;
        //                        edgeA2Id = 7;
        //                    }
        //                    else
        //                    {
        //                        //+-
        //                        edgeA1.X = aHalfWidth;
        //                        edgeA1.Y = -aHalfHeight;
        //                        edgeA1.Z = -aHalfLength;

        //                        edgeA2.X = aHalfWidth;
        //                        edgeA2.Y = aHalfHeight;
        //                        edgeA2.Z = -aHalfLength;

        //                        edgeA1Id = 1;
        //                        edgeA2Id = 3;
        //                    }
        //                }
        //                else
        //                {
        //                    if (mtdA.Z > 0)
        //                    {
        //                        //-+
        //                        edgeA1.X = -aHalfWidth;
        //                        edgeA1.Y = -aHalfHeight;
        //                        edgeA1.Z = aHalfLength;

        //                        edgeA2.X = -aHalfWidth;
        //                        edgeA2.Y = aHalfHeight;
        //                        edgeA2.Z = aHalfLength;

        //                        edgeA1Id = 4;
        //                        edgeA2Id = 6;
        //                    }
        //                    else
        //                    {
        //                        //--
        //                        edgeA1.X = -aHalfWidth;
        //                        edgeA1.Y = -aHalfHeight;
        //                        edgeA1.Z = -aHalfLength;

        //                        edgeA2.X = -aHalfWidth;
        //                        edgeA2.Y = aHalfHeight;
        //                        edgeA2.Z = -aHalfLength;

        //                        edgeA1Id = 0;
        //                        edgeA2Id = 2;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //mtd is in the X-Y plane
        //                if (mtdA.X > 0)
        //                {
        //                    if (mtdA.Y > 0)
        //                    {
        //                        //++
        //                        edgeA1.X = aHalfWidth;
        //                        edgeA1.Y = aHalfHeight;
        //                        edgeA1.Z = -aHalfLength;

        //                        edgeA2.X = aHalfWidth;
        //                        edgeA2.Y = aHalfHeight;
        //                        edgeA2.Z = aHalfLength;

        //                        edgeA1Id = 3;
        //                        edgeA2Id = 7;
        //                    }
        //                    else
        //                    {
        //                        //+-
        //                        edgeA1.X = aHalfWidth;
        //                        edgeA1.Y = -aHalfHeight;
        //                        edgeA1.Z = -aHalfLength;

        //                        edgeA2.X = aHalfWidth;
        //                        edgeA2.Y = -aHalfHeight;
        //                        edgeA2.Z = aHalfLength;

        //                        edgeA1Id = 1;
        //                        edgeA2Id = 5;
        //                    }
        //                }
        //                else
        //                {
        //                    if (mtdA.Y > 0)
        //                    {
        //                        //-+
        //                        edgeA1.X = -aHalfWidth;
        //                        edgeA1.Y = aHalfHeight;
        //                        edgeA1.Z = -aHalfLength;

        //                        edgeA2.X = -aHalfWidth;
        //                        edgeA2.Y = aHalfHeight;
        //                        edgeA2.Z = aHalfLength;

        //                        edgeA1Id = 2;
        //                        edgeA2Id = 6;
        //                    }
        //                    else
        //                    {
        //                        //--
        //                        edgeA1.X = -aHalfWidth;
        //                        edgeA1.Y = -aHalfHeight;
        //                        edgeA1.Z = -aHalfLength;

        //                        edgeA2.X = -aHalfWidth;
        //                        edgeA2.Y = -aHalfHeight;
        //                        edgeA2.Z = aHalfLength;

        //                        edgeA1Id = 0;
        //                        edgeA2Id = 4;
        //                    }
        //                }
        //            }

        //            #endregion

        //            #region Edge B

        //            if (Math.Abs(mtdB.X) < Toolbox.Epsilon)
        //            {
        //                //mtd is in the Y-Z plane.
        //                if (mtdB.Y > 0)
        //                {
        //                    if (mtdB.Z > 0)
        //                    {
        //                        //++
        //                        edgeB1.X = -bHalfWidth;
        //                        edgeB1.Y = bHalfHeight;
        //                        edgeB1.Z = bHalfLength;

        //                        edgeB2.X = bHalfWidth;
        //                        edgeB2.Y = bHalfHeight;
        //                        edgeB2.Z = bHalfLength;

        //                        edgeB1Id = 6;
        //                        edgeB2Id = 7;
        //                    }
        //                    else
        //                    {
        //                        //+-
        //                        edgeB1.X = -bHalfWidth;
        //                        edgeB1.Y = bHalfHeight;
        //                        edgeB1.Z = -bHalfLength;

        //                        edgeB2.X = bHalfWidth;
        //                        edgeB2.Y = bHalfHeight;
        //                        edgeB2.Z = -bHalfLength;

        //                        edgeB1Id = 2;
        //                        edgeB2Id = 3;
        //                    }
        //                }
        //                else
        //                {
        //                    if (mtdB.Z > 0)
        //                    {
        //                        //-+
        //                        edgeB1.X = -bHalfWidth;
        //                        edgeB1.Y = -bHalfHeight;
        //                        edgeB1.Z = bHalfLength;

        //                        edgeB2.X = bHalfWidth;
        //                        edgeB2.Y = -bHalfHeight;
        //                        edgeB2.Z = bHalfLength;

        //                        edgeB1Id = 4;
        //                        edgeB2Id = 5;
        //                    }
        //                    else
        //                    {
        //                        //--
        //                        edgeB1.X = -bHalfWidth;
        //                        edgeB1.Y = -bHalfHeight;
        //                        edgeB1.Z = -bHalfLength;

        //                        edgeB2.X = bHalfWidth;
        //                        edgeB2.Y = -bHalfHeight;
        //                        edgeB2.Z = -bHalfLength;

        //                        edgeB1Id = 0;
        //                        edgeB2Id = 1;
        //                    }
        //                }
        //            }
        //            else if (Math.Abs(mtdB.Y) < Toolbox.Epsilon)
        //            {
        //                //mtd is in the X-Z plane
        //                if (mtdB.X > 0)
        //                {
        //                    if (mtdB.Z > 0)
        //                    {
        //                        //++
        //                        edgeB1.X = bHalfWidth;
        //                        edgeB1.Y = -bHalfHeight;
        //                        edgeB1.Z = bHalfLength;

        //                        edgeB2.X = bHalfWidth;
        //                        edgeB2.Y = bHalfHeight;
        //                        edgeB2.Z = bHalfLength;

        //                        edgeB1Id = 5;
        //                        edgeB2Id = 7;
        //                    }
        //                    else
        //                    {
        //                        //+-
        //                        edgeB1.X = bHalfWidth;
        //                        edgeB1.Y = -bHalfHeight;
        //                        edgeB1.Z = -bHalfLength;

        //                        edgeB2.X = bHalfWidth;
        //                        edgeB2.Y = bHalfHeight;
        //                        edgeB2.Z = -bHalfLength;

        //                        edgeB1Id = 1;
        //                        edgeB2Id = 3;
        //                    }
        //                }
        //                else
        //                {
        //                    if (mtdB.Z > 0)
        //                    {
        //                        //-+
        //                        edgeB1.X = -bHalfWidth;
        //                        edgeB1.Y = -bHalfHeight;
        //                        edgeB1.Z = bHalfLength;

        //                        edgeB2.X = -bHalfWidth;
        //                        edgeB2.Y = bHalfHeight;
        //                        edgeB2.Z = bHalfLength;

        //                        edgeB1Id = 4;
        //                        edgeB2Id = 6;
        //                    }
        //                    else
        //                    {
        //                        //--
        //                        edgeB1.X = -bHalfWidth;
        //                        edgeB1.Y = -bHalfHeight;
        //                        edgeB1.Z = -bHalfLength;

        //                        edgeB2.X = -bHalfWidth;
        //                        edgeB2.Y = bHalfHeight;
        //                        edgeB2.Z = -bHalfLength;

        //                        edgeB1Id = 0;
        //                        edgeB2Id = 2;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //mtd is in the X-Y plane
        //                if (mtdB.X > 0)
        //                {
        //                    if (mtdB.Y > 0)
        //                    {
        //                        //++
        //                        edgeB1.X = bHalfWidth;
        //                        edgeB1.Y = bHalfHeight;
        //                        edgeB1.Z = -bHalfLength;

        //                        edgeB2.X = bHalfWidth;
        //                        edgeB2.Y = bHalfHeight;
        //                        edgeB2.Z = bHalfLength;

        //                        edgeB1Id = 3;
        //                        edgeB2Id = 7;
        //                    }
        //                    else
        //                    {
        //                        //+-
        //                        edgeB1.X = bHalfWidth;
        //                        edgeB1.Y = -bHalfHeight;
        //                        edgeB1.Z = -bHalfLength;

        //                        edgeB2.X = bHalfWidth;
        //                        edgeB2.Y = -bHalfHeight;
        //                        edgeB2.Z = bHalfLength;

        //                        edgeB1Id = 1;
        //                        edgeB2Id = 5;
        //                    }
        //                }
        //                else
        //                {
        //                    if (mtdB.Y > 0)
        //                    {
        //                        //-+
        //                        edgeB1.X = -bHalfWidth;
        //                        edgeB1.Y = bHalfHeight;
        //                        edgeB1.Z = -bHalfLength;

        //                        edgeB2.X = -bHalfWidth;
        //                        edgeB2.Y = bHalfHeight;
        //                        edgeB2.Z = bHalfLength;

        //                        edgeB1Id = 2;
        //                        edgeB2Id = 6;
        //                    }
        //                    else
        //                    {
        //                        //--
        //                        edgeB1.X = -bHalfWidth;
        //                        edgeB1.Y = -bHalfHeight;
        //                        edgeB1.Z = -bHalfLength;

        //                        edgeB2.X = -bHalfWidth;
        //                        edgeB2.Y = -bHalfHeight;
        //                        edgeB2.Z = bHalfLength;

        //                        edgeB1Id = 0;
        //                        edgeB2Id = 4;
        //                    }
        //                }
        //            }

        //            #endregion

        //            //TODO: Since the above uniquely identifies the edge from each box based on two vertices,
        //            //get the edge feature id from vertexA id combined with vertexB id.
        //            //Vertex id's are 3 bit binary 'numbers' because ---, --+, -+-, etc.


        //            Vector3.Transform(ref edgeA1, ref orientationA, out edgeA1);
        //            Vector3.Transform(ref edgeA2, ref orientationA, out edgeA2);
        //            Vector3.Transform(ref edgeB1, ref orientationB, out edgeB1);
        //            Vector3.Transform(ref edgeB2, ref orientationB, out edgeB2);
        //            Vector3.Add(ref edgeA1, ref positionA, out edgeA1);
        //            Vector3.Add(ref edgeA2, ref positionA, out edgeA2);
        //            Vector3.Add(ref edgeB1, ref positionB, out edgeB1);
        //            Vector3.Add(ref edgeB2, ref positionB, out edgeB2);

        //            float s, t;
        //            Vector3 onA, onB;
        //            Toolbox.GetClosestPointsBetweenSegments(ref edgeA1, ref edgeA2, ref edgeB1, ref edgeB2, out s, out t, out onA, out onB);
        //            //Vector3.Add(ref onA, ref onB, out point);
        //            //Vector3.Mul(ref point, .5f, out point);
        //            point = onA;

        //            //depth = (onB.X - onA.X) * mtd.X + (onB.Y - onA.Y) * mtd.Y + (onB.Z - onA.Z) * mtd.Z;

        //            id = GetContactId(edgeA1Id, edgeA2Id, edgeB1Id, edgeB2Id);
        //        }

#if ALLOWUNSAFE
        internal static void GetFaceContacts(BoxShape a, BoxShape b, ref Vector3 positionA, ref Matrix3 orientationA, ref Vector3 positionB, ref Matrix3 orientationB, bool aIsFaceOwner, ref Vector3 mtd, out BoxContactDataCache contactData)
#else
        internal static void GetFaceContacts(BoxShape a, BoxShape b, ref Vector3 positionA, ref Matrix3 orientationA, ref Vector3 positionB, ref Matrix3 orientationB, bool aIsFaceOwner, ref Vector3 mtd, out TinyStructList<BoxContactData> contactData)
#endif
        {
            float aHalfWidth = a.halfWidth;
            float aHalfHeight = a.halfHeight;
            float aHalfLength = a.halfLength;

            float bHalfWidth = b.halfWidth;
            float bHalfHeight = b.halfHeight;
            float bHalfLength = b.halfLength;

            BoxFace aBoxFace, bBoxFace;

            Vector3 negatedMtd;
            Vector3.Neg(ref mtd, out negatedMtd);
            GetNearestFace(ref positionA, ref orientationA, ref negatedMtd, aHalfWidth, aHalfHeight, aHalfLength, out aBoxFace);


            GetNearestFace(ref positionB, ref orientationB, ref mtd, bHalfWidth, bHalfHeight, bHalfLength, out bBoxFace);

            if (aIsFaceOwner)
                ClipFacesDirect(ref aBoxFace, ref bBoxFace, ref negatedMtd, out contactData);
            else
                ClipFacesDirect(ref bBoxFace, ref aBoxFace, ref mtd, out contactData);

            if (contactData.Count > 4)
                PruneContactsMaxDistance(ref mtd, contactData, out contactData);
        }

#if ALLOWUNSAFE
        private static unsafe void PruneContactsMaxDistance(ref Vector3 mtd, BoxContactDataCache input, out BoxContactDataCache output)
        {
            BoxContactData* data = &input.D1;
            int count = input.Count;
            //TODO: THE FOLLOWING has a small issue in release mode.
            //Find the deepest point.
            float deepestDepth = -1;
            int deepestIndex = 0;
            for (int i = 0; i < count; i++)
            {
                if (data[i].Depth > deepestDepth)
                {
                    deepestDepth = data[i].Depth;
                    deepestIndex = i;
                }
            }

            //Identify the furthest point away from the deepest index.
            float furthestDistance = -1;
            int furthestIndex = 0;
            for (int i = 0; i < count; i++)
            {
                float distance;
                Vector3.DistanceSquared(ref data[deepestIndex].Position, ref data[i].Position, out distance);
                if (distance > furthestDistance)
                {
                    furthestDistance = distance;
                    furthestIndex = i;
                }

            }

            Vector3 xAxis;
            Vector3.Sub(ref data[furthestIndex].Position, ref data[deepestIndex].Position, out xAxis);

            Vector3 yAxis;
            Vector3.Cross(ref mtd, ref xAxis, out yAxis);

            float minY;
            float maxY;
            int minYindex = 0;
            int maxYindex = 0;

            Vector3.Dot(ref data[0].Position, ref yAxis, out minY);
            maxY = minY;
            for (int i = 1; i < count; i++)
            {
                float dot;
                Vector3.Dot(ref yAxis, ref data[i].Position, out dot);
                if (dot < minY)
                {
                    minY = dot;
                    minYindex = i;
                }
                else if (dot > maxY)
                {
                    maxY = dot;
                    maxYindex = i;
                }
            }

            output = new BoxContactDataCache
                         {
                             Count = 4,
                             D1 = data[deepestIndex],
                             D2 = data[furthestIndex],
                             D3 = data[minYindex],
                             D4 = data[maxYindex]
                         };


            //Vector3 v;
            //var maximumOffset = new Vector3();
            //int maxIndexA = -1, maxIndexB = -1;
            //float temp;
            //float maximumDistanceSquared = -float.MaxValue;
            //for (int i = 0; i < count; i++)
            //{
            //    for (int j = i + 1; j < count; j++)
            //    {
            //        Vector3.Sub(ref data[j].Position, ref data[i].Position, out v);
            //        temp = v.LengthSquared();
            //        if (temp > maximumDistanceSquared)
            //        {
            //            maximumDistanceSquared = temp;
            //            maxIndexA = i;
            //            maxIndexB = j;
            //            maximumOffset = v;
            //        }
            //    }
            //}

            //Vector3 otherDirection;
            //Vector3.Cross(ref mtd, ref maximumOffset, out otherDirection);
            //int minimumIndex = -1, maximumIndex = -1;
            //float minimumDistance = float.MaxValue, maximumDistance = -float.MaxValue;

            //for (int i = 0; i < count; i++)
            //{
            //    if (i != maxIndexA && i != maxIndexB)
            //    {
            //        Vector3.Dot(ref data[i].Position, ref otherDirection, out temp);
            //        if (temp > maximumDistance)
            //        {
            //            maximumDistance = temp;
            //            maximumIndex = i;
            //        }
            //        if (temp < minimumDistance)
            //        {
            //            minimumDistance = temp;
            //            minimumIndex = i;
            //        }
            //    }
            //}

            //output = new BoxContactDataCache();
            //output.Count = 4;
            //output.D1 = data[maxIndexA];
            //output.D2 = data[maxIndexB];
            //output.D3 = data[minimumIndex];
            //output.D4 = data[maximumIndex];
        }
#else
        private static void PruneContactsMaxDistance(ref Vector3 mtd, TinyStructList<BoxContactData> input, out TinyStructList<BoxContactData> output)
        {
            int count = input.Count;
            //Find the deepest point.
            BoxContactData data, deepestData;
            input.Get(0, out deepestData);
            for (int i = 1; i < count; i++)
            {
                input.Get(i, out data);
                if (data.Depth > deepestData.Depth)
                {
                    deepestData = data;
                }
            }

            //Identify the furthest point away from the deepest index.
            BoxContactData furthestData;
            input.Get(0, out furthestData);
            float furthestDistance;
            Vector3.DistanceSquared(ref deepestData.Position, ref furthestData.Position, out furthestDistance);
            for (int i = 1; i < count; i++)
            {
                input.Get(i, out data);
                float distance;
                Vector3.DistanceSquared(ref deepestData.Position, ref data.Position, out distance);
                if (distance > furthestDistance)
                {
                    furthestDistance = distance;
                    furthestData = data;
                }

            }

            Vector3 xAxis;
            Vector3.Sub(ref furthestData.Position, ref deepestData.Position, out xAxis);

            Vector3 yAxis;
            Vector3.Cross(ref mtd, ref xAxis, out yAxis);

            float minY;
            float maxY;
            BoxContactData minData, maxData;
            input.Get(0, out minData);
            maxData = minData;

            Vector3.Dot(ref minData.Position, ref yAxis, out minY);
            maxY = minY;
            for (int i = 1; i < count; i++)
            {
                input.Get(i, out data);
                float dot;
                Vector3.Dot(ref yAxis, ref data.Position, out dot);
                if (dot < minY)
                {
                    minY = dot;
                    minData = data;
                }
                else if (dot > maxY)
                {
                    maxY = dot;
                    maxData = data;
                }
            }

            output = new TinyStructList<BoxContactData>();
            output.Add(ref deepestData);
            output.Add(ref furthestData);
            output.Add(ref minData);
            output.Add(ref maxData);


            //int count = input.Count;
            //Vector3 v;
            //var maximumOffset = new Vector3();
            //int maxIndexA = -1, maxIndexB = -1;
            //float temp;
            //float maximumDistanceSquared = -float.MaxValue;
            //BoxContactData itemA, itemB;
            //for (int i = 0; i < count; i++)
            //{
            //    for (int j = i + 1; j < count; j++)
            //    {
            //        input.Get(j, out itemB);
            //        input.Get(i, out itemA);
            //        Vector3.Sub(ref itemB.Position, ref itemA.Position, out v);
            //        temp = v.LengthSquared();
            //        if (temp > maximumDistanceSquared)
            //        {
            //            maximumDistanceSquared = temp;
            //            maxIndexA = i;
            //            maxIndexB = j;
            //            maximumOffset = v;
            //        }
            //    }
            //}

            //Vector3 otherDirection;
            //Vector3.Cross(ref mtd, ref maximumOffset, out otherDirection);
            //int minimumIndex = -1, maximumIndex = -1;
            //float minimumDistance = float.MaxValue, maximumDistance = -float.MaxValue;

            //for (int i = 0; i < count; i++)
            //{
            //    if (i != maxIndexA && i != maxIndexB)
            //    {
            //        input.Get(i, out itemA);
            //        Vector3.Dot(ref itemA.Position, ref otherDirection, out temp);
            //        if (temp > maximumDistance)
            //        {
            //            maximumDistance = temp;
            //            maximumIndex = i;
            //        }
            //        if (temp < minimumDistance)
            //        {
            //            minimumDistance = temp;
            //            minimumIndex = i;
            //        }
            //    }
            //}

            //output = new TinyStructList<BoxContactData>();
            //input.Get(maxIndexA, out itemA);
            //output.Add(ref itemA);
            //input.Get(maxIndexB, out itemA);
            //output.Add(ref itemA);
            //input.Get(minimumIndex, out itemA);
            //output.Add(ref itemA);
            //input.Get(maximumIndex, out itemA);
            //output.Add(ref itemA);
        }
#endif
#if EXCLUDED
        private static unsafe void clipFacesSH(ref BoxFace clipFace, ref BoxFace face, ref Vector3 mtd, out BoxContactDataCache outputData)
        {
            BoxContactDataCache contactDataCache = new BoxContactDataCache();
            BoxContactData* data = &contactDataCache.d1;

            //Set up the initial face list.
            data[0].position = face.v1;
            data[0].id = face.id1;
            data[1].position = face.v2;
            data[1].id = face.id2;
            data[2].position = face.v3;
            data[2].id = face.id3;
            data[3].position = face.v4;
            data[3].id = face.id4;
            contactDataCache.count = 4;

            BoxContactDataCache temporaryCache;
            BoxContactData* temp = &temporaryCache.d1;
            FaceEdge clippingEdge;
            Vector3 intersection;
            for (int i = 0; i < 4; i++)
            {//For each clipping edge (edges of face a)

                clipFace.GetEdge(i, ref mtd, out clippingEdge);

                temporaryCache = contactDataCache;

                contactDataCache.count = 0;

                Vector3 start = temp[temporaryCache.count - 1].position;
                int startId = temp[temporaryCache.count - 1].id;


                for (int j = 0; j < temporaryCache.count; j++)
                {//For each point in the input list
                    Vector3 end = temp[j].position;
                    int endId = temp[j].id;
                    if (clippingEdge.isPointInside(ref end))
                    {
                        if (!clippingEdge.isPointInside(ref start))
                        {
                            ComputeIntersection(ref start, ref end, ref mtd, ref clippingEdge, out intersection);
                            if (contactDataCache.count < 8)
                            {
                                data[contactDataCache.count].position = intersection;
                                data[contactDataCache.count].id = GetContactId(startId, endId, ref clippingEdge);
                                contactDataCache.count++;
                            }
                            else
                            {
                                data[contactDataCache.count - 1].position = intersection;
                                data[contactDataCache.count - 1].id = GetContactId(startId, endId, ref clippingEdge);
                            }
                        }
                        if (contactDataCache.count < 8)
                        {
                            data[contactDataCache.count].position = end;
                            data[contactDataCache.count].id = endId;
                            contactDataCache.count++;
                        }
                        else
                        {
                            data[contactDataCache.count - 1].position = end;
                            data[contactDataCache.count - 1].id = endId;
                        }
                    }
                    else if (clippingEdge.isPointInside(ref start))
                    {
                        ComputeIntersection(ref start, ref end, ref mtd, ref clippingEdge, out intersection);
                        if (contactDataCache.count < 8)
                        {
                            data[contactDataCache.count].position = intersection;
                            data[contactDataCache.count].id = GetContactId(startId, endId, ref clippingEdge);
                            contactDataCache.count++;
                        }
                        else
                        {
                            data[contactDataCache.count - 1].position = intersection;
                            data[contactDataCache.count - 1].id = GetContactId(startId, endId, ref clippingEdge);
                        }
                    }
                    start = end;
                    startId = endId;
                }
            }
            temporaryCache = contactDataCache;
            contactDataCache.count = 0;

            float depth;
            float a, b;
            Vector3.Dot(ref clipFace.v1, ref mtd, out a);
            for (int i = 0; i < temporaryCache.count; i++)
            {
                Vector3.Dot(ref temp[i].position, ref mtd, out b);
                depth = b - a;
                if (depth <= 0)
                {
                    data[contactDataCache.count].position = temp[i].position;
                    data[contactDataCache.count].id = temp[i].id;
                    contactDataCache.count++;
                }
            }

            outputData = contactDataCache;

            /*
             * 
  List outputList = subjectPolygon;
  for (Edge clipEdge in clipPolygon) do
     List inputList = outputList;
     outputList.clear();
     Point S = inputList.last;
     for (Point E in inputList) do
        if (E inside clipEdge) then
           if (S not inside clipEdge) then
              outputList.add(ComputeIntersection(S,E,clipEdge));
           end if
           outputList.add(E);
        else if (S inside clipEdge) then
           outputList.add(ComputeIntersection(S,E,clipEdge));
        end if
        S = E;
     done
  done
             */

        }
#endif

#if ALLOWUNSAFE
        private static unsafe void ClipFacesDirect(ref BoxFace clipFace, ref BoxFace face, ref Vector3 mtd, out BoxContactDataCache outputData)
        {
            var contactData = new BoxContactDataCache();
            BoxContactDataCache tempData; //Local version.
            BoxContactData* data = &contactData.D1;
            BoxContactData* temp = &tempData.D1;

            //Local directions on the clip face.  Their length is equal to the length of an edge.
            Vector3 clipX, clipY;
            Vector3.Sub(ref clipFace.V4, ref clipFace.V3, out clipX);
            Vector3.Sub(ref clipFace.V2, ref clipFace.V3, out clipY);
            float inverseClipWidth = 1 / clipFace.Width;
            float inverseClipHeight = 1 / clipFace.Height;
            float inverseClipWidthSquared = inverseClipWidth * inverseClipWidth;
            clipX.X *= inverseClipWidthSquared;
            clipX.Y *= inverseClipWidthSquared;
            clipX.Z *= inverseClipWidthSquared;
            float inverseClipHeightSquared = inverseClipHeight * inverseClipHeight;
            clipY.X *= inverseClipHeightSquared;
            clipY.Y *= inverseClipHeightSquared;
            clipY.Z *= inverseClipHeightSquared;

            //Local directions on the opposing face.  Their length is equal to the length of an edge.
            Vector3 faceX, faceY;
            Vector3.Sub(ref face.V4, ref face.V3, out faceX);
            Vector3.Sub(ref face.V2, ref face.V3, out faceY);
            float inverseFaceWidth = 1 / face.Width;
            float inverseFaceHeight = 1 / face.Height;
            float inverseFaceWidthSquared = inverseFaceWidth * inverseFaceWidth;
            faceX.X *= inverseFaceWidthSquared;
            faceX.Y *= inverseFaceWidthSquared;
            faceX.Z *= inverseFaceWidthSquared;
            float inverseFaceHeightSquared = inverseFaceHeight * inverseFaceHeight;
            faceY.X *= inverseFaceHeightSquared;
            faceY.Y *= inverseFaceHeightSquared;
            faceY.Z *= inverseFaceHeightSquared;

            Vector3 clipCenter;
            Vector3.Add(ref clipFace.V1, ref clipFace.V3, out clipCenter);
            //Defer division until after dot product (2 multiplies instead of 3)
            float clipCenterX, clipCenterY;
            Vector3.Dot(ref clipCenter, ref clipX, out clipCenterX);
            Vector3.Dot(ref clipCenter, ref clipY, out clipCenterY);
            clipCenterX *= .5f;
            clipCenterY *= .5f;

            Vector3 faceCenter;
            Vector3.Add(ref face.V1, ref face.V3, out faceCenter);
            //Defer division until after dot product (2 multiplies instead of 3)
            float faceCenterX, faceCenterY;
            Vector3.Dot(ref faceCenter, ref faceX, out faceCenterX);
            Vector3.Dot(ref faceCenter, ref faceY, out faceCenterY);
            faceCenterX *= .5f;
            faceCenterY *= .5f;

            //To test bounds, recall that clipX is the length of the X edge.
            //Going from the center to the max or min goes half of the length of X edge, or +/- 0.5.
            //Bias could be added here.
            //const float extent = .5f; //.5f is the default, extra could be added for robustness or speed.
            float extentX = .5f + .01f * inverseClipWidth;
            float extentY = .5f + .01f * inverseClipHeight;
            //float extentX = .5f + .01f * inverseClipXLength;
            //float extentY = .5f + .01f * inverseClipYLength;
            float clipCenterMaxX = clipCenterX + extentX;
            float clipCenterMaxY = clipCenterY + extentY;
            float clipCenterMinX = clipCenterX - extentX;
            float clipCenterMinY = clipCenterY - extentY;

            extentX = .5f + .01f * inverseFaceWidth;
            extentY = .5f + .01f * inverseFaceHeight;
            //extentX = .5f + .01f * inverseFaceXLength;
            //extentY = .5f + .01f * inverseFaceYLength;
            float faceCenterMaxX = faceCenterX + extentX;
            float faceCenterMaxY = faceCenterY + extentY;
            float faceCenterMinX = faceCenterX - extentX;
            float faceCenterMinY = faceCenterY - extentY;

            //Find out where the opposing face is.
            float dotX, dotY;

            //The four edges can be thought of as minX, maxX, minY and maxY.

            //Face v1
            Vector3.Dot(ref clipX, ref face.V1, out dotX);
            bool v1MaxXInside = dotX < clipCenterMaxX;
            bool v1MinXInside = dotX > clipCenterMinX;
            Vector3.Dot(ref clipY, ref face.V1, out dotY);
            bool v1MaxYInside = dotY < clipCenterMaxY;
            bool v1MinYInside = dotY > clipCenterMinY;

            //Face v2
            Vector3.Dot(ref clipX, ref face.V2, out dotX);
            bool v2MaxXInside = dotX < clipCenterMaxX;
            bool v2MinXInside = dotX > clipCenterMinX;
            Vector3.Dot(ref clipY, ref face.V2, out dotY);
            bool v2MaxYInside = dotY < clipCenterMaxY;
            bool v2MinYInside = dotY > clipCenterMinY;

            //Face v3
            Vector3.Dot(ref clipX, ref face.V3, out dotX);
            bool v3MaxXInside = dotX < clipCenterMaxX;
            bool v3MinXInside = dotX > clipCenterMinX;
            Vector3.Dot(ref clipY, ref face.V3, out dotY);
            bool v3MaxYInside = dotY < clipCenterMaxY;
            bool v3MinYInside = dotY > clipCenterMinY;

            //Face v4
            Vector3.Dot(ref clipX, ref face.V4, out dotX);
            bool v4MaxXInside = dotX < clipCenterMaxX;
            bool v4MinXInside = dotX > clipCenterMinX;
            Vector3.Dot(ref clipY, ref face.V4, out dotY);
            bool v4MaxYInside = dotY < clipCenterMaxY;
            bool v4MinYInside = dotY > clipCenterMinY;

            //Find out where the clip face is.
            //Clip v1
            Vector3.Dot(ref faceX, ref clipFace.V1, out dotX);
            bool clipv1MaxXInside = dotX < faceCenterMaxX;
            bool clipv1MinXInside = dotX > faceCenterMinX;
            Vector3.Dot(ref faceY, ref clipFace.V1, out dotY);
            bool clipv1MaxYInside = dotY < faceCenterMaxY;
            bool clipv1MinYInside = dotY > faceCenterMinY;

            //Clip v2
            Vector3.Dot(ref faceX, ref clipFace.V2, out dotX);
            bool clipv2MaxXInside = dotX < faceCenterMaxX;
            bool clipv2MinXInside = dotX > faceCenterMinX;
            Vector3.Dot(ref faceY, ref clipFace.V2, out dotY);
            bool clipv2MaxYInside = dotY < faceCenterMaxY;
            bool clipv2MinYInside = dotY > faceCenterMinY;

            //Clip v3
            Vector3.Dot(ref faceX, ref clipFace.V3, out dotX);
            bool clipv3MaxXInside = dotX < faceCenterMaxX;
            bool clipv3MinXInside = dotX > faceCenterMinX;
            Vector3.Dot(ref faceY, ref clipFace.V3, out dotY);
            bool clipv3MaxYInside = dotY < faceCenterMaxY;
            bool clipv3MinYInside = dotY > faceCenterMinY;

            //Clip v4
            Vector3.Dot(ref faceX, ref clipFace.V4, out dotX);
            bool clipv4MaxXInside = dotX < faceCenterMaxX;
            bool clipv4MinXInside = dotX > faceCenterMinX;
            Vector3.Dot(ref faceY, ref clipFace.V4, out dotY);
            bool clipv4MaxYInside = dotY < faceCenterMaxY;
            bool clipv4MinYInside = dotY > faceCenterMinY;

        #region Face Vertices

            if (v1MinXInside && v1MaxXInside && v1MinYInside && v1MaxYInside)
            {
                data[contactData.Count].Position = face.V1;
                data[contactData.Count].Id = face.Id1;
                contactData.Count++;
            }

            if (v2MinXInside && v2MaxXInside && v2MinYInside && v2MaxYInside)
            {
                data[contactData.Count].Position = face.V2;
                data[contactData.Count].Id = face.Id2;
                contactData.Count++;
            }

            if (v3MinXInside && v3MaxXInside && v3MinYInside && v3MaxYInside)
            {
                data[contactData.Count].Position = face.V3;
                data[contactData.Count].Id = face.Id3;
                contactData.Count++;
            }

            if (v4MinXInside && v4MaxXInside && v4MinYInside && v4MaxYInside)
            {
                data[contactData.Count].Position = face.V4;
                data[contactData.Count].Id = face.Id4;
                contactData.Count++;
            }

            #endregion

            //Compute depths.
            tempData = contactData;
            contactData.Count = 0;
            float depth;
            float clipFaceDot, faceDot;
            Vector3.Dot(ref clipFace.V1, ref mtd, out clipFaceDot);
            for (int i = 0; i < tempData.Count; i++)
            {
                Vector3.Dot(ref temp[i].Position, ref mtd, out faceDot);
                depth = faceDot - clipFaceDot;
                if (depth <= 0)
                {
                    data[contactData.Count].Position = temp[i].Position;
                    data[contactData.Count].Depth = depth;
                    data[contactData.Count].Id = temp[i].Id;
                    contactData.Count++;
                }
            }

            byte previousCount = contactData.Count;
            if (previousCount >= 4) //Early finish :)
            {
                outputData = contactData;
                return;
            }

        #region Clip face vertices

            Vector3 v;
            float a, b;
            Vector3.Dot(ref face.V1, ref face.Normal, out b);
            //CLIP FACE
            if (clipv1MinXInside && clipv1MaxXInside && clipv1MinYInside && clipv1MaxYInside)
            {
                Vector3.Dot(ref clipFace.V1, ref face.Normal, out a);
                Vector3.Mul(ref face.Normal, a - b, out v);
                Vector3.Sub(ref clipFace.V1, ref v, out v);
                data[contactData.Count].Position = v;
                data[contactData.Count].Id = clipFace.Id1 + 8;
                contactData.Count++;
            }

            if (clipv2MinXInside && clipv2MaxXInside && clipv2MinYInside && clipv2MaxYInside)
            {
                Vector3.Dot(ref clipFace.V2, ref face.Normal, out a);
                Vector3.Mul(ref face.Normal, a - b, out v);
                Vector3.Sub(ref clipFace.V2, ref v, out v);
                data[contactData.Count].Position = v;
                data[contactData.Count].Id = clipFace.Id2 + 8;
                contactData.Count++;
            }

            if (clipv3MinXInside && clipv3MaxXInside && clipv3MinYInside && clipv3MaxYInside)
            {
                Vector3.Dot(ref clipFace.V3, ref face.Normal, out a);
                Vector3.Mul(ref face.Normal, a - b, out v);
                Vector3.Sub(ref clipFace.V3, ref v, out v);
                data[contactData.Count].Position = v;
                data[contactData.Count].Id = clipFace.Id3 + 8;
                contactData.Count++;
            }

            if (clipv4MinXInside && clipv4MaxXInside && clipv4MinYInside && clipv4MaxYInside)
            {
                Vector3.Dot(ref clipFace.V4, ref face.Normal, out a);
                Vector3.Mul(ref face.Normal, a - b, out v);
                Vector3.Sub(ref clipFace.V4, ref v, out v);
                data[contactData.Count].Position = v;
                data[contactData.Count].Id = clipFace.Id4 + 8;
                contactData.Count++;
            }

            #endregion

            //Compute depths.
            tempData = contactData;
            contactData.Count = previousCount;

            for (int i = previousCount; i < tempData.Count; i++)
            {
                Vector3.Dot(ref temp[i].Position, ref mtd, out faceDot);
                depth = faceDot - clipFaceDot;
                if (depth <= 0)
                {
                    data[contactData.Count].Position = temp[i].Position;
                    data[contactData.Count].Depth = depth;
                    data[contactData.Count].Id = temp[i].Id;
                    contactData.Count++;
                }
            }

            previousCount = contactData.Count;
            if (previousCount >= 4) //Early finish :)
            {
                outputData = contactData;
                return;
            }

            //Intersect edges.

            //maxX maxY -> v1
            //minX maxY -> v2
            //minX minY -> v3
            //maxX minY -> v4

            //Once we get here there can only be 3 contacts or less.
            //Once 4 possible contacts have been added, switch to using safe increments.
            //float dot;

        #region CLIP EDGE: v1 v2

            FaceEdge clipEdge;
            clipFace.GetEdge(0, out clipEdge);
            if (!v1MaxYInside)
            {
                if (v2MaxYInside)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v4MaxYInside)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v2MaxYInside)
            {
                if (v1MaxYInside)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v3MaxYInside)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v3MaxYInside)
            {
                if (v2MaxYInside)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v4MaxYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v4MaxYInside)
            {
                if (v1MaxYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v3MaxYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }

            #endregion

        #region CLIP EDGE: v2 v3

            clipFace.GetEdge(1, out clipEdge);
            if (!v1MinXInside)
            {
                if (v2MinXInside && contactData.Count < 8)
                {
                    //test v1-v2 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v4MinXInside && contactData.Count < 8)
                {
                    //test v1-v3 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v2MinXInside)
            {
                if (v1MinXInside && contactData.Count < 8)
                {
                    //test v1-v2 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v3MinXInside && contactData.Count < 8)
                {
                    //test v2-v4 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v3MinXInside)
            {
                if (v2MinXInside && contactData.Count < 8)
                {
                    //test v1-v3 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v4MinXInside && contactData.Count < 8)
                {
                    //test v3-v4 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v4MinXInside)
            {
                if (v1MinXInside && contactData.Count < 8)
                {
                    //test v2-v4 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v3MinXInside && contactData.Count < 8)
                {
                    //test v3-v4 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }

            #endregion

        #region CLIP EDGE: v3 v4

            clipFace.GetEdge(2, out clipEdge);
            if (!v1MinYInside)
            {
                if (v2MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v4MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v2MinYInside)
            {
                if (v1MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v3MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v3MinYInside)
            {
                if (v2MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v4MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v4MinYInside)
            {
                if (v3MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v1MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }

            #endregion

        #region CLIP EDGE: v4 v1

            clipFace.GetEdge(3, out clipEdge);
            if (!v1MaxXInside)
            {
                if (v2MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v4MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v2MaxXInside)
            {
                if (v1MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v3MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v3MaxXInside)
            {
                if (v2MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v4MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }
            if (!v4MaxXInside)
            {
                if (v1MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Count++;
                    }
                }
                if (v3MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        data[contactData.Count].Position = v;
                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Count++;
                    }
                }
            }

            #endregion

            //Compute depths.
            tempData = contactData;
            contactData.Count = previousCount;

            for (int i = previousCount; i < tempData.Count; i++)
            {
                Vector3.Dot(ref temp[i].Position, ref mtd, out faceDot);
                depth = faceDot - clipFaceDot;
                if (depth <= 0)
                {
                    data[contactData.Count].Position = temp[i].Position;
                    data[contactData.Count].Depth = depth;
                    data[contactData.Count].Id = temp[i].Id;
                    contactData.Count++;
                }
            }
            outputData = contactData;
        }
#else
        private static void ClipFacesDirect(ref BoxFace clipFace, ref BoxFace face, ref Vector3 mtd, out TinyStructList<BoxContactData> contactData)
        {
            contactData = new TinyStructList<BoxContactData>();

            //Local directions on the clip face.  Their length is equal to the length of an edge.
            Vector3 clipX, clipY;
            Vector3.Sub(ref clipFace.V4, ref clipFace.V3, out clipX);
            Vector3.Sub(ref clipFace.V2, ref clipFace.V3, out clipY);
            float inverseClipWidth = 1 / clipFace.Width;
            float inverseClipHeight = 1 / clipFace.Height;
            float inverseClipWidthSquared = inverseClipWidth * inverseClipWidth;
            clipX.X *= inverseClipWidthSquared;
            clipX.Y *= inverseClipWidthSquared;
            clipX.Z *= inverseClipWidthSquared;
            float inverseClipHeightSquared = inverseClipHeight * inverseClipHeight;
            clipY.X *= inverseClipHeightSquared;
            clipY.Y *= inverseClipHeightSquared;
            clipY.Z *= inverseClipHeightSquared;

            //Local directions on the opposing face.  Their length is equal to the length of an edge.
            Vector3 faceX, faceY;
            Vector3.Sub(ref face.V4, ref face.V3, out faceX);
            Vector3.Sub(ref face.V2, ref face.V3, out faceY);
            float inverseFaceWidth = 1 / face.Width;
            float inverseFaceHeight = 1 / face.Height;
            float inverseFaceWidthSquared = inverseFaceWidth * inverseFaceWidth;
            faceX.X *= inverseFaceWidthSquared;
            faceX.Y *= inverseFaceWidthSquared;
            faceX.Z *= inverseFaceWidthSquared;
            float inverseFaceHeightSquared = inverseFaceHeight * inverseFaceHeight;
            faceY.X *= inverseFaceHeightSquared;
            faceY.Y *= inverseFaceHeightSquared;
            faceY.Z *= inverseFaceHeightSquared;

            Vector3 clipCenter;
            Vector3.Add(ref clipFace.V1, ref clipFace.V3, out clipCenter);
            //Defer division until after dot product (2 multiplies instead of 3)
            float clipCenterX, clipCenterY;
            Vector3.Dot(ref clipCenter, ref clipX, out clipCenterX);
            Vector3.Dot(ref clipCenter, ref clipY, out clipCenterY);
            clipCenterX *= .5f;
            clipCenterY *= .5f;

            Vector3 faceCenter;
            Vector3.Add(ref face.V1, ref face.V3, out faceCenter);
            //Defer division until after dot product (2 multiplies instead of 3)
            float faceCenterX, faceCenterY;
            Vector3.Dot(ref faceCenter, ref faceX, out faceCenterX);
            Vector3.Dot(ref faceCenter, ref faceY, out faceCenterY);
            faceCenterX *= .5f;
            faceCenterY *= .5f;

            //To test bounds, recall that clipX is the length of the X edge.
            //Going from the center to the max or min goes half of the length of X edge, or +/- 0.5.
            //Bias could be added here.
            //const float extent = .5f; //.5f is the default, extra could be added for robustness or speed.
            float extentX = .5f + .01f * inverseClipWidth;
            float extentY = .5f + .01f * inverseClipHeight;
            //float extentX = .5f + .01f * inverseClipXLength;
            //float extentY = .5f + .01f * inverseClipYLength;
            float clipCenterMaxX = clipCenterX + extentX;
            float clipCenterMaxY = clipCenterY + extentY;
            float clipCenterMinX = clipCenterX - extentX;
            float clipCenterMinY = clipCenterY - extentY;

            extentX = .5f + .01f * inverseFaceWidth;
            extentY = .5f + .01f * inverseFaceHeight;
            //extentX = .5f + .01f * inverseFaceXLength;
            //extentY = .5f + .01f * inverseFaceYLength;
            float faceCenterMaxX = faceCenterX + extentX;
            float faceCenterMaxY = faceCenterY + extentY;
            float faceCenterMinX = faceCenterX - extentX;
            float faceCenterMinY = faceCenterY - extentY;

            //Find out where the opposing face is.
            float dotX, dotY;

            //The four edges can be thought of as minX, maxX, minY and maxY.

            //Face v1
            Vector3.Dot(ref clipX, ref face.V1, out dotX);
            bool v1MaxXInside = dotX < clipCenterMaxX;
            bool v1MinXInside = dotX > clipCenterMinX;
            Vector3.Dot(ref clipY, ref face.V1, out dotY);
            bool v1MaxYInside = dotY < clipCenterMaxY;
            bool v1MinYInside = dotY > clipCenterMinY;

            //Face v2
            Vector3.Dot(ref clipX, ref face.V2, out dotX);
            bool v2MaxXInside = dotX < clipCenterMaxX;
            bool v2MinXInside = dotX > clipCenterMinX;
            Vector3.Dot(ref clipY, ref face.V2, out dotY);
            bool v2MaxYInside = dotY < clipCenterMaxY;
            bool v2MinYInside = dotY > clipCenterMinY;

            //Face v3
            Vector3.Dot(ref clipX, ref face.V3, out dotX);
            bool v3MaxXInside = dotX < clipCenterMaxX;
            bool v3MinXInside = dotX > clipCenterMinX;
            Vector3.Dot(ref clipY, ref face.V3, out dotY);
            bool v3MaxYInside = dotY < clipCenterMaxY;
            bool v3MinYInside = dotY > clipCenterMinY;

            //Face v4
            Vector3.Dot(ref clipX, ref face.V4, out dotX);
            bool v4MaxXInside = dotX < clipCenterMaxX;
            bool v4MinXInside = dotX > clipCenterMinX;
            Vector3.Dot(ref clipY, ref face.V4, out dotY);
            bool v4MaxYInside = dotY < clipCenterMaxY;
            bool v4MinYInside = dotY > clipCenterMinY;

            //Find out where the clip face is.
            //Clip v1
            Vector3.Dot(ref faceX, ref clipFace.V1, out dotX);
            bool clipv1MaxXInside = dotX < faceCenterMaxX;
            bool clipv1MinXInside = dotX > faceCenterMinX;
            Vector3.Dot(ref faceY, ref clipFace.V1, out dotY);
            bool clipv1MaxYInside = dotY < faceCenterMaxY;
            bool clipv1MinYInside = dotY > faceCenterMinY;

            //Clip v2
            Vector3.Dot(ref faceX, ref clipFace.V2, out dotX);
            bool clipv2MaxXInside = dotX < faceCenterMaxX;
            bool clipv2MinXInside = dotX > faceCenterMinX;
            Vector3.Dot(ref faceY, ref clipFace.V2, out dotY);
            bool clipv2MaxYInside = dotY < faceCenterMaxY;
            bool clipv2MinYInside = dotY > faceCenterMinY;

            //Clip v3
            Vector3.Dot(ref faceX, ref clipFace.V3, out dotX);
            bool clipv3MaxXInside = dotX < faceCenterMaxX;
            bool clipv3MinXInside = dotX > faceCenterMinX;
            Vector3.Dot(ref faceY, ref clipFace.V3, out dotY);
            bool clipv3MaxYInside = dotY < faceCenterMaxY;
            bool clipv3MinYInside = dotY > faceCenterMinY;

            //Clip v4
            Vector3.Dot(ref faceX, ref clipFace.V4, out dotX);
            bool clipv4MaxXInside = dotX < faceCenterMaxX;
            bool clipv4MinXInside = dotX > faceCenterMinX;
            Vector3.Dot(ref faceY, ref clipFace.V4, out dotY);
            bool clipv4MaxYInside = dotY < faceCenterMaxY;
            bool clipv4MinYInside = dotY > faceCenterMinY;

            #region Face Vertices
            BoxContactData item = new BoxContactData();
            if (v1MinXInside && v1MaxXInside && v1MinYInside && v1MaxYInside)
            {
                item.Position = face.V1;
                item.Id = face.Id1;
                contactData.Add(ref item);
            }

            if (v2MinXInside && v2MaxXInside && v2MinYInside && v2MaxYInside)
            {
                item.Position = face.V2;
                item.Id = face.Id2;
                contactData.Add(ref item);
            }

            if (v3MinXInside && v3MaxXInside && v3MinYInside && v3MaxYInside)
            {
                item.Position = face.V3;
                item.Id = face.Id3;
                contactData.Add(ref item);
            }

            if (v4MinXInside && v4MaxXInside && v4MinYInside && v4MaxYInside)
            {
                item.Position = face.V4;
                item.Id = face.Id4;
                contactData.Add(ref item);
            }

            #endregion

            //Compute depths.
            TinyStructList<BoxContactData> tempData = contactData;
            contactData.Clear();
            float clipFaceDot, faceDot;
            Vector3.Dot(ref clipFace.V1, ref mtd, out clipFaceDot);
            for (int i = 0; i < tempData.Count; i++)
            {
                tempData.Get(i, out item);
                Vector3.Dot(ref item.Position, ref mtd, out faceDot);
                item.Depth = faceDot - clipFaceDot;
                if (item.Depth <= 0)
                {
                    contactData.Add(ref item);
                }
            }

            int previousCount = contactData.Count;
            if (previousCount >= 4) //Early finish :)
            {
                return;
            }

            #region Clip face vertices

            Vector3 v;
            float a, b;
            Vector3.Dot(ref face.V1, ref face.Normal, out b);
            //CLIP FACE
            if (clipv1MinXInside && clipv1MaxXInside && clipv1MinYInside && clipv1MaxYInside)
            {
                Vector3.Dot(ref clipFace.V1, ref face.Normal, out a);
                Vector3.Mul(ref face.Normal, a - b, out v);
                Vector3.Sub(ref clipFace.V1, ref v, out v);
                item.Position = v;
                item.Id = clipFace.Id1 + 8;
                contactData.Add(ref item);
            }

            if (clipv2MinXInside && clipv2MaxXInside && clipv2MinYInside && clipv2MaxYInside)
            {
                Vector3.Dot(ref clipFace.V2, ref face.Normal, out a);
                Vector3.Mul(ref face.Normal, a - b, out v);
                Vector3.Sub(ref clipFace.V2, ref v, out v);
                item.Position = v;
                item.Id = clipFace.Id2 + 8;
                contactData.Add(ref item);
            }

            if (clipv3MinXInside && clipv3MaxXInside && clipv3MinYInside && clipv3MaxYInside)
            {
                Vector3.Dot(ref clipFace.V3, ref face.Normal, out a);
                Vector3.Mul(ref face.Normal, a - b, out v);
                Vector3.Sub(ref clipFace.V3, ref v, out v);
                item.Position = v;
                item.Id = clipFace.Id3 + 8;
                contactData.Add(ref item);
            }

            if (clipv4MinXInside && clipv4MaxXInside && clipv4MinYInside && clipv4MaxYInside)
            {
                Vector3.Dot(ref clipFace.V4, ref face.Normal, out a);
                Vector3.Mul(ref face.Normal, a - b, out v);
                Vector3.Sub(ref clipFace.V4, ref v, out v);
                item.Position = v;
                item.Id = clipFace.Id4 + 8;
                contactData.Add(ref item);
            }

            #endregion

            //Compute depths.
            int postClipCount = contactData.Count;
            tempData = contactData;
            for (int i = postClipCount - 1; i >= previousCount; i--) //TODO: >=?
                contactData.RemoveAt(i);


            for (int i = previousCount; i < tempData.Count; i++)
            {
                tempData.Get(i, out item);
                Vector3.Dot(ref item.Position, ref mtd, out faceDot);
                item.Depth = faceDot - clipFaceDot;
                if (item.Depth <= 0)
                {
                    contactData.Add(ref item);
                }
            }

            previousCount = contactData.Count;
            if (previousCount >= 4) //Early finish :)
            {
                return;
            }
            //Intersect edges.

            //maxX maxY -> v1
            //minX maxY -> v2
            //minX minY -> v3
            //maxX minY -> v4

            //Once we get here there can only be 3 contacts or less.
            //Once 4 possible contacts have been added, switch to using safe increments.
            //float dot;

            #region CLIP EDGE: v1 v2

            FaceEdge clipEdge;
            clipFace.GetEdge(0, out clipEdge);
            if (!v1MaxYInside)
            {
                if (v2MaxYInside)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v4MaxYInside)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v2MaxYInside)
            {
                if (v1MaxYInside)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v3MaxYInside)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v3MaxYInside)
            {
                if (v2MaxYInside)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v4MaxYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v4MaxYInside)
            {
                if (v1MaxYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v3MaxYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }

            #endregion

            #region CLIP EDGE: v2 v3

            clipFace.GetEdge(1, out clipEdge);
            if (!v1MinXInside)
            {
                if (v2MinXInside && contactData.Count < 8)
                {
                    //test v1-v2 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v4MinXInside && contactData.Count < 8)
                {
                    //test v1-v3 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v2MinXInside)
            {
                if (v1MinXInside && contactData.Count < 8)
                {
                    //test v1-v2 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v3MinXInside && contactData.Count < 8)
                {
                    //test v2-v4 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v3MinXInside)
            {
                if (v2MinXInside && contactData.Count < 8)
                {
                    //test v1-v3 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v4MinXInside && contactData.Count < 8)
                {
                    //test v3-v4 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v4MinXInside)
            {
                if (v1MinXInside && contactData.Count < 8)
                {
                    //test v2-v4 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v3MinXInside && contactData.Count < 8)
                {
                    //test v3-v4 against minXminY-minXmaxY
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }

            #endregion

            #region CLIP EDGE: v3 v4

            clipFace.GetEdge(2, out clipEdge);
            if (!v1MinYInside)
            {
                if (v2MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v4MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v2MinYInside)
            {
                if (v1MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v3MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v3MinYInside)
            {
                if (v2MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v4MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v4MinYInside)
            {
                if (v3MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v1MinYInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipX, ref v, out dot);
                    //if (dot > clipCenterMinX && dot < clipCenterMaxX)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }

            #endregion

            #region CLIP EDGE: v4 v1

            clipFace.GetEdge(3, out clipEdge);
            if (!v1MaxXInside)
            {
                if (v2MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v4MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v2MaxXInside)
            {
                if (v1MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v3MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v3MaxXInside)
            {
                if (v2MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v4MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }
            if (!v4MaxXInside)
            {
                if (v1MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
                if (v3MaxXInside && contactData.Count < 8)
                {
                    //ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
                    //Vector3.Dot(ref clipY, ref v, out dot);
                    //if (dot > clipCenterMinY && dot < clipCenterMaxY)
                    if (ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v))
                    {
                        item.Position = v;
                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
                        contactData.Add(ref item);
                    }
                }
            }

            #endregion

            //Compute depths.
            postClipCount = contactData.Count;
            tempData = contactData;
            for (int i = postClipCount - 1; i >= previousCount; i--)
                contactData.RemoveAt(i);

            for (int i = previousCount; i < tempData.Count; i++)
            {
                tempData.Get(i, out item);
                Vector3.Dot(ref item.Position, ref mtd, out faceDot);
                item.Depth = faceDot - clipFaceDot;
                if (item.Depth <= 0)
                {
                    contactData.Add(ref item);
                }
            }
        }
        //private static void ClipFacesDirect(ref BoxFace clipFace, ref BoxFace face, ref Vector3 mtd, out TinyStructList<BoxContactData> contactData)
        //{
        //    contactData = new TinyStructList<BoxContactData>();
        //    //BoxContactData* data = &contactData.d1;
        //    //BoxContactData* temp = &tempData.d1;

        //    //Local directions on the clip face.  Their length is equal to the length of an edge.
        //    Vector3 clipX, clipY;
        //    Vector3.Sub(ref clipFace.V4, ref clipFace.V3, out clipX);
        //    Vector3.Sub(ref clipFace.V2, ref clipFace.V3, out clipY);
        //    float inverse = 1 / clipX.LengthSquared();
        //    clipX.X *= inverse;
        //    clipX.Y *= inverse;
        //    clipX.Z *= inverse;
        //    inverse = 1 / clipY.LengthSquared();
        //    clipY.X *= inverse;
        //    clipY.Y *= inverse;
        //    clipY.Z *= inverse;

        //    //Local directions on the opposing face.  Their length is equal to the length of an edge.
        //    Vector3 faceX, faceY;
        //    Vector3.Sub(ref face.V4, ref face.V3, out faceX);
        //    Vector3.Sub(ref face.V2, ref face.V3, out faceY);
        //    inverse = 1 / faceX.LengthSquared();
        //    faceX.X *= inverse;
        //    faceX.Y *= inverse;
        //    faceX.Z *= inverse;
        //    inverse = 1 / faceY.LengthSquared();
        //    faceY.X *= inverse;
        //    faceY.Y *= inverse;
        //    faceY.Z *= inverse;

        //    Vector3 clipCenter;
        //    Vector3.Add(ref clipFace.V1, ref clipFace.V3, out clipCenter);
        //    //Defer division until after dot product (2 multiplies instead of 3)
        //    float clipCenterX, clipCenterY;
        //    Vector3.Dot(ref clipCenter, ref clipX, out clipCenterX);
        //    Vector3.Dot(ref clipCenter, ref clipY, out clipCenterY);
        //    clipCenterX *= .5f;
        //    clipCenterY *= .5f;

        //    Vector3 faceCenter;
        //    Vector3.Add(ref face.V1, ref face.V3, out faceCenter);
        //    //Defer division until after dot product (2 multiplies instead of 3)
        //    float faceCenterX, faceCenterY;
        //    Vector3.Dot(ref faceCenter, ref faceX, out faceCenterX);
        //    Vector3.Dot(ref faceCenter, ref faceY, out faceCenterY);
        //    faceCenterX *= .5f;
        //    faceCenterY *= .5f;

        //    //To test bounds, recall that clipX is the length of the X edge.
        //    //Going from the center to the max or min goes half of the length of X edge, or +/- 0.5.
        //    //Bias could be added here.
        //    float extent = .5f; //.5f is the default, extra could be added for robustness or speed.
        //    float clipCenterMaxX = clipCenterX + extent;
        //    float clipCenterMaxY = clipCenterY + extent;
        //    float clipCenterMinX = clipCenterX - extent;
        //    float clipCenterMinY = clipCenterY - extent;

        //    float faceCenterMaxX = faceCenterX + extent;
        //    float faceCenterMaxY = faceCenterY + extent;
        //    float faceCenterMinX = faceCenterX - extent;
        //    float faceCenterMinY = faceCenterY - extent;

        //    //Find out where the opposing face is.
        //    float dotX, dotY;

        //    //The four edges can be thought of as minX, maxX, minY and maxY.

        //    //Face v1
        //    Vector3.Dot(ref clipX, ref face.V1, out dotX);
        //    bool v1MaxXInside = dotX < clipCenterMaxX;
        //    bool v1MinXInside = dotX > clipCenterMinX;
        //    Vector3.Dot(ref clipY, ref face.V1, out dotY);
        //    bool v1MaxYInside = dotY < clipCenterMaxY;
        //    bool v1MinYInside = dotY > clipCenterMinY;

        //    //Face v2
        //    Vector3.Dot(ref clipX, ref face.V2, out dotX);
        //    bool v2MaxXInside = dotX < clipCenterMaxX;
        //    bool v2MinXInside = dotX > clipCenterMinX;
        //    Vector3.Dot(ref clipY, ref face.V2, out dotY);
        //    bool v2MaxYInside = dotY < clipCenterMaxY;
        //    bool v2MinYInside = dotY > clipCenterMinY;

        //    //Face v3
        //    Vector3.Dot(ref clipX, ref face.V3, out dotX);
        //    bool v3MaxXInside = dotX < clipCenterMaxX;
        //    bool v3MinXInside = dotX > clipCenterMinX;
        //    Vector3.Dot(ref clipY, ref face.V3, out dotY);
        //    bool v3MaxYInside = dotY < clipCenterMaxY;
        //    bool v3MinYInside = dotY > clipCenterMinY;

        //    //Face v4
        //    Vector3.Dot(ref clipX, ref face.V4, out dotX);
        //    bool v4MaxXInside = dotX < clipCenterMaxX;
        //    bool v4MinXInside = dotX > clipCenterMinX;
        //    Vector3.Dot(ref clipY, ref face.V4, out dotY);
        //    bool v4MaxYInside = dotY < clipCenterMaxY;
        //    bool v4MinYInside = dotY > clipCenterMinY;

        //    //Find out where the clip face is.
        //    //Clip v1
        //    Vector3.Dot(ref faceX, ref clipFace.V1, out dotX);
        //    bool clipv1MaxXInside = dotX < faceCenterMaxX;
        //    bool clipv1MinXInside = dotX > faceCenterMinX;
        //    Vector3.Dot(ref faceY, ref clipFace.V1, out dotY);
        //    bool clipv1MaxYInside = dotY < faceCenterMaxY;
        //    bool clipv1MinYInside = dotY > faceCenterMinY;

        //    //Clip v2
        //    Vector3.Dot(ref faceX, ref clipFace.V2, out dotX);
        //    bool clipv2MaxXInside = dotX < faceCenterMaxX;
        //    bool clipv2MinXInside = dotX > faceCenterMinX;
        //    Vector3.Dot(ref faceY, ref clipFace.V2, out dotY);
        //    bool clipv2MaxYInside = dotY < faceCenterMaxY;
        //    bool clipv2MinYInside = dotY > faceCenterMinY;

        //    //Clip v3
        //    Vector3.Dot(ref faceX, ref clipFace.V3, out dotX);
        //    bool clipv3MaxXInside = dotX < faceCenterMaxX;
        //    bool clipv3MinXInside = dotX > faceCenterMinX;
        //    Vector3.Dot(ref faceY, ref clipFace.V3, out dotY);
        //    bool clipv3MaxYInside = dotY < faceCenterMaxY;
        //    bool clipv3MinYInside = dotY > faceCenterMinY;

        //    //Clip v4
        //    Vector3.Dot(ref faceX, ref clipFace.V4, out dotX);
        //    bool clipv4MaxXInside = dotX < faceCenterMaxX;
        //    bool clipv4MinXInside = dotX > faceCenterMinX;
        //    Vector3.Dot(ref faceY, ref clipFace.V4, out dotY);
        //    bool clipv4MaxYInside = dotY < faceCenterMaxY;
        //    bool clipv4MinYInside = dotY > faceCenterMinY;

        //    var item = new BoxContactData();

        //    #region Face Vertices

        //    if (v1MinXInside && v1MaxXInside && v1MinYInside && v1MaxYInside)
        //    {
        //        item.Position = face.V1;
        //        item.Id = face.Id1;
        //        contactData.Add(ref item);
        //    }

        //    if (v2MinXInside && v2MaxXInside && v2MinYInside && v2MaxYInside)
        //    {
        //        item.Position = face.V2;
        //        item.Id = face.Id2;
        //        contactData.Add(ref item);
        //    }

        //    if (v3MinXInside && v3MaxXInside && v3MinYInside && v3MaxYInside)
        //    {
        //        item.Position = face.V3;
        //        item.Id = face.Id3;
        //        contactData.Add(ref item);
        //    }

        //    if (v4MinXInside && v4MaxXInside && v4MinYInside && v4MaxYInside)
        //    {
        //        item.Position = face.V4;
        //        item.Id = face.Id4;
        //        contactData.Add(ref item);
        //    }

        //    #endregion

        //    //Compute depths.
        //    TinyStructList<BoxContactData> tempData = contactData;
        //    contactData.Clear();
        //    float clipFaceDot, faceDot;
        //    Vector3.Dot(ref clipFace.V1, ref mtd, out clipFaceDot);
        //    for (int i = 0; i < tempData.Count; i++)
        //    {
        //        tempData.Get(i, out item);
        //        Vector3.Dot(ref item.Position, ref mtd, out faceDot);
        //        item.Depth = faceDot - clipFaceDot;
        //        if (item.Depth <= 0)
        //        {
        //            contactData.Add(ref item);
        //        }
        //    }

        //    int previousCount = contactData.Count;
        //    if (previousCount >= 4) //Early finish :)
        //    {
        //        return;
        //    }

        //    #region Clip face vertices

        //    Vector3 faceNormal;
        //    Vector3.Cross(ref faceY, ref faceX, out faceNormal);
        //    //inverse = 1 / faceNormal.LengthSquared();
        //    //faceNormal.X *= inverse;
        //    //faceNormal.Y *= inverse;
        //    //faceNormal.Z *= inverse;
        //    faceNormal.Normalize();
        //    Vector3 v;
        //    float a, b;
        //    Vector3.Dot(ref face.V1, ref faceNormal, out b);
        //    //CLIP FACE
        //    if (clipv1MinXInside && clipv1MaxXInside && clipv1MinYInside && clipv1MaxYInside)
        //    {
        //        Vector3.Dot(ref clipFace.V1, ref faceNormal, out a);
        //        Vector3.Mul(ref faceNormal, a - b, out v);
        //        Vector3.Sub(ref clipFace.V1, ref v, out v);
        //        item.Position = v;
        //        item.Id = clipFace.Id1 + 8;
        //        contactData.Add(ref item);
        //    }

        //    if (clipv2MinXInside && clipv2MaxXInside && clipv2MinYInside && clipv2MaxYInside)
        //    {
        //        Vector3.Dot(ref clipFace.V2, ref faceNormal, out a);
        //        Vector3.Mul(ref faceNormal, a - b, out v);
        //        Vector3.Sub(ref clipFace.V2, ref v, out v);
        //        item.Position = v;
        //        item.Id = clipFace.Id2 + 8;
        //        contactData.Add(ref item);
        //    }

        //    if (clipv3MinXInside && clipv3MaxXInside && clipv3MinYInside && clipv3MaxYInside)
        //    {
        //        Vector3.Dot(ref clipFace.V3, ref faceNormal, out a);
        //        Vector3.Mul(ref faceNormal, a - b, out v);
        //        Vector3.Sub(ref clipFace.V3, ref v, out v);
        //        item.Position = v;
        //        item.Id = clipFace.Id3 + 8;
        //        contactData.Add(ref item);
        //    }

        //    if (clipv4MinXInside && clipv4MaxXInside && clipv4MinYInside && clipv4MaxYInside)
        //    {
        //        Vector3.Dot(ref clipFace.V4, ref faceNormal, out a);
        //        Vector3.Mul(ref faceNormal, a - b, out v);
        //        Vector3.Sub(ref clipFace.V4, ref v, out v);
        //        item.Position = v;
        //        item.Id = clipFace.Id4 + 8;
        //        contactData.Add(ref item);
        //    }

        //    #endregion

        //    //Compute depths.
        //    int postClipCount = contactData.Count;
        //    tempData = contactData;
        //    for (int i = postClipCount - 1; i >= previousCount; i--) //TODO: >=?
        //        contactData.RemoveAt(i);


        //    for (int i = previousCount; i < tempData.Count; i++)
        //    {
        //        tempData.Get(i, out item);
        //        Vector3.Dot(ref item.Position, ref mtd, out faceDot);
        //        item.Depth = faceDot - clipFaceDot;
        //        if (item.Depth <= 0)
        //        {
        //            contactData.Add(ref item);
        //        }
        //    }

        //    previousCount = contactData.Count;
        //    if (previousCount >= 4) //Early finish :)
        //    {
        //        return;
        //    }

        //    //Intersect edges.

        //    //maxX maxY -> v1
        //    //minX maxY -> v2
        //    //minX minY -> v3
        //    //maxX minY -> v4

        //    //Once we get here there can only be 3 contacts or less.
        //    //Once 4 possible contacts have been added, switch to using safe increments.
        //    float dot;

        //    #region CLIP EDGE: v1 v2

        //    FaceEdge clipEdge;
        //    clipFace.GetEdge(0, ref mtd, out clipEdge);
        //    if (!v1MaxYInside)
        //    {
        //        if (v2MaxYInside)
        //        {
        //            ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v4MaxYInside)
        //        {
        //            ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v2MaxYInside)
        //    {
        //        if (v1MaxYInside)
        //        {
        //            ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v3MaxYInside)
        //        {
        //            ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v3MaxYInside)
        //    {
        //        if (v2MaxYInside)
        //        {
        //            ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v4MaxYInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v4MaxYInside)
        //    {
        //        if (v1MaxYInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v3MaxYInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }

        //    #endregion

        //    #region CLIP EDGE: v2 v3

        //    clipFace.GetEdge(1, ref mtd, out clipEdge);
        //    if (!v1MinXInside)
        //    {
        //        if (v2MinXInside && contactData.Count < 8)
        //        {
        //            //test v1-v2 against minXminY-minXmaxY
        //            ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v4MinXInside && contactData.Count < 8)
        //        {
        //            //test v1-v3 against minXminY-minXmaxY
        //            ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v2MinXInside)
        //    {
        //        if (v1MinXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v3MinXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v3MinXInside)
        //    {
        //        if (v2MinXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v4MinXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v4MinXInside)
        //    {
        //        if (v1MinXInside && contactData.Count < 8)
        //        {
        //            //test v2-v4 against minXminY-minXmaxY
        //            ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v3MinXInside && contactData.Count < 8)
        //        {
        //            //test v3-v4 against minXminY-minXmaxY
        //            ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }

        //    #endregion

        //    #region CLIP EDGE: v3 v4

        //    clipFace.GetEdge(2, ref mtd, out clipEdge);
        //    if (!v1MinYInside)
        //    {
        //        if (v2MinYInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v4MinYInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v2MinYInside)
        //    {
        //        if (v1MinYInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v3MinYInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v3MinYInside)
        //    {
        //        if (v2MinYInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v4MinYInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v4MinYInside)
        //    {
        //        if (v3MinYInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v1MinYInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
        //            Vector3.Dot(ref clipX, ref v, out dot);
        //            if (dot > clipCenterMinX && dot < clipCenterMaxX)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }

        //    #endregion

        //    #region CLIP EDGE: v4 v1

        //    clipFace.GetEdge(3, ref mtd, out clipEdge);
        //    if (!v1MaxXInside)
        //    {
        //        if (v2MaxXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v4MaxXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v2MaxXInside)
        //    {
        //        if (v1MaxXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v3MaxXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v3MaxXInside)
        //    {
        //        if (v2MaxXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v4MaxXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }
        //    if (!v4MaxXInside)
        //    {
        //        if (v1MaxXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //        if (v3MaxXInside && contactData.Count < 8)
        //        {
        //            ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
        //            Vector3.Dot(ref clipY, ref v, out dot);
        //            if (dot > clipCenterMinY && dot < clipCenterMaxY)
        //            {
        //                item.Position = v;
        //                item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
        //                contactData.Add(ref item);
        //            }
        //        }
        //    }

        //    #endregion

        //    //Compute depths.
        //    postClipCount = contactData.Count;
        //    tempData = contactData;
        //    for (int i = postClipCount - 1; i >= previousCount; i--)
        //        contactData.RemoveAt(i);

        //    for (int i = previousCount; i < tempData.Count; i++)
        //    {
        //        tempData.Get(i, out item);
        //        Vector3.Dot(ref item.Position, ref mtd, out faceDot);
        //        item.Depth = faceDot - clipFaceDot;
        //        if (item.Depth <= 0)
        //        {
        //            contactData.Add(ref item);
        //        }
        //    }
        //}
#endif

        private static bool ComputeIntersection(ref Vector3 edgeA1, ref Vector3 edgeA2, ref FaceEdge clippingEdge, out Vector3 intersection)
        {
            //Intersect the incoming edge (edgeA1, edgeA2) with the clipping edge's PLANE.  Nicely given by one of its positions and its 'perpendicular,'
            //which is its normal.

            Vector3 offset;
            Vector3.Sub(ref clippingEdge.A, ref edgeA1, out offset);

            Vector3 edgeDirection;
            Vector3.Sub(ref edgeA2, ref edgeA1, out edgeDirection);
            float distanceToPlane;
            Vector3.Dot(ref offset, ref clippingEdge.Perpendicular, out distanceToPlane);
            float edgeDirectionLength;
            Vector3.Dot(ref edgeDirection, ref clippingEdge.Perpendicular, out edgeDirectionLength);
            float t = distanceToPlane / edgeDirectionLength;
            if (t < 0 || t > 1)
            {
                //It's outside of the incoming edge!
                intersection = new Vector3();
                return false;
            }
            Vector3.Mul(ref edgeDirection, t, out offset);
            Vector3.Add(ref offset, ref edgeA1, out intersection);

            Vector3.Sub(ref intersection, ref clippingEdge.A, out offset);
            Vector3.Sub(ref clippingEdge.B, ref clippingEdge.A, out edgeDirection);
            Vector3.Dot(ref edgeDirection, ref offset, out t);
            if (t < 0 || t > edgeDirection.LengthSquared())
            {
                //It's outside of the clipping edge!
                return false;
            }
            return true;
        }

        private static void GetNearestFace(ref Vector3 position, ref Matrix3 orientation, ref Vector3 mtd, float halfWidth, float halfHeight, float halfLength, out BoxFace boxFace)
        {
            boxFace = new BoxFace();

            float xDot = orientation.X.X * mtd.X +
                         orientation.X.Y * mtd.Y +
                         orientation.X.Z * mtd.Z;
            float yDot = orientation.Y.X * mtd.X +
                         orientation.Y.Y * mtd.Y +
                         orientation.Y.Z * mtd.Z;
            float zDot = orientation.Z.X * mtd.X +
                         orientation.Z.Y * mtd.Y +
                         orientation.Z.Z * mtd.Z;

            float absX = Math.Abs(xDot);
            float absY = Math.Abs(yDot);
            float absZ = Math.Abs(zDot);

            Matrix4 worldTransform;
			Matrix4.FromAffineTransform(ref orientation, ref position, out worldTransform);

            Vector3 candidate;
            int bit;
            if (absX > absY && absX > absZ)
            {
                //"X" faces are candidates
                if (xDot < 0)
                {
                    halfWidth = -halfWidth;
                    bit = 0;
                }
                else
                    bit = 1;
                candidate = new Vector3(halfWidth, halfHeight, halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V1 = candidate;
                candidate = new Vector3(halfWidth, -halfHeight, halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V2 = candidate;
                candidate = new Vector3(halfWidth, -halfHeight, -halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V3 = candidate;
                candidate = new Vector3(halfWidth, halfHeight, -halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V4 = candidate;

                if (xDot < 0)
                    boxFace.Normal = orientation.Left;
                else
                    boxFace.Normal = orientation.Right;

                boxFace.Width = halfHeight * 2;
                boxFace.Height = halfLength * 2;

                boxFace.Id1 = bit + 2 + 4;
                boxFace.Id2 = bit + 4;
                boxFace.Id3 = bit + 2;
                boxFace.Id4 = bit;
            }
            else if (absY > absX && absY > absZ)
            {
                //"Y" faces are candidates
                if (yDot < 0)
                {
                    halfHeight = -halfHeight;
                    bit = 0;
                }
                else
                    bit = 2;
                candidate = new Vector3(halfWidth, halfHeight, halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V1 = candidate;
                candidate = new Vector3(-halfWidth, halfHeight, halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V2 = candidate;
                candidate = new Vector3(-halfWidth, halfHeight, -halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V3 = candidate;
                candidate = new Vector3(halfWidth, halfHeight, -halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V4 = candidate;

                if (yDot < 0)
                    boxFace.Normal = orientation.Down;
                else
                    boxFace.Normal = orientation.Up;

                boxFace.Width = halfWidth * 2;
                boxFace.Height = halfLength * 2;

                boxFace.Id1 = 1 + bit + 4;
                boxFace.Id2 = bit + 4;
                boxFace.Id3 = 1 + bit;
                boxFace.Id4 = bit;
            }
            else if (absZ > absX && absZ > absY)
            {
                //"Z" faces are candidates
                if (zDot < 0)
                {
                    halfLength = -halfLength;
                    bit = 0;
                }
                else
                    bit = 4;
                candidate = new Vector3(halfWidth, halfHeight, halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V1 = candidate;
                candidate = new Vector3(-halfWidth, halfHeight, halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V2 = candidate;
                candidate = new Vector3(-halfWidth, -halfHeight, halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V3 = candidate;
                candidate = new Vector3(halfWidth, -halfHeight, halfLength);
                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
                boxFace.V4 = candidate;

                if (zDot < 0)
                    boxFace.Normal = orientation.Front;
                else
                    boxFace.Normal = orientation.Back;

                boxFace.Width = halfWidth * 2;
                boxFace.Height = halfHeight * 2;

                boxFace.Id1 = 1 + 2 + bit;
                boxFace.Id2 = 2 + bit;
                boxFace.Id3 = 1 + bit;
                boxFace.Id4 = bit;
            }
        }


        private struct BoxFace
        {
            public int Id1, Id2, Id3, Id4;
            public Vector3 V1, V2, V3, V4;
            public Vector3 Normal;
            public float Width, Height;

            public int GetId(int i)
            {
                switch (i)
                {
                    case 0:
                        return Id1;
                    case 1:
                        return Id2;
                    case 2:
                        return Id3;
                    case 3:
                        return Id4;
                }
                return -1;
            }

            public void GetVertex(int i, out Vector3 v)
            {
                switch (i)
                {
                    case 0:
                        v = V1;
                        return;
                    case 1:
                        v = V2;
                        return;
                    case 2:
                        v = V3;
                        return;
                    case 3:
                        v = V4;
                        return;
                }
                v = Toolbox.NoVector;
            }

            internal void GetEdge(int i, out FaceEdge clippingEdge)
            {
                Vector3 insidePoint;
                switch (i)
                {
                    case 0:
                        clippingEdge.A = V1;
                        clippingEdge.B = V2;
                        insidePoint = V3;
                        clippingEdge.Id = GetEdgeId(Id1, Id2);
                        break;
                    case 1:
                        clippingEdge.A = V2;
                        clippingEdge.B = V3;
                        insidePoint = V4;
                        clippingEdge.Id = GetEdgeId(Id2, Id3);
                        break;
                    case 2:
                        clippingEdge.A = V3;
                        clippingEdge.B = V4;
                        insidePoint = V1;
                        clippingEdge.Id = GetEdgeId(Id3, Id4);
                        break;
                    case 3:
                        clippingEdge.A = V4;
                        clippingEdge.B = V1;
                        insidePoint = V2;
                        clippingEdge.Id = GetEdgeId(Id4, Id1);
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
                //TODO: Edge direction and perpendicular not normalized.
                Vector3 edgeDirection;
                Vector3.Sub(ref clippingEdge.B, ref clippingEdge.A, out edgeDirection);
                edgeDirection = edgeDirection.Normalize();
                Vector3.Cross(ref edgeDirection, ref Normal, out clippingEdge.Perpendicular);

                float dot;
                Vector3 offset;
                Vector3.Sub(ref insidePoint, ref clippingEdge.A, out offset);
                Vector3.Dot(ref clippingEdge.Perpendicular, ref offset, out dot);
                if (dot > 0)
                {
                    clippingEdge.Perpendicular.X = -clippingEdge.Perpendicular.X;
                    clippingEdge.Perpendicular.Y = -clippingEdge.Perpendicular.Y;
                    clippingEdge.Perpendicular.Z = -clippingEdge.Perpendicular.Z;
                }
                Vector3.Dot(ref clippingEdge.A, ref clippingEdge.Perpendicular, out clippingEdge.EdgeDistance);
            }
        }

        private static int GetContactId(int vertexAEdgeA, int vertexBEdgeA, int vertexAEdgeB, int vertexBEdgeB)
        {
            return GetEdgeId(vertexAEdgeA, vertexBEdgeA) * 2549 + GetEdgeId(vertexAEdgeB, vertexBEdgeB) * 2857;
        }

        private static int GetContactId(int vertexAEdgeA, int vertexBEdgeA, ref FaceEdge clippingEdge)
        {
            return GetEdgeId(vertexAEdgeA, vertexBEdgeA) * 2549 + clippingEdge.Id * 2857;
        }

        private static int GetEdgeId(int id1, int id2)
        {
            return (id1 + 1) * 571 + (id2 + 1) * 577;
        }

        private struct FaceEdge : IEquatable<FaceEdge>
        {
            public Vector3 A, B;
            public float EdgeDistance;
            public int Id;
            public Vector3 Perpendicular;

            #region IEquatable<FaceEdge> Members

            public bool Equals(FaceEdge other)
            {
                return other.Id == Id;
            }

            #endregion

            public bool IsPointInside(ref Vector3 point)
            {
                float distance;
                Vector3.Dot(ref point, ref Perpendicular, out distance);
                return distance < EdgeDistance; // +1; //TODO: Bias this a little?
            }
        }
    }
}