using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace OurGame.Engine
{
    public static class InputManager
    {
        public static bool Enabled;

        public static Point MouseResetPosition;
        public static int MouseXInput;
        public static int MouseYInput;
        private static KeyboardState oldKeyboardState;
        private static MouseState oldMouseState;

        public static void Initialize()
        {
            MouseResetPosition = new Point(GraphicConfiguration.Instance.ScreenWidth / 2, GraphicConfiguration.Instance.ScreenHeigth / 2);
            oldKeyboardState = Keyboard.GetState();
            oldMouseState = Mouse.GetState();
            Mouse.SetPosition(MouseResetPosition.X, MouseResetPosition.Y);
        }

        public static void Update()
        {
            if (Enabled)
            {
                oldKeyboardState = Keyboard.GetState();
                oldMouseState = Mouse.GetState();
                Point position = Mouse.GetState().Position;
                MouseXInput = MathHelper.Clamp(position.X - MouseResetPosition.X, -100, 100);
                MouseYInput = MathHelper.Clamp(position.Y - MouseResetPosition.Y, -100, 100);
                try
                {
                    Mouse.SetPosition(MouseResetPosition.X, MouseResetPosition.Y);
                }
                catch (Exception e)
                {
                    return;
                }
                //Debug.LogOnScreen("MOUSEX: " + MouseXInput + "\n" + "MOUSEY: " + MouseYInput, Debug.ScreenType.Other, new Vector2(600, 20));
                // Debug.LogOnScreen(MouseResetPosition.ToString(), Debug.ScreenType.Other, new Vector2(600, 40));
            }
        }


        public static bool GetKeyDown(KeyBinding key)
        {
            return (Keyboard.GetState().IsKeyDown(InputConfiguration.Configuration.Steering[key]));
        }
        /// <summary>
        /// Retruns true if key was just released
        /// </summary>
        /// <param name="key"> what key is relesed</param>
        /// <returns>bool</returns>
        public static bool GetKeyReleased(KeyBinding key)
        {
            if (!oldKeyboardState.IsKeyDown((InputConfiguration.Configuration.Steering[key])) && Keyboard.GetState().IsKeyDown((InputConfiguration.Configuration.Steering[key])))
                return true;
            else
                return false;
        }

        public static bool GetMouseButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return Mouse.GetState().LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return Mouse.GetState().RightButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        public static bool GetMouseButtonReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return oldMouseState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released;
                case MouseButton.Right:
                    return oldMouseState.RightButton == ButtonState.Pressed && Mouse.GetState().RightButton == ButtonState.Released;
                default:
                    return false;
            }
        }
    }
    public enum MouseButton
    {
        Left,
        Right
    }
}
