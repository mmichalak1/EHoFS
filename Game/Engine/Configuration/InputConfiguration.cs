using OurGame.Engine.Serialization;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System;

namespace OurGame.Engine
{
    [Serializable]
    public class InputConfiguration
    {
        public static void ReadConfiguration()
        {
            _configuration = LoadConfiguration();
        }
        public static InputConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                    _configuration = LoadConfiguration();
                return _configuration;
            }
        }


        private static XMLManager<List<InputPair>> _serializer = new XMLManager<List<InputPair>>();
        private static InputConfiguration _configuration;
        private const string FILENAME = "InputConfig";

        private static Dictionary<KeyBinding, Keys> TransformFromList(List<InputPair> list)
        {
            Dictionary<KeyBinding, Keys> res = new Dictionary<KeyBinding, Keys>();
            foreach (InputPair x in list)
            {
                res.Add(x.Key, x.Value);
            }
            return res;
        }
        private static List<InputPair> TransformFromDictionary(Dictionary<KeyBinding, Keys> dic)
        {
            List<InputPair> res = new List<InputPair>();
            foreach (KeyValuePair<KeyBinding, Keys> x in dic)
            {
                res.Add(new InputPair(x.Key, x.Value));
            }
            return res;

        }
        private static InputConfiguration CreateDefault()
        {
            InputConfiguration res = new InputConfiguration();
            res.Steering.Add(KeyBinding.Backward, Keys.S);
            res.Steering.Add(KeyBinding.DebugCamera, Keys.F3);
            res.Steering.Add(KeyBinding.DebugGeneral, Keys.F1);
            res.Steering.Add(KeyBinding.DebugLogic, Keys.F4);
            res.Steering.Add(KeyBinding.DebugPhysics, Keys.F2);
            res.Steering.Add(KeyBinding.DebugOther, Keys.F5);
            res.Steering.Add(KeyBinding.NavMeshVisible, Keys.F6);
            res.Steering.Add(KeyBinding.Forward, Keys.W);
            res.Steering.Add(KeyBinding.Jump, Keys.Space);
            res.Steering.Add(KeyBinding.Left, Keys.A);
            res.Steering.Add(KeyBinding.Right, Keys.D);
            res.Steering.Add(KeyBinding.Sprint, Keys.LeftShift);
            res.Steering.Add(KeyBinding.Escape, Keys.Escape);
            res.Steering.Add(KeyBinding.Dash, Keys.LeftShift);
            res.Steering.Add(KeyBinding.StartAIScripts, Keys.I);
            res.Steering.Add(KeyBinding.CollidersVisible, Keys.K);
            res.Steering.Add(KeyBinding.Action, Keys.E);
            return res;
        }
        private static InputConfiguration LoadConfiguration()
        {
            InputConfiguration result = new InputConfiguration();
            if (!ConfigurationManager.Container.FileExists(FILENAME))
            {
                result = CreateDefault();
                SaveConfiguration(result);
            }
            else
            {
                using (Stream loader = ConfigurationManager.Container.OpenFile(FILENAME, FileMode.Open))
                {
                    result.Steering = TransformFromList(_serializer.LoadFromFile(loader));
                }
            }
            return result;
        }
        private static void SaveConfiguration(InputConfiguration conf)
        {
            List<InputPair> list = TransformFromDictionary(conf.Steering);
            if (ConfigurationManager.Container.FileExists(FILENAME))
                ConfigurationManager.Container.DeleteFile(FILENAME);

            using (Stream stream = ConfigurationManager.Container.OpenFile(FILENAME, FileMode.CreateNew))
            {
                _serializer.SaveToFile(stream, list);
            }
            
        }      


        private InputConfiguration()
        {
            Steering = new Dictionary<KeyBinding, Keys>();
        }

        public Dictionary<KeyBinding, Keys> Steering
        {
            get;
            set;
        }

        [Serializable]
        public class InputPair
        {
            public KeyBinding Key;
            public Keys Value;
            public InputPair()
            {

            }
            public InputPair(KeyBinding key, Keys value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
