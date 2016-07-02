using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using Microsoft.Xna.Framework.Input;
using System.Runtime.Serialization;

namespace OurGame.Scripts.AI
{
    [DataContract(IsReference = true)]
    public class TwoDEnemy : IScript, IReciveDamage
    {
        [DataMember]
        private GameObject _parent;
        public void Initialize(GameObject parent)
        {
            _parent = parent;
        }

        public void ReceiveDMG(float DMG)
        {
            _parent.Destroy = true;
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
