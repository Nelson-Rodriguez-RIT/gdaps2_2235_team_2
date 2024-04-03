using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers
{
    enum GameState {
        MainMenu,
        Demo
    }

    /// <summary>
    /// Central class that controls/ maintains all elements of the game
    /// </summary>
    internal sealed class GameManager {

        private Dictionary<string, Texture2D> guiSprites;
        private Dictionary<string, SpriteFont> fonts;

        public static SpriteFont font;

        private bool displayEntityHitboxes = false;
        private bool displayTerrainHitboxes = false;

        // Game element managers
        private static GameManager _instance = null;

        // Gameplay related states
        private GameState state;
        private KeyboardState kbState;
        private MouseState msState;

        // Currently loaded entities
        private Assortment<Entity> entities;

        List<Type> types;

        //Input handling:
        private StoredInput storedInput;

        // For testing purposes
        private Vector2 cameraTarget;

        private GameManager(ContentManager content) {
            // Get content for loading needs
            Loader.Content = content;
            storedInput = new StoredInput();
            Camera.GlobalOffset = WindowManager.Instance.Center;


            //Testing for my new entity list concept
            types = new List<Type>();
            types.Add(typeof(Player));
            types.Add(typeof(Robot));
            types.Add(typeof(Enemy));
            entities = new Assortment<Entity>(types);


            // Loads UI Sprites
            {
                guiSprites = new Dictionary<string, Texture2D>();
                string directory = "../../../Content/GUI/";
                foreach (string file in Directory.GetFiles(directory)) {
                    string pathBuffer = file.Remove(file.Length - 4);
                    string nameBuffer = pathBuffer.Remove(0, directory.Length);
                    guiSprites.Add(
                        nameBuffer,
                        content.Load<Texture2D>(pathBuffer)
                        );
                }
            }


            // Loads fonts
            {
                fonts = new Dictionary<string, SpriteFont>();
                string directory = "../../../Content/Fonts/";
                foreach (string file in Directory.GetFiles(directory)) {
                    string[] splitPath = file.Split('.');
                    if (splitPath[splitPath.Length - 1] == "spritefont") {
                        string pathBuffer = file.Remove(file.Length - 11);
                        string nameBuffer = pathBuffer.Remove(0, directory.Length);
                        fonts.Add(
                            nameBuffer, 
                            content.Load<SpriteFont>(pathBuffer));
                    }
                        
                }
            }


            // Prepares neccessary elements
            Transition(GameState.MainMenu);
        }


        /// <summary>
        /// Gets GameManager's singleton instance
        /// </summary>
        /// <returns>A GameManager object</returns>
        public static GameManager GetInstance(ContentManager content) {
            if (_instance == null)
                _instance = new GameManager(content);

            return _instance;
        }

        /// <summary>
        /// Handles gameplay logic
        /// </summary>
        public void Update(GameTime gt) {
            // Get user input
            storedInput.Update();

            if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F1) && // Toggle F1 to draw entity hitboxes
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.F1))
                displayEntityHitboxes = !displayEntityHitboxes;

            if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F2) && // Toggle F2 to draw terrain hitboxes
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.F2))
                displayTerrainHitboxes = !displayTerrainHitboxes;

            if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F3) && // Press F3 to reset the current map/gamestate
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.F3))
                Transition(state);

            switch (state) {
                case GameState.MainMenu:
                        if(storedInput.CurrentMouse.LeftButton == ButtonState.Pressed &&
                                storedInput.PreviousMouse.LeftButton == ButtonState.Released) {

                        //Start button pressed
                        if (new Rectangle
                            (540, 310, 186, 66)     // Start button position and size
                            .Contains(storedInput.CurrentMouse.Position))
                                Transition(GameState.Demo);

                        //Exit button pressed
                        if (new Rectangle(
                            540, 410, 186, 66) // Exit button position    
                            .Contains(storedInput.CurrentMouse.Position))
                                GameMain.ExitGame();
                    }
                            
                    break;

                case GameState.Demo:
                    foreach (Entity entity in entities) {
                        if (entity is Player) {
                            cameraTarget = new Vector2(entity.Position.X, entity.Position.Y);
                            break;
                        }
                    }

                    if (storedInput.IsPressed(Keys.Escape))
                    {
                        Map.UnloadMap();
                        Transition(GameState.MainMenu);
                    }

                    break;
            }

            foreach (Entity entity in entities)
            {
                entity.Update(gt, storedInput);

                if (entity is IDamageable)
                {
                    IDamageable damageable = (IDamageable) entity;

                    if (damageable.Health <= 0)
                    {
                        DespawnEntity(entity);
                    }
                }

                if (entity is Projectile)
                {
                    Projectile projectile = (Projectile) entity;

                    if (projectile.Collisions <= 0)
                    {
                        DespawnEntity(entity);
                    }
                }
            }


            storedInput.UpdatePrevious();
        }

        /// <summary>
        /// Handles draw logic
        /// </summary>
        public void Draw(SpriteBatch batch, GraphicsDevice graphics) {
            // Elements draw based on game state (i.e. GUI or menu elements)
            switch (state) {
                case GameState.MainMenu:
                    graphics.Clear(Color.Black);

                    batch.DrawString(
                        fonts["MonogramTitle"],
                        "Moonwalk",
                        WindowManager.Instance.Center - new Vector2(150, 150),
                        Color.White
                        );

                    batch.Draw(
                        guiSprites["ButtonStart"],
                        new Rectangle(
                            (int)(WindowManager.Instance.Center.X - 100),
                            (int)(WindowManager.Instance.Center.Y - 50),
                            guiSprites["ButtonStart"].Width * 2,
                            guiSprites["ButtonStart"].Height * 2
                            ),
                        Color.White
                        );

                    batch.Draw(
                        guiSprites["ButtonExit"],
                        new Rectangle(
                            (int)(WindowManager.Instance.Center.X - 100),
                            (int)(WindowManager.Instance.Center.Y + 50),
                            guiSprites["ButtonExit"].Width * 2,
                            guiSprites["ButtonExit"].Height * 2
                            ),
                        Color.White
                        );

                    batch.Draw(
                        guiSprites["MenuBorder"],
                        new Rectangle(
                            (int)(WindowManager.Instance.Center.X - 230),
                            (int)(WindowManager.Instance.Center.Y - 245),
                            guiSprites["MenuBorder"].Width * 10,
                            guiSprites["MenuBorder"].Height * 10),
                        Color.White
                        );
                    break;

                case GameState.Demo:
                    graphics.Clear(Color.Gray);
                    break;
            }

            if(Map.Loaded)
                Map.Draw(batch, displayTerrainHitboxes);

            // Elements drawn ever iteration
            foreach (Entity entity in entities) {
                entity.Draw(batch);

                if (displayEntityHitboxes)
                    entity.DrawHitbox(batch);
            }
                
        }

        /// <summary>
        /// Handles logic when switching between game states
        /// </summary>
        /// <param name="nextState">The next game state to transition to</param>
        private void Transition(GameState nextState, bool clearEntities = true) {
            if (clearEntities)
                entities = new Assortment<Entity>(types);

            switch (nextState) {
                case GameState.MainMenu:
                    break;

                case GameState.Demo:

                    Map.LoadMap("StartMapTest");
                    //Map.LoadMap("Demo");

                    // Loads player + companion
                    Player player = SpawnEntity<Player>(new Vector2(400, 48));
                    Robot robot = SpawnEntity<Robot>(new Vector2(128, 48));
                    SpawnEntity<TestEnemy>(new Vector2(200, 250));

                    // Set player as the Camera's target
                    Camera.SetTarget(player);

                    //Add subscribers to player events
                    player.GetRobotPosition += robot.GetPosition;
                    player.OnGravityAbilityUsed += entities.GetAllOfType<IMovable>;
                    player.ToggleBotLock += robot.ToggleLock;
                    player.GetEnemies += entities.GetAllOfType<IHostile>;
                    player.GetDamagables += entities.GetAllOfType<IDamageable>;
                    
                    for (int i = 0; i < entities[typeof(Enemy)].Count; i++)
                    {
                        player.EnemyAI += ((Enemy)entities[typeof(Enemy)][i]).AI;
                    }
                    
                    break;
            }

            state = nextState;
        }

        /// <summary>
        /// Handles any neccassray logic when spawning an entity
        /// </summary>
        private T SpawnEntity<T>(Vector2 position, Object[] args = null) where T : Entity {
            Entity entity = (Entity)Activator.CreateInstance(typeof(T), new object[] { position });
            entities.Add(entity);

            return (T)entity;
        }

        /// <summary>
        /// Handles any neccessary logic when despawning an entity
        /// </summary>
        /// <param name="entity">Entity to despawn</param>
        private void DespawnEntity(Entity entity) {
            entities.Remove(entity);
        }

        
    }


}
