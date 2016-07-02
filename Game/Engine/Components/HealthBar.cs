using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OurGame.Scripts;
using OurGame.Scripts.AI;

namespace OurGame.Engine.Components
{
    public class HealthBar
    {
        private static HealthBar _instance;
        public static HealthBar Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HealthBar();
                return _instance;
            }
        }
        private HealthBar()
        {

        }
        Texture2D texture;
        private BossScript _boss;
        private float curHealth;
        private SpriteBatch spriteBatch;
        private GraphicsDevice _device;

        public void Initialize()
        {
            _device = OurGame.Game.GraphicsDevice;
            spriteBatch = new SpriteBatch(OurGame.Game.GraphicsDevice);
            ContentContainer.TexColor.TryGetValue("HealthBar2", out texture);
            EventSystem.Instance.RegisterForEvent("BossSpawned", x => _boss = x as BossScript);
        }

        public void Update(GameTime time)
        {
            if (_boss != null)
                curHealth = _boss.getCurHealth();
        }


        public void Draw(GameTime time)
        {
            if (_boss != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, new Rectangle(GraphicConfiguration.Instance.ScreenWidth / 2 - texture.Width / 2, 30, texture.Width, 44), new Rectangle(0, 45, texture.Width, 44), Color.Gray);
                spriteBatch.Draw(texture, new Rectangle(GraphicConfiguration.Instance.ScreenWidth / 2 - texture.Width / 2, 30, (int)(texture.Width * ((double)curHealth / 600)), 44), new Rectangle(0, 45, texture.Width, 44), Color.Red);
                spriteBatch.Draw(texture, new Rectangle(GraphicConfiguration.Instance.ScreenWidth / 2 - texture.Width / 2, 30, texture.Width, 44), new Rectangle(0, 0, texture.Width, 44), Color.White);
                spriteBatch.End();
            }
        }
    }
}
