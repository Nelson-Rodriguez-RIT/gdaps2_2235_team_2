using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Noah_s_Level_Design_Concept
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        public KeyboardState keyboardState;
        public KeyboardState oldKeyboardState;
        public MouseState mouseState;
        public MouseState oldMouseState;

        private Player player;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Vector2 playerLoc = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            Texture2D attackSheet = Content.Load<Texture2D>("Sprites/01-King Human/Attack (78x58)");
            Texture2D idleSheet = Content.Load<Texture2D>("Sprites/01-King Human/Idle (78x58)");

            player = new Player(attackSheet, idleSheet, playerLoc, PlayerState.Idle);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            oldKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            oldMouseState = mouseState;
            mouseState = Mouse.GetState();
            GetInput();

            player.UpdateAnimation(gameTime);

            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            player.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private bool SingleKeyPress(Keys key, KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyUp(key))
            {
                return true;
            }
            else { return false; }
        }
        private void GetInput()
        {
            switch (player.State)
            {
                case PlayerState.Idle:
                    if (mouseState.LeftButton == ButtonState.Pressed
                        && oldMouseState.LeftButton == ButtonState.Released)
                    {
                        player.State = PlayerState.Attack;
                        player.frame = 1;
                    }
                    break;
                case PlayerState.Attack:
                    {
                        
                    }
                    break;
            
            }
        
        }
    }
}