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
        /// Contains all loaded entities that will be update several times a frame
        /// </summary>
        private List<Entity> entities;


        /// <summary>
        /// Create an instance of the GameManager
        /// </summary>
        public GameManager(
            Dictionary<string, Dictionary<string, List<string>>> _loadedMetadata,
            Dictionary<string, Dictionary<string, Texture2D>> _loadedTextures)
        {
            this._loadedTextures = _loadedTextures;
            this._loadedMetadata = _loadedMetadata;

            entities = new List<Entity>();
        }


        /// <summary>
        /// Creates an entity and adds it the appropriate List
        /// </summary>
        /// <param name="entityID">Entity to create</param>
       public void SpawnEntity(string entityID, Vector2 initialPosition, string arg) {
            // Get loaded texture and metadata content
            Dictionary<string, Texture2D> entityTextures = _loadedTextures[entityID];
            Dictionary<string, List<string>> entityMetadata = _loadedMetadata[entityID];

            // Create an instance of the specified entity
            switch (entityID) {
                default: // If the specified entity doesn't exist, create a basic entity instead
                    // Note, any spawn argument will be ignored though this method
                    entities.Add(new Entity(entityMetadata, entityTextures, initialPosition));
                    break;
            }
        }
    }
}
