using OurGame.Engine;
using OurGame.Engine.Components;
using OurGame.Scripts.AI;

namespace OurGame.Scripts.Enviroment.Modifications
{
    public class GentelManEnemies : AbstractModification
    {
        public GentelManEnemies()
        {
            ModificatorType = Modificator.GentelMan;
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
            script.Damage *= 2;
            if (x.Name == "Ghost")
            {
                x.AddComponent(new ModelComponent(x, "GentelManGhost", null, true));
                //x.GetComponentOfType<ScriptComponent>().GetScriptOfType<Ghost>().IsGentleMan = true;
            }
            if (x.Name == "Alien")
            {
                x.AddComponent(new ModelComponent(x, "GentelManAlien", null, true));
                //x.GetComponentOfType<ScriptComponent>().GetScriptOfType<Alien>().IsGentleMan = true;
            }
        }
    }
}
