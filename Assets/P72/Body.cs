using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace P72
{
    [DefaultExecutionOrder(700)]
    public class Body : MonoBehaviour
    {
        private World world;
        public World World
        {
            get => world; set
            {
                if (world != null)
                    world.RemoveBody(this);

                world = value;

                foreach (var shape in shapes)
                {
                    shape.World = world;
                }

                if (world != null)
                    world.AddBody(this);
            }
        }

        public float3 CenterOfMass => Position;

        public float3 Position
        {
            get => transform.position; set
            {
                transform.position = value;
            }
        }

        public float3 Velocity;

        [HideInInspector] public BodyMeta BodyMeta;

        private HashSet<Shape> shapes = new HashSet<Shape>();
        private int id;

        private void Awake()
        {
            id = GetInstanceID();
        }

        private void OnEnable()
        {
            if (world == null)
                world = World.Active;

            if (world != null)
                world.AddBody(this);

            BodyMeta = new BodyMeta(this);
        }

        private void OnDisable()
        {
            if (world != null)
                world.RemoveBody(this);
        }

        public override int GetHashCode()
        {
            return id;
        }

        public void UpdateMeta()
        {

        }

        public void AddShape(Shape shape)
        {
            shapes.Add(shape);
        }

        public void RemoveShape(Shape shape)
        {
            shapes.Remove(shape);
        }

        public bool PassiveIntersection(Body body, Contact contactA, Contact contactB)
        {
            var intersectionParams = new PassiveIntersectionParams();
            var intersects = false;

            foreach (var shape in shapes)
            {
                foreach (var other in body.shapes)
                {
                    intersects |= shape.PassiveIntersection(other, intersectionParams, contactA, contactB);
                }
            }

            return intersects;
        }

        public bool PassiveIntersection(Shape other, Contact contactA, Contact contactB)
        {
            var intersectionParams = new PassiveIntersectionParams();
            var intersects = false;

            foreach (var shape in shapes)
            {
                intersects |= shape.PassiveIntersection(other, intersectionParams, contactA, contactB);
            }

            return intersects;
        }
    }
}