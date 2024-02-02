using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Dante_Test
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player player;
        private Texture2D image;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Player(100, 100);
            _graphics.PreferredBackBufferWidth = 900; // set this value to the desired width
            _graphics.PreferredBackBufferHeight = 900; // set this value to the desired height
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here


            image = Content.Load <Texture2D>("slime");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            player.HorizontalMovement();

            if (player.position.Y + image.Height < _graphics.PreferredBackBufferHeight)
            {
                player.VerticalMovement();
            }
            else
            {
                player.Jump();
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            _spriteBatch.Draw(image, player.position, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}