using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Input;
using OurGame.Engine.Components;
using OurGame.Engine.Statics;
using OurGame.Engine.ExtensionMethods;
using OurGame.Engine.ParticleSystem;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    public class FirstRoomScript : IScript
    {
        [DataMember]
        private GameObject _parent;
        [DataMember]
        private List<GameObject> floorTiles;
        [DataMember]
        public List<GameObject> enemyList;
        private bool flag = false;
        private bool flagMessage1 = true;
        private bool flagMessage2 = true;
        private bool flagMessage3 = true;
        private double timer = 0;
        private double timer2 = 0;
        private int counter = 0;
        private static Random rand;
        private SoundComponent expl;
        public void Initialize(GameObject parent)
        {
            rand = new Random();
            _parent = parent;
            floorTiles = _parent.Children.FindAll(ExtensionMethods.FindTile);
            //enemyList = new List<GameObject>();
            floorTiles.Shuffle();
            expl = _parent.GetComponentOfType<SoundComponent>();
        }

        public void Update(GameTime gameTime)
        {
            timer2 += gameTime.ElapsedGameTime.TotalSeconds;

            if(timer2 > 10 && flagMessage1)
            {
                ConsoleStoryWriter.Instance.WriteSentence("This game is boring, don't you think so?", 5000f);
                flagMessage1 = false;
            }
            if (timer2 > 20 && flagMessage2)
            {
                ConsoleStoryWriter.Instance.WriteSentence("I am going to delete it, with you inside, don't you mind?", 5000f);
                flagMessage2 = false;
            }
            if (timer2 > 30 && flagMessage3)
            {
                ConsoleStoryWriter.Instance.Clear();
                flagMessage3 = false;
            }

            if (enemyList.Count > 0)
            {
                if (enemyList[0].Destroy == true)
                {
                    enemyList.Remove(enemyList[0]);
                }
            }
            if (enemyList.Count == 0 && timer2 > 30)
            {
                flag = true;
                PhysicsEngine.floorLevel = PhysicsEngine.floorLevel - 1000;
            }
            if (flag)
            {
                timer += gameTime.ElapsedGameTime.TotalSeconds;
                if (counter<5 && timer > 0.5)
                {
                    for(int i = 0; i<3; i++)
                    {
                        expl.Play("explodingBarrel", false);
                        ParticleEmiter.Instance.AddParticle(new Vector3(floorTiles[i].Transform.Position.X, floorTiles[i].Transform.Position.Y + 20, floorTiles[i].Transform.Position.Z), "explosionBlue", Vector3.Up, 100f, 0.5f, 200f);
                        for (int j = 0; j < 5; j++)
                        {
                            ParticleEmiter.Instance.AddParticle(floorTiles[i].Transform.Position, "oneZeroEffect", ExtensionMethods.RandomDirection(rand), 100f, 0.5f, 5f);
                        }
                        Vector3 position = new Vector3(floorTiles[i].Transform.Position.X, floorTiles[0].Transform.Position.Y + 20, floorTiles[0].Transform.Position.Z) + ExtensionMethods.RandomDirection(rand) * 100;
                        expl.Play("explodingBarrel", false);
                        ParticleEmiter.Instance.AddParticle(position, "explosionBlue", Vector3.Up, 100f, 0.5f, 200f);
                        for (int j = 0; j < 5; j++)
                        {
                            ParticleEmiter.Instance.AddParticle(position, "oneZeroEffect", ExtensionMethods.RandomDirection(rand), 100f, 0.5f, 5f);
                        }
                        position = new Vector3(floorTiles[i].Transform.Position.X, floorTiles[i].Transform.Position.Y + 20, floorTiles[i].Transform.Position.Z) + ExtensionMethods.RandomDirection(rand) * 100;
                        expl.Play("explodingBarrel", false);
                        ParticleEmiter.Instance.AddParticle(position, "explosionBlue", Vector3.Up, 100f, 0.5f, 200f);
                        for (int j = 0; j < 5; j++)
                        {
                            ParticleEmiter.Instance.AddParticle(position, "oneZeroEffect", ExtensionMethods.RandomDirection(rand), 100f, 0.5f, 5f);
                        }
                    }
                    counter++;
                    floorTiles.Shuffle();
                }
                if(timer > 0.5)
                {
                    if (floorTiles.Count != 0)
                    {
                        floorTiles[0].GetComponentOfType<ColliderComponent>().Type = ColliderTypes.Physics;
                        floorTiles[0].GetComponentOfType<RigidBodyComponent>().Mass = 1f;
                        floorTiles.Remove(floorTiles[0]);
                    }
                    timer = 0;
                }
            }
        }
    }
}
