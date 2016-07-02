using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurGame.Engine.Components;
using Microsoft.Xna.Framework;
using OurGame.Engine.Components.BoundingObjects;
using OurGame.Engine;

namespace OurGame.Scripts.Player.SpecialMoves
{
    class UpperSlash : AbstractSpecial
    {
        static float Distance = 150f;
        static float DMG = 40f;

        List<BoundingObject> objects;
        float timer = 0f;
        public UpperSlash(float duration, SpeciaMoveManager parent) : base(duration, parent)
        {
            objects = new List<BoundingObject>();
            foreach (GameObject x in ScreenManager.Instance.CurrentScreen.GameObjectList.Where(x => x.Tag == "Enemy"))
                objects.Add(x.GetComponentOfType<ColliderComponent>().BoundingObject);
            parent.Parent.GetComponentOfType<AnimationComponent>().ChangeClip("UpperSlash");
        }

        public override void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.Milliseconds;
            if (timer > Duration)
                Parent.StopMove();
            if (timer >= Duration / 2)
            {
                RayCast cast = new RayCast(Parent.Parent, Vector3.Zero, Parent.Parent.Transform.Orientation);
                float? distance;
                foreach (BoundingObject x in objects)
                {
                    distance = cast.Ray.Intersects((x as Sphere).getSphere());
                    if (distance < Distance)
                        DealDMG(x.Parent);
                }
            }
        }

        private void DealDMG(GameObject enemy)
        {
            enemy.GetComponentOfType<ScriptComponent>().GetScriptOfType<IReciveDamage>().ReceiveDMG(DMG);
            Parent.StopMove();
        }
    }
}
