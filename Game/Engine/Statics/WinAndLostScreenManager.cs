using Microsoft.Xna.Framework.Graphics;
using OurGame.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Engine.Statics
{
    public class WinAndLostScreenManager
    {

        private WinAndLostScreenManager instance;

        public WinAndLostScreenManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new WinAndLostScreenManager();
                return instance;
            }
        }

        private Texture2D screen;
        private ButtonComponent button;
        private GraphicsDevice device;
        private Texture2D buttonActiveIndicator;

        private WinAndLostScreenManager()
        {

        }

        public void initialize()
        {
            device = OurGame.Game.GraphicsDevice;
            buttonActiveIndicator = ContentContainer.TexColor["ButtonBackground"];
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch batch)
        {
            batch.Begin();

            batch.End();
        }


    }
}
