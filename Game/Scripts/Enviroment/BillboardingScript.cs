using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.ExtensionMethods;
using System.Runtime.Serialization;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    public class BillboardingScript : IScript
    {
        [DataMember]
        private GameObject _parent;
        [DataMember]
        public GameObject Target;
        public void Initialize(GameObject parent)
        {
            Target = Scene.FindWithTag("MainCamera");
            _parent = parent;
        }

        public void Update(GameTime gameTime)
        {
            if (Target != null)
                _parent.Transform.Rotation = _parent.Transform.Rotation.TurnTowardsTheVector(_parent.Transform.Position - Target.Transform.Position, 0.9f);
        }
    }
}
