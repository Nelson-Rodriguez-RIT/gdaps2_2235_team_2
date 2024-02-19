using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameControls_Concept
{
    public class GameMain : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Companion mouseControlActive;
        //private MouseControlledEntity mouseControlInactive;
        private LevelManager levelManager;
        private Texture2D _texture;
        private Player WASD;

        public GameMain()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            levelManager = new LevelManager(Content.Load<Texture2D>("Sprite-0001"), levelManager);
            //levelManager.LoadFromFile("level1.csv");
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.PreferredBackBufferWidth = 1900;
            _graphics.ApplyChanges();
            mouseControlActive = new Companion(levelManager);
            WASD = new Player(levelManager, new Vector2(100, 100), mouseControlActive
                , Content.Load<SpriteFont>("File"));

            
            //mouseControlInactive = new MouseControlledEntity(Content.Load<Texture2D>("Sprite-0001"), false);

            base.Initialize();
            
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _texture = Content.Load<Texture2D>("Sprite-0001");
            WASD.LoadSprite(_texture);
            mouseControlActive.LoadSprite(_texture);
            foreach(Collider collider in levelManager.Platforms)
            {
                collider.LoadSprite(_texture);
            }

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            WASD.Update(gameTime);
            mouseControlActive.Update(gameTime);
            //mouseControlInactive.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            mouseControlActive.Draw(_spriteBatch);
            WASD.Draw(_spriteBatch);
            //mouseControlInactive.Draw(_spriteBatch);
            foreach (Collider platform in levelManager.Platforms)
            {
                platform.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}