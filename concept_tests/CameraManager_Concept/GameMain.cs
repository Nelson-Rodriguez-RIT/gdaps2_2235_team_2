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
        private KeyboardState prevKbState;
        private bool toggleFocus;

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

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            entityImage = Content.Load<Texture2D>("sprite_entity");
            entity = new Rectangle(
                100,
                100,
                entityImage.Width,
                entityImage.Height);

            // Set initial target
            Camera.RectTarget = entity;
            
            // Set a global offset (this gets the square in the center of the screen)
            Camera.GlobalOffset = new Vector2( // You would probably want this to be based on window size
                (1280 / 2) - (entityImage.Width),
                (720 / 2) - (entityImage.Height));

            image = Content.Load<Texture2D>("sprite_static");
            imagePosition = new Vector2(0, 0);
            imageOffset = new Vector2(-image.Width / 2, -image.Height / 2);



        }

        protected override void Update(GameTime gameTime) {
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

            if (kbState.IsKeyDown(Keys.Escape)) // Exit
                Exit();

            // Switch from the dynamic player view to a static view of the image
            if (prevKbState.IsKeyDown(Keys.Space) && kbState.IsKeyUp(Keys.Space))
                toggleFocus = !toggleFocus;

            /*
            Side note about this:
                I'm pretty sure Rectangle/Vector objects are immutable because when trying to change
                the values, the entity reference set in LoadObject() doesn't work and Camera
                needs a new Rectangle reference. Just keep that in mind.
           */
            if (toggleFocus)
                Camera.RectTarget = entity;
            else
                Camera.VectorTarget = imagePosition;


            prevKbState = kbState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin();

            _spriteBatch.Draw( // Draw static image
                image,
                Camera.ApplyOffset(imagePosition, imageOffset),
                Color.White);

            _spriteBatch.Draw( // Draw moveable test entity
                entityImage,
                Camera.ApplyOffset(entity),
                Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}