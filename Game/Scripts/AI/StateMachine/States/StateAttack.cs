using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.Components;

namespace OurGame.Scripts.AI.StateMachine.States
{
    public class StateAttack : IState<Alien>, IState<Ghost>
    {
        private static StateAttack _instance;
        private float _delta = 0.0f;

        public static StateAttack Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StateAttack();
                return _instance;
            }
        }

        private StateAttack() { }

        public void Enter(Alien entity, GameTime gameTime)
        {
            entity.Steering.IsArriveOn = true;
        }

        public void Execute(Alien entity, GameTime gameTime)
        {
            if (entity.PlayerInRangeOfFire())
            {
                if (entity.Parent.GetComponentOfType<SimpleAnimationComponent>().CurrentModel == 1)
                {
                    entity.Shoot(gameTime, true);
                }
                else if (entity.Parent.GetComponentOfType<SimpleAnimationComponent>().CurrentModel == 2)
                {
                    entity.Shoot(gameTime, false);
                }
            }

            entity.Steering.SetTarget(entity.Player.Transform.Position);
        }

        public void Exit(Alien entity, GameTime gameTime)
        {
            entity.Steering.IsArriveOn = false;
        }

        public void Enter(Ghost entity, GameTime gameTime)
        {
            entity.Steering.IsArriveOn = true;
            entity.Steering.IsWallAvoidanceOn = false;
        }

        public void Execute(Ghost entity, GameTime gameTime)
        {
            if ((entity.Parent.Transform.Position - entity.Player.Transform.Position).Length() < 500f)
            {
                entity.Attack(gameTime);
            }
            entity.Steering.SetTarget(entity.Player.Transform.Position);
        }

        public void Exit(Ghost entity, GameTime gameTime)
        {
            entity.Steering.IsArriveOn = false;
        }
    }
}
