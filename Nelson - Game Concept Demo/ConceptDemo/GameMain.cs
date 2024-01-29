﻿using ConceptDemo.classes;
using ConceptDemo.classes.entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using System;



namespace ConceptDemo
{
    public class GameMain : Game {
        // File Paths
        private const string EntityContentFilePath = "../../../Content/EntityContent.csv";

        /// <summary>
        /// Contains all loaded entities that will be update several times a frame
        /// </summary>
        private List<Entity> loadedEntities;

        // Functional classes that maintain core features such as gameplay or the camera
        private GameManager gameManager;
        private CameraManager camera;

        /// <summary>
        /// Contains all preloaded textures gathered from PrepareEntityContent
        /// </summary>
        private Dictionary<EntityID, List<Texture2D>> _loadedTextures;

        // Mono game specific
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public GameMain() {
            // Prepare entities
            loadedEntities = new List<Entity>();

            // Prepare functional classes
            gameManager = new GameManager(_loadedTextures);
            camera = new CameraManager();

            // Mono game specific
            _graphics = new GraphicsDeviceManager(this);
            _loadedTextures = new Dictionary<EntityID, List<Texture2D>>();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        // Mono methods ----------------------------------------------------
        protected override void Initialize() {
            base.Initialize();
        }

        // Load textures and related functionality
        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            PrepareEntityContent();
        }

        // Updates several times a frame
        protected override void Update(GameTime gameTime) {
            // Exit game upon an escape button press
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Updates the game state and any loadedentities
            gameManager.Update(gameTime, loadedEntities, camera);
            base.Update(gameTime);
        }

        // Draws textures several times a frame
        protected override void Draw(GameTime gameTime) {
            // Set background color
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(); // Begin displaying textures

            // Update each entity and go through each layer
            // TODO: Add animation capabilities
            for (int layer = 0; layer < 10; layer++)
                foreach (Entity entity in loadedEntities) {
                    // Only draw textures if they're initialized
                    if (entity.TexturesInitialized && entity.DrawHierarchy == layer)
                        entity.Draw(_spriteBatch);
                }

            _spriteBatch.End(); // End displaying textures
            base.Draw(gameTime);
        }


        // Non-mono methods ------------------------------------------------
        /// <summary>
        /// Loads all content based on the EntityContent.csv
        /// </summary>
        private void PrepareEntityContent() {
            string fileInput;
            string[] formatedFileInput;
            List<Texture2D> loadedTexture2D;

            /*
            EntityContent.txt Formatting
            
            ID
            TextureID

            For the moment all entities should at least share the same amount
            lines of information to properly carry over information.
            */

            // Start reading repective file data
            StreamReader reader = new StreamReader(EntityContentFilePath);

            // This iterates through each entity
            foreach (EntityID entityID in Enum.GetValues<EntityID>()) {
                // ID
                reader.ReadLine(); // For file editing purposes only


                // TextureID
                formatedFileInput = reader.ReadLine().Split(',');

                // Create a new list to store Texture2Ds with, add it to _loadedTextures
                _loadedTextures.Add(entityID, (loadedTexture2D = new List<Texture2D>()));

                // For each TextureID for this entity, find and load its corresponding file data
                foreach (string textureID in formatedFileInput) {
                    if (textureID != formatedFileInput[0])
                        loadedTexture2D.Add(Content.Load<Texture2D>(
                            $"textures/{formatedFileInput[0]}/{textureID}"));
                }
            }
        }
    }

    /// <summary>
    /// Contains all possible entity IDs, must match EntityContent.txt order
    /// </summary>
    public enum EntityID {
        entity
    }

    /// <summary>
    /// Contains IDs related to the game's current state
    /// </summary>
    public enum GameStateID {
        overworld_test_load, // Used to load assests
        overworld_test
    }
}