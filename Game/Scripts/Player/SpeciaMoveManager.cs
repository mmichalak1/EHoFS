using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;

using OurGame.Scripts.Player.SpecialMoves;
using OurGame.Engine.Components;
using System;
using OurGame.Engine.ExtensionMethods;

namespace OurGame.Scripts.Player
{
    [KnownType(typeof(Dash))]
    [DataContract]
    public class SpeciaMoveManager : IScript
    {
        public GameObject Parent;

        private AbstractSpecial activeMovement;

        private AnimationComponent _animation;

        private SoundComponent sound;

        private Wielding _wielding;

        private Random _rand;

        private string[] _swing = { "swing-01", "swing-02", "swing-03" };

        public bool isDashOnCooldown = false;

        float dashCooldownTimer = 0f;


        const float dashTime = 0.2f;
        const float dashCooldown = 1f;
        public float speedAndForceMultipler = 15f;


        public void Initialize(GameObject parent)
        {
            Parent = parent;
            _rand = new Random();
            _animation = parent.GetComponentOfType<AnimationComponent>();
            sound = parent.GetComponentOfType<SoundComponent>();
            _wielding = parent.GetComponentOfType<ScriptComponent>().GetScriptOfType<Wielding>();

            EventSystem.Instance.RegisterForEvent("PerformSpecialMove", x =>
            {
                if (activeMovement == null)
                {
                    switch ((Moves)x)
                    {
                        case Moves.Dash:
                            if (!isDashOnCooldown)
                            {
                                activeMovement = new Dash(dashTime, this);
                                Parent.GetComponentOfType<RigidBodyComponent>().MaximumSpeed *= speedAndForceMultipler;
                                Parent.GetComponentOfType<RigidBodyComponent>().MaximumForce *= speedAndForceMultipler;
                            }
                            break;
                        case Moves.LeftSlash:
                            
                            if (!_animation.IsPlaying && _wielding.Weapon != null)
                            {
                                activeMovement = new Slash(833, this, "Slash_L2R_D2U");
                                int i = _rand.Next(_swing.Length);
                                sound.Play(_swing[i], false);
                            }
                            if (Gun.Instance.gunActive)
                            {
                                Gun.Instance.Fire();
                                var bullet = PrefabManager.GetPrafabClone("Bullet");
                                Enviroment.BulletScript scr = bullet.GetComponentOfType<ScriptComponent>().GetScriptOfType<Enviroment.BulletScript>();
                                scr.Power = 1f;
                                scr.Speed = 200f;
                                scr.particle = false;
                                bullet.Transform.Rotation = Parent.Transform.Rotation.Copy();
                                bullet.Transform.Position = Parent.Transform.Position + Vector3.Transform(new Vector3(-20f, -5f, -70f), bullet.Transform.Rotation);
                                ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(bullet);
                            }
                            break;
                        case Moves.RightSlash:
                            if (!_animation.IsPlaying && _wielding.Weapon != null)
                            {
                                activeMovement = new Slash(833, this, "Slash_R2L_D2U");
                                int i = _rand.Next(_swing.Length);
                                sound.Play(_swing[i], false);
                            }
                            if (Gun.Instance.gunActive)
                            {
                                Gun.Instance.Fire();
                                var bullet = PrefabManager.GetPrafabClone("Bullet");
                                Enviroment.BulletScript scr = bullet.GetComponentOfType<ScriptComponent>().GetScriptOfType<Enviroment.BulletScript>();
                                scr.Power = 1f;
                                scr.Speed = 200f;
                                scr.particle = false;
                                bullet.Transform.Rotation = Parent.Transform.Rotation.Copy();
                                bullet.Transform.Position = Parent.Transform.Position + Vector3.Transform(new Vector3(-20f, -5f, -70f), bullet.Transform.Rotation);
                                ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(bullet);
                            }
                            break;
                        case Moves.UpperSlash:
                            if (!_animation.IsPlaying && _wielding.Weapon != null)
                            {
                                activeMovement = new Slash(833, this, "UpperSlash");
                                int i = _rand.Next(_swing.Length);
                                sound.Play(_swing[i], false);
                            }
                            if (Gun.Instance.gunActive)
                            {
                                Gun.Instance.Fire();
                                var bullet = PrefabManager.GetPrafabClone("Bullet");
                                Enviroment.BulletScript scr = bullet.GetComponentOfType<ScriptComponent>().GetScriptOfType<Enviroment.BulletScript>();
                                scr.Power = 1f;
                                scr.Speed = 200f;
                                scr.particle = false;
                                bullet.Transform.Rotation = Parent.Transform.Rotation.Copy();
                                bullet.Transform.Position = Parent.Transform.Position + Vector3.Transform(new Vector3(-20f, -5f, -70f), bullet.Transform.Rotation);
                                ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(bullet);
                            }
                            break;
                        default:
                            break;
                    }
                }

            });
        }

        public void Update(GameTime gameTime)
        {
            TickCooldowns(gameTime);

            if (activeMovement != null)
                activeMovement.Update(gameTime);
        }

        public void StopMove()
        {
            activeMovement = null;
        }

        private void TickCooldowns(GameTime time)
        {
            if (isDashOnCooldown)
            {
                dashCooldownTimer += (float)time.ElapsedGameTime.TotalSeconds;
                if (dashCooldownTimer > dashCooldown)
                {
                    isDashOnCooldown = false;
                    dashCooldownTimer = 0f;
                }
            }
        }
    }
}
