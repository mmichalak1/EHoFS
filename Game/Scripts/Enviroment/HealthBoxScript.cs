using OurGame.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine.Components;
using OurGame.Engine.Statics;
using OurGame.Scripts.Player;
using OurGame.Engine.Navigation.Steering;
using OurGame.Engine.ExtensionMethods;
using OurGame.Engine.Components.BoundingObjects;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    class HealthBoxScript : IScript
    {
        [DataMember]
        private GameObject _parent;
        [DataMember]
        public static int healingValue = 20;
        private float timeCounter = 0;
        [DataMember]
        private ColliderComponent physicsCollider;
        [DataMember]
        private ColliderComponent triggerCollider;
        Vector3 velocity;
        Ray ray;
        GameObject player;

        public HealthBoxScript(ColliderComponent physicsCollider, ColliderComponent triggerCollider)
        {
            this.physicsCollider = physicsCollider;
            this.triggerCollider = triggerCollider;
        }

        public void Initialize(GameObject parent)
        {
            _parent = parent;
            player = Scene.FindWithTag("MainCamera");
            velocity = Vector3.Zero;
            ray = new Ray(_parent.Transform.Position, player.Transform.Position - _parent.Transform.Position);
        }

        public void Update(GameTime gameTime)
        {
            ray.Position = _parent.Transform.Position;
            Vector3 directionToPlayer = player.Transform.Position - _parent.Transform.Position;
            directionToPlayer.Normalize();
            ray.Direction = directionToPlayer;
            Sphere collider = player.GetComponentOfType<ColliderComponent>().BoundingObject as Sphere;
            float intersects = (float)ray.Intersects(collider.getSphere());
            if (ray.Intersects(collider.getSphere()) < 500f) 
            {
                Vector3 steeringForce = SteeringBehaviour.Arrive(_parent.Transform.Position, velocity, collider.getCenter(), SteeringBehaviour.Deceleration.fast, gameTime, 250f);

                Vector3 acceleration = steeringForce / (float)gameTime.ElapsedGameTime.TotalSeconds;

                velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

                velocity = velocity.Truncate(250f);

                _parent.Transform.Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (collider.Intersects(physicsCollider.BoundingObject))
                {
                    ColliderMenager.Instance.player.Parent.GetComponentOfType<ScriptComponent>().GetScriptOfType<PlayerHealth>().Heal(healingValue);
                    _parent.Destroy = true;
                }
            }
        }
    }
}
