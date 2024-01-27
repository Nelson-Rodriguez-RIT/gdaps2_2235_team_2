using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConceptDemo {
    

    /// <summary>
    /// Contains methods that gives all Entity derived classes universal functionality
    /// </summary>
    internal class Entity {
        protected enum TextureID {
            Default,
        }

        private bool deleteQueued;

        protected const string id = "entity";
        protected TextureID activeTextureID;
        protected Vector2 position;
        protected Dictionary<TextureID, Texture2D> textures;

        public bool DeleteQueued { get { return deleteQueued; } }
        public bool TexturesInitialized { get { return textures != null; } }

        /// <summary>
        /// Create an entity at 0, 0
        /// </summary>
        public Entity() : this(new Vector2(0, 0)) { }

        /// <summary>
        /// Create an entity at a custom position
        /// </summary>
        /// <param name="position">Position to spawn entity</param>
        public Entity(Vector2 position) {

            this.position = position;
            this.textures = null;
        }
        
        /// <summary>
        /// Update this entity individually
        /// </summary>
        /// <param name="gameTime">Delta time</param>
        /// <param name="loadedEntities">List of currently loaded (interactable) entities</param>
        public virtual void Update(
            GameTime gameTime, string gameState, List<Entity> loadedEntities) {

            // Boilerplate
            // switch(gameState) {}
        }

        /// <summary>
        /// Deletes this entity at the end of the current update
        /// </summary>
        public virtual void Delete() {
            deleteQueued = true;
        }

        /// <summary>
        /// Gets this entity's associated textures from preloaded memory
        /// </summary>
        /// <param name="preloadedTextures">Preloaded textures</param>
        /// <param name="spawnTextureArgument">Modify initial spawn texture</param>
        public virtual void LoadTextures(List<Texture2D> preloadedTextures, string spawnTextureArgument) {
            // Set the initial texture to be used when entity is spawned in
            activeTextureID = TextureID.Default;

            // Add preloaded textures to this entity's textures based on
            // all of its TextureIDs in decending order
            List <Texture2D> copiedTextures = new List<Texture2D>(preloadedTextures);

            foreach (TextureID textureID in Enum.GetValues<TextureID>()) {
                textures.Add(textureID, copiedTextures.First<Texture2D>());

                copiedTextures.RemoveAt(0);
            }
                
        }

        /// <summary>
        /// Get this Entity's current Texture2D texture
        /// </summary>
        /// <returns>This Entity's current Texture2D texture</returns>
        public virtual Texture2D GetTexture() {
            if (!textures.ContainsKey(activeTextureID)) // Check if texture exists
                return null;

            return textures[activeTextureID];
        }
    }
}
