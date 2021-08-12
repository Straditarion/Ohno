using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace P72
{
    public static class Solver
    {
        [System.Serializable]
        public class Config
        {
            public float SoloSolveTranspose;
            public float SoloSolveImpulse;
            public float SoloSolveDampen;
        }

        public static Config ActiveConfig = new Config()
        {
            SoloSolveTranspose = 1f,
            SoloSolveImpulse = 2.5f,
            SoloSolveDampen = 5f,
        };

        public static void Solve(Body bodyA, Contact contactA, Contact contactB)
        {
            var comDelta = contactA.Point - bodyA.CenterOfMass;
            var comDeltaNormal = math.normalize(comDelta);

            var velocityNormal = math.normalize(bodyA.Velocity);

            var dampen = math.dot(comDeltaNormal, velocityNormal);
            dampen = math.min(1f - dampen, 1f);

            var contactDelta = contactB.Point - contactA.Point;

            bodyA.Position += contactDelta * ActiveConfig.SoloSolveTranspose;

            bodyA.Velocity *= math.lerp(1f, dampen, math.saturate(Time.fixedDeltaTime * ActiveConfig.SoloSolveDampen));
            bodyA.Velocity += contactDelta * ActiveConfig.SoloSolveImpulse;
        }
    }

    public class PassiveIntersectionParams
    {
        private float penetration;

        public PassiveIntersectionParams(float penetration = 0f) { this.penetration = penetration; }

        public bool IsUseful(float penetration)
        {
            if(penetration > this.penetration)
            {
                this.penetration = penetration;
                return true;
            } else
            {
                return false;
            }
        }
    }

    public class BodyMeta
    {
        public BodyMeta(Body body) { }
    }

    public class Contact
    {
        public float3 Point;
        public float Penetration;
    }
}