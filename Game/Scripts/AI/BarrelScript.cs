using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.Components;
using OurGame.Scripts.Player;
using OurGame.Engine.ParticleSystem;

namespace OurGame.Scripts.AI
{
    public class BarrelScript : IScript
    {
        private const int Damage = 20;
        private GameObject parent;
        private RigidBodyComponent rb;
        private GameObject player;
        private Vector3 directiontoPlayer;
        private List<ColliderComponent> colliders;
        private float ttl = 5000;
        private float force;
        public bool IsReleased = false;


        public void Initialize(GameObject parent)
        {
            this.parent = parent;
            player = Scene.FindWithTag("MainCamera");
            directiontoPlayer = player.Transform.Position - parent.Transform.Position;
            directiontoPlayer.Y = 0;
            force = directiontoPlayer.Length();
            directiontoPlayer.Normalize();
            rb = parent.GetComponentOfType<RigidBodyComponent>();
            colliders = parent.GetComponentsOfType<ColliderComponent>().Where(x => x.Type == ColliderTypes.Trigger).ToList();
        }

        public void Update(GameTime gameTime)
        {
            if (IsReleased)
            {
                IsReleased = false;
                rb.AddAffectingForce(directiontoPlayer * force, 0.25f);
            }
            foreach (ColliderComponent collider in colliders)
            {
                if (collider.isEnter(player))
                {
                    ParticleEmiter.Instance.AddParticle(parent.Transform.Position, "explosionRed", -parent.Transform.Orientation, 0f, 1.5f, 200f);
                    player.GetComponentOfType<ScriptComponent>().GetScriptOfType<PlayerHealth>().RecievedDamage(Damage);
                    colliders = null;
                    parent.Destroy = true;
                }
            }


            ttl -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (ttl < 0)
            {
                ParticleEmiter.Instance.AddParticle(parent.Transform.Position, "explosionRed", -parent.Transform.Orientation, 0f, 1.5f, 200f);
                colliders = null;
                parent.Destroy = true;
            }
        }
    }
}
