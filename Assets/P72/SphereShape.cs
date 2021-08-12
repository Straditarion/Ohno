using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace P72
{
    [AddComponentMenu("P72/Sphere Shape")]
    public class SphereShape : Shape
    {
        public float Radius;

        public override ShapeType Type => ShapeType.Sphere;

        public override bool PassiveIntersectionPlane(PlaneShape plane, PassiveIntersectionParams intersectionParams, Contact contactA, Contact contactB)
        {
            var delta = Position - plane.Position;

            var offsetRight = math.dot(delta, plane.Right);
            var offsetForward = math.dot(delta, plane.Forward);

            offsetRight = math.clamp(offsetRight, -plane.size.x, plane.size.x);
            offsetForward = math.clamp(offsetForward, -plane.size.y, plane.size.y);

            var planePoint = plane.Position + plane.Right * offsetRight + plane.Forward * offsetForward;

            delta = planePoint - Position;

            var distanceSq = math.lengthsq(delta);

            if (distanceSq >= Radius * Radius)
                return false;

            var distance = math.sqrt(distanceSq);

            var penetration = Radius - distance;

            if (!intersectionParams.IsUseful(penetration))
                return false;

            distance = math.max(float.Epsilon, distance);

            contactA.Point = Position + delta * (Radius / distance);
            contactB.Point = planePoint;

            contactA.Penetration = penetration;
            contactB.Penetration = penetration;

            return true;
        }

        public override bool PassiveIntersectionSphere(SphereShape sphere, PassiveIntersectionParams intersectionParams, Contact contactA, Contact contactB)
        {
            var rR = Radius + sphere.Radius;

            var delta = Position - sphere.Position;

            var distanceSq = math.lengthsq(delta);

            if (distanceSq >= rR * rR)
                return false;

            var distance = math.sqrt(distanceSq);

            var penetration = rR - distance;

            if (!intersectionParams.IsUseful(penetration))
                return false;

            distance = math.max(float.Epsilon, distance);

            var normal = delta / distance;

            contactA.Point = Position + normal * (-Radius);
            contactB.Point = sphere.Position + normal * sphere.Radius;

            contactA.Penetration = penetration;
            contactB.Penetration = penetration;

            return true;
        }
    }
}