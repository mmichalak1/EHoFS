using System;
using Microsoft.Xna.Framework;

namespace OurGame.Engine.Lights
{
    public class PointLight
    {
        private Vector3 _position;
        private Color _color;
        private float _range;
        private float _intensity;

        public float Intensity
        {
            get { return _intensity; }
            set { _intensity = value; }
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }

        }
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public float Range
        {
            get { return _range; }
            set { _range = value; }
        }

        public PointLight(Vector3 position, Color lightColor, float range, float intensity)
        {
            _position = position;
            _color = lightColor;
            _range = range;
            _intensity = intensity;
        }
    }
}
