using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Scripts.AI.StateMachine.States
{
    public interface IState<Enemy>
    {
        void Enter(Enemy entity, GameTime gameTime);
        void Execute(Enemy entity, GameTime gameTime);
        void Exit(Enemy entity, GameTime gameTime);
    }
}
