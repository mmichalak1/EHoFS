using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OurGame.Engine.ParticleSystem
{
    public class ParticleSystem
    {
        private static ParticleSystem _particleSystem;

        public static ParticleSystem Instance => _particleSystem ?? (_particleSystem = new ParticleSystem());

        private List<BillboardModel> particle;
        private GraphicsDevice _device;

        private ParticleSystem()
        {
            particle = new List<BillboardModel>();
        }

        public void Initialize(GraphicsDevice device)
        {
            _device = device;
        }

        public void RemoveParticle(BillboardModel model)
        {
            particle.Remove(model);
        }

        public void Draw(CameraComponent camera, GraphicsDevice device)
        {
            foreach (BillboardModel billboardModel in particle)
            {
                billboardModel.Draw(camera, device);
            }
        }

        public void Update(GameTime time)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.Z))
                particle.Add(new BillboardModel(this, new Vector3(3900, 300, 21000), ContentContainer.TexColor["FireballAtlas"], Vector3.Up, 0f, 4f, 50));

            foreach (BillboardModel billboardModel in particle.ToArray())
            {
                billboardModel.Update(time);
            }
        }
    }
}
