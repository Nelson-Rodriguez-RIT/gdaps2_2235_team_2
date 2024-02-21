using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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

        public int screenWidth;
        public int screenHeight;
        public List<World> worlds;
        public Player player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
    }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280; // set this value to the desired width
            _graphics.PreferredBackBufferHeight = 736; // set this value to the desired height
            screenWidth = _graphics.PreferredBackBufferWidth;
            screenHeight = _graphics.PreferredBackBufferHeight;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //map textures
            Texture2D startMap = Content.Load<Texture2D>("MoonwalkMapStart");
            Texture2D labMap1 = Content.Load<Texture2D>("MoonwalkMapLaboratoryState1");
            Texture2D labMap2 = Content.Load<Texture2D>("MoonwalkMapLaboratoryState2");
            //object textures
            Texture2D playerAsset = Content.Load<Texture2D>("AnimationSheet_Character");

            worlds = new List<World>
            {
                new World(startMap, new Rectangle(0, 0, screenWidth, screenHeight), true),
                new World(labMap1, new Rectangle(screenWidth, 352, screenWidth, screenHeight), true),
                new World(labMap2, new Rectangle(screenWidth, 352, screenWidth, screenHeight), false)
            };

            player = new Player(
                playerAsset,
                new Rectangle(448, 384, 64, 64));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            for (int i = 0; i < worlds.Count; i++)
            { worlds[i].Update(gameTime); }
            player.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            for (int i = 0; i < worlds.Count; i++)
            { worlds[i].Draw(_spriteBatch); }
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
        
    }
}