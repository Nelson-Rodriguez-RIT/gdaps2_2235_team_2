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

        public void Update(GameTime gameTime, List<Entity> loadedEntities) {
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
                entity.Update(gameTime, gameState, loadedEntities);

            // Check for queued entity deletion
            // A new list is made to avoid complications when deleting
            foreach (Entity entity in new List<Entity>(loadedEntities))
                if (entity.DeleteQueued)
                    loadedEntities.Remove(entity);
        }
    }
}
