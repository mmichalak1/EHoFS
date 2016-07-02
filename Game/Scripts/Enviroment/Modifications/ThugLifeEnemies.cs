using OurGame.Engine;
using OurGame.Engine.Components;
using OurGame.Scripts.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Scripts.Enviroment.Modifications
{
    class ThugLifeEnemies : AbstractModification
    {
        public ThugLifeEnemies()
        { ModificatorType = Modificator.ThugLife; }
        public override void ApplyModification()
        {
            EventSystem.Instance.RegisterForEvent("EnemySpawned", OnEnemySpawn);
            SetModification(ModificatorType);
        }

        public override void ReverseModification()
        {
            EventSystem.Instance.UnregisterForEvent("EnemySpawned", OnEnemySpawn);
            SetModification(null);
        }

        protected override void OnEnemySpawn(object enemy)
        {
            var script = enemy as Enemy;
            var x = script.Parent;

            script.Damage *= 5;
            script.Health /= 2f;

            if (x.Name == "Alien")
                x.AddComponent(new ModelComponent(x, "ThugGlassesAlien", null, true));
            else if (x.Name == "Ghost")
                x.AddComponent(new ModelComponent(x, "ThugGlassesGhost", null, true));


        }
    }
}
