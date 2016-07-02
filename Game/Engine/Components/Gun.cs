using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Engine.Components
{
    public class Gun
    {
        private static Gun _instance;
        public static Gun Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Gun();
                return _instance;
            }
        }
        private Gun()
        {
            _device = OurGame.Game.GraphicsDevice;
            spriteBatch = new SpriteBatch(OurGame.Game.GraphicsDevice);
            ContentContainer.TexColor.TryGetValue("gun", out texture);
        }

        Texture2D texture;
        private SpriteBatch spriteBatch;
        private GraphicsDevice _device;
        private double timer = 0;
        private bool shooted = false;
        public bool gunActive = true;

        public void Initialize()
        {
            _device = OurGame.Game.GraphicsDevice;
            spriteBatch = new SpriteBatch(OurGame.Game.GraphicsDevice);
            ContentContainer.TexColor.TryGetValue("gun", out texture);
        }

        public void Update(GameTime time)
        {
            if(shooted)
            {
                timer += time.ElapsedGameTime.TotalSeconds;
                if (timer > 0.2f)
                {
                    ContentContainer.TexColor.TryGetValue("gun", out texture);
                    shooted = false;
                    timer = 0;
                }
            }
        }

        public void Fire()
        {
            shooted = true;
            ContentContainer.TexColor.TryGetValue("gunFire", out texture);
        }
        public void Draw(GameTime time)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, new Rectangle(GraphicConfiguration.Instance.ScreenWidth / 2 - texture.Width/8, GraphicConfiguration.Instance.ScreenHeigth- texture.Height, texture.Width, texture.Height),Color.Gray);
            spriteBatch.End();
        }
    }
}
