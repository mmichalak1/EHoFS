using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using OurGame.Engine.Components;
using OurGame.Engine;
using OurGame.Engine.Components.BoundingObjects;

namespace OurGame.Scripts.Player.SpecialMoves
{
    [DataContract]
    public class Dash : AbstractSpecial
    {
        [DataMember]
        private float _dashSpeed = -5000f;
        private SoundComponent _soundComponent;
        private float multipler = 1000f;

        private float dashTimer = 0f;
        private List<Box> boxes = new List<Box>();

        public Dash(float duration, SpeciaMoveManager parent) : base(duration, parent)
        {
            _soundComponent = parent.Parent.GetComponentOfType<SoundComponent>();
            foreach (var item in ScreenManager.Instance.CurrentScreen.GameObjectList.Where(x => x.Name.Contains("Room")))
            {
                foreach (var wall in item.Children.Where(x => x.Name != "Floor"))
                {
                    boxes.Add(wall.GetComponentOfType<ColliderComponent>().BoundingObject as Box);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (CheckForCollision())
            {
                Parent.isDashOnCooldown = true;
                Parent.StopMove();
                Parent.Parent.GetComponentOfType<RigidBodyComponent>().MaximumSpeed /= Parent.speedAndForceMultipler;
                Parent.Parent.GetComponentOfType<RigidBodyComponent>().MaximumForce /= Parent.speedAndForceMultipler;
                return;
            }

            if (isActive)
            {
                Vector3 Direction = new Vector3();
                if (InputManager.GetKeyDown(KeyBinding.Right))
                {
                    Direction = Vector3.Left * (float)gameTime.ElapsedGameTime.TotalSeconds * _dashSpeed;
                }
                if (InputManager.GetKeyDown(KeyBinding.Left))
                {
                    Direction = Vector3.Right * (float)gameTime.ElapsedGameTime.TotalSeconds * _dashSpeed;
                }
                if (InputManager.GetKeyDown(KeyBinding.Backward))
                {
                    Direction = Vector3.Forward * (float)gameTime.ElapsedGameTime.TotalSeconds * _dashSpeed;
                }
                if (!InputManager.GetKeyDown(KeyBinding.Left) && !InputManager.GetKeyDown(KeyBinding.Right) && !InputManager.GetKeyDown(KeyBinding.Backward))
                {
                    Direction = Vector3.UnitZ * (float)gameTime.ElapsedGameTime.TotalSeconds * _dashSpeed;
                }
                Quaternion rot = Parent.Parent.Transform.Rotation;
                Vector3 res;
                Vector3.Transform(ref Direction, ref rot, out res);
                res.Y = 0f;
                Parent.Parent.GetComponentOfType<RigidBodyComponent>().AddAffectingForce(res * multipler, Duration - dashTimer);
                dashTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (dashTimer > Duration)
                {
                    _soundComponent.Play("dash", false);
                    dashTimer = 0f;
                    isActive = false;
                    Parent.isDashOnCooldown = true;
                    Parent.StopMove();
                    Parent.Parent.GetComponentOfType<RigidBodyComponent>().MaximumSpeed /= Parent.speedAndForceMultipler;
                    Parent.Parent.GetComponentOfType<RigidBodyComponent>().MaximumForce /= Parent.speedAndForceMultipler;
                }
            }
        }
        bool CheckForCollision()
        {
            RayCast cast = new RayCast(Parent.Parent, Vector3.Zero, Parent.Parent.Transform.Orientation);
            float? distance;

            foreach (var item in boxes)
            {
                distance = cast.Ray.Intersects(item.getBox());
                if (distance == null) return false;
                if (distance < 500f)
                    return true;
            }

            return false;
        }
    }
}
