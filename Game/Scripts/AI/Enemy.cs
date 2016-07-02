using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.Components;
using OurGame.Engine.Components.BoundingObjects;
using OurGame.Engine.ExtensionMethods;
using OurGame.Engine.Navigation;
using OurGame.Engine.Navigation.Steering;
using OurGame.Scripts.AI.StateMachine;
using OurGame.Scripts.Enviroment;
using System.Linq;
using System.Runtime.Serialization;

namespace OurGame.Scripts.AI
{
    [KnownType(typeof(Alien))]
    [KnownType(typeof(Ghost))]
    [DataContract(IsReference = true)]
    public abstract class Enemy : MovingEntity, IScript, IReciveDamage
    {
        protected GameObject _parent;
        protected GameObject _player;
        protected SoundComponent _audio;
        protected Ray ray;

        protected SteeringBehaviour _steering;
        [DataMember]
        protected float _maxHealth;
        [DataMember]
        protected float _health;
        [DataMember]
        protected float _speed;
        [DataMember]
        protected int _damage;
        [DataMember]
        protected bool _isRecievingDamage = false;
        [DataMember]
        protected float _recievingDamageTime = 100f;
        protected RoomScript _roomScript;
        protected int change;
        protected bool changingColor;
        protected Vector4 previousColor;
        protected string enemySound;

        public int Damage { get { return _damage; } set { _damage = value; } }
        public float Health { get { return _health; } set { _health = value; } }
        public bool IsGentleMan;

        public GameObject Player
        {
            get
            {
                return _player;
            }
        }
        public SteeringBehaviour Steering
        {
            get
            {
                return _steering;
            }
        }
        public GameObject Parent
        {
            get
            {
                return _parent;
            }
        }
        public RoomScript RoomScript
        {
            get { return _roomScript; }
            set
            {
                _roomScript = value;
                if (_steering != null)
                    _steering.GetColliders();
            }
        }

        public bool ChangingColor
        {
            get { return changingColor; }
            set { changingColor = value; }
        }

        public Enemy(float health, float speed, int damage)
        {
            _maxHealth = health;
            _health = health;
            _speed = speed;
            _damage = damage;
        }

        public virtual void Initialize(GameObject parent)
        {
            change = 0;
            changingColor = false;
            _parent = parent;
            _steering = new SteeringBehaviour(this);
            _audio = _parent.GetComponentOfType<SoundComponent>();
            _player = Scene.FindWithTag("MainCamera");
            IsGentleMan = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            Vector4 colorChanger = new Vector4(_maxHealth - _health, _health, 0.0f, 0.0f);
            colorChanger.Normalize();
            colorChanger.W = 1.0f;
            Parent.GetComponentOfType<ModelComponent>().EmissiveColorChanger = colorChanger;
            if (changingColor)
                ColorChanger();

            if (_isRecievingDamage)
            {
                _recievingDamageTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_recievingDamageTime > 0)
                {
                    _parent.GetComponentOfType<ModelComponent>().DiffuseColorChanger = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
                }
                else
                {
                    _isRecievingDamage = false;
                    _recievingDamageTime = 100f;
                    _parent.GetComponentOfType<ModelComponent>().DiffuseColorChanger = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                }
            }

            UpdateForces(gameTime);
        }

        public virtual void UpdateForces(GameTime gameTime)
        {

        }

        public void ReceiveDMG(float DMG)
        {
            _health -= DMG;
            _isRecievingDamage = true;
            _audio.Play("enemyGotHit", false, _parent.Transform.Position);
            if (_health <= 0)
            {
                _parent.Destroy = true;
                _audio.Play("enemyOnDeath", false, _parent.Transform.Position);
                GameObject deathEnemy = PrefabManager.GetPrafabClone("DeathEnemy");
                deathEnemy.Transform.Position = _parent.Transform.Position;
                deathEnemy.Transform.Rotation = _parent.Transform.Rotation;
                deathEnemy.GetComponentOfType<ScriptComponent>().GetScriptOfType<EnemyDeathScript>().setUpPosition();
                ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(deathEnemy);
                ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(deathEnemy.Children.FirstOrDefault(x => x.Name == "HealthBox"));
                RoomScript.DeleteEnemy(_parent);
            }
        }

        public bool SeePlayer()
        {
            Vector3 direction = _player.Transform.Position - _parent.Transform.Position;
            float distanceToPlayer = direction.Length();
            direction.Normalize();
            ray = new Ray(_parent.Transform.Position, direction);
            if ((ray.Intersects((_player.GetComponentOfType<ColliderComponent>().BoundingObject as Sphere).getSphere()) <= 2000 && direction.GetAngleFromVectors(_parent.Transform.Orientation) < MathHelper.PiOver4) || distanceToPlayer < 300f)
                return true;
            else
                return false;
        }

        public void ColorChanger()
        {
            Vector4 color = _parent.GetComponentOfType<ModelComponent>().DiffuseColorChanger;
            if (color == new Vector4(1.0f, 1.0f, 1.0f, 0.0f))
                color = previousColor;

            if (change == 0)
            {
                if (color.Z == 0)
                    color.Z = 255;
                color.X += 1;
                color.Z -= 1;
                if (color.X >= 255)
                    change = 1;
            }
            if (change == 1)
            {
                color.Y += 1;
                color.X -= 1;
                if (color.Y >= 255)
                    change = 2;
            }
            if (change == 2)
            {
                color.Z += 1;
                color.Y -= 1;
                if (color.Z >= 255)
                    change = 0;
            }

            previousColor = new Vector4(color.X, color.Y, color.Z, 0);
            _parent.GetComponentOfType<ModelComponent>().DiffuseColorChanger = color;
        }

    }
}
