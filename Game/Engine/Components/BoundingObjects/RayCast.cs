using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using OurGame.Engine.ExtensionMethods;
using Microsoft.Xna.Framework.Graphics;

namespace OurGame.Engine.Components.BoundingObjects
{
    [DataContract]
    class RayCast : BoundingObject
    {
        [DataMember]
        private Ray _ray;
        [DataMember]
        protected Vector3 _offset = Vector3.Zero;

        public Ray Ray
        {
            get { return _ray; }
            set { _ray = value; }
        }

        public RayCast(GameObject parent, Vector3 offset, Vector3 direction) : base(parent)
        {
            _offset = offset;
            _ray = new Ray(_offset, direction);
            _ray.Position = PointToWorld(_ray.Position);
        }
        public override void Initialize() { }
        public override void Center(Vector3 position)
        {
            _ray.Position = position + _offset;
        }

        public override Vector3 getCenter()
        {
            return _ray.Position;
        }

        public override bool Intersects(BoundingObject boundingObject)
        {
            if (boundingObject is Particle)
            {
                 return false; 
            }
            if (boundingObject is Sphere)
            {
                Sphere sphere = boundingObject as Sphere;
                if(_ray.Intersects(sphere.getSphere()) != null)
                { return true; }
                else
                { return false; }
            }
            if (boundingObject is Box)
            {
                Box box = boundingObject as Box;
                if (_ray.Intersects(box.getBox()) != null)
                { return true; }
                else
                { return false; }
            }
            if (boundingObject is RayCast)
            {
                return false;
            }

            throw new Exception("If this error occurs BoundingObject class have some wrong code");
        }

        public override ContainmentType Contains(BoundingObject boundingObject)
        {
            throw new Exception("Ray doesn't support ContainmentType.");
        }

        private Vector3 PointToWorld(Vector3 localPoint)
        {
            Matrix World = Matrix.CreateScale(
                    Parent.Transform.Scale.X,
                    Parent.Transform.Scale.Y,
                    Parent.Transform.Scale.Z) *
                Matrix.CreateFromQuaternion(-Parent.Transform.Rotation) *
                Matrix.CreateTranslation(Parent.Transform.Position);
            localPoint *= -1;
            localPoint.Y *= -1;

            Vector3 result = Vector3.Transform(localPoint, World);

            return Vector3.Transform(localPoint, World);
        }
    }
}
