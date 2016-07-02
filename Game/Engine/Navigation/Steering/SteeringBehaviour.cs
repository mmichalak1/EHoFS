using Microsoft.Xna.Framework;
using OurGame.Scripts.AI;
using System;
using System.Linq;
using OurGame.Engine.ExtensionMethods;
using OurGame.Engine.Components;
using OurGame.Engine.Components.BoundingObjects;
using OurGame.Engine.Statics;
using System.Collections.Generic;

namespace OurGame.Engine.Navigation.Steering
{
    public class SteeringBehaviour
    {
        private Enemy _owner;
        private Vector3 _currentTarget;
        private float refreshTime = 2.0f;
        private float delta = 1.9f;
        Vector3 targetGlobal = Vector3.Zero;
        private float maxDistanceToWall = 500f;
        private float multWallAvoidance = 1.5f;
        private float multObstacleAvoidance = 2f;
        private float multWanderer = 0.5f;
        private float multArrive = 1f;
        private float multSeparation = 2.5f;
        private Random rand;
        private List<ColliderComponent> wallsColliders;
        private List<ColliderComponent> obstacleColliders;

        public bool IsArriveOn { get; set; }
        public bool IsWandererOn { get; set; }
        public bool IsObstacleAvoidanceOn { get; set; }

        public bool IsWallAvoidanceOn { get; set; }
        public bool IsSeparationOn { get; set; }

        public SteeringBehaviour(Enemy owner)
        {
            rand = new Random(GetHashCode());
            _owner = owner;
            IsArriveOn = false;
            IsWandererOn = false;
            IsObstacleAvoidanceOn = true;
            IsWallAvoidanceOn = true;
            IsSeparationOn = true;
            //if (_owner.RoomScript != null)
            //    GetColliders();
        }

        private Vector3 Arrive(Vector3 targetPosition, Deceleration deceleration, GameTime gameTime)
        {
            Vector3 target = targetPosition - _owner.Parent.Transform.Position;
            float dist = target.Length() - _owner.ArrivingDistance;
            target = target.Truncate(dist);
            dist = target.Length();

            if (dist > 0)
            {
                const float decelerationTweaker = 0.3f;
                float speed = (dist / ((float)deceleration * decelerationTweaker)) * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                speed = speed.Min(_owner.MaxSpeed);
                Vector3 DesiredVelocity = target * speed / dist;
                return (DesiredVelocity - _owner.Velocity);
            }
            return new Vector3(0, 0, 0);
        }

        public static Vector3 Arrive(Vector3 position, Vector3 velocity, Vector3 targetPosition, Deceleration deceleration, GameTime gameTime, float maxSpeed)
        {
            Vector3 target = targetPosition - position;
            float dist = target.Length();
            target = target.Truncate(dist);
            dist = target.Length();

            if (dist > 0)
            {
                const float decelerationTweaker = 0.3f;
                float speed = (dist / ((float)deceleration * decelerationTweaker)) * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                speed = speed.Min(maxSpeed);
                Vector3 DesiredVelocity = target * speed / dist;
                return (DesiredVelocity - velocity);
            }
            return new Vector3(0, 0, 0);
        }

        private Vector3 Wander(GameTime gameTime)
        {
            delta += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (delta > refreshTime)
            {
                delta = 0;
                Vector3 wanderTarget = Vector3.Zero;
                float rand1 = RandomClamped();
                float rand2 = RandomClamped();
                wanderTarget = new Vector3(rand1 * _owner.WanderJitter, 0, rand2 * _owner.WanderJitter);
                wanderTarget.Normalize();
                wanderTarget *= _owner.WanderRadius;

                Vector3 targetLocal = new Vector3(wanderTarget.X, 0, wanderTarget.Z + _owner.WanderDistance);
                //targetLocal = Vector3.Transform(targetLocal, _owner.Parent.Transform.Rotation);
                targetGlobal = PointToWorld(targetLocal);
            }

            return targetGlobal - _owner.Parent.Transform.Position;
        }

        private Vector3 ObstacleAvoidance()
        {
            Vector3 result = Vector3.Zero;
            RayCast[] raysLeft =
            {
                new RayCast(_owner.Parent, new Vector3(_owner.EntityWidth, -(_owner.FloatingHeight - 100f) + 15, 0), _owner.Parent.Transform.Orientation)
            };
            RayCast[] raysRight =
            {
                new RayCast(_owner.Parent, new Vector3(-_owner.EntityWidth, -(_owner.FloatingHeight - 100f) + 15, 0), _owner.Parent.Transform.Orientation)
            };

            //Closest Intersecting Obstacle Distance
            float? CIODisance = null;
            float? CIODisanceL = null;
            float? CIODisanceR = null;

            //this will record the transformed local coordinates of the CIB
            bool turn = false;

            foreach (RayCast y in raysLeft)
                foreach (ColliderComponent x in obstacleColliders)
                {
                    Box box = x.BoundingObject as Box;
                    float? intersects = y.Ray.Intersects(box.getBox());
                    if (intersects != null)
                        if ((CIODisanceL > intersects && intersects >= 0) || CIODisanceL == null)
                            CIODisanceL = intersects;
                }

            foreach (RayCast y in raysRight)
                foreach (ColliderComponent x in obstacleColliders)
                {
                    Box box = x.BoundingObject as Box;
                    float? intersects = y.Ray.Intersects(box.getBox());
                    if (intersects != null)
                        if ((CIODisanceR > intersects && intersects >= 0) || CIODisanceR == null)
                            CIODisanceR = intersects;
                }

            if (CIODisanceL != null && CIODisanceR != null)
            {
                if (CIODisanceL < CIODisanceR)
                {
                    CIODisance = CIODisanceL;
                    turn = false;
                }
                else
                {
                    CIODisance = CIODisanceR;
                    turn = true;
                }
            }
            else
            {
                if (CIODisanceL != null)
                {
                    CIODisance = CIODisanceR;
                    turn = true;
                }
                else
                {
                    CIODisance = CIODisanceL;
                    turn = false;
                }
            }
            //if (_owner.Parent.Name == "Alien")
            //{
            //    Debug.LogOnScreen(_owner.Parent.Name + "  Ciodistance: " + CIODisance, Debug.ScreenType.Logic, new Vector2(10, 300));
            //    Debug.LogOnScreen(_owner.Parent.Name + "  Orientation: " + _owner.Parent.Transform.Orientation, Debug.ScreenType.Logic, new Vector2(10, 320));
            //}
            //else
            //{
            //    Debug.LogOnScreen(_owner.Parent.Name + "  Ciodistance: " + CIODisance, Debug.ScreenType.Logic, new Vector2(10, 360));
            //    Debug.LogOnScreen(_owner.Parent.Name + "  Orientation: " + _owner.Parent.Transform.Orientation, Debug.ScreenType.Logic, new Vector2(10, 380));
            //}

            if (CIODisance != null)
            {
                float? x;
                if (CIODisance < 500 && CIODisance > 200)
                {
                    if (CIODisance != 0)
                        x = (CIODisance * 10000) / 500;
                    else
                        x = 500;

                    result = _owner.Parent.Transform.Orientation.PerpendicularVector3YAxisConstrained();

                    if (turn)
                        result *= (float)x;
                    else
                        result *= -(float)x;
                }
                if (CIODisance < 200)
                    result = -_owner.Parent.Transform.Orientation * 250;
            }

            return result;
        }

        public void Floating(GameTime gameTime)
        {

            bool turn = false;
            float min = 180f;
            float max = 220f;

            if (turn)
            {
                if (_owner.Parent.Transform.Position.Y < max)
                    _owner.FloatingHeight += 30 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                    turn = false;
            }
            else
            {
                if (_owner.Parent.Transform.Position.Y > min)
                    _owner.FloatingHeight -= 30 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                    turn = true;
            }

        }

        private Vector3 WallAvoidance(GameTime gameTime)
        {
            RayCast[] rayCasts =
            {
                new RayCast(_owner.Parent, new Vector3(0, 100, 0), _owner.Parent.Transform.Orientation.RotateVector(MathHelper.PiOver4)),
                new RayCast(_owner.Parent, new Vector3(0, 100, 0), _owner.Parent.Transform.Orientation),
                new RayCast(_owner.Parent, new Vector3(0, 100, 0), _owner.Parent.Transform.Orientation.RotateVector(-MathHelper.PiOver4))
            };

            float?[] distances = new float?[rayCasts.Length];
            ColliderComponent[] walls = new ColliderComponent[rayCasts.Length];

            for (int i = 0; i < rayCasts.Length; i++)
            {
                foreach (ColliderComponent x in wallsColliders)
                {
                    Box box = x.BoundingObject as Box;
                    float? intersects = rayCasts[i].Ray.Intersects(box.getBox());
                    if (intersects != null)
                    {
                        distances[i] = intersects;
                        walls[i] = x;
                    }
                }
            }

            Vector3 steeringForce = Vector3.Zero;

            for (int i = 0; i < rayCasts.Length; i++)
            {
                if (distances[i] < maxDistanceToWall)
                {
                    if (walls[i].Parent.Transform.Orientation.GetAngleFromVectors(_owner.Parent.Transform.Orientation) <= MathHelper.PiOver2)
                        steeringForce += -walls[i].Parent.Transform.Orientation * (maxDistanceToWall - (float)distances[i]);
                    else
                        steeringForce += walls[i].Parent.Transform.Orientation * (maxDistanceToWall - (float)distances[i]);
                }
            }
            return steeringForce;
        }

        Vector3 Separation()
        {
            List<GameObject> neighbors = Scene.FindObjectsWithTag("Enemy");
            Vector3 steeringForce = Vector3.Zero;
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i] != _owner.Parent && (_owner.Parent.Transform.Position - neighbors[i].Transform.Position).Length() < _owner.SeparationDistance)
                {
                    Vector3 ToAgent = _owner.Parent.Transform.Position - neighbors[i].Transform.Position;
                    float length = ToAgent.Length();
                    ToAgent.Normalize();
                    steeringForce += (ToAgent / length) * 10000;
                }
            }
            return steeringForce;
        }

        public Vector3 Calculate(GameTime gameTime)
        {
            Vector3 steeringForce = Vector3.Zero;
            Vector3 force = Vector3.Zero;

            if (IsWallAvoidanceOn && wallsColliders != null)
            {
                force = WallAvoidance(gameTime) * multWallAvoidance;
                if (!AccumulateForce(ref steeringForce, force)) return steeringForce;
            }

            if (IsObstacleAvoidanceOn)
            {
                force = ObstacleAvoidance() * multObstacleAvoidance;
                if (!AccumulateForce(ref steeringForce, force)) return steeringForce;
            }

            if (IsSeparationOn)
            {
                force = Separation() * multSeparation;
                if (!AccumulateForce(ref steeringForce, force)) return steeringForce;
            }

            if (IsWandererOn)
            {
                force = Wander(gameTime) * multWanderer;
                if (!AccumulateForce(ref steeringForce, force)) return steeringForce;
            }

            if (IsArriveOn)
            {
                force = Arrive(_currentTarget, Deceleration.normal, gameTime) * multArrive;
                if (!AccumulateForce(ref steeringForce, force)) return steeringForce;
            }

            return steeringForce;
        }

        public void GetColliders()
        {
            wallsColliders = new List<ColliderComponent>();
            wallsColliders.Add(_owner.RoomScript.DownWall.GetComponentOfType<ColliderComponent>());
            wallsColliders.Add(_owner.RoomScript.UpWall.GetComponentOfType<ColliderComponent>());
            wallsColliders.Add(_owner.RoomScript.RightWall.GetComponentOfType<ColliderComponent>());
            wallsColliders.Add(_owner.RoomScript.LeftWall.GetComponentOfType<ColliderComponent>());

            obstacleColliders = new List<ColliderComponent>();
            foreach (GameObject x in _owner.RoomScript.Parent.FindWithName("Floor").Children)
            {
                ColliderComponent collider = x.GetComponentOfType<ColliderComponent>();
                if (collider != null)
                    if (collider.Type == ColliderTypes.Static)
                        obstacleColliders.Add(collider);
            }

        }


        private bool AccumulateForce(ref Vector3 RunningTot, Vector3 ForceToAdd)
        {

            float MagnitudeSoFar = RunningTot.Length();

            float MagnitudeRemaining = _owner.MaxForce - MagnitudeSoFar;

            if (MagnitudeRemaining <= 0.0) return false;

            float MagnitudeToAdd = ForceToAdd.Length();

            if (MagnitudeToAdd < MagnitudeRemaining)
            {
                RunningTot += ForceToAdd;
            }
            else
            {
                ForceToAdd.Normalize();
                RunningTot += (ForceToAdd * MagnitudeRemaining);
            }
            return true;
        }

        public void SetPath()
        {

        }

        public void SetTarget(Vector3 vector)
        {
            _currentTarget = vector;
            _currentTarget.Y = _owner.FloatingHeight;
        }

        private float RandomClamped()
        {
            return rand.NextFloat();
        }

        public void TurnToTarget(Vector3 targetPosition)
        {
            //Vector3 toTarget = (_owner.Parent.Transform.Position - targetPosition);
            if (targetPosition != Vector3.Zero || targetPosition != null)
                _owner.Parent.Transform.Rotation = _owner.Parent.Transform.Rotation.TurnTowardsTheVector(targetPosition, 0.05f);
        }


        public enum Deceleration
        {
            slow,
            normal,
            fast
        }

        private Vector3 PointToWorld(Vector3 localPoint)
        {
            Matrix World = Matrix.CreateScale(
                    _owner.Parent.Transform.Scale.X,
                    _owner.Parent.Transform.Scale.Y,
                    _owner.Parent.Transform.Scale.Z) *
                Matrix.CreateFromQuaternion(-_owner.Parent.Transform.Rotation) *
                Matrix.CreateTranslation(_owner.Parent.Transform.Position);
            //localPoint *= -1;
            //localPoint.Y *= -1;

            Vector3 result = Vector3.Transform(localPoint, World);

            return Vector3.Transform(localPoint, World);
        }

    }
}
