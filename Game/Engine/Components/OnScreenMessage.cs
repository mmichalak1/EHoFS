using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace OurGame.Engine.Components
{
    public class OnScreenMessage
    {
        private static OnScreenMessage _instance;
        public static OnScreenMessage Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new OnScreenMessage();
                return _instance;
            }
        }
        private OnScreenMessage()
        {

        }
        public bool isDisabled = true;
        private SpriteFont font;
        private SpriteBatch spriteBatch;
        private GraphicsDevice _device;

        public void Initialize()
        {
            _device = OurGame.Game.GraphicsDevice;
            spriteBatch = new SpriteBatch(OurGame.Game.GraphicsDevice);
            ContentContainer.Fonts.TryGetValue("PickUpText", out font);
        }

        public void Draw(GameTime time)
        {
            if (isDisabled == false)
            {//_device.DepthStencilState = DepthStencilState.None;
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                spriteBatch.DrawString(font, "Press [E] to pick up.", new Vector2((GraphicConfiguration.Instance.ScreenWidth / 2.35f), GraphicConfiguration.Instance.ScreenHeigth / 1.25f), Color.Orange);
                spriteBatch.End();
            }
        }
    }
}
