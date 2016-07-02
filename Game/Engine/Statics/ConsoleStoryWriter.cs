using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Engine.Statics
{
    public class ConsoleStoryWriter
    {
        private static ConsoleStoryWriter instance;
        private Rectangle textField;
        private int width;
        private int heigth;
        private Texture2D background;
        private string text;
        private string textToWrite;
        private float deltaTime;
        private float time;
        private SpriteFont font;
        private bool write = false;
        private bool pause = false;
        private SoundComponent audio;

        public static ConsoleStoryWriter Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConsoleStoryWriter();
                return instance;
            }
        }

        public void Initialize()
        {
            deltaTime = 0;
            width = GraphicConfiguration.Instance.ScreenWidth;
            heigth = GraphicConfiguration.Instance.ScreenHeigth;
            audio = new SoundComponent(null);
            background = ContentContainer.TexColor["TextBackground"];
            font = ContentContainer.Fonts["PickUpText"];
            textField = new Rectangle(0, 0, (int)(width * 1.25f), (int)(heigth * 0.25));
        }

        public void Update(GameTime gameTime)
        {
            if (write)
            {
                int lastLetterindex = (int)(deltaTime * text.Length / time);
                if (lastLetterindex > text.Length)
                {
                    lastLetterindex = text.Length;
                    audio.Stop();
                }
                else
                    audio.Play("typing", false);
                textToWrite = text.Substring(0, lastLetterindex);
                deltaTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            if (write && !pause)
            {
                batch.Begin();
                batch.Draw(background, new Vector2((int)(width * 0.05f), heigth - (int)(heigth * 0.2)), textField, new Color(255, 255, 255, 180), 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
                batch.DrawString(font, textToWrite, new Vector2(150, (int)(heigth * 0.85f)), Color.Yellow);
                batch.End();
            }
        }

        private ConsoleStoryWriter()
        {

        }

        public void WriteSentence(string text, float time)
        {
            Clear();
            this.text = text;
            this.time = time;
            write = true;

        }

        public void Clear()
        {
            write = false;
            deltaTime = 0;
            textToWrite = "";
        }

        public void Pause()
        {
            pause = true;
        }

        public void Resume()
        {
            pause = false;
        }
    }
}
