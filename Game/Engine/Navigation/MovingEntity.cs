using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Engine.Navigation
{
    [DataContract(IsReference = true)]
    public class MovingEntity
    {
        protected Vector3 _velocity;
        protected Vector3 _heading;
        protected Vector3 _side;

        [DataMember]
        protected float _mass = 20f;
        [DataMember]
        protected float _maxSpeed = 250f;
        [DataMember]
        protected float _maxForce = 250f;
        [DataMember]
        protected float _maxTurnRate = 90.0f;
        [DataMember]
        protected float _wanderRadius = 450.0f;
        [DataMember]
        protected float _wanderDistance = -750f;
        [DataMember]
        protected float _wanderJitter = 1f;
        [DataMember]
        protected float _entityWidth;
        [DataMember]
        protected float _floatingHeight;
        [DataMember]
        protected float _arrivingDistance;
        [DataMember]
        protected float _separationDistance;

        public Vector3 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }
        public Vector3 Heading
        {
            get { return _heading; }
            set { _heading = value; }
        }
        public Vector3 Side
        {
            get { return _side; }
            set { _side = value; }
        }

        public float Mass { get { return _mass; } set { _mass = value; } }
        public float MaxSpeed { get { return _maxSpeed; } set { _maxSpeed = value; } }
        public float MaxForce { get { return _maxForce; } set { _maxForce = value; } }
        public float MaxTurnRate { get { return _maxTurnRate; } }
        public float WanderRadius { get { return _wanderRadius; } }
        public float WanderDistance { get { return _wanderDistance; } }
        public float WanderJitter {  get { return _wanderJitter; } }
        public float EntityWidth { get { return _entityWidth; } }
        public float FloatingHeight
        {
            get { return _floatingHeight; }
            set { _floatingHeight = value; }
        }
        public float ArrivingDistance
        {
            get { return _arrivingDistance; }
            set { _arrivingDistance = value; }
        }
        public float SeparationDistance
        {
            get { return _separationDistance; }
            set { _separationDistance = value; }
        }
    }
}
