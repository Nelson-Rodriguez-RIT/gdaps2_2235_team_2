using DynamicWindowSizing_Concept.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DynamicWindowSizing_Concept {
    public class GameMain : Game {
        // Honestly it be worth considering storing game systems such as default window size
        // into a file format. Thoughts? - Nelson

        private const int DefaultWindowWidth = 1280;
        private const int DefaultWindowHeight = 720;

        private GameWindow gw;
        private WindowManager wm;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // This would typically go into its own class
        Vector2 testPosition; // This is an absolute location
        Texture2D testSprite;

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

            // Setup window manager
            wm = new(DefaultWindowWidth, DefaultWindowHeight); // Probably should make this a singleton

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            testSprite = Content.Load<Texture2D>("sprite");
            testPosition = new Vector2(20, 20);
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // This will update the window size ratio
            wm.Update(
                Window.ClientBounds.Size.X,
                Window.ClientBounds.Size.Y);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin();

            _spriteBatch.Draw(
                testSprite,
                // Needed or else the position for scaled images do not line up
                testPosition * WindowManager.WindowRatioScale,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                WindowManager.WindowRatioScale,
                SpriteEffects.None,
                0f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}