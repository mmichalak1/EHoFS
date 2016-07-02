using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using System.Runtime.Serialization;
using OurGame.Engine.Components;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using System.Collections.Generic;
using System.Diagnostics;

namespace OurGame
{
    [DataContract (IsReference = true)]
    public class SoundComponent : AbstractComponent
    {
        [DataMember]
        private Dictionary<string, SoundEffectInstance> _playList;

        public SoundEffect Sound { get; private set; }

        public SoundComponent(GameObject parent) : base(parent)
        {
            Initialize();
        }

        public override void Initialize()
        {
            _playList = new Dictionary<string, SoundEffectInstance>();
            base.Initialize();
        }

        protected override void LoadContent()
        {

        }

        public override void Update(GameTime time)
        {
            if (_playList.Count > 0)
                foreach (KeyValuePair<string, SoundEffectInstance> x in _playList)
                {
                    if (x.Value.State == SoundState.Stopped)
                        _playList[x.Key].Dispose();
                }
        }

        public override void Remove()
        {
            foreach (var item in _playList)
            {
                item.Value.Dispose();
            }
            base.Remove();
        }

        public void Play(string name, bool loop)
        {
            var mth = new StackTrace().GetFrame(1).GetMethod().Name;
            int hashCode = GetHashCode();
            if (!_playList.ContainsKey(mth))
                _playList.Add(mth, null);

            if (_playList[mth] == null || _playList[mth].State == SoundState.Stopped)
            {
                if(name=="Song")
                    _playList[mth] = ContentContainer.Songs[name].CreateInstance();
                else
                    _playList[mth] = ContentContainer.Sounds[name].CreateInstance();

                _playList[mth].IsLooped = loop;
                _playList[mth].Play();
            }
        }

        public void Stop()
        {
            var mth = new StackTrace().GetFrame(1).GetMethod().Name;
            if (_playList[mth] != null)
                _playList[mth].Stop();
        }

        public void Play(string name, bool loop, Vector3 position)
        {
            var mth = new StackTrace().GetFrame(1).GetMethod().Name;
            int hashCode = GetHashCode();
            if (!_playList.ContainsKey(mth))
                _playList.Add(mth, null);
            AudioListener listener = new AudioListener();
            AudioEmitter emitter = new AudioEmitter();
            listener.Position = CameraComponent.Main.Parent.Transform.Position * 0.01f;
            emitter.Position = position * 0.01f;

            if (_playList[mth] == null || _playList[mth].State == SoundState.Stopped)
            {
                _playList[mth] = ContentContainer.Sounds[name].CreateInstance();
                _playList[mth].Apply3D(listener, emitter);
                _playList[mth].IsLooped = loop;
                _playList[mth].Play();
            }
        }

        
    }
}
