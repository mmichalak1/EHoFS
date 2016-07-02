using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OurGame.Scripts.Player;
using OurGame.Scripts.Enviroment;
using System.Runtime.Serialization;

namespace OurGame.Engine.Components
{
    [DataContract(IsReference = true)]
    public abstract class AbstractComponent
    {
        private GameObject _parent;
        private bool _enabled = true;
        //protected ContentManager content;
        [DataMember]
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
        [DataMember]
        public GameObject Parent
        {
            get { return _parent; }
            private set { _parent = value; }
        }

        public AbstractComponent(GameObject parent)
        {
            Parent = parent;
        }
        protected virtual void LoadContent()
        {

        }
        public virtual void UnloadContent()
        {
            //content.Unload();
        }
        public abstract void Update(GameTime time);
        public virtual void Initialize()
        {
            LoadContent();
        }
        public virtual void Draw(GameTime time)
        {

        }

        public virtual void Draw(GameTime time, SpriteBatch batch)
        {

        }
        public virtual void DrawTransparent()
        {

        }
        public virtual void Remove()
        {

        }
    }
}
