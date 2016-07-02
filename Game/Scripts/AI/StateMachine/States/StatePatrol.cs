using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine.ExtensionMethods;

namespace OurGame.Scripts.AI.StateMachine.States
{
    public class StatePatrol : IState<Alien>, IState<Ghost>
    {
        private static StatePatrol _instance;

        public static StatePatrol Instance
        {
            get
            {
                if (_instance == null)
                    _instance=  new StatePatrol();
                return _instance;
            }
        }

        private StatePatrol() { }

        public void Enter(Alien entity, GameTime gameTime)
        {
            entity.Steering.IsWandererOn = true;
        }

        public void Execute(Alien entity, GameTime gameTime)
        {
            if(entity.SeePlayer())
            {
                entity.StateMachine.ChangeState(StateAttack.Instance, gameTime);
            }
        }

        public void Exit(Alien entity, GameTime gameTime)
        {
            entity.Steering.IsWandererOn = false;
        }

        public void Enter(Ghost entity, GameTime gameTime)
        {
            entity.Steering.IsWandererOn = true;
        }

        public void Execute(Ghost entity, GameTime gameTime)
        {
            if (entity.SeePlayer())
            {
                entity.StateMachine.ChangeState(StateAttack.Instance, gameTime);
            }
        }

        public void Exit(Ghost entity, GameTime gameTime)
        {
            entity.Steering.IsWandererOn = false;
        }
    }
}
