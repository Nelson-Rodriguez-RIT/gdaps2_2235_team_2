using ConceptDemo.classes.entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;

namespace ConceptDemo.classes {
    /// <summary>
    /// Controls gameplay related functionalities
    /// </summary>
    internal class GameManager {
        private Dictionary<string, Dictionary<string, Texture2D>> _loadedTextures;
        private Dictionary<string, Dictionary<string, List<string>>> _loadedMetadata;

        /// <summary>
        /// Contains all loaded _entities that will be update several times a frame.
        /// <para>&#160;</para> 
        /// <para>As a general rule of advice, do NOT have multiple reference</para> 
        /// <para>variables referencing one object or else bad things happen. :(</para>
        /// </summary>
        private List<Entity> _entities;

        /// <summary>
        /// State of the game
        /// </summary>
        private State gameState; 

        /// <summary>
        /// Create an instance of the GameManager
        /// </summary>
        public GameManager(
            Dictionary<string, Dictionary<string, List<string>>> _loadedMetadata,
            Dictionary<string, Dictionary<string, Texture2D>> _loadedTextures)
        {
            this._loadedTextures = _loadedTextures;
            this._loadedMetadata = _loadedMetadata;

            _entities = new List<Entity>();
        }

        /// <summary>
        /// Updates the gameplay several times a frame
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime) {
            switch (gameState) {
                case State.initialize:
                    SpawnEntity("entity", new Vector2(0,0));

                    gameState = State.gameplay_test;
                    break;

                case State.gameplay_test:

                    break;
            }

            // Update entities
            foreach (Entity entity in _entities)
                entity.Update(gameTime);

            // Follow up on entities queued to delete
            foreach (Entity entity in _entities)
                if (entity.DeleteQueued) 
                    DeleteEntity(entity);
        }

        /// <summary>
        /// Updates the graphics several times a frame
        /// </summary>
        /// <param name="gameTime">DeltaTime (normalized) variable</param>
        public void Draw(GameTime gameTime) {
            // Draw entities
            foreach (Entity entity in _entities)
                entity.Draw(gameTime);
        }

        /// <summary>
        /// Spawn an entity at a specified location
        /// </summary>
        /// <param name="entityID">ID of the enemy you wish to spawn</param>
        /// <param name="initialPosition"></param>
        public void SpawnEntity(string entityID, Vector2 initialPosition) {
             // Get loaded texture and metadata content
             Dictionary<string, Texture2D> entityTextures = _loadedTextures[entityID];
             Dictionary<string, List<string>> entityMetadata = _loadedMetadata[entityID];

             // Create an instance of the specified entity
             switch (entityID) {
                 default: // If the specified entity doesn't exist, create a basic entity instead
                     // Note, any spawn argument will be ignored though this method
                     // Additionally, logged this to the error file
                     _entities.Add(new Entity(entityMetadata, entityTextures, initialPosition));
                     break;
             }
        }

        /// <summary>
        /// Delete an entity from _entities
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        public void DeleteEntity(Entity entity) {
            _entities.Remove(entity);
        }

        /// <summary>
        /// Gameplay states of the game
        /// </summary>
        public enum State {
            initialize,
            gameplay_test
        }
    }
}
