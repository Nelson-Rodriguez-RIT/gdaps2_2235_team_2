using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers
{
    enum GameState {
        Test,
        Demo
    }

    /// <summary>
    /// Central class that controls/ maintains all elements of the game
    /// </summary>
    internal sealed class GameManager {

        public static SpriteFont font;

        private GraphicsDevice graphics;
        private bool displayHitboxes = false;

        // Game element managers
        private static GameManager _instance = null;

        // Gameplay related states
        private GameState state;
        private KeyboardState kbState;
        private MouseState msState;

        // Currently loaded entities
        private Assortment<Entity> entities;

        //Input handling:
        private StoredInput storedInput;

        // For testing purposes
        private Vector2 cameraTarget;

        private GameManager(ContentManager content, GraphicsDevice graphics) {
            // Get content for loading needs
            Loader.Content = content;
            this.graphics = graphics;
            storedInput = new StoredInput();
            //Camera.GlobalOffset = WindowManager.Instance.Center;


            //Testing for my new entity list concept
            List<Type> types = new List<Type>();
            types.Add(typeof(Player));
            types.Add(typeof(Robot));
            entities = new Assortment<Entity>(types);


            // Prepares neccessary elements
            Transition(GameState.Demo);
        }


        /// <summary>
        /// Gets GameManager's singleton instance
        /// </summary>
        /// <returns>A GameManager object</returns>
        public static GameManager GetInstance(ContentManager content, GraphicsDevice graphics) {
            if (_instance == null)
                _instance = new GameManager(content, graphics);

            return _instance;
        }

        /// <summary>
        /// Handles gameplay logic
        /// </summary>
        public void Update(GameTime gt) {
            // Get user input
            storedInput.Update();

            if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F1) && // Toggle F1 to draw hitboxes
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.F1))
                displayHitboxes = !displayHitboxes;

            switch (state) {
                case GameState.Test:

                    
                    // Set camera target to player's position
                    // 
                    // I wanted to have this in the player class but couldn't get it to work this time
                    // - Dante
                    foreach  (Entity entity in entities)
                    {
                        if (entity is Player)
                        {
                            cameraTarget = new Vector2(entity.Position.X, entity.Position.Y);
                            break;
                        }
                    }
                    

                    //Camera.VectorTarget = cameraTarget;

                    break;

                case GameState.Demo:

                    foreach (Entity entity in entities) {
                        if (entity is Player) {
                            cameraTarget = new Vector2(entity.Position.X, entity.Position.Y);
                            break;
                        }
                    }


                   // Camera.VectorTarget = cameraTarget;

                    break;
            }

            foreach (Entity entity in entities)
            {
                entity.Update(gt, storedInput);
            }


            storedInput.UpdatePrevious();
        }

        /// <summary>
        /// Handles draw logic
        /// </summary>
        public void Draw(SpriteBatch batch) {
            // Elements draw based on game state (i.e. GUI or menu elements)
            switch (state) {
                case GameState.Test:
                    
                    break;
            }

            Map.Draw(batch);

            // Elements drawn ever iteration
            foreach (Entity entity in entities) {
                entity.Draw(batch);

                if (displayHitboxes)
                    entity.DrawHitbox(batch, GameMain.ActiveScale, graphics);
            }
                
        }

        /// <summary>
        /// Handles logic when switching between game states
        /// </summary>
        /// <param name="nextState">The next game state to transition to</param>
        private void Transition(GameState nextState) {
            switch (nextState) {
                case GameState.Test:
                    

                    // Loads the "TestMap" map
                    Map.LoadMap("TestMap");

                    // Loads the test entities
                    SpawnEntity<Player>(new Vector2(200, 200));
                    SpawnEntity<Robot>(new Vector2(400, 400));

                    Robot robot = ((Robot)entities[typeof(Robot)][0]);
                    Player player = ((Player)entities[typeof(Player)][0]);

                    player.GetRobotPosition += robot.GetPosition;
                    player.OnGravityAbilityUsed += entities.GetAllOfType<IMovable>;

                    break;

                case GameState.Demo:

                    Map.LoadMap("Demo");

                    // Loads player + companion
                    SpawnEntity<Player>(new Vector2(50, 48));
                    SpawnEntity<Robot>(new Vector2(128, 48));

                    robot = ((Robot)entities[typeof(Robot)][0]);
                    player = ((Player)entities[typeof(Player)][0]);

                    player.GetRobotPosition += robot.GetPosition;
                    player.OnGravityAbilityUsed += entities.GetAllOfType<IMovable>;

                    break;
            }

            state = nextState;
        }

        /// <summary>
        /// Handles any neccassray logic when spawning an enemy
        /// </summary>
        private void SpawnEntity<T>(Vector2 position) where T : class
        { // No idea if this works by the way :P
            // FYI you would class this class like:
            // SpawnEntity(typeof(Player), new Vector(0, 0));
            // This would add the class "Player" to the entities list and spawn them at 0, 0
            entities.Add((Entity) Activator.CreateInstance(typeof(T), new object[] {position}));
        }

        /// <summary>
        /// Handles any neccessary logic when despawning an enemy
        /// </summary>
        /// <param name="entity">Entity to despawn</param>
        private void DespawnEntity(Entity entity) {
            entities.Remove(entity);
        }

        
    }


}
