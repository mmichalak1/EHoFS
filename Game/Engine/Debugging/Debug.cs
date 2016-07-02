using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OurGame.Engine
{
    public static class Debug
    {
        private static Dictionary<ScreenType, List<Log>> _logScreens;
        private static string[] _consoleLogs;
        public static Dictionary<ScreenType, bool> DebugScreensVisibility;
        public static SpriteFont debugFont;


        public static void Initialize()
        {
            _logScreens = new Dictionary<ScreenType, List<Log>>();
            _logScreens.Add(ScreenType.General, new List<Log>());
            _logScreens.Add(ScreenType.Camera, new List<Log>());
            _logScreens.Add(ScreenType.Logic, new List<Log>());
            _logScreens.Add(ScreenType.Other, new List<Log>());
            _logScreens.Add(ScreenType.Physics, new List<Log>());
            DebugScreensVisibility = new Dictionary<ScreenType, bool>();
            DebugScreensVisibility.Add(ScreenType.General, true);
            DebugScreensVisibility.Add(ScreenType.Camera, false);
            DebugScreensVisibility.Add(ScreenType.Logic, false);
            DebugScreensVisibility.Add(ScreenType.Other, false);
            DebugScreensVisibility.Add(ScreenType.Physics, false);
            _consoleLogs = new string[5];

        }

        /// <summary>
        /// Log information on screen based on page you choose
        /// WARNING: Info can be drawn on each other
        /// </summary>
        /// <param name="Message"> string with wanted message</param>
        /// <param name="type"> which screen to use</param>
        /// <param name="position">Position on screen</param>
        public static void LogOnScreen(string Message, ScreenType type, Vector2 position)
        {
            _logScreens[type].Add(new Log() { Position = position, Message = Message });
        }

        public static void LogToConsole(string message)
        {
            for (int i = 1; i < 5; i++)
            {
                _consoleLogs[i - 1] = _consoleLogs[i];
                _consoleLogs[4] = message;
            }
        }

        public static void Draw(SpriteBatch batch)
        {
            foreach (KeyValuePair<ScreenType, bool> x in DebugScreensVisibility)
            {
                if (x.Value)
                {
                    DrawScreen(x.Key, batch);
                }
                _logScreens[x.Key].Clear();
            }
        }

        private static void DrawScreen(ScreenType type, SpriteBatch batch)
        {
            batch.Begin();
            //foreach (Log x in _logScreens[type])
            //{
            //    batch.DrawString(debugFont, x.Message, x.Position, Color.Yellow);
            //}
            //for (int i = 0; i < _consoleLogs.Length; i++)
            //{
            //    if (_consoleLogs[i] != null)
            //        batch.DrawString(debugFont, _consoleLogs[i], new Vector2(10, position), Color.Red);
            //    position += 20;
            //}
            batch.End();
            _logScreens[type].Clear();


        }

        private class Log
        {
            public string Message;
            public Vector2 Position;
        }

        public enum ScreenType
        {
            General,
            Physics,
            Logic,
            Camera,
            Other
        };
    }
}
