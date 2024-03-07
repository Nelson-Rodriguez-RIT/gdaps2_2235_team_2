using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities;
using Moonwalk.Classes.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers
{
    enum GameState {
        Test,
    }

    /// <summary>
    /// Central class that controls/ maintains all elements of the game
    /// </summary>
    internal sealed class GameManager {

        // Game element managers
        private static GameManager _instance = null;

        // Gameplay related states
        private GameState state;
        private KeyboardState kbState;
        private MouseState msState;

        // Currently loaded entities
        private List<Entity> entities;
        private ListDictionary<Entity> entities2;

        //Input handling:
        private StoredInput storedInput;

        // For testing purposes
        private Vector2 cameraTarget;

        private GameManager(ContentManager content) {
            // Get content for loading needs
            Loader.Content = content;
            storedInput = new StoredInput();

            entities = new List<Entity>();
            

            //Testing for my new entity list concept
            List<Type> types = new List<Type>();
            types.Add(typeof(TestEntity));
            entities2 = new ListDictionary<Entity>(types);
            entities2.Add(new TestEntity(Vector2.Zero));
            entities2.Add(new TestEntity(Vector2.Zero));
            entities2.Add(new TestEntity(Vector2.Zero));

            // Prepares neccessary elements
            Transition(GameState.Test);
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

            switch (state) {
                case GameState.Test:

                    // This is just testing the Camera (using the map for reference)
                    if (kbState.IsKeyDown(Keys.A))
                        cameraTarget.X -= (float)(100 * gt.ElapsedGameTime.TotalSeconds);
                    if (kbState.IsKeyDown(Keys.D))
                        cameraTarget.X += (float)(100 * gt.ElapsedGameTime.TotalSeconds);

                    Camera.VectorTarget = cameraTarget;

                    break;
            }

            foreach (Entity entity in entities)
                entity.Update(gt, storedInput);

            storedInput.UpdatePrevious();
        }

        /// <summary>
        /// Handles draw logic
        /// </summary>
        public void Draw(SpriteBatch batch, Vector2 globalScale) {
            // Elements draw based on game state (i.e. GUI or menu elements)
            switch (state) {
                case GameState.Test:
                    
                    break;
            }

            Map.Draw(batch, globalScale);

            // Elements drawn ever iteration
            foreach (Entity entity in entities)
                entity.Draw(batch, globalScale);
        }

        /// <summary>
        /// Handles logic when switching between game states
        /// </summary>
        /// <param name="nextState">The next game state to transition to</param>
        private void Transition(GameState nextState) {
            switch (nextState) {
                case GameState.Test:
                    cameraTarget = new Vector2(0, 0);

                    // Loads the "TestMap" map
                    Map.LoadMap("TestMap");

                    // Loads the "TestEntity" entity
                    SpawnEntity(typeof(TestEntity), new Vector2(200, 200));

                    break;
            }

            state = nextState;
        }

        /// <summary>
        /// Handles any neccassray logic when spawning an enemy
        /// </summary>
        private void SpawnEntity(Type className, Vector2 position) { // No idea if this works by the way :P
            // FYI you would class this class like:
            // SpawnEntity(typeof(Player), new Vector(0, 0));
            // This would add the class "Player" to the entities list and spawn them at 0, 0
            entities.Add((Entity) Activator.CreateInstance(className, new object[] {position}));
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
