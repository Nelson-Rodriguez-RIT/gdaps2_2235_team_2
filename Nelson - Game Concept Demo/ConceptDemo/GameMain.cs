using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace ConceptDemo {
    public class GameMain : Game {
        // File Paths
        private const string textureIDsFilePath = "../texureIDs.csv";
        private const string texturesFilePath = "";
        private const string statsFilePath = "";

        // Contains all loaded entities
        private List<Entity> loadedEntities;

        // Current state of the game
        private string gameState;

        // Contains data loaded from file
        private Dictionary<string, Stats> _loadedStats;
        private Dictionary<string, Dictionary<string, Texture2D>> _loadedTextures;

        // Mono game specific
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public GameMain() {
            loadedEntities = new List<Entity>();
            gameState = "initialize";

            _graphics = new GraphicsDeviceManager(this);
            _loadedStats = new Dictionary<string, Stats>();
            _loadedTextures = new Dictionary<string, Dictionary<string, Texture2D>>();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            gameState = "overworld"; // Temp

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            PrepareEntityTextures();
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update each entity
            foreach (Entity entity in loadedEntities)
                entity.Update(gameTime, gameState, loadedEntities);

            // Check for queued entity deletion
            // A new list is made to avoid complications when deleting
            foreach (Entity entity in new List<Entity>(loadedEntities))
                if (entity.DeleteQueued)
                    loadedEntities.Remove(entity);

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
}