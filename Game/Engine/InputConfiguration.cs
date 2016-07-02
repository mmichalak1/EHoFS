using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using System;

namespace OurGame.Engine
{
    [Serializable]
    public class InputConfiguration
    {
        public static InputConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                    _configuration = LoadConfiguration();
                return _configuration;
            }
        }

        private static StorageDevice _device;
        private static StorageContainer _container;
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
            res.Steering.Add(KeyBinding.Forward, Keys.W);
            res.Steering.Add(KeyBinding.Jump, Keys.Space);
            res.Steering.Add(KeyBinding.Left, Keys.A);
            res.Steering.Add(KeyBinding.Right, Keys.D);
            res.Steering.Add(KeyBinding.Sprint, Keys.LeftShift);
            return res;
        }
        private static InputConfiguration LoadConfiguration()
        {
            InputConfiguration result = new InputConfiguration();
            if (!_container.FileExists(FILENAME))
            {
                result = CreateDefault();
                SaveConfiguration(result);
            }
            else
            {
                Stream loader = _container.OpenFile(FILENAME, FileMode.Open);
                XmlSerializer ser = new XmlSerializer(typeof(List<InputPair>));
                List<InputPair> list = ser.Deserialize(loader) as List<InputPair>;
                result.Steering = TransformFromList(list);
            }
            return result;
        }
        private static void SaveConfiguration(InputConfiguration conf)
        {
            List<InputPair> list = TransformFromDictionary(conf.Steering);
            if (_container.FileExists(FILENAME))
                _container.DeleteFile(FILENAME);

            Stream stream = _container.OpenFile(FILENAME, FileMode.CreateNew);
            XmlSerializer ser = new XmlSerializer(typeof(List<InputPair>));
            ser.Serialize(stream, list);
        }

        public static void Initialize()
        {
            IAsyncResult res = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            res.AsyncWaitHandle.WaitOne();
            _device = StorageDevice.EndShowSelector(res);
            res.AsyncWaitHandle.Close();
            IAsyncResult container = _device.BeginOpenContainer("Save", null, null);
            container.AsyncWaitHandle.WaitOne();
            _container = _device.EndOpenContainer(container);
            container.AsyncWaitHandle.Close();

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

    public enum KeyBinding
    {
        Forward,
        Backward,
        Left,
        Right,
        Sprint,
        Jump,
        DebugGeneral,
        DebugPhysics,
        DebugLogic,
        DebugCamera,
    }
}
