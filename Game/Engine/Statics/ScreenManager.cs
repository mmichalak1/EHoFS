using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OurGame.Engine.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace OurGame.Engine
{
    class ScreenManager
    {

        private static ScreenManager instance;
        
        public static bool IsNavMeshVisible {get; set;}
        public static bool IsAIEnabled { get; set; }
        public static bool IsColliderVisible { get; set; }


        public Scene CurrentScreen;
        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ScreenManager();
                return instance;
            }
        }

        private ScreenManager()
        {
            IsNavMeshVisible = false;
            IsAIEnabled = true;
            IsColliderVisible = false;
        }

        public void UnloadContent()
        {
            CurrentScreen.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            CurrentScreen.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            CurrentScreen.Draw(gameTime);
        }
    }
}
