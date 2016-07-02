using System;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;

namespace OurGame.Engine
{
    public static class ConfigurationManager
    {
        private static StorageDevice _device;
        private static StorageContainer _container;

        public static StorageDevice Device
        {
            get { return _device; }
        }

        public static StorageContainer Container
        {
            get { return _container; }
        }

        public static void Initialize()
        {
            IAsyncResult res = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            res.AsyncWaitHandle.WaitOne();
            _device = StorageDevice.EndShowSelector(res);
            res.AsyncWaitHandle.Close();
            IAsyncResult container = _device.BeginOpenContainer("OurGame", null, null);
            container.AsyncWaitHandle.WaitOne();
            _container = _device.EndOpenContainer(container);
            container.AsyncWaitHandle.Close();
            GraphicConfiguration.ReadConfiguration();
            InputConfiguration.ReadConfiguration();
        }
    }
}
