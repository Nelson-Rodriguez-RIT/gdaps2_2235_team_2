using ConceptDemo.classes;
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
        // Have it so a Dictionary<> is structured similar to the file system
        private const string PathToContent = "../../../Content/";

        private Dictionary<string, List<Texture2D>> _loadedTextures;
        private Dictionary<string, List<string>> _loadedMetadata;

        /// <summary>
        /// Contains all loaded entities that will be update several times a frame
        /// </summary>
        private List<Entity> loadedEntities;

        // Functional classes that maintain core features such as gameplay or the camera
        private GameManager gameManager;
        private CameraManager camera;

        // Mono game specific
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        public GameMain() {
            // Prepare loaded content storage
            _loadedTextures = new Dictionary<string, List<Texture2D>>();
            _loadedMetadata = new Dictionary<string, List<string>>();

            // Prepare loaded entities storage
            loadedEntities = new List<Entity>();
            
            // Prepare functional classes
            gameManager = new GameManager(_loadedEntityTextures);
            camera = new CameraManager();

            // Mono game specific
            _graphics = new GraphicsDeviceManager(this);

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

            // PrepareEntityContent();
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
        private void PrepareContent() {
            
        }





        /// <summary>
        /// Loads all content based on the EntityContent.csv
        /// </summary>
        private void PrepareEntityContent() {
            string fileEntityID; // ?

            string fileInput;
            string[] formatedFileInput;

            List<Texture2D> loadedTexture2D;

            /*
            EntityContent.txt Formatting
            
            ID=""
            TextureID="","","", ... "" 
            Size=#,#

            Entities do not need fill/have all this information. Only the bits
            they plan on usings

            Besides the headers, treat regular information as you would in a csv
            */

            // Start reading repective file data
            StreamReader fileIn = new StreamReader(EntityContentFilePath);

            // This iterates through each entity
            foreach (EntityID entityID in Enum.GetValues<EntityID>()) {
                while ((fileInput = fileIn.ReadLine()) != "$") {
                    // Get the header to find out type of data in line
                    string fileContentHeader = "";
                    foreach (char c in fileInput)
                        if (c == '=')
                            break;
                        else
                            fileContentHeader += c;

                    // Get data after the header
                    formatedFileInput = fileInput.TrimStart(new char[fileContentHeader.Length]).Split(',');

                    // Associate related data
                    switch (fileContentHeader) {
                        case "ID":

                    }

                }

                /* Old File Systems


                // ID
                fileEntityID = reader.ReadLine();

                // TextureID
                formatedFileInput = reader.ReadLine().Split(',');

                // Create a new list to store Texture2Ds with, add it to _loadedTextures
                _loadedTextures.Add(entityID, (loadedTexture2D = new List<Texture2D>()));

                // For each TextureID for this entity, find and load its corresponding file data
                foreach (string textureID in formatedFileInput) {
                    loadedTexture2D.Add(Content.Load<Texture2D>(
                        $"textures/{fileEntityID}/{textureID}"));
                }
                */
            }
        }
    }


    /// <summary>
    /// Contains all possible entity IDs, must match EntityContent.txt order
    /// </summary>
    public enum EntityID {
        entity,
        character
    }

    /// <summary>
    /// Contains IDs related to the game's current state
    /// </summary>
    public enum GameStateID {
        initialize,
        overworld_test_load, // Used to load assests
        overworld_test
    }
}