using ConceptDemo.classes.entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConceptDemo.classes {
    /// <summary>
    /// Controls gameplay related functionalities
    /// </summary>
    internal class GameManager {
        /// <summary>
        /// Stores previously loaded textures. Treat as read-only
        /// </summary>
        private Dictionary<EntityID, List<Texture2D>> _loadedTextures;

        /// <summary>
        /// Current state of the game
        /// </summary>
        private GameStateID gameState;

        public GameStateID GameState { get { return gameState; } }


        /// <summary>
        /// Initialize the game normally
        /// </summary>
        public GameManager(
            Dictionary<EntityID, List<Texture2D>> _loadedEntityTextures,
            Dictionary<EntityID, List<string>> _loadedEntityContents) : 
            this(_loadedEntityTextures, GameStateID.initialize) {}

        /// <summary>
        /// Initialize the game in a custom state
        /// </summary>
        /// <param name="gameState">State to initialize game in</param>
        public GameManager(
            Dictionary<EntityID, List<Texture2D>> _loadedEntityTextures,
            Dictionary<EntityID, List<string>> _loadedEntityContents,
            GameStateID gameState)  {

            this._loadedTextures = _loadedEntityTextures;
            this.gameState = gameState;
        }


        /// <summary>
        /// Attempts to update all gameplay related elements
        /// </summary>
        /// <param name="gameTime">Delta time variable</param>
        /// <param name="loadedEntities">Currently loaded and interactable entities</param>
        public void Update(GameTime gameTime, List<Entity> loadedEntities, CameraManager camera) {
            switch (gameState) {
                case GameStateID.initialize:
                    gameState = GameStateID.overworld_test_load;
                    break;

                case GameStateID.overworld_test_load:
                    loadedEntities.Add(InitializeEntity(new Entity()));

                    gameState = GameStateID.overworld_test;
                    break;

                case GameStateID.overworld_test:
                    break;
            }

            // Update each entity
            foreach (Entity entity in loadedEntities)
                entity.Update(gameTime, gameState, loadedEntities, camera);

            // Check for queued entity deletion
            foreach (Entity entity in new List<Entity>(loadedEntities))
                if (entity.DeleteQueued)
                    loadedEntities.Remove(entity);
        }

        private Entity InitializeEntity(Entity entity) {
            entity.LoadTextures(_loadedEntityTextures[entity.ID], "default");
            entity.LoadContent(_loadedEntityContent[entity.ID]);
            
            return entity;
        }
    }
}
