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
    public class RainbowEnemies : AbstractModification
    {
        public RainbowEnemies()
        {
            ModificatorType = Modificator.Rainbow;
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
            script.MaxSpeed *= 1.5f;
            script.MaxForce *= 1.5f;
            script.Mass *= 0.5f;
            var x = script.Parent;
            if (x.Name == "Ghost")
                x.GetComponentOfType<ModelComponent>().DiffuseMap = ContentContainer.TexColor["GhostRainbow"];
            if (x.Name == "Alien")
                x.GetComponentOfType<ModelComponent>().DiffuseMap = ContentContainer.TexColor["AlienRainbow"];
        }
    }
}
