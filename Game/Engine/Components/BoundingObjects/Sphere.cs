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
    class Sphere : BoundingObject
    {
        [DataMember]
        private BoundingSphere _boundingSphere;
        [DataMember]
        private Vector3 _offset = Vector3.Zero;
        public Vector3 Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }
        public float Radius
        {
            get { return _boundingSphere.Radius; }
            set {_boundingSphere.Radius = value;}
        }

        private VertexBuffer vertexBuffer;
        private BasicEffect basicEffect;
        private VertexPositionColor[] _verticesX;
        private VertexPositionColor[] _verticesY;
        private VertexPositionColor[] _verticesZ;

        public Sphere(GameObject parent, Vector3 offset, float radius) : base(parent)
        {
            _offset = offset;
            SetUpVertices(24);
            _boundingSphere = new BoundingSphere(Parent.Transform.Position + _offset, radius);
        }

        public override void Initialize()
        {
            SetUpVertices(24);
        }

        public override void Center(Vector3 position)
        {
            _boundingSphere.Center = position + _offset;
        }

        public override Vector3 getCenter()
        {
            return _boundingSphere.Center;
        }

        public BoundingSphere getSphere()
        {
            return _boundingSphere;
        }

        public override bool Intersects(BoundingObject boundingObject)
        {
            if (boundingObject is Particle)
            {
                if (_boundingSphere.Contains(boundingObject.getCenter()) == ContainmentType.Contains ||
                        _boundingSphere.Contains(boundingObject.getCenter()) == ContainmentType.Intersects)
                { return true; }
                else { return false; }
            }
            if (boundingObject is Sphere)
            {
                BoundingSphere sphere = (boundingObject as Sphere).getSphere();
                return _boundingSphere.Intersects(sphere);
            }
            if (boundingObject is Box)
            {
                Box box = boundingObject as Box;
                return _boundingSphere.Intersects(box.getBox());
            }

            throw new Exception("If this error occurs BoundingObject class have some wrong code");
        }
        public override ContainmentType Contains(BoundingObject boundingObject)
        {
            if (boundingObject is Particle)
            {
                return _boundingSphere.Contains(boundingObject.getCenter());
            }
            if (boundingObject is Sphere)
            {
                Sphere sphere = boundingObject as Sphere;
                return _boundingSphere.Contains(sphere.getSphere());
            }
            if (boundingObject is Box)
            {
                Box box = boundingObject as Box;
                return _boundingSphere.Contains(box.getBox());
            }
            throw new Exception("If this error occurs BoundingObject class have some wrong code");
        }

        public override void Draw(GameTime gameTime)
        {

            basicEffect.Alpha = 1f;
            basicEffect.World =

                Matrix.CreateFromQuaternion(Parent.Transform.Rotation) *
                Matrix.CreateTranslation(Parent.Transform.Position);
            basicEffect.View = CameraComponent.Main.View;
            basicEffect.Projection = CameraComponent.Main.Projection;
            basicEffect.VertexColorEnabled = true;

            OurGame.Game.GraphicsDevice.SetVertexBuffer(vertexBuffer);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            OurGame.Game.GraphicsDevice.RasterizerState = rasterizerState;

            basicEffect.Alpha = 1f;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                OurGame.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _verticesX, 0, _verticesX.Count() / 2, VertexPositionColor.VertexDeclaration);
            }
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                OurGame.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _verticesY, 0, _verticesX.Count() / 2, VertexPositionColor.VertexDeclaration);
            }
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                OurGame.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _verticesZ, 0, _verticesZ.Count() / 2, VertexPositionColor.VertexDeclaration);
            }
        }

        public void SetUpVertices(int smoothness)
        {
            float radius;
            if (Parent.Transform != null)
                radius = Radius / Parent.Transform.Scale.X;
            else
                radius = Radius;
            _verticesX = new VertexPositionColor[smoothness * 2];
            _verticesY = new VertexPositionColor[smoothness * 2];
            _verticesZ = new VertexPositionColor[smoothness * 2];
            basicEffect = new BasicEffect(OurGame.Game.GraphicsDevice);
            vertexBuffer = new VertexBuffer(OurGame.Game.GraphicsDevice, typeof(VertexPositionColor), _verticesX.Count(), BufferUsage.WriteOnly);
            float alpha = 0;
            float step = 2 * ((float)Math.PI / smoothness);

            _verticesZ[0] = new VertexPositionColor(new Vector3((float)(Radius * Math.Cos(alpha)), (float)(Radius * Math.Sin(alpha)), 0) + _offset, Color.Green);
            for (int i = 1; i < smoothness; i++)
            {
                alpha += step;
                for (int j = 0; j < 2; j++)
                    _verticesZ[(j + i * 2) - 1] = new VertexPositionColor(new Vector3((float)(Radius * Math.Cos(alpha)), (float)(Radius * Math.Sin(alpha)), 0) + _offset, Color.Green);
            }
            alpha += step;
            _verticesZ[2 * smoothness - 1] = new VertexPositionColor(new Vector3((float)(Radius * Math.Cos(alpha)), (float)(Radius * Math.Sin(alpha)), 0) + _offset, Color.Green);
            alpha = 0;
            _verticesX[0] = new VertexPositionColor(new Vector3(0, (float)(Radius * Math.Sin(alpha)), (float)(Radius * Math.Cos(alpha)))+_offset, Color.Green);
            for (int i = 1; i < smoothness; i++)
            {
                alpha += step;
                for (int j = 0; j < 2; j++)
                    _verticesX[(j + i * 2) - 1] = new VertexPositionColor(new Vector3(0, (float)(Radius * Math.Sin(alpha)), (float)(Radius * Math.Cos(alpha))) + _offset, Color.Green);
            }
            alpha += step;
            _verticesX[2 * smoothness - 1] = new VertexPositionColor(new Vector3(0, (float)(Radius * Math.Sin(alpha)), (float)(Radius * Math.Cos(alpha))) + _offset, Color.Green);
            alpha = 0;
            _verticesY[0] = new VertexPositionColor(new Vector3((float)(Radius * Math.Cos(alpha)), 0, (float)(Radius * Math.Sin(alpha) )) + _offset, Color.Green);
            for (int i = 1; i < smoothness; i++)
            {
                alpha += step;
                for (int j = 0; j < 2; j++)
                    _verticesY[(j + i * 2) - 1] = new VertexPositionColor(new Vector3((float)(Radius * Math.Cos(alpha) ),0 , (float)(Radius * Math.Sin(alpha))) + _offset, Color.Green);
            }
            alpha += step;
            _verticesY[2 * smoothness - 1] = new VertexPositionColor(new Vector3((float)(Radius * Math.Cos(alpha) ), 0, (float)(Radius * Math.Sin(alpha) )) + _offset , Color.Green);
        }
    }
}
