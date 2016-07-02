using OurGame.Engine;
using OurGame.Scripts.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Scripts.Enviroment.Modifications
{
    class BigEnemies : AbstractModification
    {
        public BigEnemies()
        {
            ModificatorType = Modificator.BigEnemies;
        }
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
            
            x.Transform.Scale *= 1.5f;
        }
    }
}
