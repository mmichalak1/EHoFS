using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace OurGame.Engine.Components.BoundingObjects
{
    [DataContract]
    class Particle : BoundingObject
    {
        [DataMember]
        private Vector3 _point;
        [DataMember]
        protected Vector3 _offset = Vector3.Zero;

        public Particle(GameObject parent, Vector3 offset) : base(parent)
        {
            _offset = offset;
            _point = Parent.Transform.Position + _offset;
        }
        public override void Initialize() { }
        public override void Center(Vector3 position)
        {
            _point = Parent.Transform.Position + _offset;
        }
        public override Vector3 getCenter()
        {
            return _point;
        }
        public override bool Intersects(BoundingObject boundingObject)
        {
            if (boundingObject is Particle)
            {
                Particle particle = boundingObject as Particle;
                return _point == particle.getCenter();
            }
            if (boundingObject is Sphere)
            {
                Sphere sphere = boundingObject as Sphere;
                if (sphere.getSphere().Contains(_point) == ContainmentType.Contains ||
                    sphere.getSphere().Contains(_point) == ContainmentType.Intersects)
                { return true; }
                else { return false; }
            }
            if (boundingObject is Box)
            {
                Box box = boundingObject as Box;
                if (box.getBox().Contains(_point) == ContainmentType.Contains ||
                    box.getBox().Contains(_point) == ContainmentType.Intersects)
                { return true; }
                else { return false; }
            }
            if (boundingObject is RayCast)
            {
                return false;
            }

            throw new Exception("If this error occurs BoundingObject class have some wrong code");
        }
        public override ContainmentType Contains(BoundingObject boundingObject)
        {
            if (boundingObject is Particle)
            {
                Particle particle = boundingObject as Particle;
                if (_point == particle.getCenter())
                { return ContainmentType.Contains; }
                else { return ContainmentType.Disjoint; }
            }
            if (boundingObject is Sphere)
            {
                Sphere sphere = boundingObject as Sphere;
                if (sphere.getSphere().Contains(_point) == ContainmentType.Contains ||
                    sphere.getSphere().Contains(_point) == ContainmentType.Intersects)
                { return ContainmentType.Intersects; }
                else { return ContainmentType.Disjoint; }
            }
            if (boundingObject is Box)
            {
                Box box = boundingObject as Box;
                if (box.getBox().Contains(_point) == ContainmentType.Contains ||
                    box.getBox().Contains(_point) == ContainmentType.Intersects)
                { return ContainmentType.Intersects; }
                else { return ContainmentType.Disjoint; }
            }
            if (boundingObject is RayCast)
            {
                throw new Exception("Box doesn't support ContainmentType for RayCast.");
            }
            throw new Exception("If this error occurs BoundingObject class have some wrong code");
        }
    }
}
