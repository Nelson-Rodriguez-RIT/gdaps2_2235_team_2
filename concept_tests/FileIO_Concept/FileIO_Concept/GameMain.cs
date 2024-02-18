using FileIO_Concept.Classes.Archived;
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

namespace FileIO_Concept
{

    public class GameMain : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Entity testEntity; // For testing

        public GameMain() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Entity.Load(); // This loads all relevant file data
            Entity.Sprite = Content.Load<Texture2D>(Entity.SpritePath); // Loads sprite

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
    }
}