using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace P72
{
    [AddComponentMenu("P72/Plane Shape")]
    public class PlaneShape : Shape
    {
        public float2 size;

        public override ShapeType Type => ShapeType.Plane;

        public override bool PassiveIntersectionPlane(PlaneShape plane, PassiveIntersectionParams intersectionParams, Contact contactA, Contact contactB)
        {
            throw new System.NotImplementedException();
        }

        public override bool PassiveIntersectionSphere(SphereShape sphere, PassiveIntersectionParams intersectionParams, Contact contactA, Contact contactB)
        {
            return sphere.PassiveIntersection(this, intersectionParams, contactB, contactA);
        }
    }
}