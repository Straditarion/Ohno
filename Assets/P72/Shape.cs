using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace P72
{
    [DefaultExecutionOrder(707)]
    public abstract class Shape : MonoBehaviour
    {
        private static readonly Func<Shape, Shape, PassiveIntersectionParams, Contact, Contact, bool>[] passiveIntersectionLookup = new Func<Shape, Shape, PassiveIntersectionParams, Contact, Contact, bool>[]
        {
            PassiveIntersectionPlane,
            PassiveIntersectionSphere,
        };

        private static bool PassiveIntersectionPlane(Shape shape, Shape plane, PassiveIntersectionParams intersectionParams, Contact contactA, Contact contactB) => shape.PassiveIntersectionPlane(plane as PlaneShape, intersectionParams, contactA, contactB);
        private static bool PassiveIntersectionSphere(Shape shape, Shape sphere, PassiveIntersectionParams intersectionParams, Contact contactA, Contact contactB) => shape.PassiveIntersectionSphere(sphere as SphereShape, intersectionParams, contactA, contactB);

        private World world;
        public World World
        {
            get => world; set
            {
                if (body == null)
                {
                    if (world != null)
                        world.RemoveShape(this);

                    world = value;

                    if (world != null)
                        world.AddShape(this);
                }
                else if(body.World == value)
                {
                    world = value;
                }
            }
        }

        private Body body;
        public Body Body
        {
            get => body; set
            {
                if (world != null && body == value)
                    return;

                if(body == null)
                {
                    if (world != null)
                        world.RemoveShape(this);
                } else
                {
                    body.RemoveShape(this);
                }

                body = value;

                if(body == null)
                {
                    if (world == null)
                        world = World.Active;

                    if (world != null)
                        world.AddShape(this);
                } else
                {
                    body.AddShape(this);
                }
            }
        }

        public abstract ShapeType Type { get; }
        public float3 Position => transform.position;
        public quaternion Rotation => transform.rotation;
        public float3 Right => transform.right;
        public float3 Up => transform.up;
        public float3 Forward => transform.forward;

        private int id;

        private void Awake()
        {
            id = GetInstanceID();
        }

        private void OnEnable()
        {
            UpdateBody();
        }

        private void OnTransformParentChanged()
        {
            UpdateBody();
        }

        private void OnDisable()
        {
            ResetBody();
        }

        private void UpdateBody()
        {
            Body = GetComponentInParent<Body>();
        }

        private void ResetBody()
        {
            Body = null;
        }

        public override int GetHashCode()
        {
            return id;
        }

        public bool PassiveIntersection(Shape shape, PassiveIntersectionParams intersectionParams, Contact contactA, Contact contactB)
        {
            return passiveIntersectionLookup[(int)shape.Type](this, shape, intersectionParams, contactA, contactB);
        }

        public abstract bool PassiveIntersectionPlane(PlaneShape plane, PassiveIntersectionParams intersectionParams, Contact contactA, Contact contactB);
        public abstract bool PassiveIntersectionSphere(SphereShape sphere, PassiveIntersectionParams intersectionParams, Contact contactA, Contact contactB);
    }

    public enum ShapeType
    {
        Plane,
        Sphere,
    }
}