using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using System.Runtime.Serialization;
using OurGame.Engine.Components;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    public class FanScript : IScript
    {
        private SoundComponent _sound;
        GameObject _parent;
        public void Initialize(GameObject parent)
        {
            _parent = parent;
            _sound = _parent.GetComponentOfType<SoundComponent>();
        }

        public void Update(GameTime gameTime)
        {
            if(_parent.Name == "FanOnly")
            {
                _parent.Transform.Rotation *= Quaternion.CreateFromYawPitchRoll(0, 0, (float)Math.PI * (float)gameTime.ElapsedGameTime.TotalSeconds);
                //_sound.Play("fan", true,_parent.Transform.Position);
            }
        }
    }
}
