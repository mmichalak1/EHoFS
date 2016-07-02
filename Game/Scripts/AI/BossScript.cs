using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.ExtensionMethods;
using System.Runtime.Serialization;
using OurGame.Engine.Components;
using OurGame.Engine.Components.BoundingObjects;
using OurGame.Scripts.Enviroment;
using OurGame.Scripts.Player;

namespace OurGame.Scripts.AI
{
    [DataContract(IsReference = true)]
    public class BossScript : IScript, IReciveDamage
    {
        private GameObject parent;
        private GameObject player;
        private bool holdingBarrel;
        private SimpleAnimationComponent simpleAnimationComponent;
        private SoundComponent audio;
        private GameObject projectile;
        private float health;
        private float maxHealth;
        private float stunTime;
        private float forceTime;
        private ColliderComponent collider;
        public bool IsStunned;
        private bool isRecievingDamage;
        protected float recievingDamageTime;

        public void Initialize(GameObject parent)
        {
            this.parent = parent;
            maxHealth = 600;
            health = 600;
            player = Scene.FindWithTag("MainCamera");
            holdingBarrel = false;
            IsStunned = false;
            isRecievingDamage = false;
            recievingDamageTime = 100f;
            stunTime = 15000;
            forceTime = 3f;
            audio = parent.GetComponentOfType<SoundComponent>();
            collider = parent.GetComponentOfType<ColliderComponent>();
            simpleAnimationComponent = parent.GetComponentOfType<SimpleAnimationComponent>();
            EventSystem.Instance.Send("BossSpawned", this);
        }

        public void Update(GameTime gameTime)
        {
            HealthBar.Instance.Update(gameTime);
            if (player.GetComponentOfType<ScriptComponent>().GetScriptOfType<MovementStable>().MovingEnabled == false)
                if (forceTime < 0)
                {
                    player.GetComponentOfType<ScriptComponent>().GetScriptOfType<MovementStable>().MovingEnabled = true;
                    forceTime = 3f;
                }
                else
                {
                    forceTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            if (isRecievingDamage)
            {
                recievingDamageTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (recievingDamageTime > 0)
                {
                    parent.GetComponentOfType<ModelComponent>().DiffuseColorChanger = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
                }
                else
                {
                    isRecievingDamage = false;
                    recievingDamageTime = 100f;
                    parent.GetComponentOfType<ModelComponent>().DiffuseColorChanger = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                }
            }
            if (!IsStunned)
            {
                Vector3 target = player.Transform.Position - parent.Transform.Position;
                target.Y = parent.Transform.Position.Y;
                ExtensionMethods.TurnToTarget(ref parent, target);

                if (simpleAnimationComponent.CurrentModel == 2 && !holdingBarrel)
                {
                    CreateBarrel();
                    holdingBarrel = true;
                    audio.Play("dkMove", false, parent.Transform.Position);
                }
                if (simpleAnimationComponent.CurrentModel == 3 && holdingBarrel)
                {
                    if(projectile != null)
                    {
                        if (projectile.GetComponentOfType<RigidBodyComponent>() != null)
                        {
                            projectile.GetComponentOfType<RigidBodyComponent>().AffectedByGravity = true;
                            holdingBarrel = false;
                            projectile.GetComponentOfType<ScriptComponent>().GetScriptOfType<BarrelScript>().IsReleased = true;
                            projectile = null;
                            audio.Play("barrelThrow", false, parent.Transform.Position);
                        }
                        audio.Play("barrelThrow", false, parent.Transform.Position);
                    }
                }
            }
            else
            {
                stunTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (stunTime < 0)
                {
                    foreach (GameObject x in parent.Parent.Parent.GetComponentOfType<ScriptComponent>().GetScriptOfType<BossRoomScript>().Targets)
                    {
                        x.GetComponentOfType<ScriptComponent>().GetScriptOfType<TargetScript>().IsActive = false;
                        x.GetComponentOfType<ScriptComponent>().GetScriptOfType<TargetScript>().IsRotating = true;
                    }
                    stunTime = 15000;
                    IsStunned = false;
                    ForcePush();
                }
            }
        }
        private void CreateBarrel()
        {
            GameObject barrel = new GameObject(parent.Transform.Position + new Vector3(0, 1000 * parent.Transform.Scale.Y, 0), parent.Transform.Rotation);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(barrel);
            barrel.Tag = "Projectile";
            barrel.Name = "Barrel";
            barrel.AddComponent(new ModelComponent(barrel, "Barrel", "Default", false));
            barrel.AddComponent(new RigidBodyComponent(barrel));
            barrel.GetComponentOfType<RigidBodyComponent>().AffectedByGravity = false;
            barrel.GetComponentOfType<RigidBodyComponent>().Mass = 1;
            barrel.GetComponentOfType<RigidBodyComponent>().Bounciness = 0.3f;
            barrel.GetComponentOfType<RigidBodyComponent>().Coeffecient = 0.002f;
            barrel.GetComponentOfType<RigidBodyComponent>().MaximumForce = 5000f;
            barrel.AddComponent(new ColliderComponent(barrel, new Sphere(barrel, Vector3.Zero, 50), ColliderTypes.Trigger));
            barrel.AddComponent(new ColliderComponent(barrel, new Sphere(barrel, Vector3.Zero, 70), ColliderTypes.Physics));
            //barrel.AddComponent(new ColliderComponent(barrel, new Sphere(barrel, new Vector3(15, 0, 0), 30), ColliderTypes.Trigger));
            barrel.AddComponent(new ScriptComponent(barrel));
            barrel.GetComponentOfType<ScriptComponent>().AddScript(new BarrelScript());
            barrel.Transform.Scale = new Scale(8 * parent.Transform.Scale.X, 8 * parent.Transform.Scale.Y, 8 * parent.Transform.Scale.Z);
            foreach (ColliderComponent colliderComponent in barrel.GetComponentsOfType<ColliderComponent>())
            {
                colliderComponent.Initialize();
            }
            barrel.GetComponentOfType<ScriptComponent>().Initialize();
            projectile = barrel;
        }

        public void ReceiveDMG(float DMG)
        {
            health -= DMG;
            isRecievingDamage = true;
            audio.Play("enemyGotHit", false, parent.Transform.Position);
            if (health <= 0)
            {
                Scene.FindWithName("RoomBoss").GetComponentOfType<ScriptComponent>().GetScriptOfType<RoomScript>().DeleteEnemy(parent);
                EventSystem.Instance.Send("BossIsDead", null);
                parent.Destroy = true;
                audio.Play("enemyOnDeath", false, parent.Transform.Position);
            }
        }

        public void ForcePush()
        {
            Vector3 force;
            player.GetComponentOfType<ScriptComponent>().GetScriptOfType<MovementStable>().MovingEnabled = false;
            force = player.Transform.Position - parent.Transform.Position;
            force.Y = 0.1f;
            force.Normalize();
            force *= 15000f;
            player.GetComponentOfType<RigidBodyComponent>().AddAffectingForce(force, forceTime);
        }

        public float getCurHealth()
        {
            return health;
        }

        //public float getMaxHealth()
        //{
        //    return maxHealth;
        //}

        //public Vector3 getBossPosition()
        //{
        //    return parent.Transform.Position;
        //}
    }
}
