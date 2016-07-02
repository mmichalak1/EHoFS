using System.Linq;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.Components;
using System.Runtime.Serialization;
using OurGame.Engine.ParticleSystem;
using OurGame.Engine.Statics;
using OurGame.Scripts.AI;
using System;
using OurGame.Engine.ExtensionMethods;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    public class BulletScript : IScript
    {
        public bool particle;
        public static Random rand;
        const float LifeSpan = 5f;
        public float Power { get; set; }
        public float Speed { get; set; }
        private float time = LifeSpan;

        GameObject _parent;
        Transform myTransform;
        ColliderComponent myCollider;
        RigidBodyComponent myRigidBody;

        public void Initialize(GameObject parent)
        {
            rand = new Random();
            _parent = parent;
            time = LifeSpan;
            myTransform = parent.Transform;
            myCollider = parent.GetComponentOfType<ColliderComponent>();
            myRigidBody = parent.GetComponentOfType<RigidBodyComponent>();
            particle = true;

            _parent.Transform.Rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateConstrainedBillboard(_parent.Transform.Position, ColliderMenager.Instance.player.Parent.Transform.Position, Vector3.Up, ColliderMenager.Instance.player.Parent.Transform.Orientation, _parent.Transform.Orientation));
        }

        public void Update(GameTime gameTime)
        {

            var enemy = myCollider.getActualListOfColidingObjects().FirstOrDefault(x => x.Tag == "Enemy");
            if(particle)
            {
                ParticleEmiter.Instance.AddParticle(_parent.Transform.Position, "bullet", -_parent.Transform.Orientation, 10f, 0.1f, 10f);
                ParticleEmiter.Instance.AddParticle(_parent.Transform.Position - _parent.Transform.Orientation, "bullet", -_parent.Transform.Orientation, 10f, 0.1f, 10f);
                ParticleEmiter.Instance.AddParticle(_parent.Transform.Position, "oneZeroEffect", ExtensionMethods.RandomDirection(rand), 100f, 0.5f, 5f);
            }
            if (enemy?.Tag == "Enemy" && enemy.GetComponentOfType<ScriptComponent>() != null)
            {
                enemy.GetComponentOfType<ScriptComponent>().GetScriptOfType<IReciveDamage>().ReceiveDMG(Power);
                if(particle)
                {
                    ParticleEmiter.Instance.AddParticle(_parent.Transform.Position, "explosion", -_parent.Transform.Orientation, 0f, 1f, 50f);
                }
                _parent.Destroy = true;
            }

            time -= gameTime.ElapsedGameTime.Milliseconds * 0.001f;
            if (time < 0f)
            {
                _parent.Destroy = true;
            }
            else
            {
                //myRigidBody.AddAffectingForce(myTransform.Orientation * Speed);
                var vec = myTransform.Position;
                vec += myTransform.Orientation * Speed;
                myTransform.Position = vec;
            }
        }
    }
}
