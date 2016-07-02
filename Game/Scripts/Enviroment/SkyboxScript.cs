using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using System.Runtime.Serialization;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    class SkyboxScript : IScript
    {
        private GameObject _parent;
        public void Initialize(GameObject parent)
        {
            _parent = parent;
        }

        public void Update(GameTime gameTime)
        {
            _parent.Transform.Position = Scene.FindWithTag("MainCamera").Transform.Position;
        }
    }
}
