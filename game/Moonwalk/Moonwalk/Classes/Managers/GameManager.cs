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
        Demo
    }

    /// <summary>
    /// Central class that controls/ maintains all elements of the game
    /// </summary>
    internal sealed class GameManager {

        public static SpriteFont font;

        private GraphicsDevice graphics;
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

        //Input handling:
        private StoredInput storedInput;

        // For testing purposes
        private Vector2 cameraTarget;

        private GameManager(ContentManager content, GraphicsDevice graphics) {
            // Get content for loading needs
            Loader.Content = content;
            this.graphics = graphics;
            storedInput = new StoredInput();
            Camera.GlobalOffset = WindowManager.Instance.Center;


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

            if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F1) && // Toggle F1 to draw entity hitboxes
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.F1))
                displayEntityHitboxes = !displayEntityHitboxes;

            if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F2) && // Toggle F2 to draw terrain hitboxes
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.F2))
                displayTerrainHitboxes = !displayTerrainHitboxes;

            switch (state) {

                case GameState.Demo:

                    foreach (Entity entity in entities) {
                        if (entity is Player) {
                            cameraTarget = new Vector2(entity.Position.X, entity.Position.Y);
                            break;
                        }
                    }


                    Camera.VectorTarget = cameraTarget;

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
                case GameState.Demo:
                    break;
            }

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
        private void Transition(GameState nextState) {
            switch (nextState) {
                case GameState.Demo:

                    //Map.LoadMap("StartMap");
                    Map.LoadMap("Demo");

                    // Loads player + companion
                    Player player = (Player) SpawnEntity<Player>(new Vector2(50, 48));
                    Robot robot = (Robot) SpawnEntity<Robot>(new Vector2(128, 48));


                    player.GetRobotPosition += robot.GetPosition;
                    player.OnGravityAbilityUsed += entities.GetAllOfType<IMovable>;

                    break;
            }

            state = nextState;
        }

        /// <summary>
        /// Handles any neccassray logic when spawning an enemy
        /// </summary>
        private Entity SpawnEntity<T>(Vector2 position, Object[] args = null) where T : class {
            Entity entity = (Entity)Activator.CreateInstance(typeof(T), new object[] { position, args });
            entities.Add(entity);

            return entity;
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
