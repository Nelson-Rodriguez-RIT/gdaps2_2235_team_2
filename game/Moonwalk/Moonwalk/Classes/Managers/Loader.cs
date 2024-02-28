using Moonwalk.Classes.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System;

namespace Moonwalk.Classes.Managers {
    internal static class Loader {
        private static ContentManager content = null;

        public static ContentManager Content {
            set {
                if (content == null)
                    content = value;
            }
        }

        public static List<string> LoadFile(string path) {
            StreamReader reader = new StreamReader(path); ;
            List<string> data = new();
            string incomingData;

            // Get all file contents
            while ((incomingData = reader.ReadLine()) != null)
                data.Add(incomingData);


            return data;
        }

        public static Texture2D LoadTexture(string path) {
            return content.Load<Texture2D>(path);
        }

        public static (
                List<int[][]> tiles, 
                Dictionary<int, Texture2D> sprites,
                List<Terrain> geometry) 
                LoadMap(string path) {
            // In the middle of refactoring this class - Nelson
        }

        public static (
                Dictionary<string, string> properties,
                Dictionary<int, (int totalSprites, int framesPerSprite)> animationMetadata,
                Texture2D spritesheet,
                Vector2 spriteSize)
                LoadEntity(string path) {
            // Temporary holders for collected file data
            Dictionary<string, string> bufferedProperties = new();
            Dictionary<int, (int totalSprites, int framesPerSprite)> 
                bufferedAnimationData = new();
            Texture2D bufferedSpritesheet;
            Vector2 bufferedSpriteSize;

            Queue<string> fileData;
            string[] splitData;


            // Get entity properties
            fileData = new Queue<string>(LoadFile($"{path}/properties.dat"));

            while (fileData.Count != 0) {
                if (fileData.Peek()[0] == '/') { // Ignore comments
                    fileData.Dequeue();
                    continue;
                }

                // Break data into its header and body components
                splitData = fileData.Dequeue().Split('=');

                // Format and store data
                bufferedProperties.Add(splitData[0], splitData[1]);
            }


            // Get animation data
            fileData = new Queue<string>(LoadFile($"{path}/animations.dat"));

            // First line always contains sprite's WidthxHeight
            splitData = fileData.Dequeue().Split('x');
            bufferedSpriteSize = new Vector2(
                int.Parse(splitData[0]),    // Individual sprite width
                int.Parse(splitData[1]));   // Individual sprite height

            for (int i = 0; fileData.Count != 0; i++) {
                splitData = fileData.Dequeue().Split(',');
                bufferedAnimationData.Add(
                    i, (                        // Matches its respective Animations enum value
                    int.Parse(splitData[0]),    // Number of sprites for this animation
                    int.Parse(splitData[1])));  // Number of frames each sprite gets in this animation
            }
                

            // Get sprite sheet
            bufferedSpritesheet = LoadTexture($"{path}/spritesheet");


            return (bufferedProperties, bufferedAnimationData, bufferedSpritesheet, bufferedSpriteSize);
        }
    }
}
