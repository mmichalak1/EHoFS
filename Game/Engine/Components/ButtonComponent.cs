using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace OurGame.Engine.Components
{
    public class ButtonComponent
    {
        Texture2D texture;
        Texture2D activeTexture;
        Vector2 position;
        bool active = false;
        Rectangle rect, rect2;
        Action ac;
        int scale = 1;
        Color colour = new Color(255, 255, 255, 0);

        public Vector2 size;

        public ButtonComponent(Texture2D newTex, GraphicsDevice graphics, Action action)
        {
            texture = newTex;
            if (newTex.Name == "Textures/Color/RespawnButton" || newTex.Name == "Textures/Color/ExitButton")
                activeTexture = ContentContainer.TexColor["ButtonBackground"];
            else
                activeTexture = ContentContainer.TexColor["ButtonActiveIndicator"];
            size = new Vector2(graphics.Viewport.Width / 10, graphics.Viewport.Height / 20);
            ac = action;
        }


        bool down;
        public bool isClicked;

        public void Update(MouseState state)
        {
            rect = new Rectangle(0, 0, 427, 115);
            rect2 = rect;
            rect2.Size = new Point(rect.Size.X * scale, rect.Size.Y * scale);
            rect2.Offset(position);

            Rectangle mouseRect = new Rectangle(state.X, state.Y, 1, 1);
            active = false;

            if (mouseRect.Intersects(rect2))
            {
                if (colour.A == 255) down = false;
                if (colour.A == 0) down = true;
                if (down) colour.A += 1; else colour.A -= 1;
                if (state.LeftButton == ButtonState.Pressed) ac.Invoke();
                active = true;
            }
            else if (colour.A < 255)
            {
                colour.A += 1;
                isClicked = false;
            }


        }

        public void setPos(Vector2 newPos)
        {
            position = newPos;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            if (active)
                spriteBatch.Draw(activeTexture, position, rect, new Color(255,255,255,130), 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, position, rect, colour, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
