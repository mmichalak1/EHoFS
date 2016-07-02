using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using System.Runtime.Serialization;

namespace OurGame.Scripts.AI
{
    [DataContract(IsReference = true)]
    class AIMovement : IScript
    {
        private GameObject _parent;
        [DataMember]
        private float _speed = 50f;
        private bool _clockWise = true;

        public AIMovement()
        {

        }
        public AIMovement(float speed)
        {
            _speed = speed;
        }

        public void Initialize(GameObject parent)
        {
            _parent = parent;
        }

        public void Update(GameTime gameTime)
        {
            
            // SimpleMovement(gameTime);
            TravelAround(gameTime, new Vector3(0,100,0), _speed);
        }

        private void SimpleMovement(GameTime gameTime)
        {
            Vector3 Direction = Vector3.Zero;

            if (_parent.Transform.Position.Y >= 200 || _parent.Transform.Position.Y <= 50)
                _speed = -_speed;

            Direction.Y += _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 result;
            Quaternion rot = _parent.Transform.Rotation;
            Vector3.Transform(ref Direction, ref rot, out result);
            _parent.Transform.Position = Vector3.Add(_parent.Transform.Position, result);
        }

        private void TravelAround(GameTime gameTime, Vector3 centerPoint, float range)
        {
            Vector3 Direction = Vector3.Zero;

            if (InputManager.GetKeyReleased(KeyBinding.Jump))
                _clockWise = !_clockWise;
            

            Direction = centerPoint - _parent.Transform.Position;
            Direction.Normalize();

            Debug.LogOnScreen("Direction: " + Direction, Debug.ScreenType.Other, new Vector2(10f, 220f));

            Direction = PerpendicularVector(Direction);

            Vector3 result;
            Quaternion rot = _parent.Transform.Rotation;
            Vector3.Transform(ref Direction, ref rot, out result);
            _parent.Transform.Position = Vector3.Add(_parent.Transform.Position, (result * gameTime.ElapsedGameTime.Milliseconds * _speed));
        }

        private Vector3 PerpendicularVector(Vector3 vect)
        {
            float _swapBuffer;

            _swapBuffer = vect.X;
            vect.X = vect.Z;
            vect.Z = -_swapBuffer;

            if (!_clockWise)
            {
                vect.X = -vect.X;
                vect.Z = -vect.Z;
            }

            return vect;
        }

        
    }
}
