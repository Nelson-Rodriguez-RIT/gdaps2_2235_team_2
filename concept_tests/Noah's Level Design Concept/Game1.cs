using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Noah_s_Level_Design_Concept
{
    //maybe use a delegate to detect single key presses?
    
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public KeyboardState keyboardState;

        public int screenWidth;
        public int screenHeight;
        public List<World> worlds;
        public List<Platform> platforms;

        public Player player;
        public Vector2 playerSpawn;
        public bool toggleFocus = true;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
    }

        protected override void Initialize()
        {
            screenWidth = _graphics.PreferredBackBufferWidth = 1280;
            screenHeight = _graphics.PreferredBackBufferHeight = 736;

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
            Texture2D texture = Content.Load<Texture2D>("White_box_28x52");

            worlds = new List<World>
            {
                new World(startMap, new Rectangle(0, 0, screenWidth, screenHeight), true),
                new World(labMap2, new Rectangle(screenWidth, 352, screenWidth, screenHeight), true)
            };

            playerSpawn = new Vector2(448, 354);
            player = new Player(
                playerAsset,
                new Rectangle((int)playerSpawn.X, (int)playerSpawn.Y, 64, 64));

            platforms = new List<Platform>
            {
                new Platform(new Rectangle(88 * 4, 112 * 4, 136 * 4, 32 * 4), texture),
                new Platform(new Rectangle(32 * 4, 104 * 4, 56 * 4, 8 * 4), texture),
                new Platform(new Rectangle(224 * 4, 120 * 4, 96 * 4, 32 * 4), texture)
                /*
                new Platform(new Rectangle(), texture),
                    Terrain= + 0, screenHeight + 0,320,8
                    Terrain=168,32,152,8
Terrain=0,32,104,8
Terrain=168,40,96,48
Terrain=248,88,56,48
Terrain=80,120,168,16
Terrain=104,112,64,8
Terrain=16,40,88,48
Terrain=0,88,32,88
Terrain=0,176,320,8
Terrain=288,136,16,40
Terrain=31,151,24,24
Terrain=56,160,8,16
Terrain=64,168,8,8
Terrain=56,120,24,8
*/
            };

            Camera.RectTarget = player.position;
            Camera.GlobalOffset = new Vector2(
                (screenWidth/2) - player.position.Width, 
                (screenHeight/2) - player.position.Height);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            player.Update(gameTime);

            foreach (Platform platform in platforms)
            { platform.CheckCollision(player); }

            if (toggleFocus)
                Camera.RectTarget = player.position;
            /*
             * if not focused (boss) make the current room the focus
             * i currently have no way of checking current room
            else
                Camera.VectorTarget = ;
            */
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            player.Draw(_spriteBatch);

            foreach (Platform platform in platforms)
            { platform.Draw(_spriteBatch); }

            foreach (World world in worlds)
            { world.Draw(_spriteBatch); }

            _spriteBatch.End();
            base.Draw(gameTime);

        }
    }
}