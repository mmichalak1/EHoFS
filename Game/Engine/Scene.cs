using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using OurGame.Engine.Components;
using Microsoft.Xna.Framework.Content;
using System.IO;
using OurGame.Scripts.Player;
using OurGame.Engine.Lights;

namespace OurGame.Engine
{
    [DataContract]
    public class Scene
    {
        private List<GameObject> _gameObjectList;
        [DataMember]
        public List<GameObject> GameObjectList
        {
            get { return _gameObjectList; }
            set { _gameObjectList = value; }
        }
        [DataMember]
        public List<Lights.DirectionalLight> DirectionalLights;
        [DataMember]
        public List<PointLight> PointLights;
        [DataMember]
        public GameObject Menu;
        public static List<GameObject> Walls;

        public Scene()
        {
            _gameObjectList = new List<GameObject>();
            DirectionalLights = new List<Lights.DirectionalLight>();
            PointLights = new List<PointLight>();
            Walls = new List<GameObject>();
            Menu = new GameObject(Vector3.Zero, Quaternion.Identity);
        }

        public void Initialize()
        {
            Menu.Initialize();
            for (int i = 0; i < _gameObjectList.Count; i++)
            {
                if (_gameObjectList[i].Tag != "Enemy")
                    _gameObjectList[i].Initialize();
            }
        }

        public void UnloadContent()
        {

        }

        public void Draw(GameTime gameTime)
        {

            if (_gameObjectList != null)
                foreach (GameObject x in _gameObjectList)
                {
                    x.Draw(gameTime);
                }
        }

        public void DrawMenu(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Menu.Draw(gameTime, spriteBatch);
        }

        public void UpdateMenu(GameTime gameTime)
        {
            Menu.Update(gameTime);
        }

        public void Update(GameTime time)
        {
            if (_gameObjectList != null)
                for (int x = 0; x < _gameObjectList.Count; x++)
                {
                    if (_gameObjectList[x].Destroy)
                    {
                        _gameObjectList[x].Remove();
                        _gameObjectList.Remove(_gameObjectList[x]);
                    }
                    else
                    {
                        _gameObjectList[x].CheckIfChildrenNeedToBeDestroyed();
                    }
                }

            if (_gameObjectList != null)
                for (int x = 0; x < _gameObjectList.Count; x++)
                {
                    //Debug.LogOnScreen(_gameObjectList[x].Name + " " + _gameObjectList[x].Transform.Position.ToString(), Debug.ScreenType.Other, new Vector2(10f, offset + 20f));
                    //offset += 20f;
                    _gameObjectList[x].Update(time);
                }

        }

        public GameObject CreateGameObject(Vector3 position, Quaternion rotation)
        {
            GameObject newobj = new GameObject(position, rotation);
            _gameObjectList.Add(newobj);
            return newobj;
        }

        public void AddGameObjectToScene(GameObject gameObject)
        {
            _gameObjectList.Add(gameObject);
        }

        public void AddLightToScene(PointLight light)
        {
            PointLights.Add(light);
        }

        public void AddLightToScene(Lights.DirectionalLight light)
        {
            DirectionalLights.Add(light);
        }

        public static void SaveScene(Scene scene, string filepath)
        {
            //remove prefabs from scene
            foreach (GameObject x in scene.GameObjectList.ToList())
            {
                if (x.IsPrefab)
                    scene.GameObjectList.Remove(x);
            }
            //Save Scene
            string path = "ScenesData//" + filepath;
            if (File.Exists(path))
                File.Delete(path);
            using (Stream writer = File.Create(path))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(Scene));
                ser.WriteObject(writer, scene);
            }
        }
        public static Scene LoadScene(string filepath)
        {
            string path = "ScenesData//" + filepath;
            if (!File.Exists(path))
                return null;
            Scene res;
            using (Stream reader = File.OpenRead(path))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(Scene));
                res = ser.ReadObject(reader) as Scene;
            }
            res.Initialize();
            return res;
        }
        public static GameObject FindWithTag(string tag)
        {
            return ScreenManager.Instance.CurrentScreen.GameObjectList.FirstOrDefault(x => x.Tag == tag);
        }

        public static GameObject FindWithName(string name)
        {
            return ScreenManager.Instance.CurrentScreen.GameObjectList.FirstOrDefault(x => x.Name == name);
        }

        public static List<GameObject> FindObjectsWithTag(string tag)
        {
            return ScreenManager.Instance.CurrentScreen.GameObjectList.FindAll(x => x.Tag == tag);
        }

        public static void Remove(GameObject go)
        {
            if (go != null)
            {
                go.Remove();
            }
        }
    }
}