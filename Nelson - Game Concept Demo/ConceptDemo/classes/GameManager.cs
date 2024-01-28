using ConceptDemo.classes.entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConceptDemo.classes {
    internal class GameManager {
        /// <summary>
        /// Current state of the game
        /// </summary>
        private GameStateID gameState;


        public GameStateID GameState { get { return gameState; } }


        /// <summary>
        /// Initialize the game normally
        /// </summary>
        public GameManager() : this(GameStateID.initialize) {}

        /// <summary>
        /// Initialize the game in a custom state
        /// </summary>
        /// <param name="gameState">State to initialize game in</param>
        public GameManager(GameStateID gameState)  {
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
                    loadedEntities.Add(new Entity());

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
    }
}
