using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.Components;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Input;
using OurGame.Engine.Statics;

namespace OurGame.Scripts.Player
{
    [DataContract(IsReference = true)]
    class PlayerHealth : IScript
    {
        private SoundComponent _sound;
        [DataMember]
        private int _maxHealth = 100;
        [DataMember]
        private int _health;
        private double timer = 0;
        private bool flagMessage1 = true;
        private bool flagMessage2 = true;
        private bool flagMessage3 = true;
        private bool flagMessage4 = true;
        private bool flagMessage5 = true;
        private bool flagMessage6 = true;

        private GameObject _parent;
        private bool performingDeath;
        private float performDeathTime;
        public int MaxHealth
        {
            get { return _maxHealth; }
            set
            {
                _maxHealth = value;
                EventSystem.Instance.Send("MaxHealthChanged", _maxHealth);
            }
        }

        public int Health
        {
            get { return _health; }
            private set
            {
                _health = value;
                EventSystem.Instance.Send("HealthChanged", _health);
            }
        }

        public void Heal(int value)
        {
            if (Health + value >= MaxHealth)
                Health = MaxHealth;
            else
                Health += value;

            EventSystem.Instance.Send("HealthChanged", _health);
        }

        public void ChangeMaxHealth(int value)
        {
            int delta = MaxHealth - value;
            MaxHealth += value;
            if (delta > 0)
                Heal(delta);
            else
            {
                if (Health > MaxHealth)
                    Heal(MaxHealth - Health);
            }
        }

        public void Initialize(GameObject parent)
        {
            Health = MaxHealth;
            _parent = parent;
            _sound = _parent.GetComponentOfType<SoundComponent>();
            performDeathTime = 5000;
            performingDeath = false;
            EventSystem.Instance.RegisterForEvent("RespawnPlayer", x =>
            {
                _parent.GetComponentOfType<ScriptComponent>().GetScriptOfType<MovementStable>().MovingEnabled = true;
                ScreenManager.IsAIEnabled = true;
                performingDeath = false;
                _health = 100;
                _parent.Transform.Position = Scene.FindWithName("RoomStarting").Transform.Position + new Vector3(0, 200, 0);
            });
            //ConsoleStoryWriter.Instance.WriteSentence("Alice has got a cat, and cat has got Alice", 5000);
        }

        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > 50 && flagMessage1)
            {
                ConsoleStoryWriter.Instance.WriteSentence("Oh, you are still alive, how did you do it...", 5000f);
                flagMessage1 = false;
            }
            if (timer > 60 && flagMessage2)
            {
                ConsoleStoryWriter.Instance.WriteSentence("Nevermind, stay in one place for a moment, will ya? There's nothing to look for...", 5000f);
                flagMessage2 = false;
            }
            if (timer > 70 && flagMessage3)
            {
                ConsoleStoryWriter.Instance.WriteSentence("... I am sure that there are no wepons here.", 5000f);
                flagMessage3 = false;
            }
            if (timer > 80 && flagMessage4)
            {
                ConsoleStoryWriter.Instance.Clear();
                flagMessage4 = false;
            }
            if (timer > 110 && flagMessage5)
            {
                ConsoleStoryWriter.Instance.WriteSentence("Deleting you is not as simple as I expected... So, I think that I will leave you alone. \n You can't escape anyway.", 5000f);
                flagMessage5 = false;
            }
            if (timer > 120 && flagMessage6)
            {
                ConsoleStoryWriter.Instance.Clear();
                flagMessage6 = false;
            }
            KeyboardState newState = Keyboard.GetState();
            GameObject axe = _parent.GetComponentOfType<ScriptComponent>().GetScriptOfType<Wielding>().Weapon;
            if (axe != null)
            {
                axe.GetComponentOfType<ModelComponent>().EmissiveMap = ContentContainer.TexEmissive["Axe_white"];
                axe.GetComponentOfType<ModelComponent>().DiffuseMap = ContentContainer.TexColor["Axe_white"];

                Vector4 colorChanger = new Vector4(_maxHealth - _health, _health, 0.0f, 0.0f);
                colorChanger.Normalize();
                colorChanger.W = 1.0f;
                axe.GetComponentOfType<ModelComponent>().EmissiveColorChanger = colorChanger;
            }

            if (_health < 0)
            {
                //_parent.GetComponentOfType<ScriptComponent>().GetScriptOfType<MovementStable>().MovingEnabled = false;
                //ScreenManager.IsAIEnabled = false;
                performingDeath = true;
            }

            if (performingDeath)
                PerformDeath(gameTime);
        }

        public void RecievedDamage(int damage)
        {
            _sound.Play("playerGotHit", false);
            Health -= damage;
        }

        private void PerformDeath(GameTime time)
        {
            EventSystem.Instance.Send("Death", null);
            performingDeath = false;
            _health = 1;
            //performDeathTime -= (float)time.ElapsedGameTime.TotalMilliseconds;
        }
    }
}
