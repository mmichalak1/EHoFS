using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using OurGame.Engine.Serialization;

namespace OurGame.Engine
{
    public static class ModelContener
    {
        private const string FILENAME = "ModelRegister.xml";
        private static Dictionary<string, Model> _models;

        public static void LoadContent(ContentManager Content)
        {
            string[] _modelsNames = GetModelsFromXML();
            _models = new Dictionary<string, Model>();
            foreach(string name in _modelsNames)
                _models.Add(name, Content.Load<Model>("Models/" + name));
            return;
        }
        
        public static Model GetModel(string name)
        {
            return _models[name];
        }

        private static string[] GetModelsFromXML()
        {
            XMLManager<string[]> serializer = new XMLManager<string[]>();
            using (System.IO.Stream stream = (new System.IO.FileStream(FILENAME, System.IO.FileMode.Open)))
                return serializer.LoadFromFile(stream);
        }
        public static void SaveModelsToXML(string[] modelsNames)
        {
            XMLManager<string[]> serializer = new XMLManager<string[]>();
            using (System.IO.Stream stream = (new System.IO.FileStream(FILENAME, System.IO.FileMode.Create)))
            {
                serializer.SaveToFile(stream, modelsNames);
            }
            
        }
    }
}
