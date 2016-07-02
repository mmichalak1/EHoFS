using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace OurGame.Engine.Utilities
{
    public class QuadRender
    {
        VertexBuffer _vBuffer;
        IndexBuffer _iBuffer;

        public QuadRender(GraphicsDevice device)
        {
            VertexPositionTexture[] ver =
            {
                new VertexPositionTexture(new Vector3(1,-1,0), new Vector2(1,1)),
                new VertexPositionTexture(new Vector3(-1,-1,0), new Vector2(0,1)),
                new VertexPositionTexture(new Vector3(-1,1,0), new Vector2(0,0)),
                new VertexPositionTexture(new Vector3(1,1,0), new Vector2(1,0))
            };

            _vBuffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, ver.Length, BufferUsage.None);
            _vBuffer.SetData(ver);
            ushort[] indices = { 0, 1, 2, 2, 3, 0 };
            _iBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits,
                    indices.Length, BufferUsage.None);
            _iBuffer.SetData(indices);

        }

        public void Draw(GraphicsDevice device)
        {
            device.SetVertexBuffer(_vBuffer);
            device.Indices = _iBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        }
    }
}
