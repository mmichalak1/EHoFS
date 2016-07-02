using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.Components;
using OurGame.Engine.ExtensionMethods;
using OurGame.Scripts.AI.StateMachine;
using OurGame.Scripts.AI.StateMachine.States;
using OurGame.Scripts.Player;
using System;
using System.Linq;
using System.Runtime.Serialization;
using static OurGame.Engine.Navigation.Steering.SteeringBehaviour;

namespace OurGame.Scripts.AI
{
    [DataContract(IsReference = true)]
    public class Ghost : Enemy
    {
        private SoundComponent _sound;
        private StateMachine<Ghost> _stateMachine;
        [DataMember]
        private float _performAttackTime;
        [DataMember]
        private float _attackInteraval;

        public StateMachine<Ghost> StateMachine
        {
            get
            {
                return _stateMachine;
            }
        }
        public Ghost(float health, float speed, int damage) : base(health, speed, damage)
        {
            _floatingHeight = 200f;
            _arrivingDistance = 200f;
            _separationDistance = 200f;
            _performAttackTime = 200;
            _attackInteraval = 3000f;
        }

        public override void Initialize(GameObject parent)
        {
            base.Initialize(parent);
            _entityWidth = 175f * Parent.Transform.Scale.X;
            _stateMachine = new StateMachine<Ghost>(this);
            _stateMachine.CurrentState = StatePatrol.Instance;
            _steering.IsWandererOn = true;
            _steering.IsObstacleAvoidanceOn = true;
            _sound = _parent.GetComponentOfType<SoundComponent>();
            enemySound = "ghostMovement";
        }


        public override void UpdateForces(GameTime gameTime)
        {
            if (ScreenManager.IsAIEnabled)
            {
                Vector3 steeringForce = _steering.Calculate(gameTime);

                Vector3 acceleration = steeringForce / (_mass * (float)gameTime.ElapsedGameTime.TotalSeconds);

                _velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

                _velocity = _velocity.Truncate(_maxSpeed);

                _sound.Play(enemySound, false, _parent.Transform.Position);

                _parent.Transform.Position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_velocity.Y > 200)
                    _velocity += Vector3.Zero;

                if (_velocity.LengthSquared() > 0.0000001)
                {
                    _heading = _velocity;
                    _heading.Normalize();

                    _side = _heading.PerpendicularVector3YAxisConstrained();
                }
                if (_stateMachine.CurrentState == StateAttack.Instance)
                    _steering.TurnToTarget(_player.Transform.Position - _parent.Transform.Position);
                else
                    _steering.TurnToTarget(_velocity);
                _stateMachine.Update(gameTime);
            }
        }

        private Vector3 CorrectHeight()
        {
            Vector3 force = Vector3.Zero;

            force.Y = (_floatingHeight - _parent.Transform.Position.Y) * 100f;

            return force;
        }

        public void Attack(GameTime gameTime)
        {
            Vector3 helper = Vector3.Zero;
            Vector3 force = Vector3.Zero;
            _attackInteraval -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_attackInteraval <= 0)
            {
                _performAttackTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_performAttackTime > 0)
                {
                    force = Vector3.Transform(_parent.Transform.Orientation, Quaternion.CreateFromYawPitchRoll(0, MathHelper.PiOver4, 0)) * 1000f; ;
                    _parent.Transform.Position += force * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_performAttackTime < 100 && _performAttackTime > 80)
                        if ((_parent.Transform.Position - _player.Transform.Position).Length() < 200f)
                        { 
                            _player.GetComponentOfType<ScriptComponent>().GetScriptOfType<PlayerHealth>().RecievedDamage(_damage);
                            _player.GetComponentOfType<RigidBodyComponent>().AddAffectingForce(Vector3.Up * 100, 0.1f); ///////////////////////////tu zmienilem 
                        } 
                }
                else
                {
                    _performAttackTime = 200;
                    _attackInteraval = 3000;
                }
            }
        }
    }
}
