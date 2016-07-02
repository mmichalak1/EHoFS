using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OurGame.Engine.Utilities.Geometrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Engine.Navigation.Draw
{
    public class NavMeshDraw
    {

        private List<Triangle3> _triangles;
        private VertexPositionColor[] _vertices;

        VertexBuffer vertexBuffer;

        BasicEffect basicEffect;

        public NavMeshDraw(List<Triangle3> triangles)
        {
            _triangles = triangles;
        }

        public void SetUpVertieces()
        {

            basicEffect = new BasicEffect(OurGame.Game.GraphicsDevice);

            _vertices = new VertexPositionColor[_triangles.Count() * 3];
            for (int i = 0; i < _triangles.Count(); i++)
            {
                _vertices[3 * i] = new VertexPositionColor(_triangles[i].A, Color.Aqua);
                _vertices[3 * i + 1] = new VertexPositionColor(_triangles[i].B, Color.Aqua);
                _vertices[3 * i + 2] = new VertexPositionColor(_triangles[i].C, Color.Aqua);
            }

            vertexBuffer = new VertexBuffer(OurGame.Game.GraphicsDevice, typeof(VertexPositionColor), _triangles.Count() * 3, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(_vertices);
        }

        public void Draw(GameTime gameTime, Transform parentTransform)
        {
            basicEffect.Alpha = 0.5f;
            basicEffect.World = Matrix.CreateScale(
                    parentTransform.Scale.X,
                    parentTransform.Scale.Y,
                    parentTransform.Scale.Z) *
                Matrix.CreateFromQuaternion(parentTransform.Rotation) *
                Matrix.CreateTranslation(parentTransform.Position);
            basicEffect.View = CameraComponent.Main.View;
            basicEffect.Projection = CameraComponent.Main.Projection;
            basicEffect.VertexColorEnabled = true;

            OurGame.Game.GraphicsDevice.SetVertexBuffer(vertexBuffer);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            OurGame.Game.GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                OurGame.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, _triangles.Count, VertexPositionColor.VertexDeclaration);
            }

            basicEffect.Alpha = 1f;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                OurGame.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _vertices, 0, _vertices.Count() / 2, VertexPositionColor.VertexDeclaration);
            }
        }
    }
}
