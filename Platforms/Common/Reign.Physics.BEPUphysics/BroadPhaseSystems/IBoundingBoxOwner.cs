using Reign.Core;

namespace BEPUphysics.BroadPhaseSystems
{
    ///<summary>
    /// Requires that a class have a BoundingBox3.
    ///</summary>
    public interface IBoundingBoxOwner
    {
        ///<summary>
        /// Gets the bounding box of the object.
        ///</summary>
        BoundingBox3 BoundingBox { get; }
    }
}
