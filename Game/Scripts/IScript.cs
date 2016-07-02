using Microsoft.Xna.Framework;
using OurGame.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Scripts
{
    internal interface IScript
    {
        void Initialize(GameObject parent);
        void Update(GameTime gameTime);
    }
}
