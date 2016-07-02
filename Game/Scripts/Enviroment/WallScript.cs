using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using OurGame.Engine;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    class WallScript : IScript
    {
        static Vector3 _moveVector = new Vector3(0f, 3600f, 0f);
        private static float _loweringSpeed = 0.01f;
        private static float _low = -3500f;
        private static float _high = 75f;
        [DataMember]
        private GameObject _parent;
        [DataMember]
        public bool IsUp = true;

        public void HideWalls()
        {
            IsUp = false;
            _isLowering = true;
        }

        public void RaiseWalls()
        {
            IsUp = true;
            _isRaising = true;
        }

        [DataMember]
        private bool _isRaising, _isLowering;

        public void Initialize(GameObject parent)
        {
            _parent = parent;
            IsUp = true;
        }


        public void Update(GameTime gameTime)
        {
            if (_parent.Name == "Immovable") return;

            if (_isLowering)
            {
                if (_parent.Transform.Position.Y <= _low)
                {
                    IsUp = false;
                    _isLowering = false;
                    return;
                }
                _parent.Transform.Position = Vector3.Lerp(_parent.Transform.Position,
                    _parent.Transform.Position - _moveVector, _loweringSpeed);
            }

            if (_isRaising)
            {
                if (_parent.Transform.Position.Y >= _high)
                {
                    _isRaising = false;
                    IsUp = true;
                    return;
                }

                _parent.Transform.Position = Vector3.Lerp(_parent.Transform.Position,
                    _parent.Transform.Position + _moveVector, _loweringSpeed);

            }
        }
    }
}
