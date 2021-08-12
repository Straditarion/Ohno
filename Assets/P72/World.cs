using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace P72
{
    [DefaultExecutionOrder(693)]
    public class World : MonoBehaviour
    {
        public static World Active { get; private set; }

        public float3 gravity;

        private HashSet<Shape> shapes = new HashSet<Shape>();
        private HashSet<Body> bodies = new HashSet<Body>();

        private void Awake()
        {
            if(Active == null)
            {
                Active = this;
            }
        }

        public void AddShape(Shape shape)
        {
            shapes.Add(shape);
        }

        public void RemoveShape(Shape shape)
        {
            shapes.Remove(shape);
        }

        public void AddBody(Body body)
        {
            bodies.Add(body);
        }

        public void RemoveBody(Body body)
        {
            bodies.Remove(body);
        }

        private void FixedUpdate()
        {
            foreach (var body in bodies)
            {
                body.Position += body.Velocity * Time.fixedDeltaTime;
                body.Velocity += gravity * Time.fixedDeltaTime;
            }

            var contactA = new Contact();
            var contactB = new Contact();

            foreach (var body in bodies)
            {
                foreach (var other in bodies)
                {
                    if (body.PassiveIntersection(other, contactA, contactB))
                    {
                        Solver.Solve(body, contactA, contactB);
                    }
                }

                foreach (var other in shapes)
                {
                    if (body.PassiveIntersection(other, contactA, contactB))
                    {
                        Solver.Solve(body, contactA, contactB);
                    }
                }
            }

            foreach (var body in bodies)
            {
                body.UpdateMeta();
            }
        }
    }
}