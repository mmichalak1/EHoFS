using Microsoft.Xna.Framework;
using OurGame.Engine.Components;
using OurGame.Engine.Components.BoundingObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Engine.Statics
{
    class PhysicsEngine
    {
        private static PhysicsEngine instance;

        public static PhysicsEngine Instance
        {
            get
            {
                if (instance == null)
                    instance = new PhysicsEngine();
                return instance;
            }
        }
        private double _gravitiAcceleration;
        public double GravitiAcceleration
        { get { return _gravitiAcceleration; } set { _gravitiAcceleration = value; } }

        private float _densityOfAir;
        public float DensityOfAir
        { get { return _densityOfAir; } set { _densityOfAir = value; } }

        private float _minDistanceBetweenPhysicCollider;
        public float MinDistanceBetweenPhysicCollider
        { get { return _minDistanceBetweenPhysicCollider; } set { _minDistanceBetweenPhysicCollider = value; } }

        private HashSet<ColliderComponent> PhysicColliderList;
        public static float floorLevel = 101;

        private PhysicsEngine()
        {
            _gravitiAcceleration = 100f;
            _densityOfAir = 0.001f;
            _minDistanceBetweenPhysicCollider = 0.025f;
            PhysicColliderList = ColliderMenager.Instance.PhysicsColliderList;
        }

        public void Update(GameTime gameTime)
        {
            foreach (ColliderComponent colliderComponent in PhysicColliderList)
            {
                float step = (float)gameTime.ElapsedGameTime.TotalSeconds; //* (float)0.5;
                RigidBodyComponent rigidBody = colliderComponent.Parent.GetComponentOfType<RigidBodyComponent>();
                UpdateVelocity(rigidBody, colliderComponent, step);

                UpdatePosition(rigidBody, step);

                HashSet<GameObject> listOfCollision = colliderComponent.getActualListOfColidingObjects();
                DetectAndResolveCollision(listOfCollision, rigidBody, colliderComponent);
            }
            AddTMPVelocity();
            MakeMinDistanceBetweenPhysicsColliders(_minDistanceBetweenPhysicCollider);
        }
        public void MakeMinDistanceBetweenPhysicsColliders(float minDistance)
        {
            foreach (ColliderComponent colliderComponent in PhysicColliderList)
            {
                HashSet<GameObject> listOfCollision = colliderComponent.getActualListOfColidingObjects();
                RigidBodyComponent rigidBody = colliderComponent.Parent.GetComponentOfType<RigidBodyComponent>();
                
                PushFromEachOtherFor(listOfCollision, colliderComponent, rigidBody, minDistance);
            }
        }
        private void PushFromEachOtherFor(HashSet<GameObject> listOfCollision, ColliderComponent thisCollider, RigidBodyComponent rigidBody, float minDistance)
        {
            foreach (GameObject colidingObject in listOfCollision)
            {
                if (thisCollider.isCollide(colidingObject))
                {
                    if (rigidBody.Mass != 0)
                    {
                        if (colidingObject.GetComponentOfType<RigidBodyComponent>() != null)
                        {
                            BoundingObject colidingBounding = colidingObject.GetComponentOfType<ColliderComponent>().BoundingObject;
                            if (colidingBounding is Box)
                            {
                                Vector3 diffrence = colidingBounding.getCenter() - thisCollider.BoundingObject.getCenter();
                                Vector3 directionToPush = MakeNormalForSphereBoxCollision(diffrence, (colidingBounding as Box).Size);
                                float distanceToPush = 0;
                                if (thisCollider.BoundingObject is Sphere)
                                {
                                    distanceToPush = Math.Abs((diffrence * directionToPush).Length() - (thisCollider.BoundingObject as Sphere).Radius - ((colidingBounding as Box).Size * directionToPush).Length());
                                }
                                if (thisCollider.BoundingObject is Box)
                                {
                                    distanceToPush = Math.Abs((diffrence * directionToPush).Length() - ((thisCollider.BoundingObject as Box).Size * directionToPush).Length() - ((colidingBounding as Box).Size * directionToPush).Length());
                                }
                                Vector3 newDistance = directionToPush * (distanceToPush + minDistance);
                                thisCollider.Parent.Transform.Position += newDistance;
                            }
                            if (colidingBounding is Sphere)
                            {
                                Vector3 diffrence = colidingBounding.getCenter() - thisCollider.BoundingObject.getCenter();
                                Vector3 directionToPush = Vector3.Zero;
                                if(diffrence!=Vector3.Zero)
                                    directionToPush = Vector3.Normalize(Vector3.Negate(diffrence));
                                float distanceToPush = 0;
                                if (thisCollider.BoundingObject is Sphere)
                                {
                                    distanceToPush = Math.Abs(diffrence.Length() - (thisCollider.BoundingObject as Sphere).Radius - (colidingBounding as Sphere).Radius);
                                }
                                if (thisCollider.BoundingObject is Box)
                                {
                                    distanceToPush = Math.Abs(diffrence.Length() - ((thisCollider.BoundingObject as Box).Size * directionToPush).Length() - (colidingBounding as Sphere).Radius);
                                }
                                Vector3 newDistance = directionToPush * (distanceToPush + minDistance);
                                thisCollider.Parent.Transform.Position += newDistance;
                            }
                        }
                    }
                }
            }
        }
        private HashSet<GameObject> MakeListOfCollision(GameObject gameObject)
        {
            HashSet<GameObject> listOfCollision = new HashSet<GameObject>();
            if (gameObject.GetComponentOfType<ColliderComponent>() != null)
            {
                listOfCollision = gameObject.GetComponentOfType<ColliderComponent>().getActualListOfColidingObjects();
            }
            return listOfCollision;
        }
        private void AddTMPVelocity()
        {
            foreach (ColliderComponent colliderComponent in PhysicColliderList)
            {
                RigidBodyComponent rigidBody = colliderComponent.Parent.GetComponentOfType<RigidBodyComponent>();
                if (rigidBody.VelocityTMP != Vector3.Zero)
                {
                    rigidBody.Velocity = rigidBody.Velocity + rigidBody.VelocityTMP;
                    rigidBody.VelocityTMP = Vector3.Zero;
                }
            }
        }

        private Vector3 Gravity(double mass)
        {
            if (mass < 0)
            {
                return Vector3.Multiply(Vector3.Up, (float)(mass * _gravitiAcceleration));
            }
            return Vector3.Multiply(Vector3.Down, (float)(mass * _gravitiAcceleration));
        }
        private Vector3 AirDrag(Vector3 velocity, float coeffecient)
        {
            Vector3 force = Vector3.Multiply(Vector3.Multiply(velocity, velocity), 0.5f * (float)DensityOfAir * coeffecient);
            if (velocity.X < 0)
            {
                force.X = -force.X;
            }
            if (velocity.Y < 0)
            {
                force.Y = -force.Y;
            }
            if (velocity.Z < 0)
            {
                force.Z = -force.Z;
            }
            return Vector3.Negate(force);
        }
        private Vector3 MakeOpositeVectorDirectionFromTwoPosition(GameObject fist, GameObject second)
        {
            return Vector3.Normalize(Vector3.Negate(fist.Transform.Position - second.Transform.Position));
        }
        private Vector3 CalculateForces(RigidBodyComponent rigidBody, ColliderComponent colliderComponent, float step)
        {
            Vector3 force = rigidBody.AffectingForce + AirDrag(rigidBody.Velocity, rigidBody.Coeffecient);
            if (colliderComponent.BoundingObject is Sphere)
            {
                Sphere sphere = colliderComponent.BoundingObject as Sphere;
                if ((sphere.getCenter().Y - sphere.Radius) > floorLevel && rigidBody.AffectedByGravity)
                {
                    force += Gravity(rigidBody.Mass);
                }
            }
            if (colliderComponent.BoundingObject is Box)
            {
                Box box = colliderComponent.BoundingObject as Box;
                if ((box.getCenter().Y - box.Size.Y) > floorLevel && rigidBody.AffectedByGravity)
                {
                    force += Gravity(rigidBody.Mass);
                }
            }
            rigidBody.ReduceAffectingForceVelocity(step);
            return force;
        }
        private void UpdateVelocity(RigidBodyComponent rigidBody, ColliderComponent colliderComponent, float step)
        {
            Vector3 force = CalculateForces(rigidBody, colliderComponent, step);
            Vector3 acceleration = Vector3.Multiply(force, (float)rigidBody.OneDivMass);

            rigidBody.PreviousVelocity = rigidBody.Velocity;
            if (float.IsNaN(rigidBody.Velocity.X))
                throw new Exception();
            rigidBody.Velocity = rigidBody.Velocity + acceleration * step;
            if (float.IsNaN(rigidBody.Velocity.X))
                throw new Exception();
        }
        private void UpdatePosition(RigidBodyComponent rigidBody, float step)
        {
            Vector3 avgVelocity = Vector3.Multiply((rigidBody.Velocity + rigidBody.PreviousVelocity), 0.5f);
            if (float.IsNaN(step))
                throw new Exception();
            rigidBody.Parent.Transform.Position += Vector3.Multiply(avgVelocity, step);
        }
        private void DetectAndResolveCollision(HashSet<GameObject> listOfCollision, RigidBodyComponent rigidBody, ColliderComponent collider)
        {
            foreach (GameObject colidingObject in listOfCollision)
            {
                if (collider.isEnter(colidingObject))
                {
                    if (colidingObject.GetComponentOfType<RigidBodyComponent>() != null)
                    {
                        RigidBodyComponent colidingObjectRigidBody = colidingObject.GetComponentOfType<RigidBodyComponent>();
                        ColliderComponent colidingObjectCollider = colidingObject.GetComponentOfType<ColliderComponent>();
                        if (rigidBody.Mass != 0)
                        {
                            Vector3 directionOfMove = Vector3.Normalize(rigidBody.PreviousVelocity);
                            if (float.IsNaN(directionOfMove.X) || float.IsNaN(directionOfMove.Y) || float.IsNaN(directionOfMove.Z) || float.IsInfinity(directionOfMove.X) || float.IsInfinity(directionOfMove.Y) || float.IsInfinity(directionOfMove.Z))
                                directionOfMove = Vector3.Zero;
                            Vector3 directionToPointOfContact = Vector3.Normalize(colidingObjectCollider.getCenter() - collider.getCenter());
                            if (float.IsNaN(directionToPointOfContact.X) || float.IsNaN(directionToPointOfContact.Y) || float.IsNaN(directionToPointOfContact.Z) || float.IsInfinity(directionToPointOfContact.X) || float.IsInfinity(directionToPointOfContact.Y) || float.IsInfinity(directionToPointOfContact.Z))
                                directionToPointOfContact = Vector3.Zero;
                            if (colidingObjectRigidBody.Mass == 0)
                            {
                                if (collider.BoundingObject is Sphere && colidingObjectCollider.BoundingObject is Box)
                                {
                                    if (directionOfMove == directionToPointOfContact)
                                    {
                                        SimpleReverseCollision(rigidBody);
                                    }
                                    else
                                    {
                                        ReverseCollision(rigidBody, collider, colidingObjectCollider, directionOfMove);
                                    }
                                }
                                else
                                {
                                    SimpleReverseCollision(rigidBody);
                                }

                            }
                            else
                            {
                                if (colidingObjectCollider.BoundingObject is Sphere && collider.BoundingObject is Sphere)
                                {
                                    if (directionOfMove == directionToPointOfContact)
                                    {
                                        CentralCollision(rigidBody, colidingObjectRigidBody);
                                    }
                                    else
                                    {
                                        NoCentralCollision(rigidBody, colidingObjectRigidBody, directionToPointOfContact, directionOfMove, collider, colidingObjectCollider);
                                    }
                                }
                                else
                                {
                                    CentralCollision(rigidBody, colidingObjectRigidBody);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ReverseCollision(RigidBodyComponent rigidBody, ColliderComponent collider, ColliderComponent colidingObjectCollider, Vector3 directionOfMove)
        {
            Box box = (colidingObjectCollider.BoundingObject as Box);
            Sphere sphere = (collider.BoundingObject as Sphere);
            Vector3 distance = sphere.getCenter() - box.getCenter();
            Vector3 boxSize = box.Size;

            Vector3 normal = MakeNormalForSphereBoxCollision(distance, boxSize);

            Vector3 directionOfReflection = directionOfMove - 2 * Vector3.Multiply(normal, Vector3.Dot(directionOfMove, normal)); //formula
            float speed = rigidBody.Velocity.Length();

            Vector3 negVelocity = Vector3.Multiply(directionOfReflection, speed);
            rigidBody.Velocity = Vector3.Multiply(negVelocity, rigidBody.Bounciness);
        }

        private void SimpleReverseCollision(RigidBodyComponent rigidBody)
        {
            Vector3 negVelocity = Vector3.Negate(rigidBody.Velocity);
            rigidBody.Velocity = Vector3.Multiply(negVelocity, rigidBody.Bounciness);
        }
        private void CentralCollision(RigidBodyComponent rigidBody, RigidBodyComponent colidingObjectRigidBody)
        {
            // (v1(m1+m2) + 2 m2 V2) / m1 + m2
            float M1 = (float)rigidBody.Mass;
            float M2 = (float)colidingObjectRigidBody.Mass;
            Vector3 V1 = rigidBody.PreviousVelocity;
            Vector3 V2 = colidingObjectRigidBody.PreviousVelocity;
            Vector3 Numerator = Vector3.Multiply(V1, M1 - M2) + Vector3.Multiply(V2, 2 * M2); // (v1(m1+m2) + 2 m2 V2)
            float Denumerator = M1 + M2;
            Vector3 Velocity = Vector3.Divide(Numerator, Denumerator);
            rigidBody.Velocity = Vector3.Multiply(Velocity, rigidBody.Bounciness);
        }
        private void NoCentralCollision(RigidBodyComponent rigidBody, RigidBodyComponent colidingObjectRigidBody, Vector3 directionToPointOfContact, Vector3 directionOfMove, ColliderComponent collider, ColliderComponent colidingObjectCollider)
        {
            float M1 = (float)rigidBody.Mass;
            float M2 = (float)colidingObjectRigidBody.Mass;
            float V1 = rigidBody.PreviousVelocity.Length();
            float V2 = colidingObjectRigidBody.PreviousVelocity.Length();

            float dot = Vector3.Dot(directionToPointOfContact, directionOfMove);
            if (Math.Abs(dot) > 1)
                dot = 1;

            double angle = Math.Acos(dot); //angle between two vectors

            float oneDivSumRadius = 1 / ((collider.BoundingObject as Sphere).getSphere().Radius + (colidingObjectCollider.BoundingObject as Sphere).getSphere().Radius);
            float tmpY = (collider.getCenter().Y - colidingObjectCollider.getCenter().Y) * oneDivSumRadius;
            float tmpX = (collider.getCenter().X - colidingObjectCollider.getCenter().X) * oneDivSumRadius;
            float tmpZ = (collider.getCenter().Z - colidingObjectCollider.getCenter().Z) * oneDivSumRadius;
            Vector3 directionOfReflection = new Vector3(tmpX, tmpY, tmpZ);

            double numeretor1 = V1 * (Math.Sqrt((M1 * M1) + (M2 * M2) + (2 * M1 * M2 * Math.Cos(angle)))); //  V1 * sqrt(m1^2+m2^2+2m1m2cos(angle))
            float speed1 = (float)(numeretor1 / (M1 + M2));
            Vector3 Velocity1 = Vector3.Multiply(directionOfReflection, speed1);

            double numerator2 = V2 * (2 * M1) * Math.Sin(angle / 2); // V2 * 2m1sin(angle/2)
            float speed2 = (float)(numerator2 / (M1 + M2));
            Vector3 Velocity2 = Vector3.Multiply(directionToPointOfContact, speed2);

            colidingObjectRigidBody.VelocityTMP = Vector3.Multiply(Velocity2, colidingObjectRigidBody.Bounciness);
            rigidBody.Velocity = Vector3.Multiply(Velocity1, rigidBody.Bounciness);
        }
        private Vector3 MakeNormalForSphereBoxCollision(Vector3 distance, Vector3 boxSize)
        {
            float disX = Math.Abs(distance.X) - boxSize.X;
            float disY = Math.Abs(distance.Y) - boxSize.Y;
            float disZ = Math.Abs(distance.Z) - boxSize.Z;
            float max = Math.Max(Math.Max(disX, disY), disZ);

            Vector3 normal = Vector3.Zero;
            if (max == disX)
            {
                if (distance.X > 0)
                {
                    normal = Vector3.Right;
                }
                else
                {
                    normal = Vector3.Left;
                }
            }
            if (max == disY)
            {
                if (distance.Y > 0)
                {
                    normal = Vector3.Up;
                }
                else
                {
                    normal = Vector3.Down;
                }
            }
            if (max == disZ)
            {
                if (distance.Z > 0)
                {
                    normal = Vector3.Backward;
                }
                else
                {
                    normal = Vector3.Forward;
                }
            }
            return Vector3.Negate(normal);
        }
    }
}
