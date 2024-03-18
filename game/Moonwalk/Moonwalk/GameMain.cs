using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;
using System;
using System.Reflection.Metadata;

namespace Moonwalk {
    public class GameMain : Game {
        private const int DefaultWindowWidth = 1280;
        private const int DefaultWindowHeight = 720;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        GameManager gameManager;
        WindowManager windowManager;

        //private Vector2 DefaultScale = new Vector2(5, 5);
        private Vector2 DefaultScale = new Vector2(1, 1);

        private Vector2 activeScale;

        public GameMain() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            // Set initial window properties
            Window.AllowUserResizing = true;

            _graphics.PreferredBackBufferWidth = DefaultWindowWidth;
            _graphics.PreferredBackBufferHeight = DefaultWindowHeight;
            _graphics.ApplyChanges();

            windowManager = WindowManager.GetInstance(DefaultWindowWidth, DefaultWindowHeight);

            base.Initialize();
        }

        protected override void LoadContent() {


            _spriteBatch = new SpriteBatch(GraphicsDevice);
            gameManager = GameManager.GetInstance(Content, _graphics.GraphicsDevice);
            GameManager.font = Content.Load<SpriteFont>("File");
        }



        protected override void Update(GameTime gameTime) {
            activeScale = Vector2.One;
            // Non-game logic (i.e. updating the window size ratio) should go below //
            windowManager.Update( // Updates the scaling factor based on the window size
                Window.ClientBounds.Size.X,
                Window.ClientBounds.Size.Y);
            activeScale = windowManager.WindowRatioScale;

            // Keep game logic inside of gameManager //
            gameManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp); // Prevents blurry sprites

            gameManager.Draw(_spriteBatch, activeScale * DefaultScale);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}