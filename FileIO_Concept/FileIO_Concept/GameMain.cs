using FileIO_Concept.Content.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Data;
using System.Dynamic;

// Nelson Rodriguez
// 2.16.2024
// File IO Concept Test

namespace FileIO_Concept {

    public class GameMain : Game {
        private const string RootDirectory = "../../../";

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Entity testEntity; // For testing

        public GameMain() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // This portion of file loading is subject to change
            Entity.Load($"{RootDirectory}Content/Entity/");
            Entity.LoadedSprite = Content.Load<Texture2D>("Entity/sprite");


            
            testEntity = new Entity(new Vector2(0, 0));
        }

        protected override void Update(GameTime gameTime) {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin();

            testEntity.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void LogToConsole(string error) {
            // TODO
        }

        // Used to load relevant entity data
        enum Entities {
            Entity
        }
    }
}