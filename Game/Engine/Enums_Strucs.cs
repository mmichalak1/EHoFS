using System;
namespace OurGame.Engine
{
    public enum KeyBinding
    {
        Escape,
        Forward,
        Backward,
        Left,
        Right,
        Sprint,
        Jump,
        Dash,
        DebugGeneral,
        DebugPhysics,
        DebugLogic,
        DebugCamera,
        DebugOther,
        NavMeshVisible,
        StartAIScripts,
        CollidersVisible,
        Action
    }

    public enum DisplayMode
    {
        Borderless,
        Windowed,
        Fullscreen
    }

    public enum GameState
    {
        Playing,
        Menu,
        Paused,
        Exiting,
        Death,
        Win
    }

    public enum ColliderTypes
    {
        Static,
        Physics,
        Trigger,
        Navigation,
        Normal
    }

    [Serializable]
    public struct Scale
    {
        private float _x;
        private float _y;
        private float _z;
        public Scale( float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public float X
        {
            get { return _x; }
            set { _x = value; }
        }
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }
        public float Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public static Scale operator *(Scale scale, float value)
        {
            return new Scale()
            {
                X = scale.X *= value,
                Y = scale.Y *= value,
                Z = scale.Z *= value
            };
        }
    };

}