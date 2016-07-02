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
    public class OpalescentEnemies : AbstractModification
    {
        public OpalescentEnemies()
        {
            ModificatorType = Modificator.Opalescent;
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
            if (x.Name == "Ghost")
                x.GetComponentOfType<ScriptComponent>().GetScriptOfType<Ghost>().ChangingColor = true;
            if (x.Name == "Alien")
                x.GetComponentOfType<ScriptComponent>().GetScriptOfType<Alien>().ChangingColor = true;
        }
    }
}
