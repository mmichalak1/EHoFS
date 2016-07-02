using System;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.Components;
using OurGame.Scripts.Player;
using OurGame.Engine.Statics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Input;
using OurGame.Engine.ParticleSystem;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    public class GlovesScript : IScript
    {
        private GameObject _parent;
        private ColliderComponent _referenceToTrigger;
        private GameObject _referenceToPlayer;


        public void Initialize(GameObject parent)
        {
            _parent = parent;
            _referenceToPlayer = Scene.FindWithTag("MainCamera");
            _referenceToTrigger = _parent.GetComponentOfType<ColliderComponent>();
            OnScreenMessage.Instance.Initialize();
        }

        public void Update(GameTime gameTime)
        { 
            if (_referenceToTrigger != null)
                if (_referenceToTrigger.isCollide(_referenceToPlayer))
                {
                    OnScreenMessage.Instance.isDisabled = false;
                    if (InputManager.GetKeyReleased(KeyBinding.Action))
                    {
                    _referenceToPlayer.GetComponentOfType<ScriptComponent>().GetScriptOfType<Wielding>().HasGloves = false;
                    _parent.Destroy = true;
                        OnScreenMessage.Instance.isDisabled = true;
                        RoomScript.StartingRoom.Weapons++;
                    }
                }
            if (_referenceToTrigger != null)
            {
                if (_referenceToTrigger.isExit(_referenceToPlayer))
                {
                    OnScreenMessage.Instance.isDisabled = true;
                }
            }
        }
        }
    }