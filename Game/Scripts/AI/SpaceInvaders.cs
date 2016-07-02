using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.Components;
using OurGame.Engine.Components.BoundingObjects;
using OurGame.Engine.ExtensionMethods;

namespace OurGame.Scripts.AI
{
    public class SpaceInvaders : IScript
    {
        private GameObject _parent;
        private List<List<GameObject>> _invaders;
        private float _interval = 1000;
        private Vector3 _down = new Vector3(0, 0, 50);
        private Vector3 _left = new Vector3(-50, 0, 0);
        private Vector3 _right = new Vector3(50, 0, 0);
        private float _leftLimit = -1100;
        private float _rightLimit = 1100;
        private Vector3 _active;
        private Random rand;


        public List<List<GameObject>> Invaders
        {
            get { return _invaders; }
            set { _invaders = value; }
        }

        public void Initialize(GameObject parent)
        {
            _parent = parent;
            _invaders = new List<List<GameObject>>();
            _active = _right;
            rand = new Random();
            CreateSpaceInvaders();
        }

        public void Update(GameTime gameTime)
        {
            if (_interval > 0)
            {
                _interval -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                _interval = 1000;
                foreach (List<GameObject> x in _invaders)
                    foreach (GameObject y in x)
                        if (_active == _right && y.Transform.Position.X >= _rightLimit)
                        {
                            move(_down);
                            _active = _left;
                        }
                        else if (_active == _left && y.Transform.Position.X <= _leftLimit)
                        {
                            move(_down);
                            _active = _right;
                        }
                move(_active);
            }
        }

        private void CreateSpaceInvaders()
        {
            List<GameObject> buffer;

            string[] models =
            {
                "Alien1",
                "Alien2",
                "Alien3",
                "Alien4"
            };

            for (int i = 0; i < 5; i++)
            {
                buffer = new List<GameObject>();
                for (int j = 0; j < 6; j++)
                {
                    buffer.Add(CreateInvader(new Vector3(-1100 + j * 300, 200, -1100 + i * 150), models, (i * 6) + j));
                }
                _invaders.Add(buffer);
            }
        }

        private GameObject CreateInvader(Vector3 postion, string[] models, int LP)
        {
            GameObject go = new GameObject(postion, Quaternion.CreateFromYawPitchRoll(MathHelper.Pi, 0, 0));
            go.Tag = "Enemy";
            go.Name = "SpaceInvader" + LP;
            go.AddComponent(new ModelComponent(go, "Alien1", "AxeMaterial", false));
            go.AddComponent(new RigidBodyComponent(go));
            go.AddComponent(new ColliderComponent(go, new Sphere(go, new Vector3(0, 0, 0), 138f), ColliderTypes.Normal));
            go.AddComponent(new SimpleAnimationComponent(go, models, 500f));
            go.AddComponent(new ScriptComponent(go));
            go.GetComponentOfType<ScriptComponent>().AddScript(new Invader());
            go.GetComponentOfType<ScriptComponent>().GetScriptOfType<Invader>().Interval = ((rand.NextFloat() + 1) * 1000) * LP;
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(go);

            return go;
        }

        private void move(Vector3 vect)
        {
            foreach (List<GameObject> x in _invaders)
                foreach (GameObject y in x)
                    y.Transform.Position += vect;
        }


        public void DestroyInvader(GameObject go)
        {
            foreach (List<GameObject> x in _invaders)
                foreach (GameObject y in x)
                    if (y == go)
                    {
                        x.Remove(y);
                        break;
                    }
        }
    }
}
