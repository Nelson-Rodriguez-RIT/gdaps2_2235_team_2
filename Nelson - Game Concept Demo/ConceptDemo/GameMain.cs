using ConceptDemo.classes;
using ConceptDemo.classes.entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;



namespace ConceptDemo
{
    public class GameMain : Game {
        // File Paths
        private const string textureIDsFilePath = "../texureIDs.csv";

        /// <summary>
        /// Contains all loaded entities that will be update several times a frame
        /// </summary>
        private List<Entity> loadedEntities;

        // Functional classes that maintain core features such as gameplay or the camera
        private GameManager gameManager;
        private CameraManager camera;

        /// <summary>
        /// Contains all preloaded textures gathered from PrepareEntityTextures
        /// </summary>
        private Dictionary<string, Dictionary<string, Texture2D>> _loadedTextures;

        // Mono game specific
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public GameMain() {
            // Prepare entities
            loadedEntities = new List<Entity>();

            // Prepare functional classes
            gameManager = new GameManager();
            camera = new CameraManager();

            // Mono game specific
            _graphics = new GraphicsDeviceManager(this);
            _loadedTextures = new Dictionary<string, Dictionary<string, Texture2D>>();

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
            PrepareEntityTextures();
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
        /// Loads all textures based on the TextureID.csv
        /// </summary>
        private void PrepareEntityTextures() {
            string fileInput;
            string[] formatedFileInput;

            // Start reading repective file data
            StreamReader textureReader = new StreamReader(textureIDsFilePath);
            textureReader.ReadLine(); // Ignore first line descriptor

            // Iterate through each group of textures
            while ((fileInput = textureReader.ReadLine()) != null) {
                formatedFileInput = fileInput.Split(',');
                Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();

                // For each textureID in this group, find and load its corresponding file data
                foreach (string textureID in formatedFileInput) {
                    if (textureID != formatedFileInput[0])
                        loadedTextures.Add(textureID, Content.Load<Texture2D>($"{textureID}"));
                }


                _loadedTextures.Add(formatedFileInput[0], loadedTextures);
            }

        }
    }

    /// <summary>
    /// Contains IDs related to the game's current state
    /// </summary>
    public enum GameStateID {
        initialize,
        overworld_test
    }

    /// <summary>
    /// Contains all possible entity IDs
    /// </summary>
    public enum EntityID {
        entity
    }
}