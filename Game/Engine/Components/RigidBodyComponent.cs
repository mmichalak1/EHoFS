using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace OurGame.Engine.Components
{
    [DataContract(IsReference = true)]
    public class RigidBodyComponent : AbstractComponent
    {
        [DataMember]
        private double _mass;
        public double Mass
        {
            get { return _mass; }
            set { _mass = value;
                if(_mass != 0 )
                _oneDivMass = 1 / _mass;
                else
                { _oneDivMass = 0; }
            }
        }

        [DataMember]
        private double _oneDivMass;
        public double OneDivMass
        { get { return _oneDivMass; } }

        [DataMember]
        private float _bounciness;
        public float Bounciness
        { get { return _bounciness; } set { _bounciness = value; } }

        private Vector3 _previousVelocity;
        public Vector3 PreviousVelocity
        { get { return _previousVelocity; } set { _previousVelocity = value; } }

        private Vector3 _velocityTMP;
        public Vector3 VelocityTMP
        { get { return _velocityTMP; } set { _velocityTMP = value; } }

        private Vector3 _velocity;
        public Vector3 Velocity
        { get { return _velocity; }
            set
            {
                if (float.IsNaN(value.X))
                    throw new Exception();
                _velocity = value;
            }
        }

        [DataMember]
        private float _coeffecient;
        public float Coeffecient
        { get { return _coeffecient; } set { _coeffecient = value; } }

        private Vector3 _affectingForce;
        public Vector3 AffectingForce
        { get { return _affectingForce; } set { _affectingForce = value; } }

        private float _timeOfAffectingForce;
        public float TimeOfAffectingForce
        { get { return _timeOfAffectingForce; } set { _timeOfAffectingForce = value; } }

        private float _time;
        public float Time
        { get { return _time; } set { _time = value; } }

        private bool _constantForce;
        public bool ConstantForce
        { get { return _constantForce; } set { _constantForce = value; } }

        [DataMember]
        private float _maximumForce;
        public float MaximumForce
        { get { return _maximumForce; } set { _maximumForce = value; } }

        [DataMember]
        private float _maximumSpeed;
        public float MaximumSpeed
        { get { return _maximumSpeed; } set { _maximumSpeed = value; } }

        [DataMember]
        private bool _affectedByGravity = true;
        public bool AffectedByGravity
        { get { return _affectedByGravity; } set { _affectedByGravity = value; } }

        protected override void LoadContent()
        {
            ResetForcesVelocity();
            base.LoadContent();
        }

        public RigidBodyComponent(GameObject parent) : base(parent)
        {
            _mass = 1f;
            _bounciness = 1f;
            _coeffecient = 0.001f;
            _maximumForce = 2000f;
            _maximumSpeed = 500f;
            _velocity = Vector3.Zero;
            _velocityTMP = Vector3.Zero;
            _affectingForce = Vector3.Zero;
            _constantForce = false;
        }

        public override void Update(GameTime time)
        {
            
        }

        public void SetAffectingForce(Vector3 force)
        {
            _affectingForce = force;
            if (_affectingForce.Length() > _maximumForce)
            {
                _affectingForce.Normalize();
                _affectingForce *= MaximumForce;
            }
        }
        public void SetAffectingForce(Vector3 force, bool constantForce)
        {
            _affectingForce = force;
            _constantForce = constantForce;
            if (_affectingForce.Length() > _maximumForce)
            {
                _affectingForce.Normalize();
                _affectingForce *= MaximumForce;
            }
            if (constantForce == false)
            {
                TimeOfAffectingForce = 1;
            }
        }
        public void SetAffectingForce(Vector3 force, float sec)
        {
            _affectingForce = force;
            _constantForce = false;
            if (_affectingForce.Length() > _maximumForce)
            {
                _affectingForce.Normalize();
                _affectingForce *= MaximumForce;
            }
            TimeOfAffectingForce = sec;
        }
        public void AddAffectingForce(Vector3 force)
        {
            _affectingForce += force;
            if(_affectingForce.Length() >_maximumForce)
            {
                _affectingForce.Normalize();
                _affectingForce *= MaximumForce;
            }
            TimeOfAffectingForce = 1;
        }
        public void AddAffectingForce(Vector3 force, float sec)
        {
            _affectingForce += force;
            if (_affectingForce.Length() > _maximumForce)
            {
                _affectingForce.Normalize();
                _affectingForce *= MaximumForce;
            }
            TimeOfAffectingForce = sec;
        }
        public void ReduceAffectingForceVelocity(float step)
        {
            if (_affectingForce.Length() > _maximumForce)
            {
                _affectingForce.Normalize();
                _affectingForce *= _maximumForce;
            }
            if (_velocity.Length() > _maximumSpeed)
            {
                _velocity.Normalize();
                _velocity *= _maximumSpeed;
            }
            if (_constantForce == false)
            {
                Time += step;
                if (Time >= TimeOfAffectingForce)
                {
                    _affectingForce = Vector3.Zero;
                    Time = 0f;
                    TimeOfAffectingForce = 0f;
                }
            } 
        }

        public void ResetForcesVelocity()
        {
            _velocity = Vector3.Zero;
            _velocityTMP = Vector3.Zero;
            _affectingForce = Vector3.Zero;
            _constantForce = false;
            TimeOfAffectingForce = 0;
        }
    }
}
