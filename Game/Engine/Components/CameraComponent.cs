using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using OurGame.Engine.Components;
using System;
using OurGame.Engine.Statics;

namespace OurGame.Engine
{
    [DataContract]
    public class CameraComponent : AbstractComponent
    {
        private static CameraComponent _main;
        private static Vector3 _direction = new Vector3(0, 0, -10);
        public Matrix World, View, Projection;
        public float FarFrustrum { get { return _farFrustrum; } }
        public int CameraReverse = 1;

        public static CameraComponent Main
        {
            get
            {
                if (_main == null)
                    _main = Scene.FindWithTag("MainCamera").GetComponentOfType<CameraComponent>();
                return _main;
            }
            set { _main = value; }
        }

        [DataMember]
        private float _yaw = 0f;
        [DataMember]
        private float _pitch = MathHelper.PiOver2;
        [DataMember]
        public float VerticalScrollSpeed = 0.0005f;
        [DataMember]
        public float HorizontalScrollSpeed = 0.0005f;
        [DataMember]
        private float _lerpSpeed = 0.25f;
        [DataMember]
        private float _farFrustrum = 10000f;

        public CameraComponent(GameObject parent) : base(parent)
        {
            Main = this;
            if (parent.GetComponentOfType<ColliderComponent>() == null)
            {
                throw new Exception("Camera needs some collider");
            }
            else
            {
                ColliderMenager.Instance.player = parent.GetComponentOfType<ColliderComponent>();
            }
        }


        public override void Update(GameTime time)
        {
            ManageInput(time);
            World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            Matrix rotation = Matrix.CreateFromQuaternion(Parent.Transform.Rotation);
            Vector3 reference = Vector3.Transform(_direction, rotation);
            Vector3 cameraLookAt = Parent.Transform.Position + reference;
            //Debug.LogOnScreen("looking at: " +cameraLookAt.ToString(), Debug.ScreenType.Camera, new Vector2(10f, 40f));
            //Debug.LogOnScreen("Camera positon: " + Parent.Transform.Position, Debug.ScreenType.Camera, new Vector2(10f, 60f));
            //Debug.LogOnScreen(Parent.Transform.Rotation.ToString(), Debug.ScreenType.Camera, new Vector2(10, 80));
            //Debug.LogOnScreen("Yaw: " + _yaw, Debug.ScreenType.Camera, new Vector2(10, 100));
            //Debug.LogOnScreen("Pitch" + _pitch, Debug.ScreenType.Camera, new Vector2(10, 120));
            View = Matrix.CreateLookAt(Parent.Transform.Position, cameraLookAt, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)16 / 9, 1f, _farFrustrum);
        }

        private void ManageInput(GameTime time)
        {
            float MouseX = InputManager.MouseXInput * CameraReverse;
            float MouseY = InputManager.MouseYInput;

            _yaw -= MouseX * HorizontalScrollSpeed * (float)time.ElapsedGameTime.TotalMilliseconds;
            _pitch -= MouseY * VerticalScrollSpeed * (float)time.ElapsedGameTime.TotalMilliseconds;
            if (_pitch > 1.5f)
                _pitch = 1.5f;
            if (_pitch < -1.5f)
                _pitch = -1.5f;
            Quaternion destinationRot = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, 0f);
            Quaternion actualRot = Parent.Transform.Rotation;
            Quaternion resultRot = Quaternion.Lerp(actualRot,destinationRot,_lerpSpeed);

            Parent.Transform.Rotation = resultRot;
            //Parent.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, 0f);
        }
    }
}
