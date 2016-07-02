using System;
using System.Xml.Serialization;

namespace OurGame.Engine.Utilities
{
    [Serializable]
    public class Clip
    {
        public Clip()
        {

        }
        public Clip(string ClipName, int Separator, int Duration)
        {
            this.ClipName = ClipName;
            this.Separator = Separator;
            this.Duration = Duration;
        }
        public string ClipName;
        public int Separator;
        public int Duration;
    }
}
