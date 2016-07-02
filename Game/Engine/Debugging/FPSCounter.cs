using Microsoft.Xna.Framework;

namespace OurGame.Engine
{
    public class FPSCounter
    {
        private static int FrameCounter = 0;
        private static float elapsedTime = 0.0f;
        public static int FPS = 64;
       
        public static void Update(GameTime gameTime)
        {
            FrameCounter++;
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime >= 1000f)
            {
                FPS = FrameCounter;
                elapsedTime = 0f;
                FrameCounter = 0;
            }

            //Debug.LogOnScreen("FPS: " + FPS, Debug.ScreenType.General, new Vector2(10f, 20f));
            //Debug.LogOnScreen("Time since last frame: " + (float)gameTime.ElapsedGameTime.Milliseconds, Debug.ScreenType.General, new Vector2(10, 100));
        }
    }
}
