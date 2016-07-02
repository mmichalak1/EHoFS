using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Scripts.AI.StateMachine;
using OurGame.Engine.ExtensionMethods;
using OurGame.Scripts.AI.StateMachine.States;
using OurGame.Engine.Components;
using System.Runtime.Serialization;
using OurGame.Engine.Components.BoundingObjects;
using System;

namespace OurGame.Scripts.AI
{
    [DataContract(IsReference = true)]
    public class Alien : Enemy
    {
        private SoundComponent _sound;
        private StateMachine<Alien> _stateMachine;
        [DataMember]
        private float _shootInterval;

        public StateMachine<Alien> StateMachine
        {
            get
            {
                return _stateMachine;
            }
        }
        public Alien(float health, float speed, int damage) : base(health, speed, damage)
        {
            _floatingHeight = 200f;
            _arrivingDistance = 750f;
            _separationDistance = 250f;
            _shootInterval = 500f;
        }

        public override void Initialize(GameObject parent)
        {
            base.Initialize(parent);
            _entityWidth = 138f * Parent.Transform.Scale.X;
            _stateMachine = new StateMachine<Alien>(this);
            _stateMachine.CurrentState = StatePatrol.Instance;
            _steering.IsWandererOn = true;
            _steering.IsObstacleAvoidanceOn = true;
            _sound = _parent.GetComponentOfType<SoundComponent>();
            enemySound = "invaderMovement";
        }

        public override void UpdateForces(GameTime gameTime)
        {

            if (ScreenManager.IsAIEnabled)
            {
                Vector3 steeringForce = _steering.Calculate(gameTime);
                steeringForce.Y = (_floatingHeight - _parent.Transform.Position.Y) * 100f;
                Vector3 acceleration = steeringForce / (_mass * (float)gameTime.ElapsedGameTime.TotalSeconds);

                _velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

                _velocity = _velocity.Truncate(_maxSpeed);

                _sound.Play(enemySound, false, _parent.Transform.Position);

                _parent.Transform.Position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

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

        public bool PlayerInRangeOfFire()
        {
            if ((_player.Transform.Position - _parent.Transform.Position).Length() < 1000)
                return true;
            else
                return false;
        }

        public void Shoot(GameTime gameTime, bool leftHand)
        {
            _shootInterval -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_shootInterval < 0)
            {
                _shootInterval = _parent.GetComponentOfType<SimpleAnimationComponent>().ChangeModelInterval;
                Vector3 bulletOffset = new Vector3(133f, -30f, 34f);
                if (leftHand)
                    bulletOffset.X *= -1;
                Vector3 bulletPosition = ExtensionMethods.PointToWorld(bulletOffset, _parent.Transform);
                CreateBullet(bulletPosition, _player.Transform.Position - bulletPosition);
                _audio.Play("shot-02", false, _parent.Transform.Position);
            }
        }

        private void CreateBullet(Vector3 position, Vector3 direction)
        {
            GameObject go = new GameObject(position, Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(go);
            go.Name = "Bullet";
            go.AddComponent(new ModelComponent(go, "Bullet", null, true));
            go.AddComponent(new ColliderComponent(go, new Sphere(go, Vector3.Zero, 1f), ColliderTypes.Normal));
            go.AddComponent(new ScriptComponent(go));
            go.GetComponentOfType<ScriptComponent>().AddScript(new BulletScript(go, position, direction, _damage));
            go.Transform.Scale = new Scale(10, 10, 10);
        }
    }
}
