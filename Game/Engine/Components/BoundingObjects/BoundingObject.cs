using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OurGame.Engine.Components.BoundingObjects;
using Microsoft.Xna.Framework.Graphics;

namespace OurGame.Engine.Components
{
    [DataContract(IsReference = true)]  
    public abstract class BoundingObject
    {
        private GameObject _parent;
        [DataMember]
        public GameObject Parent
        {
            get { return _parent; }
            private set { _parent = value; }
        }
        public abstract void Initialize();
        public BoundingObject(GameObject parentObject)
        {
            Parent = parentObject;
        }
        public abstract void Center(Vector3 position);
        public abstract Vector3 getCenter();
        public abstract bool Intersects(BoundingObject boundingObject);
        public abstract ContainmentType Contains(BoundingObject boundingObject);

        public virtual void Draw(GameTime gameTime)
        {

        }
    }
}
