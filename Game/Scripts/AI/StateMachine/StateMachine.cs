using Microsoft.Xna.Framework;
using OurGame.Scripts.AI.StateMachine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Scripts.AI.StateMachine
{
    public class StateMachine<Enemy>
    {
        private Enemy _owner;
        private IState<Enemy> _currentState;
        private IState<Enemy> _previousState;
        private IState<Enemy> _globalState;


        public IState<Enemy> CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState = value;
            }
        }
        public IState<Enemy> PreviousState
        {
            get
            {
                return _previousState;
            }
            set
            {
                _previousState = value;
            }
        }
        public IState<Enemy> GlobalState
        {
            get
            {
                return _globalState;
            }
            set
            {
                _globalState = value;
            }
        }

        public StateMachine(Enemy owner)
        {
            _owner = owner;
            CurrentState = null;
            PreviousState = null;
            GlobalState = null;
        }

        public void Update(GameTime gameTime)
        {
            if (_globalState != null)
                _globalState.Execute(_owner, gameTime);

            if (_currentState != null)
                _currentState.Execute(_owner, gameTime);
        }

        public void ChangeState(IState<Enemy> newState, GameTime gameTime)
        {
            _previousState = _currentState;
            _currentState.Exit(_owner, gameTime);
            _currentState = newState;
            _currentState.Enter(_owner, gameTime);
        }

        public void RevertToPreviousState(GameTime gameTime)
        {
            ChangeState(_previousState, gameTime);
        }

        public bool IsInState(IState<Enemy> state)
        {
            if (_currentState.GetType() == state.GetType())
                return true;
            else
                return false;
        }

    }
}
