using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Scripts.Player
{
    [DataContract(IsReference = true)]
    class MovmentFlying : IScript
    {
        private GameObject _parent;
        private SoundComponent _soundComponent;
        private float time = 0;
        //private string[] _footsteps = { "footsteps-01", "footsteps-02", "footsteps-03" };
        Random _rand;
        [DataMember]
        private float _speed = 10000f;
        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public void Initialize(GameObject parent)
        {
            _parent = parent;
            _rand = new Random();
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
        }

        private void ManageNormalMovement(GameTime gameTime)
        {
            Vector3 Direction = Vector3.Zero;
            if (InputManager.GetKeyDown(KeyBinding.Forward))
            {
                Direction.Z -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (InputManager.GetKeyDown(KeyBinding.Backward))
            {
                Direction.Z += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (InputManager.GetKeyDown(KeyBinding.Left))
            {
                Direction.X -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (InputManager.GetKeyDown(KeyBinding.Right))
            {
                Direction.X += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Direction != Vector3.Zero)
            {
                //int i = _rand.Next(_footsteps.Length);
                //_soundComponent.Play(_footsteps[i], false);
                Vector3 result;
                Quaternion rot = _parent.Transform.Rotation;
                Vector3.Transform(ref Direction, ref rot, out result);
                result.Normalize();
                result *= _speed;
                //result.Y = 0f;
                this._parent.GetComponentOfType<RigidBodyComponent>().SetAffectingForce(result);

                time = 0;
            }
            else
            {
                float eps = 0.1f;
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector3 velocity = this._parent.GetComponentOfType<RigidBodyComponent
                    >().Velocity;
                Vector3 previousVelocity = this._parent.GetComponentOfType<RigidBodyComponent>().PreviousVelocity;
                if ((1 - time) < eps)
                {
                    this._parent.GetComponentOfType<RigidBodyComponent>().Velocity = new Vector3(0, velocity.Y, 0);
                    this._parent.GetComponentOfType<RigidBodyComponent>().PreviousVelocity = new Vector3(0, previousVelocity.Y, 0);
                }
                else
                {
                    this._parent.GetComponentOfType<RigidBodyComponent>().Velocity = Vector3.Lerp(velocity, new Vector3(0, velocity.Y, 0), time);
                    this._parent.GetComponentOfType<RigidBodyComponent>().PreviousVelocity = Vector3.Lerp(velocity, new Vector3(0, previousVelocity.Y, 0), time);
                }
            }
        }
    }
}
