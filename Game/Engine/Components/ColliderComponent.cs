using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using OurGame.OurException;
using OurGame.Engine.Components.BoundingObjects;
using OurGame.Engine.Statics;

namespace OurGame.Engine.Components
{
    [KnownType(typeof(BoundingObject))]
    [KnownType(typeof(Box))]
    [KnownType(typeof(Particle))]
    [KnownType(typeof(Sphere))]
    [DataContract(IsReference = true)]
    public class ColliderComponent : AbstractComponent
    {
        [DataMember]
        private BoundingObject _boundingObject;
        private HashSet<GameObject> listOfCollidingObject0;
        private HashSet<GameObject> listOfCollidingObject1;
        private int actualList = 0;
        [DataMember]
        private ColliderTypes _type;
        public ColliderTypes Type
        {
            set
            {
                switch (_type)
                {
                    case ColliderTypes.Static:
                        ColliderMenager.Instance.StaticColliderList.Remove(this);
                        break;
                    case ColliderTypes.Physics:
                        ColliderMenager.Instance.PhysicsColliderList.Remove(this);
                        break;
                    case ColliderTypes.Trigger:
                        ColliderMenager.Instance.TriggerColliderList.Remove(this);
                        break;
                    case ColliderTypes.Navigation:
                        ColliderMenager.Instance.NavigationColliderList.Remove(this);
                        break;
                    case ColliderTypes.Normal:
                        ColliderMenager.Instance.NormalColliderList.Remove(this);
                        break;
                }
                _type = value;
                switch (_type)
                {
                    case ColliderTypes.Static:
                        ColliderMenager.Instance.StaticColliderList.Add(this);
                        break;
                    case ColliderTypes.Physics:
                        ColliderMenager.Instance.PhysicsColliderList.Add(this);
                        break;
                    case ColliderTypes.Trigger:
                        ColliderMenager.Instance.TriggerColliderList.Add(this);
                        break;
                    case ColliderTypes.Navigation:
                        ColliderMenager.Instance.NavigationColliderList.Add(this);
                        break;
                    case ColliderTypes.Normal:
                        ColliderMenager.Instance.NormalColliderList.Add(this);
                        break;
                }
            }
            get { return _type; }
        }

        public BoundingObject BoundingObject
        { get { return _boundingObject; } }
        public ColliderComponent(GameObject parent, BoundingObject boundingObject, ColliderTypes type) : base(parent)
        {
            _type = type;
            _boundingObject = boundingObject;
            listOfCollidingObject0 = new HashSet<GameObject>();
            listOfCollidingObject1 = new HashSet<GameObject>();
            switch (_type)
            {
                case ColliderTypes.Static:
                    if (parent.GetComponentOfType<RigidBodyComponent>() == null)
                    {
                        throw new Exception("Static Collider needs RigidBodyComponent with 0 mass");
                    }
                    break;
                case ColliderTypes.Physics:
                    if (parent.GetComponentOfType<RigidBodyComponent>() == null)
                    {
                        throw new Exception("Physics Collider needs RigidBodyComponent");
                    }
                    break;
                default:
                    break;
            }
        }
        public override void Initialize()
        {
            listOfCollidingObject0 = new HashSet<GameObject>();
            listOfCollidingObject1 = new HashSet<GameObject>();
            switch (_type)
            {
                case ColliderTypes.Static:
                    ColliderMenager.Instance.StaticColliderList.Add(this);
                    break;
                case ColliderTypes.Physics:
                    ColliderMenager.Instance.PhysicsColliderList.Add(this);
                    break;
                case ColliderTypes.Trigger:
                    ColliderMenager.Instance.TriggerColliderList.Add(this);
                    break;
                case ColliderTypes.Navigation:
                    ColliderMenager.Instance.NavigationColliderList.Add(this);
                    break;
                case ColliderTypes.Normal:
                    ColliderMenager.Instance.NormalColliderList.Add(this);
                    break;
            }
            _boundingObject.Initialize();
        }
        public override void Update(GameTime time)
        {
            _boundingObject.Center(Parent.Transform.Position);
            makelistOfCollidingObjects();
        }

        public override void Draw(GameTime time)
        {
            //if (ScreenManager.IsColliderVisible)
            //    _boundingObject.Draw(time);
            base.Draw(time);
        }

        private void makelistOfCollidingObjects()
        {
            changeListForActualFrame();
            HashSet<GameObject> listOfCollidingObject = getActualListOfColidingObjects();
            listOfCollidingObject?.Clear();

            switch (_type)
            {
                case ColliderTypes.Static:
                    //dont doing anything;
                    break;
                case ColliderTypes.Physics:
                    CheckCollision(ColliderMenager.Instance.PhysicsColliderList, listOfCollidingObject);
                    CheckCollision(ColliderMenager.Instance.StaticColliderList, listOfCollidingObject);
                    break;
                case ColliderTypes.Trigger:
                    CheckCollision(ColliderMenager.Instance.player, listOfCollidingObject);
                    break;
                case ColliderTypes.Navigation:
                    CheckCollision(ColliderMenager.Instance.PhysicsColliderList, listOfCollidingObject);
                    CheckCollision(ColliderMenager.Instance.StaticColliderList, listOfCollidingObject);
                    break;
                case ColliderTypes.Normal:
                    CheckCollision(ColliderMenager.Instance.PhysicsColliderList, listOfCollidingObject);
                    CheckCollision(ColliderMenager.Instance.StaticColliderList, listOfCollidingObject);
                    CheckCollision(ColliderMenager.Instance.TriggerColliderList, listOfCollidingObject);
                    CheckCollision(ColliderMenager.Instance.NavigationColliderList, listOfCollidingObject);
                    CheckCollision(ColliderMenager.Instance.NormalColliderList, listOfCollidingObject);
                    break;
            }
        }
        private void CheckCollision(HashSet<ColliderComponent> possibleColidingObjectList, HashSet<GameObject> listOfCollidingObject)
        {
            foreach (ColliderComponent possibleColidingObject in possibleColidingObjectList)
            {
                if (possibleColidingObject != this)
                {
                    if (this.BoundingObject.Intersects(possibleColidingObject.BoundingObject))
                    {
                        listOfCollidingObject.Add(possibleColidingObject.Parent);
                    }
                }
            }
        }
        private void CheckCollision(ColliderComponent possibleColidingObject, HashSet<GameObject> listOfCollidingObject)
        {
            if (possibleColidingObject != this)
            {
                if (this.BoundingObject.Intersects(possibleColidingObject.BoundingObject))
                {
                    listOfCollidingObject.Add(possibleColidingObject.Parent);
                }
            }
        }

        public bool isCollide(GameObject gameObject)
        {
            if (isNotNullNorThisOne(gameObject) == false)
            {
                throw new NoColliderException("this gameComponent(given as method parametr) doesn't have collider");
            }
            if (getActualListOfColidingObjects().Contains(gameObject))
            {
                return true;
            }
            else
            {
                return false;
            }
            //return listOfCollidingObject1.Find(x => x.GetComponentOfType<ColliderComponent>().BoundingObject == gameObject.GetComponentOfType<ColliderComponent>().BoundingObject) != null;
        }
        public bool isWholeInside(GameObject gameObject)
        {
            if (isNotNullNorThisOne(gameObject) == false)
            {
                throw new NoColliderException("this gameComponent(given as method parametr) doesn't have collider");
            }
            if (this.isCollide(gameObject)) //whole inside object also intersects
            {
                if (BoundingObject.Contains(gameObject.GetComponentOfType<ColliderComponent>().BoundingObject) == ContainmentType.Contains)
                {
                    return true;
                }
            }
            return false;
        }
        public bool isEnter(GameObject gameObject)
        {
            if (isNotNullNorThisOne(gameObject) == false)
            {
                throw new NoColliderException("this gameComponent(given as method parametr) doesn't have collider");
            }
            if (isCollide(gameObject))
            {
                if (getPreviousListOfColidingObjects().Contains(gameObject))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public bool isExit(GameObject gameObject)
        {
            if (isNotNullNorThisOne(gameObject) == false)
            {
                throw new NoColliderException("this gameComponent(given as method parametr) doesn't have collider");
            }
            if (getPreviousListOfColidingObjects().Contains(gameObject))
            {
                if (getActualListOfColidingObjects().Contains(gameObject) == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #region assistanceMethods
        private bool isNotNullNorThisOne(GameObject gameObject)
        {
            if (gameObject.GetComponentOfType<ColliderComponent>() != null)
            {
                if (gameObject.GetComponentOfType<ColliderComponent>() != this)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        private void changeListForActualFrame()
        {
            if (actualList == 0) { actualList = 1; }
            else { actualList = 0; }
        }
        #endregion
        public HashSet<GameObject> getActualListOfColidingObjects()
        {
            if (actualList == 0)
            {
                return listOfCollidingObject0;
            }
            else
            {
                return listOfCollidingObject1;
            }
        }
        public HashSet<GameObject> getPreviousListOfColidingObjects()
        {
            if (actualList == 0)
            {
                return listOfCollidingObject1;
            }
            else
            {
                return listOfCollidingObject0;
            }
        }
        public Vector3 getCenter()
        {
            return _boundingObject.getCenter();
        }

        public override void Remove()
        {
            switch (_type)
            {
                case ColliderTypes.Navigation:
                    {
                        ColliderMenager.Instance.NavigationColliderList.Remove(this);
                    }
                    break;
                case ColliderTypes.Normal:
                    {
                        ColliderMenager.Instance.NormalColliderList.Remove(this);
                    }
                    break;
                case ColliderTypes.Physics:
                    {
                        ColliderMenager.Instance.PhysicsColliderList.Remove(this);
                    }
                    break;
                case ColliderTypes.Static:
                    {
                        ColliderMenager.Instance.StaticColliderList.Remove(this);
                    }
                    break;
                case ColliderTypes.Trigger:
                    {
                        ColliderMenager.Instance.TriggerColliderList.Remove(this);
                    }
                    break;
            }
            _boundingObject = null;
            base.Remove();
        }
    }
}
