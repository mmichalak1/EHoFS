using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using OurGame.Engine.Components;
using OurGame.Scripts.AI;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    public class BossRoomScript : IScript
    {
        private GameObject parent;
        private GameObject bossPlatform;
        private List<GameObject> targets;
        private float counter;
        private bool isCounting;

        public List<GameObject> Targets { get { return targets; } }

        public void Initialize(GameObject parent)
        {
            this.parent = parent;
            List<GameObject> temporaryList;
            counter = 5000;
            isCounting = false;
            targets = new List<GameObject>();
            bossPlatform = parent.FindWithTag("BossPlatform");
            temporaryList = parent.FindObjectsWithTag("Thing");
            foreach (GameObject x in temporaryList)
                targets.Add(x.FindWithTag("Target"));
            EventSystem.Instance.RegisterForEvent("BossIsDead", x =>
            {
                isCounting = true;
            });
        }

        public void Update(GameTime gameTime)
        {
            if(parent.FindWithTag("BossPlatform").FindWithTag("BOSS") != null)
            {
                if (targets.All(x => x.GetComponentOfType<ScriptComponent>().GetScriptOfType<TargetScript>().IsActive == true))
                {
                    parent.FindWithTag("BossPlatform").FindWithTag("BOSS").GetComponentOfType<ScriptComponent>().GetScriptOfType<BossScript>().IsStunned = true;
                    if (bossPlatform.Transform.Position.Y > -35)
                        bossPlatform.Transform.Position -= new Vector3(0, 1000, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    if (bossPlatform.Transform.Position.Y < 250)
                        bossPlatform.Transform.Position += new Vector3(0, 100, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            else if(isCounting)
            {
                counter -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (counter < 0)
                    EventSystem.Instance.Send("Win", null); 
            }
        }
    }
}
