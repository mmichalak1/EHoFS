using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.ExtensionMethods;
using OurGame.Engine.Components;
using OurGame.Engine.Components.BoundingObjects;

namespace OurGame.Scripts.AI
{
    public class Invader : IScript, IReciveDamage
    {
        private GameObject _parent;
        private float _interval;
        private float _health;
        private SpaceInvaders _manager;

        public float Interval
        {
            set { _interval = value; }
        }

        public void Initialize(GameObject parent)
        {
            _parent = parent;
            _health = 100.0f;
            _manager = Scene.FindWithTag("InvadersManager").GetComponentOfType<ScriptComponent>().GetScriptOfType<SpaceInvaders>();
        }

        public void Update(GameTime gameTime)
        {
            if(_interval > 0)
            {
                _interval -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                Random rand = new Random();
                _interval = (rand.NextFloat() + 1 ) * 10000;
                Shoot();
            }
        }

        public void Shoot()
        {
            Vector3 bulletOffset = new Vector3(0, -50f, 0);
            Vector3 bulletPosition = ExtensionMethods.PointToWorld(bulletOffset, _parent.Transform);
            CreateBullet(bulletPosition, _parent.Transform.Orientation);
        }

        private void CreateBullet(Vector3 position, Vector3 direction)
        {
            GameObject go = new GameObject(position, Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(go);
            go.Name = "Bullet";
            go.AddComponent(new ModelComponent(go, "Bullet", null, true));
            go.AddComponent(new ColliderComponent(go, new Sphere(go, Vector3.Zero, 1f), ColliderTypes.Normal));
            go.AddComponent(new ScriptComponent(go));
            go.GetComponentOfType<ScriptComponent>().AddScript(new BulletScript(go, position, direction, 10f));
            go.Transform.Scale = new Scale(10, 10, 10);
        }

        public void ReceiveDMG(float DMG)
        {
            _health -= DMG;
            if (_health <= 0)
            {
                _manager.DestroyInvader(_parent);
                _parent.Destroy = true;
            }
        }
    }
}
