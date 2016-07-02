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
    class DeathBoxRandomizationTMPScript : IScript
    {
        public float Y;
        public float Z;
        public float X;

        public void Initialize(GameObject parent)
        {
            //throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }
    }
}
