using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using OurGame.Engine.Serialization;
using OurGame.Engine.Utilities;

namespace OurGame.Engine
{
    public static class ContentContainer
    {
        private static Dictionary<string, Model> _models;
        private static Dictionary<string, Texture2D> _texColor;
        private static Dictionary<string, Texture2D> _texSpecular;
        private static Dictionary<string, Texture2D> _texNormal;
        private static Dictionary<string, Texture2D> _texEmissive;
        private static Dictionary<string, Effect> _shaders;
        private static Dictionary<string, SpriteFont> _fonts;
        private static Dictionary<string, Material> _materials;
        private static Dictionary<string, Model> _animatedModels;
        private static Dictionary<string, Clip[]> _animationProperties;
        private static Dictionary<string, TextureCube> _texCube;
        private static Dictionary<string, SoundEffect> _sound;
        private static Dictionary<string, SoundEffect> _song;
        private static Dictionary<string, ParticleSystem.ParticleData> _particle;
        public static Dictionary<string, Model> Models
        {
            get { return _models; }
        }
        public static Dictionary<string, Texture2D> TexColor
        {
            get { return _texColor; }
        }
        public static Dictionary<string, Texture2D> TexSpecular
        {
            get { return _texSpecular; }
        }
        public static Dictionary<string, Texture2D> TexNormal
        {
            get { return _texNormal; }
        }
        public static Dictionary<string, Texture2D> TexEmissive
        {
            get { return _texEmissive; }
        }
        public static Dictionary<string, Effect> Shaders
        {
            get { return _shaders; }
        }
        public static Dictionary<string, SpriteFont> Fonts
        {
            get { return _fonts; }
        }
        public static Dictionary<string, Material> Materials
        {
            get { return _materials; }
        }
        public static Dictionary<string, Model> AnimatedModels
        {
            get { return _animatedModels; }
        }
        public static Dictionary<string, Clip[]> AnimationProperties
        {
            get { return _animationProperties; }
        }
        public static Dictionary<string, TextureCube> TexCube
        {
            get { return _texCube; }
        }
        public static Dictionary<string, SoundEffect> Sounds
        {
            get { return _sound; }
        }
        public static Dictionary<string, SoundEffect> Songs
        {
            get { return _song; }
        }

        public static Dictionary<string, ParticleSystem.ParticleData> Particles
        {
            get { return _particle; }
        }

        public static void LoadContent(ContentManager Content)
        {
            LoadDictionary(Content, "ModelRegister.xml", "Models/", ref _models);
            LoadDictionary(Content, "TextureColorRegister.xml", "Textures/Color/", ref _texColor);
            LoadDictionary(Content, "TextureSpecularRegister.xml", "Textures/Speculars/", ref _texSpecular);
            LoadDictionary(Content, "TextureNormalRegister.xml", "Textures/Normals/", ref _texNormal);
            LoadDictionary(Content, "TextureEmissiveRegister.xml", "Textures/Emissive/", ref _texEmissive);
            LoadDictionary(Content, "SkyboxTextureRegister.xml", "Textures/Skybox/", ref _texCube);
            LoadDictionary(Content, "ShaderRegister.xml", "Shaders/", ref _shaders);
            LoadDictionary(Content, "FontRegister.xml", "Fonts/", ref _fonts);
            LoadDictionary(Content, "MaterialRegister.xml", "Content/Materials/", ref _materials);
            LoadDictionary(Content, "AnimatedModelRegister.xml", "Models/", ref _animatedModels);
            LoadDictionary(Content, "AnimationProperties.xml", ref _animationProperties);
            LoadDictionary(Content, "SoundEffectsRegister.xml", "Audio/Effects/", ref _sound);
            LoadDictionary(Content, "SongRegister.xml", "Audio/Songs/", ref _song);
            LoadDictionary(Content, "ParticleRegister.xml", "Content/Particle/", ref _particle);
            return;
        }

        private static void LoadDictionary<T>(ContentManager content, string fileName, string prefix, ref Dictionary<string, T> dictionary)
        {
            string[] _names = GetFromXML(fileName);
            dictionary = new Dictionary<string, T>();
            foreach (string name in _names)
                dictionary.Add(name, content.Load<T>(prefix + name));
        }

        private static void LoadDictionary(ContentManager content, string fileName, string prefix, ref Dictionary<string, Material> dictionary)
        {
            XMLManager<Material> materialSerializer = new XMLManager<Material>();
            string[] names = GetFromXML(fileName);
            dictionary = new Dictionary<string, Material>();
            foreach (string name in names)
                dictionary.Add(name, materialSerializer.LoadFromFile(prefix + name + ".xml"));
        }

        private static void LoadDictionary(ContentManager content, string fileName, string prefix, ref Dictionary<string, ParticleSystem.ParticleData> dictionary)
        {
            XMLManager<ParticleSystem.ParticleData> particleSerializer = new XMLManager<ParticleSystem.ParticleData>();
            string[] names = GetFromXML(fileName);
            dictionary = new Dictionary<string, ParticleSystem.ParticleData>();
            foreach (string name in names)
                dictionary.Add(name, particleSerializer.LoadFromFile(prefix + name + ".xml"));
        }

        private static void LoadDictionary(ContentManager content, string fileName, ref Dictionary<string, Clip[]> dictionary)
        {
            XMLManager<List<DictionaryItem<string, Clip[]>>> ser =
                new XMLManager<List<DictionaryItem<string, Clip[]>>>();
            dictionary = new Dictionary<string, Clip[]>();
            List<DictionaryItem<string, Clip[]>> list = ser.LoadFromFile("Content/" + fileName);
            foreach (DictionaryItem<string, Clip[]> x in list)
            {
                dictionary.Add(x.Key, x.Value);
            }
        }

        private static string[] GetFromXML(string fileName)
        {
            XMLManager<string[]> serializer = new XMLManager<string[]>();
            using (System.IO.Stream stream = (new System.IO.FileStream("Content/" + fileName, System.IO.FileMode.Open)))
                return serializer.LoadFromFile(stream);
        }

        public static void SaveToXML(string[] names, string fileName)
        {
            XMLManager<string[]> serializer = new XMLManager<string[]>();
            using (System.IO.Stream stream = (new System.IO.FileStream("Content/" + fileName, System.IO.FileMode.Create)))
            {
                serializer.SaveToFile(stream, names);
            }

        }

        public static void Clear()
        {
            _models.Clear();
            _texColor.Clear();
            _texSpecular.Clear();
            _texNormal.Clear();
            _texEmissive.Clear();
            _shaders.Clear();
            _fonts.Clear();
            _materials.Clear();
            _animatedModels.Clear();
            _animationProperties.Clear();
            _texCube.Clear();
            _sound.Clear();
            _song.Clear();
            _particle.Clear();
        }
    }
}
