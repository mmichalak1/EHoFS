using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using OurGame.Engine.ExtensionMethods;
using OurGame.Engine;
using OurGame.Engine.Components;
using System.Linq;
using OurGame.Engine.ParticleSystem;
using System;

namespace OurGame.Scripts.Player
{
    [DataContract(IsReference = true)]
    class Shooting : IScript
    {
        const float MAXCHARGE = 100f;
        const float CHARGEINCREMENT = 66f;
        private float _coolDown = 5000f;
        Vector3 OFFSET = new Vector3(-20f, -5f, -70f);

        private AnimationComponent _animation;
        private GameObject _parent;
        private Wielding _wielding;
        private SoundComponent _soundComponent;
        private Random rand;
        private bool _startShooting = false;
        private double time = 0;

        private float _currentCharge;


        public void Initialize(GameObject parent)
        {
            rand = new Random();
            _parent = parent;
            _animation = _parent.GetComponentOfType<AnimationComponent>();
            _wielding = _parent.GetComponentOfType<ScriptComponent>().GetScriptOfType<Wielding>();
            _currentCharge = 0f;
            _soundComponent = _parent.GetComponentOfType<SoundComponent>();
        }

        public void Update(GameTime gameTime)
        {
            if (_coolDown <= 0)
            {
                _coolDown = 0;
                
                if (InputManager.GetMouseButtonDown(MouseButton.Right) && _wielding.HasGloves == true)
                {
                    time += gameTime.ElapsedGameTime.Milliseconds;
                    if(time >250)
                    {
                        Matrix[] worldTransforms = _animation.AnimatedModel.GetWorldTransforms();

                        Vector3 position = new Vector3(worldTransforms[28].M41, worldTransforms[28].M42, worldTransforms[28].M43) + _parent.Transform.Orientation * 35;
                        Vector3 positionRand = position + ExtensionMethods.RandomDirection(rand) * 20;

                        ParticleEmiter.Instance.AddParticle(positionRand, "chargingPower", Vector3.Normalize(position - positionRand), 8f, 1.5f, 2.5f);
                        time = 0;
                    }
                    if (!_startShooting)
                    {
                        _animation.ChangeClip("ShootStart");
                        _startShooting = true;
                    }
                    if (!_animation.IsPlaying)
                        _animation.ChangeClip("ShootCharging");
                    _currentCharge += CHARGEINCREMENT * (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
                    _soundComponent.Play("chargeShot", false);
                    if (_currentCharge > MAXCHARGE)
                        _currentCharge = MAXCHARGE;
                }
                if (InputManager.GetMouseButtonReleased(MouseButton.Right) && _wielding.HasGloves == true)
                {
                    _animation.Stop();
                    _animation.ChangeClip("ShootRelease");
                    Shoot(_currentCharge);
                    _coolDown = 5000f;
                    _currentCharge = 10f;
                    _soundComponent.Play("chargedShot",false);
                }
            }
            else
            {
                _coolDown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }


        private void Shoot(float Power)
        {
            var bullet = PrefabManager.GetPrafabClone("Bullet");
            Enviroment.BulletScript scr = bullet.GetComponentOfType<ScriptComponent>().GetScriptOfType<Enviroment.BulletScript>();
            scr.Power = Power;
            scr.Speed = 40f;
            bullet.Transform.Rotation = _parent.Transform.Rotation.Copy();
            bullet.Transform.Position = _parent.Transform.Position + Vector3.Transform(OFFSET, bullet.Transform.Rotation);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(bullet);
            _soundComponent.Play("shot-01", false);
        }
    }
}
