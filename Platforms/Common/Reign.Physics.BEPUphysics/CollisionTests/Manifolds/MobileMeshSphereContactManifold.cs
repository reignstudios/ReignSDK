﻿using System;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionShapes;
using BEPUphysics.ResourceManagement;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using Reign.Core;

namespace BEPUphysics.CollisionTests.Manifolds
{
    ///<summary>
    /// Manages persistent contacts between a convex and an instanced mesh.
    ///</summary>
    public class MobileMeshSphereContactManifold : MobileMeshContactManifold
    {

        UnsafeResourcePool<TriangleSpherePairTester> testerPool = new UnsafeResourcePool<TriangleSpherePairTester>();
        protected override void GiveBackTester(CollisionAlgorithms.TrianglePairTester tester)
        {
            testerPool.GiveBack((TriangleSpherePairTester)tester);
        }

        protected override CollisionAlgorithms.TrianglePairTester GetTester()
        {
            return testerPool.Take();
        }

    }
}
