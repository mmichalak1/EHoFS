using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;

using OurGame.Engine.Components;
using OurGame.Engine.Serialization;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace OurGame.Engine
{
    [KnownType(typeof(ModelComponent))]
    [KnownType(typeof(CameraComponent))]
    [KnownType(typeof(ScriptComponent))]
    [KnownType(typeof(ColliderComponent))]
    [KnownType(typeof(RigidBodyComponent))]
    [KnownType(typeof(NavMeshComponent))]
    [KnownType(typeof(NavMeshAgent))]
    [KnownType(typeof(SkyboxRenderer))]
    [KnownType(typeof(SimpleAnimationComponent))]
    [KnownType(typeof(AnimationComponent))]
    [KnownType(typeof(SoundComponent))]
    [KnownType(typeof(MenuComponent))]
    [DataContract(IsReference = true)]
    public class GameObject
    {
        #region Fields
        private string _name; // Imie GameObject-u
        private string _tag; //Tag GameObject-u
        private bool _enabled = true;

        [DataMember(Name = "Components")]
        private List<AbstractComponent> _myComponents; //Li
        private GameObject _parent;
        private List<GameObject> _children;
        private Transform _transform;

        [DataMember]
        public bool IsPrefab
        {
            get;
            private set;
        }
        #endregion

        #region Accessors
        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [DataMember]
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        [DataMember]
        public bool Destroy { get; set; }

        [DataMember]
        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        [DataMember]
        public List<GameObject> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        [DataMember]
        public GameObject Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                {
                    _parent.Children.Remove(this);
                    _parent = value;
                    if (value != null)
                        value.Children.Add(this);
                }
                else
                {
                    if (value != null)
                    {
                        _parent = value;
                        if (_parent.Children != null)
                            _parent.Children.Add(this);
                    }
                    else
                    {
                        if (_parent != null)
                        {
                            _parent.Children.Remove(this);
                            _parent = null;
                        }
                    }
                }
            }
        }
        [DataMember]
        public Transform Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }
        #endregion

        private GameObject()
        {
            Children = new List<GameObject>();
        }

        public GameObject(Vector3 Position, Quaternion Rotation)
        {
            Children = new List<GameObject>();
            _myComponents = new List<AbstractComponent>();
            _transform = new Transform(this);
            _transform.Position = Position;
            _transform.Rotation = Rotation;
            Destroy = false;
        }

        #region Base Methods
        public void Initialize()
        {
            if (_children != null)
                foreach (var child in _children)
                    child.Initialize();
            foreach (AbstractComponent x in _myComponents)
            {
                if (x.GetType() == typeof(ScriptComponent) || x.GetType() == typeof(ColliderComponent) || x.GetType() == typeof(ModelComponent) || x.GetType() == typeof(SkyboxRenderer) || x.GetType() == typeof(SimpleAnimationComponent) || x.GetType() == typeof(SoundComponent))
                    x.Initialize();
            }
        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            if (Enabled && !Destroy)
            {
                foreach (AbstractComponent x in _myComponents)
                {
                    x.Update(gameTime);
                }
                foreach (GameObject go in Children)
                {
                    go.Update(gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (Enabled && !Destroy)
            {
                foreach (AbstractComponent component in _myComponents)
                    component.Draw(gameTime);
                foreach (GameObject go in Children)
                    go.Draw(gameTime);
            }
        }

        public void DrawTransparent()
        {
            if (Enabled)
            {
                foreach (AbstractComponent component in _myComponents)
                    component.DrawTransparent();
                foreach (GameObject go in Children)
                    go.DrawTransparent();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Enabled && !Destroy)
            {
                foreach (AbstractComponent component in _myComponents)
                    component.Draw(gameTime, spriteBatch);
                foreach (GameObject go in Children)
                    go.Draw(gameTime, spriteBatch);
            }
        }
        #endregion

        #region Methods
        public T GetComponentOfType<T>()
        {
            return _myComponents.OfType<T>().FirstOrDefault();
        }

        public List<T> GetComponentsOfType<T>()
        {
            return _myComponents.OfType<T>().ToList();
        }

        public AbstractComponent AddComponent(AbstractComponent component)
        {
            _myComponents.Add(component);
            return component;
        }

        public void RemoveComponent(AbstractComponent component)
        {
            _myComponents.Remove(component);
        }

        #endregion

        public static void SaveGameObject(GameObject obj, string filename)
        {
            obj.IsPrefab = true;
            if (File.Exists(filename))
                File.Delete(filename);
            using (Stream writer = new FileStream(filename, FileMode.CreateNew))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(GameObject));
                ser.WriteObject(writer, obj);
                writer.Flush();
            }
        }

        public static GameObject LoadGameObject(string filename)
        {
            GameObject res;
            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(GameObject));
                res = ser.ReadObject(reader) as GameObject;
            }
            return res;
        }

        public GameObject FindWithTag(string tag)
        {
            return _children.FirstOrDefault(x => x.Tag == tag);
        }

        public GameObject FindWithName(string name)
        {
            return _children.FirstOrDefault(x => x.Name == name);
        }

        public List<GameObject> FindObjectsWithTag(string tag)
        {
            return _children.FindAll(x => x.Tag == tag);
        }

        public GameObject Clone()
        {
            GameObject res = ObjectCloner.Clone(this);
            //res.LoadContent(OurGame.Game.Content);
            return res;
        }

        public void CheckIfChildrenNeedToBeDestroyed()
        {
            if (_children != null)
                for (int x = 0; x < _children.Count; x++)
                {
                    if (_children[x].Destroy)
                    {
                        _children[x].Remove();
                        _children.Remove(_children[x]);
                    }
                    else
                    {
                        _children[x].CheckIfChildrenNeedToBeDestroyed();
                    }
                }
        }


        public void Remove()
        {
            if (_children != null)
            {
                foreach (GameObject x in _children)
                {
                    x.Remove();
                }
                _children.Clear();
            }
            if (_myComponents != null)
            {
                foreach (AbstractComponent x in _myComponents)
                {
                    x.Remove();
                }
                _myComponents.Clear();
            }

        }
        //[OnDeserialized]
        //private void OnDeserialize(StreamingContext context)
        //{
        //    foreach (var item in _myComponents)
        //        item.Initialize();
        //}
    }
}
