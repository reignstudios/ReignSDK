using BEPUphysics.Settings;
using BEPUphysics.DataStructures;
using System.Diagnostics;
using System;
using Reign.Core;

namespace BEPUphysics.CollisionTests
{
    ///<summary>
    /// Helper class that refreshes manifolds to keep them recent.
    ///</summary>
    public class ContactRefresher
    {

        /// <summary>
        /// Refreshes the contact manifold, removing any out of date contacts
        /// and updating others.
        /// </summary>
        public static void ContactRefresh(RawList<Contact> contacts, RawValueList<ContactSupplementData> supplementData, ref RigidTransform3 transformA, ref RigidTransform3 transformB, RawList<int> toRemove)
        {
            //TODO: Could also refresh normals with some trickery.
            //Would also need to refresh depth using new normals, and would require some extra information.

            for (int k = 0; k < contacts.count; k++)
            {
                ContactSupplementData data = supplementData.Elements[k];
                Vector3 newPosA, newPosB;
                Vector3.Transform(ref data.LocalOffsetA, ref transformA, out newPosA);
                Vector3.Transform(ref data.LocalOffsetB, ref transformB, out newPosB);

                //ab - (ab*n)*n
                //Compute the horizontal offset.
                Vector3 ab;
                Vector3.Sub(ref newPosB, ref newPosA, out ab);
                float dot;
                Vector3.Dot(ref ab, ref contacts.Elements[k].Normal, out dot);
                Vector3 temp;
                Vector3.Mul(ref contacts.Elements[k].Normal, dot, out temp);
                Vector3.Sub(ref ab, ref temp, out temp);
                dot = temp.LengthSquared();
                if (dot > CollisionDetectionSettings.ContactInvalidationLengthSquared)
                {
                    toRemove.Add(k);
                }
                else
                {
                    //Depth refresh:
                    //Find deviation (IE, (Ra-Rb)*N) and add to base depth.
                    Vector3.Dot(ref ab, ref contacts.Elements[k].Normal, out dot);
                    contacts.Elements[k].PenetrationDepth = data.BasePenetrationDepth - dot;
                    if (contacts.Elements[k].PenetrationDepth < -CollisionDetectionSettings.maximumContactDistance)
                        toRemove.Add(k);
                    else
                    {
                        //Refresh position and ra/rb.
                        Vector3 newPos;
                        Vector3.Add(ref newPosB, ref newPosA, out newPos);
                        Vector3.Mul(ref newPos, .5f, out newPos);
                        contacts.Elements[k].Position = newPos;
                        //This is an interesting idea, but has very little effect one way or the other.
                        //data.BasePenetrationDepth = contacts.Elements[k].PenetrationDepth;
                        //Vector3.TransformInversed(ref newPos, ref transformA, out data.LocalOffsetA);
                        //Vector3.TransformInversed(ref newPos, ref transformB, out data.LocalOffsetB);
                    }
                }
               
            }
        }
    }
}
