using ConceptDemo.classes.entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConceptDemo.classes.entities
{


    /// <summary>
    /// Contains methods that gives all Entity derived classes universal functionality
    /// </summary>
    internal class Entity
    {
        /// <summary>
        /// ID of this entity
        /// </summary>
        protected const EntityID entityID = EntityID.entity;

        /// <summary>
        /// ID's of this entity's specific textures. Case-insensitive
        /// </summary>
        protected enum TextureID
        {
            temp
        }

        private bool deleteQueued;

        /// <summary>
        /// Absolute position on a graphical plane
        /// </summary>
        protected Vector2 absPosition;

        /// <summary>
        /// Relative position to the camera (or from the top left corner of the screen)
        /// </summary>
        protected Vector2 relPosition;

        /// <summary>
        /// Maps texture IDs to their relevant Texture2D references
        /// </summary>
        protected Dictionary<TextureID, Texture2D> textures;
        protected TextureID activeTextureID;
        protected int drawHierarchy; // 0 is drawn first, 9 is drawn last (therefore on top)

        public bool DeleteQueued { get { return deleteQueued; } }
        public bool TexturesInitialized { get { return textures != null; } }
        public int DrawHierarchy {  get { return drawHierarchy; } }
        public EntityID ID { get { return entityID; } }

        /// <summary>
        /// Create an entity at 0, 0
        /// </summary>
        public Entity() : this(new Vector2(0, 0)) { }

        /// <summary>
        /// Create an entity at a custom position
        /// </summary>
        /// <param name="position">Position to spawn entity</param>
        public Entity(Vector2 absPosition)
        {
            drawHierarchy = 0;

            this.absPosition = absPosition;
            relPosition = new Vector2(0, 0); // Updated after first update cycle

            // Textures are gathered after the first update cycle 
            textures = null;
        }

        /// <summary>
        /// Update this entity individually
        /// </summary>
        /// <param name="gameTime">Delta time</param>
        /// <param name="loadedEntities">List of currently loaded (interactable) entities</param>
        public virtual void Update(
            GameTime gameTime, GameStateID gameState, List<Entity> loadedEntities, CameraManager camera)
        {
            // Update relPosition based on the camera's position
            relPosition = camera.GetRelativePosition(absPosition);
        }

        /// <summary>
        /// Draw this entity's relevant textures to the screen
        /// </summary>
        /// <param name="_spriteBatch">Texture container (Sprite Batch)</param>
        public virtual void Draw(SpriteBatch _spriteBatch) {
            // Entity is draw relative to the camera
            _spriteBatch.Draw(textures[activeTextureID], relPosition, Color.White);

            // Animations will be done manually by each entity object
        }

        /// <summary>
        /// Deletes this entity at the end of the current update
        /// </summary>
        public virtual void Delete()
        {
            deleteQueued = true;
        }

        /// <summary>
        /// Gets this entity's associated textures from preloaded memory
        /// </summary>
        /// <param name="preloadedTextures">Preloaded textures</param>
        /// <param name="spawnTextureArgument">Modify initial spawn texture</param>
        public virtual void LoadTextures(List<Texture2D> preloadedTextures, string spawnTextureArgument)
        {
            textures = new Dictionary<TextureID, Texture2D>();

            // Set the initial texture to be used when entity is spawned in
            switch (spawnTextureArgument) {
                case "default":
                    activeTextureID = TextureID.temp;
                    break;
            }
            

            // Add preloaded textures to this entity's textures based on
            // all of its TextureIDs in decending order
            List<Texture2D> copiedTextures = new List<Texture2D>(preloadedTextures);

            foreach (TextureID textureID in Enum.GetValues<TextureID>())
            {
                textures.Add(textureID, copiedTextures.First());

                copiedTextures.RemoveAt(0);
            }
        }
    }
}
