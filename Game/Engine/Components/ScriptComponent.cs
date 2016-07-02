using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using OurGame.Scripts;
using System.Runtime.Serialization;
using OurGame.Scripts.Player;
using OurGame.Scripts.Enviroment;
using OurGame.Scripts.AI;

namespace OurGame.Engine.Components
{
    [KnownType(typeof(TwoDEnemy))]
    [KnownType(typeof(MovementSliding))]
    [KnownType(typeof(BillboardingScript))]
    [KnownType(typeof(MovementStable))]
    [KnownType(typeof(MovmentFlying))]
    [KnownType(typeof(SpeciaMoveManager))]
    [KnownType(typeof(RoomScript))]
    [KnownType(typeof(FirstRoomScript))]
    [KnownType(typeof(FirstRoomCatchingPlayerScript))]
    [KnownType(typeof(SkyboxScript))]
    [KnownType(typeof(SceneGenerator))]
    [KnownType(typeof(Shooting))]
    [KnownType(typeof(Shooting))]
    [KnownType(typeof(Wielding))]
    [KnownType(typeof(SpaceInvaders))]
    [KnownType(typeof(Invader))]
    [KnownType(typeof(FanScript))]
    [KnownType(typeof(Alien))]
    [KnownType(typeof(Ghost))]
    [KnownType(typeof(BossScript))]
    [KnownType(typeof(BarrelScript))]
    [KnownType(typeof(TargetScript))]
    [KnownType(typeof(BossRoomScript))]
    [KnownType(typeof(Scripts.Enviroment.BulletScript))]
    [KnownType(typeof(Scripts.Enviroment.EnemyDeathScript))]
    [KnownType(typeof(Scripts.Enviroment.DeathBoxRandomizationTMPScript))]
    [KnownType(typeof(Scripts.Enviroment.HealthBoxScript))]
    [KnownType(typeof(Scripts.AI.BulletScript))]
    [KnownType(typeof(Attack))]
    [KnownType(typeof(PlayerHealth))]
    [KnownType(typeof(AxeScript))]
    [KnownType(typeof(GlovesScript))]
    [KnownType(typeof(WallScript))]
    [DataContract(IsReference = true)]
    class ScriptComponent : AbstractComponent
    {
        [DataMember]
        private List<IScript> _scripts;

        public ScriptComponent(GameObject parent) : base(parent)
        {
            _scripts = new List<IScript>();
        }

        public override void Initialize()
        {
            if (_scripts != null)
                foreach (IScript x in _scripts)
                    x.Initialize(Parent);
        }

        public override void Update(GameTime time)
        {
            if (_scripts != null)
                foreach (IScript x in _scripts)
                    x.Update(time);
        }

        public void AddScript(IScript script)
        {
            _scripts.Add(script);
        }

        public void RemoveScript(IScript script)
        {
            _scripts.Remove(script);
        }

        public IScript GetScriptOfType<IScript>()
        {
            return _scripts.OfType<IScript>().FirstOrDefault();
        }

        public override void Remove()
        {
            _scripts.Clear();
            base.Remove();
        }

    }
}
