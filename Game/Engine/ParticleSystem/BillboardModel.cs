using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OurGame.Engine.Utilities;

namespace OurGame.Engine.ParticleSystem
{
    [DataContract(IsReference = true)]
    public class BillboardModel
    {
        private Vector3 _position;
        private Texture2D _texture;
        private Vector3 _direction;
        private float _speed;
        private float _lifeSpan;
        private float _actualTime;
        private float _scale;
        private float _timeOffset;
        private int _currentFrame;
        private Effect _billboardEffect;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private ParticleEmiter _emiter;

        public BillboardModel(ParticleEmiter emiter, Vector3 position, Texture2D texture, Vector3 direction, float speed, float lifeSpan, float scale)
        {
            _position = position;
            _texture = texture;
            _billboardEffect = ContentContainer.Shaders["BillboardEffect"];
            _direction = direction;
            _speed = speed;
            _lifeSpan = lifeSpan;
            _scale = scale;
            _timeOffset = lifeSpan/16.0f;
            _currentFrame = 1;
            _emiter = emiter;
        }

        public void Draw(CameraComponent camera, GraphicsDevice device)
        {
            CreateVertexBuffer(device);
            _billboardEffect.Parameters["World"].SetValue(camera.World * Matrix.CreateScale(_scale) * Matrix.CreateBillboard(_position, camera.Parent.Transform.Position, Vector3.Up, null));
            _billboardEffect.Parameters["View"].SetValue(camera.View);
            _billboardEffect.Parameters["Projection"].SetValue(camera.Projection);
            _billboardEffect.Parameters["billboardTexture"].SetValue(_texture);
            _billboardEffect.Parameters["frameX"].SetValue(_currentFrame%4);
            _billboardEffect.Parameters["frameY"].SetValue(_currentFrame/4);

            _billboardEffect.CurrentTechnique.Passes[0].Apply();
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        }

        public void Update(GameTime time)
        {
            _actualTime += (float)time.ElapsedGameTime.TotalSeconds;
            _position += _direction * _speed * (float)time.ElapsedGameTime.TotalSeconds;
            if (_actualTime >= _currentFrame * _timeOffset)
                _currentFrame++;
            if (_actualTime > _lifeSpan)
            {
                _vertexBuffer?.Dispose();
                _indexBuffer?.Dispose();
                _emiter.RemoveParticle(this);   
            }
                
        }

        void CreateVertexBuffer(GraphicsDevice device)
        {
            _vertexBuffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 6, BufferUsage.None);
            VertexPositionTexture[] verticles = new[]
            {
                new VertexPositionTexture(Vector3.Zero, Vector2.Zero),
                new VertexPositionTexture(Vector3.Up, Vector2.UnitY),
                new VertexPositionTexture(Vector3.Right, Vector2.UnitX),
                new VertexPositionTexture(Vector3.Right + Vector3.Up, new Vector2(1, 1))
            };
            _indexBuffer.SetData(new short[] { 1, 3, 4, 2, 4, 1 });
            _vertexBuffer.SetData(verticles);

        }

    }
}
