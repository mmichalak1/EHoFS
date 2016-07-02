using System.Collections.Generic;
using OurGame.Engine.Components;
using OurGame.Engine;
using OurGame.Engine.Components.BoundingObjects;
using Microsoft.Xna.Framework;
using System.Linq;

namespace OurGame.Scripts.Player.SpecialMoves
{
    class LeftSlash : AbstractSpecial
    {
        static float Distance = 150f;
        static float DMG = 40f;

        List<BoundingObject> objects;
        float timer = 0f;
        public LeftSlash(float duration, SpeciaMoveManager manager) : base(duration, manager)
        {
            //Debug.LogToConsole("Slashing");
            objects = new List<BoundingObject>();
            foreach (GameObject x in ScreenManager.Instance.CurrentScreen.GameObjectList.Where(x => x.Tag == "Enemy"))
                objects.Add(x.GetComponentOfType<ColliderComponent>().BoundingObject);
            manager.Parent.GetComponentOfType<AnimationComponent>().ChangeClip("Slash_L2R_D2U");
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
                    Debug.LogToConsole("Distance to detected enemy:" + distance);
                    if (distance < Distance)
                        DealDMG(x.Parent);
                }
            }
        }

        private void DealDMG(GameObject enemy)
        {
            enemy.GetComponentOfType<ScriptComponent>().GetScriptOfType<IReciveDamage>().ReceiveDMG(DMG);
            Debug.LogToConsole("Hit!");
            Parent.StopMove();
        }
    }
}
