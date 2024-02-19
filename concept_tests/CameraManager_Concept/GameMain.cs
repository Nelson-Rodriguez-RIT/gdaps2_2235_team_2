using CameraManager_Concept.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CameraManager_Concept {
    public class GameMain : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Rectangle entity;
        private Texture2D entityImage;
        private int moveSpeed = 1000;

        private Vector2 imagePosition;
        private Texture2D image;
        private Vector2 imageOffset;

        private KeyboardState kbState;

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

            entityImage = Content.Load<Texture2D>("sprite_entity");
            entity = new Rectangle(
                0,
                0,
                entityImage.Width,
                entityImage.Height);

            Camera.RectTarget = entity;

            image = Content.Load<Texture2D>("sprite_static");
            imagePosition = new Vector2(0, 0);
            imageOffset = new Vector2(image.Width / 2, image.Height / 2);



        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Basic controls for testing
            kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.W)) // Up
                entity.Y -= (int)(moveSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            if (kbState.IsKeyDown(Keys.S)) // Down
                entity.Y += (int)(moveSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            if (kbState.IsKeyDown(Keys.D)) // Right
                entity.X += (int)(moveSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            if (kbState.IsKeyDown(Keys.A)) // Right
                entity.X -= (int)(moveSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

           // _spriteBatch.Draw( // Draw static image
           //     image,
           //     Camera.ApplyOffset(imagePosition, imageOffset),
           //     Color.White);

            _spriteBatch.Draw( // Draw moveable test entity
                entityImage,
                Camera.ApplyOffset(entity),
                Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}