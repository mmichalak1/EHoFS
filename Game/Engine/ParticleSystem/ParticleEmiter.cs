using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OurGame.Engine.Serialization;

namespace OurGame.Engine.ParticleSystem
{
    public class ParticleEmiter
    {
        private static ParticleEmiter _particleEmiter;

        public static ParticleEmiter Instance => _particleEmiter ?? (_particleEmiter = new ParticleEmiter());

        private List<BillboardModel> particle;
        private GraphicsDevice _device;

        private ParticleEmiter()
        {
            particle = new List<BillboardModel>();
        }

        public void Initialize(GraphicsDevice device)
        {
            _device = device;
            ParticleData data = new ParticleData()
            {
               LifeSpan = 3f,
               Direction = Vector3.Zero,
               Scale = 50f,
               Speed = 0f,
               TextureName = "FireballAtlas"
            };

            SaveParticleData(data);
        }

        public void RemoveParticle(BillboardModel model)
        {
            particle.Remove(model);
        }

        public void AddParticle(Vector3 position, string particleAtlasName, Vector3 direction, float speed,
            float lifeSpan, float scale)
        {
            particle.Add(new BillboardModel(this, position, ContentContainer.TexColor[particleAtlasName], direction, speed, lifeSpan, scale));
        }

        public void AddParticle(ParticleData data, Vector3 position)
        {
            particle.Add(new BillboardModel(this, position, ContentContainer.TexColor[data.TextureName], data.Direction, data.Speed, data.LifeSpan, data.Scale));
        }


        public void Draw(CameraComponent camera, GraphicsDevice device)
        {
            foreach (BillboardModel billboardModel in particle)
            {
                billboardModel.Draw(camera, device);
            }
        }

        public void Update(GameTime time)
        {
            foreach (BillboardModel billboardModel in particle.ToArray())
            {
                billboardModel.Update(time);
            }
        }

        private static void SaveParticleData(ParticleData data)
        {
            string name = "particle";
            if(File.Exists(name))
                File.Delete(name);
            using (Stream stream = File.Open(name, FileMode.CreateNew))
            {
                XMLManager<ParticleData> serializer = new XMLManager<ParticleData>();
                serializer.SaveToFile(stream, data);
            }
        }
    }

    [DataContract]
    public struct ParticleData
    {
        [DataMember]
        public string TextureName;
        [DataMember]
        public Vector3 Direction;
        [DataMember]
        public float Speed;
        [DataMember]
        public float LifeSpan;
        [DataMember]
        public float Scale;

    }


    
}
