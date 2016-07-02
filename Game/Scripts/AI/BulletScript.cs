using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.ExtensionMethods;
using OurGame.Engine.Components;
using OurGame.Engine.Statics;
using System.Runtime.Serialization;
using OurGame.Scripts.Player;
using OurGame.Engine.ParticleSystem;

namespace OurGame.Scripts.AI
{
    [DataContract(IsReference = true)]
    public class BulletScript : IScript
    {
        private GameObject _parent;
        private Vector3 _position, _direction, _velocity;
        private float _deltaTime = 0.0f;
        private float _ttl = 5.0f;
        private float _speed;
        private float _damage;
        private bool everySecond = true;
        private ColliderComponent _collider;

        public BulletScript(GameObject parent, Vector3 position, Vector3 direction, float damage)
        {
            _parent = parent;
            _position = position;
            _direction = direction;
            _direction.Normalize();
            _speed = 600.0f;
            _damage = damage;
            _collider = _parent.GetComponentOfType<ColliderComponent>();

            ModelComponent modelComponent = _parent.GetComponentOfType<ModelComponent>();
            _parent.RemoveComponent(modelComponent);
            //_parent.AddComponent(new ModelComponent(_parent, "BulletEnemy", null, true));
            //_parent.Transform.Rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateConstrainedBillboard(_parent.Transform.Position, ColliderMenager.Instance.player.Parent.Transform.Position, Vector3.Up, ColliderMenager.Instance.player.Parent.Transform.Orientation, _parent.Transform.Orientation));
        }

        public void Initialize(GameObject parent)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            if(everySecond ==true)
            {
                ParticleEmiter.Instance.AddParticle(_parent.Transform.Position, "bulletRed", -_parent.Transform.Orientation, 10f, 0.1f, 10f);
                everySecond = false;
            }
            else
            {
                everySecond = true;
            }

            _deltaTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool intersects = false;
            ColliderComponent bulletCollider = _parent.GetComponentOfType<ColliderComponent>();

            foreach (ColliderComponent x in ColliderMenager.Instance.StaticColliderList)
            {
                if (bulletCollider.BoundingObject.Intersects(x.BoundingObject))
                    intersects = true;
            }

            var entity = _collider.getActualListOfColidingObjects().Where(x => x.Tag == "MainCamera" || x.Tag == "Enemy").FirstOrDefault();
            if (entity?.Tag == "MainCamera")
            {
                ParticleEmiter.Instance.AddParticle(_parent.Transform.Position, "explosionRed", -_parent.Transform.Orientation, 0f, 1f, 50f);
                entity.GetComponentOfType<ScriptComponent>().GetScriptOfType<PlayerHealth>().RecievedDamage(10);
                entity.GetComponentOfType<RigidBodyComponent>().AddAffectingForce(Vector3.Up * 500, 0.1f); 
                _parent.Destroy = true;
            }
            if (entity?.Tag == "Enemy")
            {
                ParticleEmiter.Instance.AddParticle(_parent.Transform.Position, "explosionRed", -_parent.Transform.Orientation, 0f, 1f, 50f);
                if (entity.Name.Contains("Alien"))
                    entity.GetComponentOfType<ScriptComponent>().GetScriptOfType<Alien>().ReceiveDMG(10.0f);
                if (entity.Name.Contains("Ghost"))
                    entity.GetComponentOfType<ScriptComponent>().GetScriptOfType<Ghost>().ReceiveDMG(10.0f);
                _parent.Destroy = true;
            }
            if (_deltaTime > _ttl || intersects)
            {
                _parent.Destroy = true;
            }
            else
            {
                _velocity = _direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                _parent.Transform.Position += _velocity;
            }
        }
    }
}
