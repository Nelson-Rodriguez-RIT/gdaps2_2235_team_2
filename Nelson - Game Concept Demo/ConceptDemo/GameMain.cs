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

// Coded by Nelson Rodriguez
// Start Date: 1.30.2024
// A demo showcasing potential ideas and features, possibly alongside implementations

namespace ConceptDemo
{
    public class GameMain : Game {
        // Have it so a Dictionary<> is structured similar to the file system
        private const string ContentFilePath = "../../../Content/";
        private const string EntitiesFilePath = ContentFilePath + "Entities";
        private const string TextureFilePath = "/textures";
        private const string MetadataFilePath = "/data.csv";

        private Dictionary<string, Dictionary<string, Texture2D>> _loadedTextures;
        private Dictionary<string, Dictionary<string, List<string>>> _loadedMetadata;

        // Functional classes that maintain core features such as gameplay or the camera
        private GameManager gm;

        // Mono game specific
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        public GameMain() {
            // Prepare loaded content storage
            _loadedTextures = new Dictionary<string, Dictionary<string, Texture2D>>();
            _loadedMetadata = new Dictionary<string, Dictionary<string, List<string>>>();

            // Prepare functional classes
            gm = new GameManager(_loadedMetadata, _loadedTextures);

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
            PrepareContent();
        }

        // Updates several times a frame
        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }

        // Draws textures several times a frame
        protected override void Draw(GameTime gameTime) {
            // Set background color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(); // Begin displaying textures



            _spriteBatch.End(); // End displaying textures

            base.Draw(gameTime);
        }


        // Non-mono methods ------------------------------------------------
        /// <summary>
        /// Loads textures and entity specific data (metadata) into memory. Final results of which
        /// are stored in _loadedTextures and _loadedMetadata respectivily
        /// </summary>
        private void PrepareContent() {
            // The starting folder is expected to be Content //

            // Load content for entities
            StreamReader fileIn = null;
            string[] contentDirectories = Directory.GetDirectories(EntitiesFilePath);

            foreach (string path in contentDirectories) {
                // Get an ID in order to properly store internally
                string entityID = path.Split("\\")[1];

                try {
                    // Get entity metadata from metadata.csv
                    Dictionary<string, List<string>> bufferedMetadata = new Dictionary<string, List<string>>();
                    fileIn = new StreamReader(path + MetadataFilePath);
                    string metadata;

                    while ((metadata = fileIn.ReadLine()) != null) {
                        // Comments are skipped over when reading from file
                        if (metadata[0] == '/') continue;

                        // Denotes the following line of metadata's purpose
                        string dataHeader = metadata.Split('=')[0];

                        // Get the actual metadata from the rest of the line
                        string[] unsplitDataBody = metadata.Split('=')[1].Split(',');

                        // Add each piece of data to a list so it can be stored with its header
                        List<string> dataBody = new List<string>();
                        foreach (string chunk in unsplitDataBody)
                            dataBody.Add(chunk);

                        // Store this set of data with its header
                        bufferedMetadata.Add(dataHeader, dataBody);
                    }

                    // Store metadata
                    _loadedMetadata.Add(entityID, bufferedMetadata);


                    // Get textures from textures directory
                    string[] textureFiles = Directory.GetFiles(path + TextureFilePath);
                    Dictionary<string, Texture2D> bufferedTextures = new Dictionary<string, Texture2D>();

                    foreach (string fileName in textureFiles)
                        bufferedTextures.Add(fileName, Content.Load<Texture2D>(path + TextureFilePath + fileName));

                    // Store textures
                    _loadedTextures.Add(entityID, bufferedTextures);
                }
                catch { } // Have this error logged to a file
                finally {
                    if (fileIn != null)
                        fileIn.Close();
                }
            }
        }
    } 
}