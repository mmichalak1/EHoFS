using System.IO;
using System;
using OurGame.Engine.Serialization;
using System.Xml.Serialization;

namespace OurGame.Engine
{
    [Serializable]
    public class GraphicConfiguration
    {
        #region Static Fields
        private static string FILENAME = "GraphicsConfig";
        private static GraphicConfiguration _instance;
        private static XMLManager<GraphicConfiguration> serializer = new XMLManager<GraphicConfiguration>();
        #endregion


        public static GraphicConfiguration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = ReadConfiguration();
                return _instance;
            }
        }

        public static GraphicConfiguration ReadConfiguration()
        {
            GraphicConfiguration result = new GraphicConfiguration();
            if (!ConfigurationManager.Container.FileExists(FILENAME))
            {
                SaveConfig(result);
            }
            else
            {
                using (Stream reader = ConfigurationManager.Container.OpenFile(FILENAME, FileMode.Open))
                {
                    result = serializer.LoadFromFile(reader);
                }
            }
            return result;

        }

        public static void SaveConfig(GraphicConfiguration conf)
        {
            if (ConfigurationManager.Container.FileExists(FILENAME))
                ConfigurationManager.Container.DeleteFile(FILENAME);

            using (Stream writer = ConfigurationManager.Container.OpenFile(FILENAME, FileMode.CreateNew))
            {
                serializer.SaveToFile(writer, conf);
            }
        }

        #region Settings
        public int ScreenWidth = 1366;
        public int ScreenHeigth = 768;
        public bool IsDebuging = true;
        public DisplayMode ViewMode = DisplayMode.Borderless;
        #endregion

    }
}
