using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace OurGame.Engine.Components
{
    [DataContract(IsReference =true)]
    class AudioComponent : AbstractComponent
    {
        private List<SoundEffect> _soundEffects;

        public List<SoundEffect> SoundEffects
        {
            get { return _soundEffects; }
            private set { _soundEffects = value; }
        }

        private string[] _myEffects =
        {
            "deded","dedede"
        };


        #region Basic Methods
        public AudioComponent(GameObject parent) : base(parent)
        {
            Initialize();
        }

        public override void LoadContent(ContentManager Content)
        {
            foreach (string x in _myEffects)
            {
                SoundEffects.Add(Content.Load<SoundEffect>(x));
            }
            base.LoadContent(Content);
        }

        public override void Initialize()
        {
            SoundEffects = new List<SoundEffect>();
        }

        public override void Update(GameTime time)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
