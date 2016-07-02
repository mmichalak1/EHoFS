using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OurGame.Engine;
using System.Linq;
using OurGame.Engine.Components;
using OurGame.Scripts.AI;
using OurGame.Scripts.Enviroment.Modifications;
using OurGame.Engine.Components.BoundingObjects;

namespace OurGame.Scripts.Enviroment
{
    [DataContract(IsReference = true)]
    public class RoomScript : IScript
    {
        private const int MinEnemies = 1;
        private const int MaxEnemies = 3;
        private const float ReinforceRate = 5.0f;
        private const float UpperHeight = 100f;
        private const float LowerHeight = -3500f;
        private const float WallMovementSpeed = 0.05f;
        private static readonly Vector3 MoveVector = new Vector3(0, 3600, 0);

        public bool IsBoss;
        public bool IsRunning;
        public bool IsStarting;
        public int Weapons
        {
            get { return _weapons; }
            set
            {
                if(value==2)
                {
                    GenerateEnemies();
                    IsRunning = true;
                }
                _weapons = value;
            }
        }


        public static RoomScript StartingRoom;

        [DataMember]
        private SoundComponent _sound;
        [DataMember]
        public bool IsFinished;
        [DataMember]
        private int _enemyCounter;
        [DataMember]
        private GameObject _left;
        [DataMember]
        private GameObject _right;
        [DataMember]
        private GameObject _up;
        [DataMember]
        private GameObject _down;
        [DataMember]
        private GameObject _floor;
        [DataMember]
        private GameObject _parent;

        private AbstractModification _modification;

        public Random Random;
        private GameObject _player;
        private ColliderComponent _myColliderComponent;
        private List<GameObject> _positions;
        private GameObject[] _walls;

        private int _weapons;
        private float timer;
        private bool _hasSpawnedEnemies, _hasSpawnedBoss;
        public int GhostCount, AlienCount;

        public GameObject LeftWall
        {
            get { return _left; }
            set { _left = value; }
        }

        public GameObject RightWall
        {
            get { return _right; }
            set { _right = value; }
        }

        public GameObject DownWall
        {
            get { return _down; }
            set { _down = value; }
        }

        public GameObject UpWall
        {
            get { return _up; }
            set { _up = value; }
        }

        public GameObject Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public List<GameObject> Enemies;

        public RoomScript()
        {
            Enemies = new List<GameObject>();
        }

        public void Initialize(GameObject parent)
        {
            _parent = parent;
            _player = Scene.FindWithTag("MainCamera");
            _myColliderComponent = _parent.GetComponentOfType<ColliderComponent>();
            _floor = _parent.Children.First(x => x.Name == "Floor");
            _modification = AbstractModification.GetRandomModification(new Random(_parent.Transform.Position.GetHashCode()));
            _sound = _parent.GetComponentOfType<SoundComponent>();
            _positions = _parent.Children.First(x => x.Name == "Floor").Children.Where(x => x.Tag == "EnemySpawn").ToList();
            _walls = new[]
            {
                _up, _down, _right, _left
            };
            EventSystem.Instance.RegisterForEvent("RespawnPlayer", x =>
            {
                if (Enemies != null && IsRunning && Enemies.Count > 0)
                    foreach (var go in Enemies)
                    {
                        go.Enabled = false;
                        _modification.ReverseModification();
                        foreach (var item in _walls)
                        {
                            if (item.GetComponentOfType<ScriptComponent>().GetScriptOfType<WallScript>().IsUp)
                                item.GetComponentOfType<ScriptComponent>().GetScriptOfType<WallScript>().HideWalls();
                        }
                        IsRunning = false;
                    }
            });
            //{
            //    if (Enemies.Contains(x as GameObject))
            //        DeleteEnemy(x as GameObject);
            //});
        }

        public void Update(GameTime gameTime)
        {
            if (_myColliderComponent.isEnter(_player))
            {
                if (!IsFinished)
                {
                    foreach (var gameObject in _walls)
                        if (!gameObject.GetComponentOfType<ScriptComponent>().GetScriptOfType<WallScript>().IsUp)
                            gameObject.GetComponentOfType<ScriptComponent>().GetScriptOfType<WallScript>().RaiseWalls();

                    _modification.ApplyModification();
                    IsRunning = true;
                    EventSystem.Instance.Send("ChangeActiveRoom", this);
                    if (!IsBoss && !_hasSpawnedEnemies)
                        GenerateEnemies();
                    if (!_hasSpawnedBoss && IsBoss)
                        SpawnBoss();

                    if (Enemies?.Count > 0)
                        foreach (var enemy in Enemies)
                            enemy.Enabled = true;


                }
            }
            if (!IsRunning) return;
            timer += gameTime.ElapsedGameTime.Milliseconds * 0.001f;
            if (timer > ReinforceRate)
            {
                SpawnEnemy(_positions[Random.Next(0, _positions.Count - 1)].Transform.Position);
                timer = 0;
            }

        }

        public void GenerateEnemies()
        {
            _enemyCounter = Random.Next(MinEnemies, MaxEnemies);
            List<GameObject> spawns = _parent.Children.First(x => x.Name == "Floor").Children.Where(x => x.Tag == "EnemySpawn").ToList();
            for (int i = 0; i < spawns.Count(); i++)
            {
                var pos = Random.Next(0, spawns.Count());
                SpawnEnemy(spawns[pos].Transform.Position);
                spawns.RemoveAt(pos);
            }
            _hasSpawnedEnemies = true;
        }
        private void SpawnBoss()
        {
            _hasSpawnedBoss = true;
            string[] modelNames =
            {
                "DonkeyKong1",
                "DonkeyKong2",
                "DonkeyKong3",
                "DonkeyKong4"
            };

            GameObject boss = new GameObject(new Vector3(0f, 170f, 0f), Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(boss);
            boss.Tag = "BOSS";
            boss.Name = "DonkeyKong";
            boss.AddComponent(new ModelComponent(boss, "DonkeyKong1", "Default", false));
            boss.AddComponent(new SimpleAnimationComponent(boss, modelNames, 1000));
            boss.AddComponent(new RigidBodyComponent(boss));
            boss.AddComponent(new SoundComponent(boss));
            boss.AddComponent(new ColliderComponent(boss, new Box(boss, new Vector3(250, 200, 250), new Vector3(0, 100, 0)), ColliderTypes.Static));
            boss.GetComponentOfType<RigidBodyComponent>().AffectedByGravity = false;
            boss.GetComponentOfType<RigidBodyComponent>().Bounciness = 0.00f;
            boss.GetComponentOfType<RigidBodyComponent>().Coeffecient = 0.00f;
            boss.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            boss.AddComponent(new ScriptComponent(boss));
            boss.GetComponentOfType<ScriptComponent>().AddScript(new BossScript());
            boss.Transform.Scale = new Scale(0.5f, 0.5f, 0.5f);
            boss.Parent = Scene.FindWithName("RoomBoss").FindWithName("Floor").FindWithTag("BossPlatform");
            boss.Initialize();
            //Enemies.Add(boss);
        }

        private void SpawnEnemy(Vector3 position)
        {
            if (_enemyCounter < 0) return;
            var res = Random.Next(0, 2);
            GameObject go = null;
            switch (res)
            {
                case 0:
                    go = PrefabManager.GetPrafabClone("Alien");
                    AlienCount++;
                    break;
                case 1:
                    go = PrefabManager.GetPrafabClone("Ghost");
                    GhostCount++;
                    break;
            }
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(go);
            go.GetComponentOfType<ScriptComponent>().GetScriptOfType<Enemy>().RoomScript = this;
            go.Transform.Position = position;
            if (Enemies == null)
                Enemies = new List<GameObject>();
            Enemies.Add(go);
            EventSystem.Instance.Send("EnemySpawned", go.GetComponentOfType<ScriptComponent>().GetScriptOfType<Enemy>());
            _enemyCounter--;
            if (_enemyCounter < 0)
                IsRunning = !IsRunning;
        }

        public void DeleteEnemy(GameObject enemy)
        {
            if (enemy.Name == "Alien")
                AlienCount--;
            else if (enemy.Name == "Ghost")
                GhostCount--;
            Enemies.Remove(enemy);
            if (Enemies.Count == 0)
            {
                EventSystem.Instance.Send("ChangeActiveRoom", null);
                IsFinished = true;
                _floor.GetComponentOfType<ModelComponent>().DiffuseMap = ContentContainer.TexColor["Floor2"];
                foreach (var wall in _walls)
                {
                    if (wall.GetComponentOfType<ScriptComponent>().GetScriptOfType<WallScript>().IsUp)
                        wall.GetComponentOfType<ScriptComponent>().GetScriptOfType<WallScript>().HideWalls();
                }
                _modification.ReverseModification();
            }
        }
    }
}
