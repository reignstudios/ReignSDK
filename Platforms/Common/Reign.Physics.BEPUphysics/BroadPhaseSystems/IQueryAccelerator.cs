using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using Reign.Core;

namespace BEPUphysics.BroadPhaseSystems
{
    ///<summary>
    /// Defines a system that accelerates bounding volume and ray cast queries.
    ///</summary>
    public interface IQueryAccelerator
    {
        /// <summary>
        /// Gets the broad phase associated with this query accelerator, if any.
        /// </summary>
        BroadPhase BroadPhase { get; }
        ///<summary>
        /// Gets the broad phase entries overlapping the ray.
        ///</summary>
        ///<param name="ray">Ray3 to test.</param>
        ///<param name="outputIntersections">Overlapped entries.</param>
        ///<returns>Whether or not the ray hit anything.</returns>
        bool RayCast(Ray3 ray, IList<BroadPhaseEntry> outputIntersections);
        ///<summary>
        /// Gets the broad phase entries overlapping the ray.
        ///</summary>
        ///<param name="ray">Ray3 to test.</param>
        /// <param name="maximumLength">Maximum length of the ray in units of the ray's direction's length.</param>
        ///<param name="outputIntersections">Overlapped entries.</param>
        ///<returns>Whether or not the ray hit anything.</returns>
        bool RayCast(Ray3 ray, float maximumLength, IList<BroadPhaseEntry> outputIntersections);

        //There's no single-hit version because the TOI on queries isn't really meaningful.
        //TODO: IQueryAccelerator + BroadPhase.  Both have add methods.  A user might expect to be able to add separately, but that doesn't really work.
        //Consider pulling the query accelerator into the broadphase so people consider it to be a part of the broadphase- it accelerates queries against the broadphase.
        //If someone wanted to raycast against something other than the broadphase, they can create an IQueryAccelerator of some kind in isolation.

        /// <summary>
        /// Gets the entries with bounding boxes which overlap the bounding shape.
        /// </summary>
        /// <param name="boundingShape">Bounding shape to test.</param>
        /// <param name="overlaps">Overlapped entries.</param>
        void GetEntries(BoundingBox3 boundingShape, IList<BroadPhaseEntry> overlaps);
        /// <summary>
        /// Gets the entries with bounding boxes which overlap the bounding shape.
        /// </summary>
        /// <param name="boundingShape">Bounding shape to test.</param>
        /// <param name="overlaps">Overlapped entries.</param>
        void GetEntries(BoundingSphere3 boundingShape, IList<BroadPhaseEntry> overlaps);
        /// <summary>
        /// Gets the entries with bounding boxes which overlap the bounding shape.
        /// </summary>
        /// <param name="boundingShape">Bounding shape to test.</param>
        /// <param name="overlaps">Overlapped entries.</param>
        void GetEntries(BoundingFrustum3 boundingShape, IList<BroadPhaseEntry> overlaps);
    }
}
