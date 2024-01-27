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
        GameManager gameManager;
        CameraManager cameraManager;

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
            cameraManager = new CameraManager();

            // Mono game specific
            _graphics = new GraphicsDeviceManager(this);
            _loadedTextures = new Dictionary<string, Dictionary<string, Texture2D>>();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            

            base.Initialize();
        }

        protected override void LoadContent() {
            // Load textures from ROM
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            PrepareEntityTextures();
        }

        protected override void Update(GameTime gameTime) {
            // Exit game upon an escape button press
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Updates the game state and any loadedentities
            gameManager.Update(gameTime, loadedEntities);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            // _spriteBatch.Draw(ballTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

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