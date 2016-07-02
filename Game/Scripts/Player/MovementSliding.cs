using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using OurGame.Engine;

using System.Runtime.Serialization;
using System;
using OurGame.Engine.Components;

namespace OurGame.Scripts.Player
{
    [DataContract(IsReference = true)]
    public class MovementSliding : IScript
    {
        private GameObject _parent;
        [DataMember]
        private float _speed = 120f;
        private SoundComponent _soundComponent;
        private float time = 0;
        private string[] _footsteps = { "footsteps-01", "footsteps-02", "footsteps-03" };
        Random _rand;

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public void Initialize(GameObject parent)
        {
            _parent = parent;
            _soundComponent = _parent.GetComponentOfType<SoundComponent>();
        }

        public void Update(GameTime gameTime)
        {
            if (InputManager.GetKeyReleased(KeyBinding.Dash))
            {
                EventSystem.Instance.Send("PerformSpecialMove", Moves.Dash);
            }
            ManageNormalMovement(gameTime);
            if (InputManager.GetKeyReleased(KeyBinding.Escape))
            {
                EventSystem.Instance.Send("Paused", null);
            }
            //if (InputManager.GetKeyDown(KeyBinding.Jump))
            //    _parent.GetComponentOfType<RigidBodyComponent>().AddAffectingForce(Vector3.Up * 50, 0.5f);
            //Debug.LogOnScreen(_parent.Transform.Position.ToString(), Debug.ScreenType.General, new Vector2(10f, 40f));
        }

        private void ManageNormalMovement(GameTime gameTime)
        {
            Vector3 Direction = Vector3.Zero;
            if (InputManager.GetKeyDown(KeyBinding.Forward))
            {
                Direction.Z += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (InputManager.GetKeyDown(KeyBinding.Backward))
            {
                Direction.Z -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (InputManager.GetKeyDown(KeyBinding.Left))
            {
                Direction.X += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (InputManager.GetKeyDown(KeyBinding.Right))
            {
                Direction.X -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if(Direction!=Vector3.Zero)
            {
                int i = _rand.Next(_footsteps.Length);
                _soundComponent.Play(_footsteps[i], false);
                Vector3 result;
                Quaternion rot = _parent.Transform.Rotation;
                Vector3.Transform(ref Direction, ref rot, out result);
                result.Normalize();
                result *= _speed;
                result.Y = 0f;
                this._parent.GetComponentOfType<RigidBodyComponent>().AddAffectingForce(result, 0.5f);
            }
        }
    }
}
