using OurGame.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Scripts.Enviroment.Modifications
{
    public abstract class AbstractModification
    {
        private static int count = 0;
        public static Random random;


        private static List<AbstractModification> _modifications = new List<AbstractModification>
        {
            new OpalescentEnemies(),
            new RainbowEnemies(),
            new GentelManEnemies(),
            new ThugLifeEnemies(),
        };

        private static List<AbstractModification> _randomizableModifications = new List<AbstractModification>(_modifications);

        public static AbstractModification GetRandomModification(Random rand)
        {
            count++;
            if (count == 2)
                return _modifications[2];
            var res = _randomizableModifications[random.Next(0, _randomizableModifications.Count)];
            _randomizableModifications.Remove(res);
            if (_randomizableModifications.Count == 0)
                _randomizableModifications = new List<AbstractModification>(_modifications);          
            return res;
        }

        public Modificator ModificatorType { get; protected set; }

        public abstract void ApplyModification();
        public abstract void ReverseModification();

        protected virtual void OnEnemySpawn(object enemy) { }

        protected void SetModification(object modificator)
        {
            EventSystem.Instance.Send("ModificatorSet", modificator);
        }
    }

    public enum Modificator
    {
        BigEnemies,
        Opalescent,
        GentelMan,
        ThugLife,
        Rainbow
    }
}
