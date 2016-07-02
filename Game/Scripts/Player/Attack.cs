using System;
using System.Runtime.Serialization;
using OurGame.Engine.Statics;
using Microsoft.Xna.Framework;
using OurGame.Engine;

namespace OurGame.Scripts.Player
{
    [DataContract]
    public class Attack : IScript
    {
        private int damage;

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public void Initialize(GameObject parent)
        {
            damage = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (InputManager.GetMouseButtonReleased(MouseButton.Left))
            {
                if (InputManager.GetKeyDown(KeyBinding.Forward))
                {
                    EventSystem.Instance.Send("PerformSpecialMove", Moves.UpperSlash);
                }
                else if (InputManager.GetKeyDown(KeyBinding.Left))
                {
                    EventSystem.Instance.Send("PerformSpecialMove", Moves.RightSlash);
                }
                else if (InputManager.GetKeyDown(KeyBinding.Right))
                {
                    EventSystem.Instance.Send("PerformSpecialMove", Moves.LeftSlash);
                }
                else
                {
                    EventSystem.Instance.Send("PerformSpecialMove", Moves.LeftSlash);
                }
            }
        }
    }
}
