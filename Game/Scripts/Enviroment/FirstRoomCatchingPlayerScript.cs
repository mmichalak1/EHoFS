using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using System.Runtime.Serialization;
using OurGame.Engine.Components;
using OurGame.Engine.Statics;
using OurGame.Scripts.Player;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    class FirstRoomCatchingPlayerScript : IScript
    {
        [DataMember]
        private GameObject _parent;
        private GameObject _player;
        public void Initialize(GameObject parent)
        {
            _parent = parent;
            _player = Scene.FindWithTag("MainCamera");
        }

        public void Update(GameTime gameTime)
        {
            if(_parent.GetComponentOfType<ColliderComponent>().isCollide(_player))
            {
                PhysicsEngine.floorLevel = 101;
                Gun.Instance.gunActive = false;
                Scene.FindWithTag("RoomGenerator").GetComponentOfType<ScriptComponent>().GetScriptOfType<SceneGenerator>().SetPlayerOnLevel();
                _parent.Parent.Destroy = true;
            }
        }
    }
}
