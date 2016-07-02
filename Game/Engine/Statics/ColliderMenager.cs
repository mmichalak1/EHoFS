using OurGame.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Engine.Statics
{
    class ColliderMenager
    {
        private static ColliderMenager instance;

        public static ColliderMenager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ColliderMenager();
                return instance;
            }
        }
        public ColliderComponent player = null;
        public HashSet<ColliderComponent> StaticColliderList=new HashSet<ColliderComponent>();
        public HashSet<ColliderComponent> PhysicsColliderList=new HashSet<ColliderComponent>();
        public HashSet<ColliderComponent> TriggerColliderList=new HashSet<ColliderComponent>();
        public HashSet<ColliderComponent> NavigationColliderList=new HashSet<ColliderComponent>();
        public HashSet<ColliderComponent> NormalColliderList=new HashSet<ColliderComponent>();
        private ColliderMenager()
        {

        }
    }
}
