using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using OurGame.Engine;
using OurGame.Engine.Components;
using OurGame.Scripts.Player;
using OurGame.Scripts.Enviroment;
using OurGame.Engine.Statics;
using OurGame.Engine.Components.BoundingObjects;
using OurGame.Scripts.AI;
using OurGame.Engine.Debugging;
using Microsoft.Xna.Framework.Audio;
using OurGame.Engine.ParticleSystem;
using System.Collections.Generic;

namespace OurGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OurGame : Game
    {
        private GameState _gameState;
        private GameState gameState
        {
            get { return _gameState; }
            set
            {
                EventSystem.Instance.Send("GameStateChanged", value);
                _gameState = value;
            }
        }
        private bool _isDebugging;
        private bool IsDebugging
        {
            get { return _isDebugging; }
            set
            {
                _isDebugging = value;
            }
        }
        private static OurGame _game;
        private GameObject _go;
        private GameObject _prefab;
        SoundEffectInstance song;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public static OurGame Game
        {
            get
            {
                if (_game == null)
                    _game = new OurGame();
                return _game;
            }
        }

        public OurGame()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO Add your initialization logic here
            ConfigurationManager.Initialize();
            InputManager.Initialize();
            Debug.Initialize();
            PrefabManager.Initialize();
            InputManager.Enabled = false;
            EventSystem.Instance.RegisterForEvent("Death", x =>
            {
                gameState = GameState.Death;
                InputManager.Enabled = false;
                IsMouseVisible = true;
            });
            EventSystem.Instance.RegisterForEvent("Win", x =>
            {
                gameState = GameState.Win;
                InputManager.Enabled = false;
                IsMouseVisible = true;
            });
            EventSystem.Instance.RegisterForEvent("ToMenu", x =>
            {
                gameState = GameState.Paused;
                InputManager.Enabled = false;
                IsMouseVisible = true;
            });
            EventSystem.Instance.RegisterForEvent("QuitGame", x =>
            {
                gameState = GameState.Exiting;
                Scene.SaveScene(ScreenManager.Instance.CurrentScreen as Scene, "scene.xml");
            });
            EventSystem.Instance.RegisterForEvent("GameStart", x =>
            {
                gameState = GameState.Playing;
                InputManager.Enabled = true;
                IsMouseVisible = false;
            });
            EventSystem.Instance.RegisterForEvent("Respawn", x =>
            {
                gameState = GameState.Playing;
                InputManager.Enabled = true;
                IsMouseVisible = false;
                //ContentContainer.Clear();
                //PrefabManager.
                //createFirstRoom();
                //GenerateSceneObjects();
                //Initialize();
                EventSystem.Instance.Send("RespawnPlayer", null);

            });
            EventSystem.Instance.RegisterForEvent("Paused", x =>
            {
                gameState = GameState.Paused;
                InputManager.Enabled = false;
                IsMouseVisible = true;
            });
            SetupDisplay();
            IsDebugging = false;
            DeferredRenderer.Instance.Initialize(GraphicsDevice);
            base.Initialize();
            song.IsLooped = true;
            song.Play();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenManager.Instance.CurrentScreen = new Scene();
            ContentContainer.LoadContent(Content);
            HealthBar.Instance.Initialize();
            PrefabManager.LoadContent(Content);
            _go = new GameObject(Vector3.Zero, Quaternion.Identity);
            _go.AddComponent(new MenuComponent(_go, gameState));
            ScreenManager.Instance.CurrentScreen.Menu = _go;
            //ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(PrefabManager.GetPrafabClone("Room"));
            //ScreenManager.Instance.CurrentScreen = Scene.LoadScene("scene.xml");
            #region Add Object To Scene
            createFirstRoom();
            //CreateSceneForDoingPrefab();
            GenerateSceneObjects();
            //GenerateSceneObjectsAIandAnimation();
            //CreateDemoLevel();
            #endregion
            #region Create Prefab
            //CreatePrefab();
            #endregion
            CameraComponent.Main = Scene.FindWithTag("MainCamera").GetComponentOfType<CameraComponent>();
            ScreenManager.Instance.CurrentScreen.Initialize();
            ConsoleStoryWriter.Instance.Initialize();
            DeferredRenderer.Instance.LoadContent(Content);
            ContentContainer.Fonts.TryGetValue("MyFont", out Debug.debugFont);
            //ContentContainer.Songs.TryGetValue("Song", out song);
            song = ContentContainer.Songs["Song"].CreateInstance();
            Debug.DebugScreensVisibility[Debug.ScreenType.Camera] = true;
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            ScreenManager.Instance.UnloadContent();
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,    
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            if (gameState == GameState.Exiting)
            {
                Exit();
            }
            if (gameState == GameState.Menu || gameState == GameState.Paused || gameState == GameState.Death || gameState == GameState.Win)
            {
                ConsoleStoryWriter.Instance.Pause();
                ScreenManager.Instance.CurrentScreen.UpdateMenu(gameTime);
                return;
            }
            else
            {
                if (IsDebugging)
                {
                    FPSCounter.Update(gameTime);
                    if (InputManager.GetKeyReleased(KeyBinding.DebugGeneral))
                        Debug.DebugScreensVisibility[Debug.ScreenType.General] = !Debug.DebugScreensVisibility[Debug.ScreenType.General];
                    if (InputManager.GetKeyReleased(KeyBinding.DebugPhysics))
                        Debug.DebugScreensVisibility[Debug.ScreenType.Physics] = !Debug.DebugScreensVisibility[Debug.ScreenType.Physics];

                    if (InputManager.GetKeyReleased(KeyBinding.DebugCamera))
                        Debug.DebugScreensVisibility[Debug.ScreenType.Camera] = !Debug.DebugScreensVisibility[Debug.ScreenType.Camera];

                    if (InputManager.GetKeyReleased(KeyBinding.DebugLogic))
                        Debug.DebugScreensVisibility[Debug.ScreenType.Logic] = !Debug.DebugScreensVisibility[Debug.ScreenType.Logic];

                    if (InputManager.GetKeyReleased(KeyBinding.DebugOther))
                        Debug.DebugScreensVisibility[Debug.ScreenType.Other] = !Debug.DebugScreensVisibility[Debug.ScreenType.Other];
                }

                if (InputManager.GetKeyReleased(KeyBinding.CollidersVisible))
                    ScreenManager.IsColliderVisible = !ScreenManager.IsColliderVisible;


                if (InputManager.GetKeyReleased(KeyBinding.NavMeshVisible))
                    ScreenManager.IsNavMeshVisible = !ScreenManager.IsNavMeshVisible;


                if (InputManager.GetKeyReleased(KeyBinding.StartAIScripts))
                    ScreenManager.IsAIEnabled = !ScreenManager.IsAIEnabled;

                ConsoleStoryWriter.Instance.Resume();
                ConsoleStoryWriter.Instance.Update(gameTime);
                ScreenManager.Instance.CurrentScreen.Update(gameTime);
                ParticleEmiter.Instance.Update(gameTime);
                InputManager.Update();
                PhysicsEngine.Instance.Update(gameTime);
     
                base.Update(gameTime);
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.Playing:
                    DeferredRenderer.Instance.Draw(spriteBatch, gameTime);
                    break;
                case GameState.Menu:

                    ScreenManager.Instance.CurrentScreen.DrawMenu(gameTime, spriteBatch);
                    break;
                case GameState.Death:

                    ScreenManager.Instance.CurrentScreen.DrawMenu(gameTime, spriteBatch);
                    break;
                case GameState.Win:

                    ScreenManager.Instance.CurrentScreen.DrawMenu(gameTime, spriteBatch);
                    break;
                case GameState.Paused:

                    ScreenManager.Instance.CurrentScreen.DrawMenu(gameTime, spriteBatch);
                    break;

                default:
                    break;
            }
            Debug.Draw(spriteBatch);
            ConsoleStoryWriter.Instance.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }
        private void SetupDisplay()
        {
            graphics.PreferredBackBufferHeight = GraphicConfiguration.Instance.ScreenHeigth;
            graphics.PreferredBackBufferWidth = GraphicConfiguration.Instance.ScreenWidth;
            IsFixedTimeStep = false;
            Window.Position = new Point(0, 0);
            graphics.SynchronizeWithVerticalRetrace = true;
            switch (GraphicConfiguration.Instance.ViewMode)
            {
                case Engine.DisplayMode.Borderless:
                    Window.IsBorderless = true;
                    break;
                case Engine.DisplayMode.Fullscreen:
                    graphics.IsFullScreen = true;
                    break;
                case Engine.DisplayMode.Windowed:
                    graphics.IsFullScreen = false;
                    break;
                default:
                    break;
            }
            graphics.ApplyChanges();
        }
        private void GenerateSceneObjects()
        {
            //Add Light Scene
            //ScreenManager.Instance.CurrentScreen.AddLightToScene(new Engine.Lights.DirectionalLight(Color.Yellow, new Vector3(0, -1, 0)));
       //     ScreenManager.Instance.CurrentScreen.AddLightToScene(new Engine.Lights.DirectionalLight(Color.White, new Vector3(0, -1, 0)));

            // Player. Main camera
            //CreatePlayer();

            //ScreenGenerator
            _go = new GameObject(Vector3.Zero, Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Name = "RoomGenerator";
            _go.Tag = "RoomGenerator";
            _go.AddComponent(new ScriptComponent(_go));
            _go.GetComponentOfType<ScriptComponent>().AddScript(new SceneGenerator());

            //Mirror
            _go = new GameObject(new Vector3(2100, 2100, 0), Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Transform.Scale = new Scale(50f, 50f, 50f);
            _go.Name = "Reflection";
            //_go.AddComponent(new SkyboxRenderer(_go, "DeathStar", null, true));
            //_go.AddComponent(new ModelComponent(_go, "DeathStar", "Default", true));
            _go.AddComponent(new ReflectionComponent(_go, null, "Skybox"));

            
            //CreateFan();

        }
        private void GenerateSceneObjectsAIandAnimation()
        {

            //Add Light Scene
            ScreenManager.Instance.CurrentScreen.AddLightToScene(new Engine.Lights.DirectionalLight(Color.White, new Vector3(0, -1, 0)));

            CreatePlayer();

            _go = new GameObject(new Vector3(500f, 150f, 10f), Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Tag = "Gloves";
            _go.Name = "Gloves";
            _go.AddComponent(new ModelComponent(_go, "Gloves", "Default", false));
            _go.AddComponent(new ColliderComponent(_go, new Box(_go, 50f, new Vector3(0, 0, 0)), ColliderTypes.Trigger));
            _go.AddComponent(new ScriptComponent(_go));
            _go.GetComponentOfType<ScriptComponent>().AddScript(new GlovesScript());

            GameObject axe = new GameObject(new Vector3(0f, 150f, 200f), Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(axe);
            axe.Tag = "Weapon";
            axe.Name = "Axe";
            axe.AddComponent(new ModelComponent(axe, "Axe", "AxeMaterial", false));
            axe.AddComponent(new ColliderComponent(axe, new Box(axe, 1250f, new Vector3(0, 0, 0)), ColliderTypes.Trigger));
            axe.AddComponent(new ScriptComponent(axe));
            axe.GetComponentOfType<ScriptComponent>().AddScript(new AxeScript());
            axe.Transform.Scale = new Scale(0.04f, 0.05f, 0.04f);

            GameObject room = PrefabManager.GetPrafabClone("Room1");
            room.RemoveComponent(room.GetComponentOfType<ScriptComponent>());
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(room);

            //GameObject ghost = PrefabManager.GetPrafabClone("Ghost");
            //ghost.Transform.Position = new Vector3(0, 100, 0);
            //ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(ghost);

            //GameObject alien = PrefabManager.GetPrafabClone("Alien");
            //alien.Transform.Position = new Vector3(500, 200, 0);
            //ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(alien);

            GameObject barrel = new GameObject(new Vector3(1000,400,100), Quaternion.Identity); 
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(barrel);
            barrel.Tag = "Projectile";
            barrel.Name = "Barrel";
            barrel.AddComponent(new ModelComponent(barrel, "Barrel", "Default", false));
            barrel.Transform.Scale = new Scale(10, 10, 10);
            barrel.GetComponentOfType<ColliderComponent>().Initialize();

        }
        private void CreateDemoLevel()
        {
            //Add Light Scene
            ScreenManager.Instance.CurrentScreen.AddLightToScene(new Engine.Lights.DirectionalLight(Color.White, new Vector3(0, -1, 0)));

            CreatePlayer();

            GameObject room = PrefabManager.GetPrafabClone("Room");
            room.Transform.Scale = new Scale(0.75f, 0.75f, 0.75f);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(room);

            GenerateShields();

            _go = new GameObject(Vector3.Zero, Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Tag = "Music";
            _go.Name = "BackgroundMusic";
            

            _go = new GameObject(Vector3.Zero, Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Tag = "InvadersManager";
            _go.Name = "InvadersManager";
            _go.AddComponent(new ScriptComponent(_go));
            _go.GetComponentOfType<ScriptComponent>().AddScript(new SpaceInvaders());

            _go = PrefabManager.GetPrafabClone("Processor");
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Transform.Position = new Vector3(0, 120f, 800);
            _go.Transform.Scale = new Scale(0.15f, 0.15f, 0.15f);

            _go = new GameObject(new Vector3(0, 135f, 800f), Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Tag = "Gloves";
            _go.Name = "Gloves";
            _go.AddComponent(new ModelComponent(_go, "Gloves", "Default", false));
            _go.AddComponent(new ColliderComponent(_go, new Box(_go, 50f, new Vector3(0, 0, 0)), ColliderTypes.Trigger));
            _go.AddComponent(new ScriptComponent(_go));
            _go.GetComponentOfType<ScriptComponent>().AddScript(new GlovesScript());

            GameObject axe = new GameObject(new Vector3(200f, 150f, 850f), Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(axe);
            axe.Tag = "Weapon";
            axe.Name = "Axe";
            axe.AddComponent(new ModelComponent(axe, "Axe", "AxeMaterial", false));
            axe.AddComponent(new ColliderComponent(axe, new Box(axe, 1250f, new Vector3(0, 0, 0)), ColliderTypes.Trigger));
            axe.AddComponent(new ScriptComponent(axe));
            axe.GetComponentOfType<ScriptComponent>().AddScript(new AxeScript());
            axe.Transform.Scale = new Scale(0.04f, 0.05f, 0.04f);
        }
        private void CreateSceneForDoingPrefab()
        {
            ScreenManager.Instance.CurrentScreen.AddLightToScene(new Engine.Lights.DirectionalLight(Color.White, new Vector3(0, -1, 0)));
            CreatePlayer();
            ColliderMenager.Instance.player.Parent.Transform.Position = new Vector3(600, 200, 600);
            GameObject room = PrefabManager.GetPrafabClone("Room");
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(room);
            GameObject floor = room.Children.Find(x => x.Name == "Floor");
            room.AddComponent(new ScriptComponent(room));
            room.GetComponentOfType<ScriptComponent>().AddScript(new RoomScript());
            room.AddComponent(new ColliderComponent(room, new Box(room, new Vector3(1600), Vector3.Zero), ColliderTypes.Trigger));
            room.AddComponent(new SoundComponent(room));

            #region singleObjects

            ////fan - standing - notRotating
            //GameObject fan = new GameObject(new Vector3(0, 200, 0), Quaternion.Identity);
            //fan.Name = "Fan";
            //fan.AddComponent(new ModelComponent(fan, "Fan", null, false));
            //fan.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            //fan.AddComponent(new RigidBodyComponent(fan));
            //fan.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            //fan.AddComponent(new ColliderComponent(fan, new Box(fan, new Vector3(100, 60, 100), new Vector3(0, -40, 0)), ColliderTypes.Static));
            //fan.Parent = floor;

            //fan - wall
            //GameObject fan = new GameObject(new Vector3(0, 400, 0), Quaternion.Identity);
            //fan.Name = "Fan";
            //fanChild = new GameObject(Vector3.Zero, Quaternion.Identity);
            //fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            //fanChild.Name = "FanOnly";
            //fanChild.AddComponent(new ScriptComponent(fanChild));
            //fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            //fanChild.Transform.Scale = new Scale(4, 4, 4);
            //fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            //fanChild.Parent = fan;
            //fanChild = new GameObject(Vector3.Zero, Quaternion.Identity);
            //fanChild.Name = "FanBase";
            //fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            //fanChild.AddComponent(new RigidBodyComponent(fanChild));
            //fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            //fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 100, 60), new Vector3(0, 0, 40)), ColliderTypes.Static));
            //fanChild.Transform.Scale = new Scale(4, 4, 4);
            //fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            //fanChild.Parent = fan;
            //fan.Parent = room;

            ////procesor
            //GameObject processor = new GameObject(new Vector3(0, 100, 0), Quaternion.Identity);
            //processor.Name = "Processor";
            //processor.AddComponent(new ModelComponent(processor, "Processor", null, false));
            //processor.AddComponent(new RigidBodyComponent(processor));
            //processor.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            //processor.AddComponent(new ColliderComponent(processor, new Box(processor, new Vector3(280, 180, 280), new Vector3(0, 0, 0)), ColliderTypes.Static));
            //processor.Parent = floor;

            ////radiator
            //GameObject radiator = new GameObject(new Vector3(0, 100, 0), Quaternion.Identity);
            //radiator.Name = "Radiator";
            //radiator.AddComponent(new ModelComponent(radiator, "Radiator", null, false));
            //radiator.AddComponent(new RigidBodyComponent(radiator));
            //radiator.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            //radiator.AddComponent(new ColliderComponent(radiator, new Box(radiator, new Vector3(100, 120, 100), new Vector3(-30, 20, 0)), ColliderTypes.Static));
            //radiator.Parent = floor;

            ////thing
            //GameObject thing = new GameObject(new Vector3(0, 100, 0), Quaternion.Identity);
            //thing.Name = "Thing";
            //thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            //thing.AddComponent(new RigidBodyComponent(thing));
            //thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            //thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            //thing.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            //thing.Transform.Scale = new Scale(2, 2, 2);
            //thing.Parent = floor;

            ////ram
            //GameObject ram = new GameObject(new Vector3(0, 170, 0), Quaternion.Identity);
            //ram.Name = "Ram";
            //ram.AddComponent(new ModelComponent(ram, "ram", null, false));
            //ram.AddComponent(new RigidBodyComponent(ram));
            //ram.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            //ram.AddComponent(new ColliderComponent(ram, new Box(ram, new Vector3(320, 50, 200), new Vector3(0, -30, 0)), ColliderTypes.Static));
            //ram.Transform.Scale = new Scale(1, 1.5f, 1);
            //ram.Parent = floor;

            ////comp1
            //GameObject comp1 = new GameObject(new Vector3(0, 100, 0), Quaternion.Identity);
            //comp1.Name = "CompA_1";
            //comp1.AddComponent(new ModelComponent(comp1, "CompA_1", null, false));
            //comp1.AddComponent(new RigidBodyComponent(comp1));
            //comp1.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            //comp1.AddComponent(new ColliderComponent(comp1, new Box(comp1, new Vector3(100, 40, 100), new Vector3(0, 0, 0)), ColliderTypes.Static));
            //comp1.Parent = floor;

            ////comp2
            //GameObject comp2 = new GameObject(new Vector3(0, 100, 0), Quaternion.Identity);
            //comp2.Name = "CompA_2";
            //comp2.AddComponent(new ModelComponent(comp2, "CompA_2", null, false));
            //comp2.AddComponent(new RigidBodyComponent(comp2));
            //comp2.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            //comp2.AddComponent(new ColliderComponent(comp2, new Box(comp2, new Vector3(80, 100, 80), new Vector3(0, 0, 0)), ColliderTypes.Static));
            //comp2.Transform.Scale = new Scale(2, 1, 2);
            //comp2.Parent = floor;

            ////comp3 - not prepered
            //GameObject comp3 = new GameObject(new Vector3(0, 100, 0), Quaternion.Identity);
            //comp3.Name = "CompA_3";
            //comp3.AddComponent(new ModelComponent(comp3, "CompA_3", null, false));
            //comp3.AddComponent(new RigidBodyComponent(comp3));
            //comp3.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            //comp3.AddComponent(new ColliderComponent(comp3, new Box(comp3, new Vector3(80, 100, 80), new Vector3(0, 0, 0)), ColliderTypes.Static));
            //comp3.Parent = floor;

            ////comp4  
            //GameObject comp4 = new GameObject(new Vector3(0, 200, 0), Quaternion.Identity);
            //comp4.Name = "CompA_4";
            //comp4.AddComponent(new ModelComponent(comp4, "CompA_4", null, false));
            //comp4.AddComponent(new RigidBodyComponent(comp4));
            //comp4.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            //comp4.AddComponent(new ColliderComponent(comp4, new Box(comp4, new Vector3(500, 100, 130), new Vector3(0, 0, -130)), ColliderTypes.Static));
            //comp4.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            //comp4.Parent = floor;

            ////comp9  
            //GameObject comp9 = new GameObject(new Vector3(0, 100, 0), Quaternion.Identity);
            //comp9.Name = "CompA_9";
            //comp9.AddComponent(new ModelComponent(comp9, "CompA_9", null, false));
            //comp9.AddComponent(new RigidBodyComponent(comp9));
            //comp9.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            //comp9.AddComponent(new ColliderComponent(comp9, new Box(comp9, new Vector3(90, 180, 90), new Vector3(0, 0, 0)), ColliderTypes.Static));
            //comp9.Parent = floor;

            #endregion

            //createFloor1(floor);
            //createFloor2(floor);
            //createFloor3(floor);
            //createFloor4(floor);
            createFloorBoss(floor);
            //createFloorStartingRoom(floor,room);

            //createFirstRoom();

            //CreateGhost(new Vector3(-500, 200, 0), Quaternion.Identity);
            //CreateAlien(new Vector3(500, 200, 0), Quaternion.Identity);

            _prefab = room;
            _prefab.Name = "RoomBoss";
            CreatePrefab();
        }
        private void createFloor1(GameObject floor)
        {
            GameObject thing = new GameObject(new Vector3(-600, 100, 0), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            thing = new GameObject(new Vector3(-800, 100, -400), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            thing = new GameObject(new Vector3(-800, 100, 400), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            GameObject fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            GameObject fanChild = new GameObject(new Vector3(-1300, 400, 1300), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(-1300, 400, 1300), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 100, 60), new Vector3(0, 0, 40)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            fanChild.Parent = fan;
            fan.Parent = floor;

            fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            fanChild = new GameObject(new Vector3(-400, 400, 1300), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(-400, 400, 1300), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 100, 60), new Vector3(0, 0, 40)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            fanChild.Parent = fan;
            fan.Parent = floor;

            fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            fanChild = new GameObject(new Vector3(-1300, 400, -1300), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(-1300, 400, -1300), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 100, 60), new Vector3(0, 0, -40)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Parent = fan;
            fan.Parent = floor;

            fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            fanChild = new GameObject(new Vector3(-400, 400, -1300), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(-400, 400, -1300), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 100, 60), new Vector3(0, 0, -40)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Parent = fan;
            fan.Parent = floor;

            GameObject ram = new GameObject(new Vector3(1000, 170, 1000), Quaternion.Identity);
            ram.Name = "Ram";
            ram.AddComponent(new ModelComponent(ram, "ram", null, false));
            ram.AddComponent(new RigidBodyComponent(ram));
            ram.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            ram.AddComponent(new ColliderComponent(ram, new Box(ram, new Vector3(320, 50, 200), new Vector3(0, -30, 0)), ColliderTypes.Static));
            ram.Transform.Scale = new Scale(1, 1.5f, 1);
            ram.Parent = floor;

            ram = new GameObject(new Vector3(1000, 170, -1000), Quaternion.Identity);
            ram.Name = "Ram";
            ram.AddComponent(new ModelComponent(ram, "ram", null, false));
            ram.AddComponent(new RigidBodyComponent(ram));
            ram.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            ram.AddComponent(new ColliderComponent(ram, new Box(ram, new Vector3(320, 50, 200), new Vector3(0, -30, 0)), ColliderTypes.Static));
            ram.Transform.Scale = new Scale(1, 1.5f, 1);
            ram.Parent = floor;

            GameObject enemySpawn = new GameObject(new Vector3(1000, 200, 0), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(500, 200, 0), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(400, 200, 1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(400, 200, -1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(-800, 200, -1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(-800, 200, 1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;
        }
        private void createFloor2(GameObject floor)
        {
            GameObject thing = new GameObject(new Vector3(800, 100, 800), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            thing = new GameObject(new Vector3(-800, 100, 800), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            thing = new GameObject(new Vector3(800, 100, -800), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            thing = new GameObject(new Vector3(-800, 100, -800), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            GameObject fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            GameObject fanChild = new GameObject(new Vector3(0, 150, 0), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(0, 150, 0), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 40, 90), new Vector3(0, -20, 0)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fan.Parent = floor;

            GameObject enemySpawn = new GameObject(new Vector3(-1000, 200, 0), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(1000, 200, 0), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(0, 200, 1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(0, 200, -1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(-400, 200, -400), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(400, 200, 400), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;
        }
        private void createFloor3(GameObject floor)
        {
            GameObject fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            GameObject fanChild = new GameObject(new Vector3(0, 400, 1300), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(0, 400, 1300), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 100, 60), new Vector3(0, 0, 40)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            fanChild.Parent = fan;
            fan.Parent = floor;


            fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            fanChild = new GameObject(new Vector3(0, 400, -1300), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(0, 400, -1300), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 100, 60), new Vector3(0, 0, -40)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Parent = fan;
            fan.Parent = floor;

            GameObject ram = new GameObject(new Vector3(0, 170, 0), Quaternion.Identity);
            ram.Name = "Ram";
            ram.AddComponent(new ModelComponent(ram, "ram", null, false));
            ram.AddComponent(new RigidBodyComponent(ram));
            ram.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            ram.AddComponent(new ColliderComponent(ram, new Box(ram, new Vector3(320, 50, 200), new Vector3(0, -30, 0)), ColliderTypes.Static));
            ram.Transform.Scale = new Scale(1, 1.5f, 1);
            ram.Parent = floor;

            fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            fanChild = new GameObject(new Vector3(-1000, 150, -1000), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(-1000, 150, -1000), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 40, 90), new Vector3(0, -20, 0)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fan.Parent = floor;

            fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            fanChild = new GameObject(new Vector3(1000, 150, -1000), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(1000, 150, -1000), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 40, 90), new Vector3(0, -20, 0)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fan.Parent = floor;

            fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            fanChild = new GameObject(new Vector3(-1000, 150, 1000), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(-1000, 150, 1000), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 40, 90), new Vector3(0, -20, 0)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fan.Parent = floor;

            fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            fanChild = new GameObject(new Vector3(1000, 150, 1000), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(1000, 150, 1000), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 40, 90), new Vector3(0, -20, 0)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fan.Parent = floor;

            fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            fanChild = new GameObject(new Vector3(1000, 150, 0), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(1000, 150, 0), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 40, 90), new Vector3(0, -20, 0)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fan.Parent = floor;

            fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            fanChild = new GameObject(new Vector3(-1000, 150, 0), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(-1000, 150, 0), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 40, 90), new Vector3(0, -20, 0)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fan.Parent = floor;

            GameObject enemySpawn = new GameObject(new Vector3(-1000, 200, -400), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(-1000, 200, 400), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(1000, 200, -400), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(1000, 200, 400), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(0, 200, 800), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(0, 200, -800), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;
        }
        private void createFloor4(GameObject floor)
        {
            GameObject thing = new GameObject(new Vector3(0, 100, 0), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            thing = new GameObject(new Vector3(400, 100, 400), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            thing = new GameObject(new Vector3(-400, 100, 400), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            thing = new GameObject(new Vector3(400, 100, -400), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            thing = new GameObject(new Vector3(-400, 100, -400), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            GameObject fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            GameObject fanChild = new GameObject(new Vector3(-1000, 150, -1000), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(-1000, 150, -1000), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 40, 90), new Vector3(0, -20, 0)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(2, 2, 2);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            fanChild.Parent = fan;
            fan.Parent = floor;

            GameObject processor = new GameObject(new Vector3(1000, 100, 1000), Quaternion.Identity);
            processor.Name = "Processor";
            processor.AddComponent(new ModelComponent(processor, "Processor", null, false));
            processor.AddComponent(new RigidBodyComponent(processor));
            processor.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            processor.AddComponent(new ColliderComponent(processor, new Box(processor, new Vector3(280, 180, 280), new Vector3(0, 0, 0)), ColliderTypes.Static));
            processor.Transform.Scale = new Scale(0.5f, 0.5f, 0.5f);
            processor.Parent = floor;

            fan = new GameObject(Vector3.Zero, Quaternion.Identity);
            fan.Name = "Fan";
            fanChild = new GameObject(new Vector3(-1000, 400, 1300), Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            fanChild.Parent = fan;
            fanChild = new GameObject(new Vector3(-1000, 400, 1300), Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(90, 100, 60), new Vector3(0, 0, 40)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            fanChild.Parent = fan;
            fan.Parent = floor;

            GameObject enemySpawn = new GameObject(new Vector3(-1000, 200, 0), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(1000, 200, 0), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(0, 200, 1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(0, 200, -1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(-1000, 200, 1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(1000, 200, -1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;
        }
        private void createFloorBoss(GameObject floor)
        {
            GameObject thing = new GameObject(new Vector3(-1450, 100, -1450), Quaternion.Identity);
            thing.Tag = "Thing";
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            GameObject target = new GameObject(new Vector3(0, 600, 0), Quaternion.CreateFromYawPitchRoll(MathHelper.PiOver2, 0, 0));
            target.Tag = "Target";
            target.Name = "Target";
            target.AddComponent(new ModelComponent(target, "Target", null, true));
            target.AddComponent(new ColliderComponent(target, new Box(target, new Vector3(40, 80, 80), new Vector3(0, 80, 0)), ColliderTypes.Normal));
            target.AddComponent(new ScriptComponent(target));
            target.GetComponentOfType<ScriptComponent>().AddScript(new TargetScript());
            target.Parent = thing;

            thing = new GameObject(new Vector3(1450, 100, -1450), Quaternion.Identity);
            thing.Tag = "Thing";
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            target = new GameObject(new Vector3(0, 600, 0), Quaternion.CreateFromYawPitchRoll(-MathHelper.PiOver2, 0, 0));
            target.Tag = "Target";
            target.Name = "Target";
            target.AddComponent(new ModelComponent(target, "Target", null, true));
            target.AddComponent(new ColliderComponent(target, new Box(target, new Vector3(40, 80, 80), new Vector3(0, 80, 0)), ColliderTypes.Normal));
            target.AddComponent(new ScriptComponent(target));
            target.GetComponentOfType<ScriptComponent>().AddScript(new TargetScript());
            target.Parent = thing;

            thing = new GameObject(new Vector3(-1450, 100, 1450), Quaternion.Identity);
            thing.Tag = "Thing";
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            target = new GameObject(new Vector3(0, 600, 0), Quaternion.CreateFromYawPitchRoll(MathHelper.PiOver2, 0, 0));
            target.Tag = "Target";
            target.Name = "Target";
            target.AddComponent(new ModelComponent(target, "Target", null, true));
            target.AddComponent(new ColliderComponent(target, new Box(target, new Vector3(40, 80, 80), new Vector3(0, 80, 0)), ColliderTypes.Normal));
            target.AddComponent(new ScriptComponent(target));
            target.GetComponentOfType<ScriptComponent>().AddScript(new TargetScript());
            target.Parent = thing;

            thing = new GameObject(new Vector3(1450, 100, 1450), Quaternion.Identity);
            thing.Tag = "Thing";
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            target = new GameObject(new Vector3(0, 600, 0), Quaternion.CreateFromYawPitchRoll(-MathHelper.PiOver2, 0, 0));
            target.Tag = "Target";
            target.Name = "Target";
            target.AddComponent(new ModelComponent(target, "Target", null, true));
            target.AddComponent(new ColliderComponent(target, new Box(target, new Vector3(40, 80, 80), new Vector3(0, 80, 0)), ColliderTypes.Normal));
            target.AddComponent(new ScriptComponent(target));
            target.GetComponentOfType<ScriptComponent>().AddScript(new TargetScript());
            target.Parent = thing;

            thing = new GameObject(new Vector3(1450, 100, 0), Quaternion.Identity);
            thing.Tag = "Thing";
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            target = new GameObject(new Vector3(0, 600, 0), Quaternion.CreateFromYawPitchRoll(-MathHelper.PiOver2, 0, 0));
            target.Tag = "Target";
            target.Name = "Target";
            target.AddComponent(new ModelComponent(target, "Target", null, true));
            target.AddComponent(new ColliderComponent(target, new Box(target, new Vector3(40, 80, 80), new Vector3(0, 80, 0)), ColliderTypes.Normal));
            target.AddComponent(new ScriptComponent(target));
            target.GetComponentOfType<ScriptComponent>().AddScript(new TargetScript());
            target.Parent = thing;

            thing = new GameObject(new Vector3(-1450, 100, 0), Quaternion.Identity);
            thing.Tag = "Thing";
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            target = new GameObject(new Vector3(0, 600, 0), Quaternion.CreateFromYawPitchRoll(MathHelper.PiOver2, 0, 0));
            target.Tag = "Target";
            target.Name = "Target";
            target.AddComponent(new ModelComponent(target, "Target", null, true));
            target.AddComponent(new ColliderComponent(target, new Box(target, new Vector3(40, 80, 80), new Vector3(0, 80, 0)), ColliderTypes.Normal));
            target.AddComponent(new ScriptComponent(target));
            target.GetComponentOfType<ScriptComponent>().AddScript(new TargetScript());
            target.Parent = thing;

            GameObject processor = new GameObject(new Vector3(0, -35, 0), Quaternion.Identity);
            processor.Tag = "BossPlatform";
            processor.Name = "Processor";
            processor.AddComponent(new ModelComponent(processor, "Processor", null, true));
            processor.AddComponent(new RigidBodyComponent(processor));
            processor.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            processor.AddComponent(new ColliderComponent(processor, new Box(processor, new Vector3(280, 250, 280), new Vector3(0, -220, 0)), ColliderTypes.Static));
            processor.AddComponent(new ColliderComponent(processor, new Box(processor, new Vector3(250, 50, 250), new Vector3(0, 0, 0)), ColliderTypes.Static));
            processor.Parent = floor;

            floor.AddComponent(new ScriptComponent(floor));
            floor.GetComponentOfType<ScriptComponent>().AddScript(new BossRoomScript());

            //_go = new GameObject(new Vector3(0, 135f, 800f), Quaternion.Identity);
            //ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            //_go.Tag = "Gloves";
            //_go.Name = "Gloves";
            //_go.AddComponent(new ModelComponent(_go, "Gloves", "Default", false));
            //_go.AddComponent(new ColliderComponent(_go, new Box(_go, 50f, new Vector3(0, 0, 0)), ColliderTypes.Trigger));
            //_go.AddComponent(new ScriptComponent(_go));
            //_go.GetComponentOfType<ScriptComponent>().AddScript(new GlovesScript());

            GameObject enemySpawn = new GameObject(new Vector3(0, 400, 0), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(0, 200, 1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(0, 200, -1000), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

        }
        private void createFloorStartingRoom(GameObject floor, GameObject room)
        {
            GameObject comp1 = new GameObject(new Vector3(-600, 100, 800), Quaternion.Identity);
            comp1.Name = "CompA_1";
            comp1.AddComponent(new ModelComponent(comp1, "CompA_1", null, false));
            comp1.AddComponent(new RigidBodyComponent(comp1));
            comp1.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            comp1.AddComponent(new ColliderComponent(comp1, new Box(comp1, new Vector3(100, 5, 100), new Vector3(0, 0, 0)), ColliderTypes.Static));
            comp1.Parent = floor;

            comp1 = new GameObject(new Vector3(-600, 100, -800), Quaternion.Identity);
            comp1.Name = "CompA_1";
            comp1.AddComponent(new ModelComponent(comp1, "CompA_1", null, false));
            comp1.AddComponent(new RigidBodyComponent(comp1));
            comp1.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            comp1.AddComponent(new ColliderComponent(comp1, new Box(comp1, new Vector3(100, 5, 100), new Vector3(0, 0, 0)), ColliderTypes.Static));
            comp1.Parent = floor;

            GameObject gloves = new GameObject(new Vector3(-600, 150f, 800f), Quaternion.Identity);
            gloves.Tag = "Gloves";
            gloves.Name = "Gloves";
            gloves.AddComponent(new ModelComponent(gloves, "Gloves", "Default", false));
            gloves.AddComponent(new ColliderComponent(gloves, new Box(gloves, 50f, new Vector3(0, 0, 0)), ColliderTypes.Trigger));
            gloves.AddComponent(new ScriptComponent(gloves));
            gloves.GetComponentOfType<ScriptComponent>().AddScript(new GlovesScript());
            gloves.Parent = room;

            GameObject axe = new GameObject(new Vector3(-600f, 150f, -800f), Quaternion.Identity);
            axe.Tag = "Weapon";
            axe.Name = "Axe";
            axe.AddComponent(new ModelComponent(axe, "Axe", "AxeMaterial", false));
            axe.AddComponent(new ColliderComponent(axe, new Box(axe, 1250f, new Vector3(0, 0, 0)), ColliderTypes.Trigger));
            axe.AddComponent(new ScriptComponent(axe));
            axe.GetComponentOfType<ScriptComponent>().AddScript(new AxeScript());
            axe.Transform.Scale = new Scale(0.04f, 0.05f, 0.04f);
            axe.Parent = room;

            GameObject comp4 = new GameObject(new Vector3(0, 200, 1600), Quaternion.Identity);
            comp4.Name = "CompA_4";
            comp4.AddComponent(new ModelComponent(comp4, "CompA_4", null, false));
            comp4.AddComponent(new RigidBodyComponent(comp4));
            comp4.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            comp4.AddComponent(new ColliderComponent(comp4, new Box(comp4, new Vector3(500, 100, 130), new Vector3(0, 0, -130)), ColliderTypes.Static));
            comp4.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            comp4.Parent = floor;

            GameObject thing = new GameObject(new Vector3(1200, 100, -1000), Quaternion.Identity);
            thing.Name = "Thing";
            thing.AddComponent(new ModelComponent(thing, "thing", null, false));
            thing.AddComponent(new RigidBodyComponent(thing));
            thing.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            thing.AddComponent(new ColliderComponent(thing, new Box(thing, new Vector3(110, 250, 110), new Vector3(0, 0, 0)), ColliderTypes.Static));
            thing.Transform.Scale = new Scale(2, 2, 2);
            thing.Parent = floor;

            GameObject comp2 = new GameObject(new Vector3(1000, 100, 1000), Quaternion.Identity);
            comp2.Name = "CompA_2";
            comp2.AddComponent(new ModelComponent(comp2, "CompA_2", null, false));
            comp2.AddComponent(new RigidBodyComponent(comp2));
            comp2.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            comp2.AddComponent(new ColliderComponent(comp2, new Box(comp2, new Vector3(80, 100, 80), new Vector3(0, 0, 0)), ColliderTypes.Static));
            comp2.Transform.Scale = new Scale(2, 1, 2);
            comp2.Parent = floor;

            GameObject enemySpawn = new GameObject(new Vector3(600, 200, 600), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(1000, 200, -600), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(600, 200, -600), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(1000, 200, -200), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(600, 200, -200), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

            enemySpawn = new GameObject(new Vector3(1000, 200, 600), Quaternion.Identity);
            enemySpawn.Name = "EnemySpawn";
            enemySpawn.Tag = "EnemySpawn";
            enemySpawn.Parent = floor;

        }
        private void createFirstRoom()
        {
            ScreenManager.Instance.CurrentScreen.AddLightToScene(new Engine.Lights.DirectionalLight(Color.White, new Vector3(0, -1, 0)));
            CreatePlayer();
            ColliderMenager.Instance.player.Parent.Transform.Position = new Vector3(200, 2100, 350);
            PhysicsEngine.floorLevel = 2101;

            GameObject startingRoom = new GameObject(new Vector3(0, 2000, 400), Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(startingRoom);
            startingRoom.Name = "StartingRoom";
            startingRoom.AddComponent(new SoundComponent(startingRoom));
            startingRoom.GetComponentOfType<SoundComponent>();
            startingRoom.AddComponent(new ModelComponent(startingRoom, "startingFloor", "StartingRoom", true));
            startingRoom.Transform.Scale = new Scale(100, 100, 100);
            //startingRoom.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(-90));
            startingRoom.AddComponent(new ScriptComponent(startingRoom));
            startingRoom.GetComponentOfType<ScriptComponent>().AddScript(new FirstRoomScript());
            

            GameObject startingRoomCollider;

            startingRoomCollider = new GameObject(new Vector3(0, 0, 0), Quaternion.Identity);
            startingRoomCollider.Name = "StartingRoomCollider";
            startingRoomCollider.AddComponent(new RigidBodyComponent(startingRoomCollider));
            startingRoomCollider.GetComponentOfType<RigidBodyComponent>().Mass = 0;
            startingRoomCollider.AddComponent(new ColliderComponent(startingRoomCollider, new Box(startingRoomCollider, new Vector3(1600, 200, 200), new Vector3(1200, 100, -1350)), ColliderTypes.Static));
            startingRoomCollider.Parent = startingRoom;

            startingRoomCollider = new GameObject(new Vector3(0, 0, 0), Quaternion.Identity);
            startingRoomCollider.Name = "StartingRoomCollider";
            startingRoomCollider.AddComponent(new RigidBodyComponent(startingRoomCollider));
            startingRoomCollider.GetComponentOfType<RigidBodyComponent>().Mass = 0;
            startingRoomCollider.AddComponent(new ColliderComponent(startingRoomCollider, new Box(startingRoomCollider, new Vector3(1600, 200, 200), new Vector3(1200, 100, 950)), ColliderTypes.Static));
            startingRoomCollider.Parent = startingRoom;

            startingRoomCollider = new GameObject(new Vector3(0, 0, 0), Quaternion.Identity);
            startingRoomCollider.Name = "StartingRoomCollider";
            startingRoomCollider.AddComponent(new RigidBodyComponent(startingRoomCollider));
            startingRoomCollider.GetComponentOfType<RigidBodyComponent>().Mass = 0;
            startingRoomCollider.AddComponent(new ColliderComponent(startingRoomCollider, new Box(startingRoomCollider, new Vector3(200, 200, 1600), new Vector3(2950, 100, 0)), ColliderTypes.Static));
            startingRoomCollider.Parent = startingRoom;

            startingRoomCollider = new GameObject(new Vector3(0, 0, 0), Quaternion.Identity);
            startingRoomCollider.Name = "StartingRoomCollider";
            startingRoomCollider.AddComponent(new RigidBodyComponent(startingRoomCollider));
            startingRoomCollider.GetComponentOfType<RigidBodyComponent>().Mass = 0;
            startingRoomCollider.AddComponent(new ColliderComponent(startingRoomCollider, new Box(startingRoomCollider, new Vector3(170, 400, 170), new Vector3(-160, 100, -180)), ColliderTypes.Static));
            startingRoomCollider.Parent = startingRoom;

            startingRoomCollider = new GameObject(new Vector3(0, 0, 0), Quaternion.Identity);
            startingRoomCollider.Name = "StartingRoomCollider";
            startingRoomCollider.AddComponent(new RigidBodyComponent(startingRoomCollider));
            startingRoomCollider.GetComponentOfType<RigidBodyComponent>().Mass = 0;
            startingRoomCollider.AddComponent(new ColliderComponent(startingRoomCollider, new Box(startingRoomCollider, new Vector3(400, 200, 400), new Vector3(425, 100, -750)), ColliderTypes.Static));
            startingRoomCollider.Parent = startingRoom;

            startingRoomCollider = new GameObject(new Vector3(0, 0, 0), Quaternion.Identity);
            startingRoomCollider.Name = "StartingRoomCollider";
            startingRoomCollider.AddComponent(new RigidBodyComponent(startingRoomCollider));
            startingRoomCollider.GetComponentOfType<RigidBodyComponent>().Mass = 0;
            startingRoomCollider.AddComponent(new ColliderComponent(startingRoomCollider, new Box(startingRoomCollider, new Vector3(400, 200, 400), new Vector3(425, 100, 400)), ColliderTypes.Static));
            startingRoomCollider.Parent = startingRoom;

            GameObject floor = new GameObject(new Vector3(0, 0, 0), Quaternion.Identity);
            floor.AddComponent(new ModelComponent(floor, "startingFloorTile", "StartingRoom", false));
            floor.Name = "floorTile";
            floor.AddComponent(new RigidBodyComponent(floor));
            floor.GetComponentOfType<RigidBodyComponent>().Mass = 0;
            floor.AddComponent(new ColliderComponent(floor, new Box(floor, new Vector3(0.85f, 1, 0.85f), new Vector3(1, -1, -1)), ColliderTypes.Static));
            floor.Transform.Scale = new Scale(200, 200, 200);
            floor.Parent = startingRoom;

            floor = new GameObject(new Vector3(400, 0, 0), Quaternion.Identity);
            floor.AddComponent(new ModelComponent(floor, "startingFloorTile", "StartingRoom", false));
            floor.Name = "floorTile";
            floor.AddComponent(new RigidBodyComponent(floor));
            floor.GetComponentOfType<RigidBodyComponent>().Mass = 0;
            floor.AddComponent(new ColliderComponent(floor, new Box(floor, new Vector3(0.85f, 1, 0.85f), new Vector3(1, -1, -1)), ColliderTypes.Static));
            floor.Transform.Scale = new Scale(200, 200, 200);
            floor.Parent = startingRoom;

            for (int i = 0; i < 5; i++)
            {
                int position = i * 400 + 800;

                floor = new GameObject(new Vector3(position, 0, 0), Quaternion.Identity);
                floor.AddComponent(new ModelComponent(floor, "startingFloorTile", "StartingRoom", false));
                floor.Name = "floorTile";
                floor.AddComponent(new RigidBodyComponent(floor));
                floor.GetComponentOfType<RigidBodyComponent>().Mass = 0;
                floor.AddComponent(new ColliderComponent(floor, new Box(floor, new Vector3(0.85f, 1, 0.85f), new Vector3(1, -1, -1)), ColliderTypes.Static));
                floor.Transform.Scale = new Scale(200, 200, 200);
                floor.Parent = startingRoom;

                floor = new GameObject(new Vector3(position, 0, -400), Quaternion.Identity);
                floor.AddComponent(new ModelComponent(floor, "startingFloorTile", "StartingRoom", false));
                floor.Name = "floorTile";
                floor.AddComponent(new RigidBodyComponent(floor));
                floor.GetComponentOfType<RigidBodyComponent>().Mass = 0;
                floor.AddComponent(new ColliderComponent(floor, new Box(floor, new Vector3(0.85f, 1, 0.85f), new Vector3(1, -1, -1)), ColliderTypes.Static));
                floor.Transform.Scale = new Scale(200, 200, 200);
                floor.Parent = startingRoom;

                floor = new GameObject(new Vector3(position, 0, 400), Quaternion.Identity);
                floor.AddComponent(new ModelComponent(floor, "startingFloorTile", "StartingRoom", false));
                floor.Name = "floorTile";
                floor.AddComponent(new RigidBodyComponent(floor));
                floor.GetComponentOfType<RigidBodyComponent>().Mass = 0;
                floor.AddComponent(new ColliderComponent(floor, new Box(floor, new Vector3(0.85f, 1, 0.85f), new Vector3(1, -1, -1)), ColliderTypes.Static));
                floor.Transform.Scale = new Scale(200, 200, 200);
                floor.Parent = startingRoom;

                floor = new GameObject(new Vector3(position, 0, -800), Quaternion.Identity);
                floor.AddComponent(new ModelComponent(floor, "startingFloorTile", "StartingRoom", false));
                floor.Name = "floorTile";
                floor.AddComponent(new RigidBodyComponent(floor));
                floor.GetComponentOfType<RigidBodyComponent>().Mass = 0;
                floor.AddComponent(new ColliderComponent(floor, new Box(floor, new Vector3(0.85f, 1, 0.85f), new Vector3(1, -1, -1)), ColliderTypes.Static));
                floor.Transform.Scale = new Scale(200, 200, 200);
                floor.Parent = startingRoom;

                floor = new GameObject(new Vector3(position, 0, 800), Quaternion.Identity);
                floor.AddComponent(new ModelComponent(floor, "startingFloorTile", "StartingRoom", false));
                floor.Name = "floorTile";
                floor.AddComponent(new RigidBodyComponent(floor));
                floor.GetComponentOfType<RigidBodyComponent>().Mass = 0;
                floor.AddComponent(new ColliderComponent(floor, new Box(floor, new Vector3(0.85f, 1, 0.85f), new Vector3(1, -1, -1)), ColliderTypes.Static));
                floor.Transform.Scale = new Scale(200, 200, 200);
                floor.Parent = startingRoom;
            }

            GameObject startingRoomCatchingPlayerCollider = new GameObject(new Vector3(0, 0, 0), Quaternion.Identity);
            startingRoomCatchingPlayerCollider.Name = "StartingRoomCatchingPlayerCollider";
            startingRoomCatchingPlayerCollider.AddComponent(new ColliderComponent(startingRoomCollider, new Box(startingRoomCollider, new Vector3(8000, 200, 8000),new Vector3(0,-1000,0)), ColliderTypes.Trigger));
            startingRoomCatchingPlayerCollider.AddComponent(new ScriptComponent(startingRoomCatchingPlayerCollider));
            startingRoomCatchingPlayerCollider.GetComponentOfType<ScriptComponent>().AddScript(new FirstRoomCatchingPlayerScript());
            startingRoomCatchingPlayerCollider.Parent = startingRoom;
            floor.Parent = startingRoom;

            GameObject enemy = Create2Denemy(new Vector3(2500, 100, 0), Quaternion.Identity);
            enemy.Parent = startingRoom;
            startingRoom.GetComponentOfType<ScriptComponent>().GetScriptOfType<FirstRoomScript>().enemyList = new List<GameObject>();
            startingRoom.GetComponentOfType<ScriptComponent>().GetScriptOfType<FirstRoomScript>().enemyList.Add(enemy);


        }
        private void CreateFan()
        {
            GameObject fan = new GameObject(new Vector3(3500, 500, 21000), Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(fan);
            fan.Name = "Fan";
            GameObject fanChild = new GameObject(Vector3.Zero, Quaternion.Identity);
            fanChild.AddComponent(new ModelComponent(fanChild, "FanOnly", null, true));
            fanChild.Name = "FanOnly";
            Texture2D texture = null;
            ContentContainer.TexColor.TryGetValue("Fan", out texture);
            fanChild.GetComponentOfType<ModelComponent>().DiffuseMap = texture;
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new SoundComponent(fanChild));
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(80, 100, 60), new Vector3(0, 0, 40)), ColliderTypes.Static));
            fanChild.AddComponent(new ScriptComponent(fanChild));
            fanChild.GetComponentOfType<ScriptComponent>().AddScript(new FanScript());
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            fanChild.Parent = fan;
            fanChild = new GameObject(Vector3.Zero, Quaternion.Identity);
            fanChild.Name = "FanBase";
            fanChild.AddComponent(new ModelComponent(fanChild, "FanBase", null, true));
            ContentContainer.TexColor.TryGetValue("Fan", out texture);
            fanChild.GetComponentOfType<ModelComponent>().DiffuseMap = texture;
            fanChild.AddComponent(new RigidBodyComponent(fanChild));
            fanChild.GetComponentOfType<RigidBodyComponent>().Mass = 0f;
            fanChild.AddComponent(new ColliderComponent(fanChild, new Box(fanChild, new Vector3(80, 100, 60), new Vector3(0, 0, 40)), ColliderTypes.Static));
            fanChild.Transform.Scale = new Scale(4, 4, 4);
            fanChild.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(180));
            fanChild.Parent = fan;

            _prefab = fan;
            CreatePrefab();

        }
        private void CreatePlayer()
        {
            // Player. Main camera
            _go = new GameObject(new Vector3(200, 200f, 200), Quaternion.Identity);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Tag = "MainCamera";
            _go.Name = "Player";
            _go.AddComponent(new RigidBodyComponent(_go));
            _go.GetComponentOfType<RigidBodyComponent>().Mass = 1f;
            _go.GetComponentOfType<RigidBodyComponent>().Bounciness = 0.003f;
            _go.GetComponentOfType<RigidBodyComponent>().Coeffecient = 0.002f;
            _go.GetComponentOfType<RigidBodyComponent>().MaximumForce = 35000f;
            _go.AddComponent(new ColliderComponent(_go, new Sphere(_go, new Vector3(0, 0, 0), 25f), ColliderTypes.Normal));
            _go.AddComponent(new ColliderComponent(_go, new Sphere(_go, new Vector3(0, -80, 0), 20f), ColliderTypes.Physics));
            _go.AddComponent(new CameraComponent(_go));
            _go.AddComponent(new AnimationComponent(_go, "Hands", null, true));
            _go.AddComponent(new SoundComponent(_go));
            _go.AddComponent(new ScriptComponent(_go));
            ScriptComponent scripts = _go.GetComponentOfType<ScriptComponent>();
            scripts.AddScript(new MovementStable());
            scripts.AddScript(new Wielding());
            scripts.AddScript(new SpeciaMoveManager());
            scripts.AddScript(new Shooting());
            scripts.AddScript(new Attack());
            scripts.AddScript(new PlayerHealth());
            _go.AddComponent(new SkyboxRenderer(_go, "Skybox", null, true));
        }
        private void CreateGhost(Vector3 position, Quaternion rotation)
        {
            string[] ghostModels =
            {
                "Ghost1",
                "Ghost2"
            };
            //Ghost
            _go = new GameObject(position, rotation);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Tag = "Enemy";
            _go.Name = "Ghost";
            _go.AddComponent(new ModelComponent(_go, "Ghost1", "AxeMaterial", false));
            _go.AddComponent(new RigidBodyComponent(_go));
            _go.AddComponent(new ColliderComponent(_go, new Sphere(_go, new Vector3(0f, 175f, 0), 175f), ColliderTypes.Physics));
            _go.GetComponentOfType<RigidBodyComponent>().Bounciness = 0.003f;
            _go.GetComponentOfType<RigidBodyComponent>().Coeffecient = 0.002f;
            _go.GetComponentOfType<RigidBodyComponent>().Mass = 1f;
            _go.AddComponent(new SimpleAnimationComponent(_go, ghostModels, 500f));
            _go.AddComponent(new SoundComponent(_go));
            _go.AddComponent(new ScriptComponent(_go));
            _go.GetComponentOfType<ScriptComponent>().AddScript(new Ghost(100, 30.0f, 10));
            _go.Transform.Scale = new Scale(0.3f, 0.3f, 0.3f);


            _prefab = _go;
            CreatePrefab();
        }
        private void CreateAlien(Vector3 position, Quaternion rotation)
        {
            string[] models =
            {
                "Alien1",
                "Alien2",
                "Alien3",
                "Alien4"
            };

            _go = new GameObject(position, rotation);
            _go.Tag = "Enemy";
            _go.Name = "Alien";
            _go.AddComponent(new ModelComponent(_go, "Alien1", "AxeMaterial", false));
            _go.AddComponent(new RigidBodyComponent(_go));
            _go.GetComponentOfType<RigidBodyComponent>().Bounciness = 0.003f;
            _go.GetComponentOfType<RigidBodyComponent>().Coeffecient = 0.002f;
            _go.GetComponentOfType<RigidBodyComponent>().Mass = 1f;
            _go.GetComponentOfType<RigidBodyComponent>().AffectedByGravity = false;
            _go.AddComponent(new ColliderComponent(_go, new Sphere(_go, new Vector3(0, 0, 0), 110f), ColliderTypes.Physics));
            _go.AddComponent(new SimpleAnimationComponent(_go, models, 500f));
            _go.AddComponent(new SoundComponent(_go));
            _go.AddComponent(new ScriptComponent(_go));
            _go.GetComponentOfType<ScriptComponent>().AddScript(new Alien(100, 30.0f, 10));
            _go.Transform.Scale = new Scale(0.5f, 0.5f, 0.5f);
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);

            _prefab = _go;
            CreatePrefab();
        }
        private GameObject Create2Denemy(Vector3 position, Quaternion rotation)
        {
            GameObject enemy = new GameObject(position, rotation);
            enemy.Tag = "Enemy";
            enemy.AddComponent(new ModelComponent(enemy, "2Denemy", null, false));
            enemy.AddComponent(new RigidBodyComponent(enemy));
            enemy.GetComponentOfType<RigidBodyComponent>().Bounciness = 0.003f;
            enemy.GetComponentOfType<RigidBodyComponent>().Coeffecient = 0.002f;
            enemy.GetComponentOfType<RigidBodyComponent>().Mass = 1f;
            enemy.GetComponentOfType<RigidBodyComponent>().AffectedByGravity = false;
            enemy.AddComponent(new ColliderComponent(enemy, new Sphere(enemy, new Vector3(0, 0, 0), 110f), ColliderTypes.Physics));
            enemy.AddComponent(new ScriptComponent(enemy));
            enemy.GetComponentOfType<ScriptComponent>().AddScript(new TwoDEnemy());
            enemy.GetComponentOfType<ScriptComponent>().AddScript(new BillboardingScript());
            return enemy;
        }
        private void GenerateShields()
        {
            _go = PrefabManager.GetPrafabClone("Radiator");
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Transform.Position = new Vector3(800, 100, 500);

            _go = PrefabManager.GetPrafabClone("Radiator");
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Transform.Position = new Vector3(300, 100, 500);

            _go = PrefabManager.GetPrafabClone("Radiator");
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Transform.Position = new Vector3(-300, 100, 500);

            _go = PrefabManager.GetPrafabClone("Radiator");
            ScreenManager.Instance.CurrentScreen.AddGameObjectToScene(_go);
            _go.Transform.Position = new Vector3(-800, 100, 500);
        }
        private void CreatePrefab()
        {
            string[] prefabs = new[]
            {
                "Room1", "Room2", "Room3", "Room4", "RoomStarting", "RoomBoss"
            };
            foreach (var s in prefabs)
            {
                _prefab = PrefabManager.GetPrafabClone(s);
                var walls = _prefab.Children.Where(x => x.Name == "Wall");
                foreach (var wall in walls)
                {
                    ScriptComponent compo = wall.AddComponent(new ScriptComponent(wall)) as ScriptComponent;
                    compo.AddScript(new WallScript());
                    compo.Initialize();
                }
                GameObject.SaveGameObject(_prefab, _prefab.Name + ".xml");
            }
           
        }
        private void CreateSkyboxSphere(Vector3 position, Quaternion rotation)
        {
            var go = new GameObject(position, rotation);
            go.Name = "ReflectiveCube";
            go.AddComponent(new ModelComponent(go, "DeathStar", "Mirror", true));

        }
    }
}
