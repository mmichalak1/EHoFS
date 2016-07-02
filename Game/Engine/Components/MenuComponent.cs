using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OurGame.Engine.Statics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.Serialization;

namespace OurGame.Engine.Components
{
    [DataContract]
    class MenuComponent : AbstractComponent
    {
        [DataMember]
        private string _playButtonTextureName = "playBtn";
        [DataMember]
        private string _quitButtonTextureName = "quitBtn";
        [DataMember]
        private string _resumeButtonTextureName = "resumeBtn";
        [DataMember]
        private string _backgroundTextureName = "background";

        GraphicsDevice _device;
        ButtonComponent btnPlay, btnExit, btnResume, buttonRespawn, buttonToMenu;
        Texture2D deathScreen, winScreen;
        GameState _gameState;
        public MenuComponent(GameObject parent, GameState gameState) : base(parent)
        {
            _gameState = gameState;
            _device = OurGame.Game.GraphicsDevice;
            Initialize();
        }

        public override void Initialize()
        {

            EventSystem.Instance.RegisterForEvent("GameStateChanged", x =>
            {
                _gameState = (GameState)x;
            });
            LoadContent();
        }

        protected override void LoadContent()
        {
            deathScreen = ContentContainer.TexColor["DeathScreen"];
            buttonRespawn = new ButtonComponent(ContentContainer.TexColor["RespawnButton"], _device, 
                () => EventSystem.Instance.Send("Respawn", null));
            buttonRespawn.setPos(new Vector2((GraphicConfiguration.Instance.ScreenWidth / 2) - 105, GraphicConfiguration.Instance.ScreenHeigth / 1.75f));

            winScreen = ContentContainer.TexColor["WinScreen"];
            buttonToMenu = new ButtonComponent(ContentContainer.TexColor["ExitButton"], _device, 
                () => EventSystem.Instance.Send("ToMenu", null));
            buttonToMenu.setPos(new Vector2((GraphicConfiguration.Instance.ScreenWidth / 2) - 105, GraphicConfiguration.Instance.ScreenHeigth / 1.75f));

            btnPlay = new ButtonComponent(ContentContainer.TexColor[_playButtonTextureName], _device,
                () => EventSystem.Instance.Send("GameStart", null));
            btnPlay.setPos(new Vector2((GraphicConfiguration.Instance.ScreenWidth / 5) - 105, GraphicConfiguration.Instance.ScreenHeigth / 2.00f));
            btnExit = new ButtonComponent(ContentContainer.TexColor[_quitButtonTextureName], _device,
                () => EventSystem.Instance.Send("QuitGame", null));
            btnExit.setPos(new Vector2((GraphicConfiguration.Instance.ScreenWidth / 5) - 105, GraphicConfiguration.Instance.ScreenHeigth / 1.50f));
            btnResume = new ButtonComponent(ContentContainer.TexColor[_resumeButtonTextureName], _device,
                () => EventSystem.Instance.Send("GameStart", null));
            btnResume.setPos(new Vector2((GraphicConfiguration.Instance.ScreenWidth / 5) - 105, GraphicConfiguration.Instance.ScreenHeigth / 3.00f));
            EventSystem.Instance.Send("GameStart", null);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            switch (_gameState)
            {
                case GameState.Menu:
                    OurGame.Game.IsMouseVisible = true;
                    btnPlay.Update(mouse);
                    btnExit.Update(mouse);
                    break;
                case GameState.Paused:
                    OurGame.Game.IsMouseVisible = true;
                    btnResume.Update(mouse);
                    btnPlay.Update(mouse);
                    btnExit.Update(mouse);
                    break;
                case GameState.Death:
                    OurGame.Game.IsMouseVisible = true;
                    buttonRespawn.Update(mouse);
                    break;
                case GameState.Win:
                    OurGame.Game.IsMouseVisible = true;
                    buttonToMenu.Update(mouse);
                    break;
                default:
                    break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            switch (_gameState)
            {
                case GameState.Menu:
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    spriteBatch.Draw(ContentContainer.TexColor[_backgroundTextureName], new Rectangle(0, 0, GraphicConfiguration.Instance.ScreenWidth, GraphicConfiguration.Instance.ScreenHeigth), Color.White);
                    spriteBatch.End();
                    btnPlay.Draw(spriteBatch);
                    btnExit.Draw(spriteBatch);
                    break;

                case GameState.Paused:
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    spriteBatch.Draw(ContentContainer.TexColor[_backgroundTextureName], new Rectangle(0, 0, GraphicConfiguration.Instance.ScreenWidth, GraphicConfiguration.Instance.ScreenHeigth), Color.White);
                    spriteBatch.End();
                    btnResume.Draw(spriteBatch);
                    btnPlay.Draw(spriteBatch);
                    btnExit.Draw(spriteBatch);
                    break;

                case GameState.Death:
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    spriteBatch.Draw(deathScreen, new Rectangle(0, 0, GraphicConfiguration.Instance.ScreenWidth, GraphicConfiguration.Instance.ScreenHeigth), new Color(255,255,255,130));
                    spriteBatch.End();
                    buttonRespawn.Draw(spriteBatch);
                    break;

                case GameState.Win:
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    spriteBatch.Draw(winScreen, new Rectangle(0, 0, GraphicConfiguration.Instance.ScreenWidth, GraphicConfiguration.Instance.ScreenHeigth), Color.White);
                    spriteBatch.End();
                    buttonToMenu.Draw(spriteBatch);
                    break;

            }
        }

    }
}
