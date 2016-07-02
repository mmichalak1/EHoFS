using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace OurGame.Engine.Components.BoundingObjects
{
    [DataContract]
    class Box : BoundingObject
    {
        
        [DataMember]
        private BoundingBox _boundingBox;
        [DataMember]
        private Vector3 _size;
        public Vector3 Size
        {
            get { return _size; }
            set { _size = value; }
        }
        [DataMember]
        private Vector3 _center;
        [DataMember]
        protected Vector3 _offset = Vector3.Zero;
        public Vector3 Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }
        private VertexPositionColor[] _vertices;
        private VertexBuffer _vertexBuffer;

        public Box(GameObject parent, float size, Vector3 offset) : base(parent)
        {
            _size = new Vector3(size);
            _offset = offset;
            _center = Parent.Transform.Position + _offset;
            _boundingBox = new BoundingBox
                    (Vector3.Add(_center, Vector3.Negate(_size)),
                    Vector3.Add(_center, _size));
            //SetUpVertices();
        }

        public Box(GameObject parent, Vector3 size, Vector3 offset) : base(parent)
        {
            _size = size;
            _offset = offset;
            _center = Parent.Transform.Position + _offset;
            _boundingBox = new BoundingBox
                    (Vector3.Add(_center, Vector3.Negate(_size)),
                    Vector3.Add(_center, _size));
            //SetUpVertices();
        }

        public override void Initialize()
        {
            //SetUpVertices();
        }

        public override void Center(Vector3 position)
        {
            _center = Parent.Transform.Position + _offset;
            _boundingBox.Min = _center + Vector3.Negate(_size);
            _boundingBox.Max = _center + _size;
        }

        public BoundingBox getBox()
        {
            return _boundingBox;
        }

        public override Vector3  getCenter()
        {
            return _center;
        }

        public override bool Intersects(BoundingObject boundingObject)
        {
            if (boundingObject is Particle)
            {
                Particle particle = boundingObject as Particle;
                if (_boundingBox.Contains(particle.getCenter()) == ContainmentType.Contains ||
                    _boundingBox.Contains(particle.getCenter()) == ContainmentType.Intersects)
                { return true; }
                else { return false; }
            }
            if (boundingObject is Sphere)
            {
                BoundingSphere sphere = (boundingObject as Sphere).getSphere();
                return _boundingBox.Intersects(sphere);
            }
            if (boundingObject is Box)
            {
                Box box = boundingObject as Box;
                return _boundingBox.Intersects(box.getBox());
            }
            if(boundingObject is RayCast)
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
                return _boundingBox.Contains(particle.getCenter());
            }
            if (boundingObject is Sphere)
            {
                Sphere sphere = boundingObject as Sphere;
                return _boundingBox.Contains(sphere.getSphere());
            }
            if (boundingObject is Box)
            {
                Box box = boundingObject as Box;
                return _boundingBox.Contains(box.getBox());
            }
            if (boundingObject is RayCast)
            {
                throw new Exception("Box doesn't support ContainmentType for RayCast.");
            }
            throw new Exception("If this error occurs BoundingObject class have some wrong code");
        }

        public override void Draw(GameTime gameTime)
        {

            //_basicEffect.Alpha = 1f;
            //_basicEffect.World = Matrix.CreateTranslation(Parent.Transform.Position);
            //_basicEffect.View = CameraComponent.Main.View;
            //_basicEffect.Projection = CameraComponent.Main.Projection;
            //_basicEffect.VertexColorEnabled = true;

            //OurGame.Game.GraphicsDevice.SetVertexBuffer(_vertexBuffer);

            //RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.CullMode = CullMode.None;
            //OurGame.Game.GraphicsDevice.RasterizerState = rasterizerState;

            //_basicEffect.Alpha = 1f;

            //foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    OurGame.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _vertices, 0, _vertices.Count()/2, VertexPositionColor.VertexDeclaration);
            //}
        }

        public void SetUpVertices()
        {
            _vertices = new VertexPositionColor[24];
            //_basicEffect = new BasicEffect(OurGame.Game.GraphicsDevice);
            _vertexBuffer = new VertexBuffer(OurGame.Game.GraphicsDevice, typeof(VertexPositionColor), _vertices.Count(), BufferUsage.WriteOnly);

            _vertices[0]  = new VertexPositionColor(new Vector3(-_size.X, -_size.Y, -_size.Z) + _offset, Color.Green);
            _vertices[1]  = new VertexPositionColor(new Vector3(-_size.X, -_size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[2]  = new VertexPositionColor(new Vector3(-_size.X, -_size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[3]  = new VertexPositionColor(new Vector3( _size.X, -_size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[4]  = new VertexPositionColor(new Vector3( _size.X, -_size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[5]  = new VertexPositionColor(new Vector3( _size.X, -_size.Y, -_size.Z) + _offset, Color.Green);
            _vertices[6]  = new VertexPositionColor(new Vector3( _size.X, -_size.Y, -_size.Z) + _offset, Color.Green);
            _vertices[7]  = new VertexPositionColor(new Vector3(-_size.X, -_size.Y, -_size.Z) + _offset, Color.Green);

            _vertices[8]  = new VertexPositionColor(new Vector3(-_size.X,  _size.Y, -_size.Z) + _offset, Color.Green);
            _vertices[9]  = new VertexPositionColor(new Vector3(-_size.X,  _size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[10] = new VertexPositionColor(new Vector3(-_size.X,  _size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[11] = new VertexPositionColor(new Vector3( _size.X,  _size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[12] = new VertexPositionColor(new Vector3( _size.X,  _size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[13] = new VertexPositionColor(new Vector3( _size.X,  _size.Y, -_size.Z) + _offset, Color.Green);
            _vertices[14] = new VertexPositionColor(new Vector3( _size.X,  _size.Y, -_size.Z) + _offset, Color.Green);
            _vertices[15] = new VertexPositionColor(new Vector3(-_size.X,  _size.Y, -_size.Z) + _offset, Color.Green);

            _vertices[16] = new VertexPositionColor(new Vector3(-_size.X, -_size.Y, -_size.Z) + _offset, Color.Green);
            _vertices[17] = new VertexPositionColor(new Vector3(-_size.X,  _size.Y, -_size.Z) + _offset, Color.Green);
            _vertices[18] = new VertexPositionColor(new Vector3(-_size.X, -_size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[19] = new VertexPositionColor(new Vector3(-_size.X,  _size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[20] = new VertexPositionColor(new Vector3( _size.X, -_size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[21] = new VertexPositionColor(new Vector3( _size.X,  _size.Y,  _size.Z) + _offset, Color.Green);
            _vertices[22] = new VertexPositionColor(new Vector3( _size.X, -_size.Y, -_size.Z) + _offset, Color.Green);
            _vertices[23] = new VertexPositionColor(new Vector3( _size.X,  _size.Y, -_size.Z) + _offset, Color.Green);

        }
        
    }
}
