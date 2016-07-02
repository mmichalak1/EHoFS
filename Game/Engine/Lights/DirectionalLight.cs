using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace OurGame.Engine.Lights
{
    [DataContract(IsReference = true)]
    public class DirectionalLight
    {
        [DataMember]
        private Color _color;
        [DataMember]
        private Vector3 _direction;

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public Vector3 Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public DirectionalLight(Color color, Vector3 direction)
        {
            _color = color;
            _direction = direction;
        }
    }
}
