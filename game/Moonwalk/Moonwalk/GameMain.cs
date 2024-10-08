﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;
using System;
using System.Reflection.Metadata;

namespace Moonwalk {
    public class GameMain : Game {
        private const int DefaultWindowWidth = 1280;
        private const int DefaultWindowHeight = 720;

        private static GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static GraphicsDeviceManager Graphics
        {
            get { return _graphics; }
        }

        GameManager gameManager;
        WindowManager windowManager;

        private Vector2 DefaultScale = new Vector2(2, 2);

        private static Vector2 activeScale;

        private static bool exitGameFlag = false;

        public static Vector2 ActiveScale {
            get { return activeScale; }
        }

        public GameMain() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            // Set initial window properties
            Window.AllowUserResizing = false; // This is going to be disabled until I come up with a position fix

            _graphics.PreferredBackBufferWidth = DefaultWindowWidth;
            _graphics.PreferredBackBufferHeight = DefaultWindowHeight;
            _graphics.ApplyChanges();

            windowManager = WindowManager.GetInstance(DefaultWindowWidth, DefaultWindowHeight);

            base.Initialize();
        }

        protected override void LoadContent() {


            _spriteBatch = new SpriteBatch(GraphicsDevice);
            gameManager = GameManager.GetInstance(Content);
        }



        protected override void Update(GameTime gameTime) {
            if (exitGameFlag)
                Exit();

            activeScale = Vector2.One;
            // Non-game logic (i.e. updating the window size ratio) should go below //
            windowManager.Update( // Updates the scaling factor based on the window size
                Window.ClientBounds.Size.X,
                Window.ClientBounds.Size.Y);
            activeScale = windowManager.WindowRatioScale;

            // Keep game logic inside of gameManager //
            gameManager.Update(gameTime);

            activeScale *= DefaultScale;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp); // Prevents blurry sprites

            gameManager.Draw(_spriteBatch, GraphicsDevice);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void ExitGame() {
            exitGameFlag = true;
        }
    }
}