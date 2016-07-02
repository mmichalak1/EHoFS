using OurGame.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SkinnedModel;
using OurGame.Engine.Components;
using Microsoft.Xna.Framework.Input;
using System.Runtime.Serialization;
using OurGame.Engine.ParticleSystem;

namespace OurGame.Scripts.Player
{
    [DataContract(IsReference = true)]
    public class Wielding : IScript
    {
        private GameObject _parent;
        private GameObject _weapon;
        private AnimationComponent _animation;
        private bool? _hasGloves = null;
        private float _timeNeededTOWearGloves;

        public bool? HasGloves
        {
            get { return _hasGloves; }
            set { _hasGloves = value; }
        }
        public GameObject Weapon
        {
            get { return _weapon; }
            set { _weapon = value; }
        }

        public void Initialize(GameObject parent)
        {
            _parent = parent;
            _timeNeededTOWearGloves = 3000;

            _animation = _parent.GetComponentOfType<AnimationComponent>();
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            if (_weapon != null)
            {
                UpdatePosition();
                _parent.GetComponentOfType<ScriptComponent>().GetScriptOfType<Attack>().Damage = 40;
            }

            if (_hasGloves == false)
            {
                PutOnGloves(gameTime);
            }
        }

        private void UpdatePosition()
        {
            Matrix[] worldTransforms = _animation.AnimatedModel.GetWorldTransforms();

            _weapon.Transform.Position = new Vector3(worldTransforms[24].M41, worldTransforms[24].M42, worldTransforms[24].M43);

            Matrix pos = Matrix.CreateTranslation(_weapon.Transform.Position);

            _weapon.Transform.Rotation = Quaternion.CreateFromRotationMatrix(worldTransforms[24]) * Quaternion.CreateFromYawPitchRoll(-MathHelper.PiOver2, -MathHelper.PiOver2, 0);
        }

        private void PutOnGloves(GameTime gameTime)
        {
            if (_timeNeededTOWearGloves > 0)
            {
                if (_timeNeededTOWearGloves > 1500)
                {
                    _timeNeededTOWearGloves -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    _animation.RotOffset *= Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver4 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                }
                else
                {
                    _animation.DiffuseMap = ContentContainer.TexColor["Gloves"];
                    _timeNeededTOWearGloves -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    _animation.RotOffset *= Quaternion.CreateFromYawPitchRoll(0, MathHelper.PiOver4 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                }
            }
            else
            {
                _hasGloves = true;
                _animation.RotOffset = Quaternion.Identity;
            }
        }
    }
}
