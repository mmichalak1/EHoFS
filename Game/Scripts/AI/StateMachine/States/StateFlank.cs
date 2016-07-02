using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace OurGame.Scripts.AI.StateMachine.States
{
    public class StateFlank : IState<Alien>, IState<Ghost>
    {
        private static StateFlank _instance;

        public static StateFlank Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StateFlank();
                return _instance;
            }
        }

        private StateFlank() { }

        public void Enter(Alien entity, GameTime gameTime)
        {
            
        }

        public void Execute(Alien entity, GameTime gameTime)
        {
            
        }

        public void Exit(Alien entity, GameTime gameTime)
        {
            
        }

        public void Enter(Ghost entity, GameTime gameTime)
        {
            
        }

        public void Execute(Ghost entity, GameTime gameTime)
        {
            
        }

        public void Exit(Ghost entity, GameTime gameTime)
        {
            
        }
    }
}
