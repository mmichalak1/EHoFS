using Microsoft.Xna.Framework;
using OurGame.Engine.Components;
using OurGame.Engine.Components.BoundingObjects;
using System;
using System.Runtime.Serialization;

namespace OurGame.Engine
{
    [DataContract(IsReference = true)]
    public class Transform
    {
        private Vector3 _position;
        private Quaternion _rotation;
        private Scale _scale;
        private GameObject _gameObject;

        [DataMember]
        public GameObject GameObject
        {
            get { return _gameObject; }
            private set { _gameObject = value; }
        }
        [DataMember]
        public Vector3 Position
        {
            get
            {
                if (GameObject.Parent != null)
                    return _position + GameObject.Parent.Transform.Position;
                else
                    return _position;
            }
            set
            {
                if (float.IsNaN(value.X))
                    throw new Exception();
                if (GameObject.Parent != null)
                {
                    if (GameObject.Parent.Transform != null)
                        _position = value - GameObject.Parent.Transform.Position;
                    else
                        _position = value;
                }
                else
                    _position = value;
            }
        }
        [DataMember]
        public Quaternion Rotation
        {
            get
            {
                if (GameObject.Parent != null)
                    return Quaternion.Multiply(GameObject.Parent.Transform.Rotation, _rotation);
                else
                    return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        [DataMember]
        public Scale Scale
        {
            get { return _scale; }
            set
            {
                if (_gameObject.GetComponentOfType<ColliderComponent>() != null)
                {
                    if (_scale.X == 0 || _scale.Y == 0 || _scale.Z == 0)
                    {

                    }
                    else
                    {
                        Scale average = new Scale(value.X / _scale.X, value.Y / _scale.Y, value.Z / _scale.Z);
                        BoundingObject boundingObject = _gameObject.GetComponentOfType<ColliderComponent>().BoundingObject;
                        if (boundingObject is Box)
                        {
                            (boundingObject as Box).Size = Vector3.Multiply((boundingObject as Box).Size, new Vector3(average.X, average.Y, average.Z));
                            (boundingObject as Box).Offset *= new Vector3(average.X, average.Y, average.Z);
                            //(boundingObject as Box).SetUpVertices();
                        }
                        if (boundingObject is Sphere)
                        {
                            (boundingObject as Sphere).Radius *= average.Y;
                            (boundingObject as Sphere).Offset *= new Vector3(average.X, average.Y, average.Z);
                            //(boundingObject as Sphere).SetUpVertices(24);
                        }
                    }
                }
                _scale = value;
            }
        }

        public Vector3 Orientation
        {
            get
            {
                Vector3 orientation = Vector3.Transform(Vector3.Forward, Rotation);
                orientation.Normalize();
                return orientation;
            }
        }

        public Transform(GameObject gameObject)
        {
            GameObject = gameObject;
            _position = new Vector3();
            _rotation = new Quaternion();
            _scale = new Scale(1f, 1f, 1f);

        }
        public Transform()
        {
            _position = new Vector3();
            _rotation = new Quaternion();
            _scale = new Scale(1f, 1f, 1f);
        }
        public void Move(Vector3 direction, float speed)
        {
            Position += direction * speed;
        }
        public void Move(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public override string ToString()
        {
            return _position.ToString();
        }
    }
}
