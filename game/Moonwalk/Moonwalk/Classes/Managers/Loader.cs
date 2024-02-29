using Moonwalk.Classes.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Linq;

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
                List<Terrain> geometry,
                Dictionary<int, Texture2D> sprites,
                Vector2 tileSize) 
                LoadMap(string path) {
            List<int[][]> bufferedTiles = new();
            List<Terrain> bufferedGeometry = new();
            Dictionary<int, Texture2D> bufferedSprites = new();
            Vector2 bufferdTileSize;

            Queue<string> fileData;
            MatchCollection parsedData;
            List<string> data;


            // Begin reading .tmx file data
            fileData = new Queue<string>(LoadFile($"{path}map.tmx"));

            // First line contains tmx file formating information (we don't care about this)
            fileData.Dequeue();

            // Second line contains general information about the map
            // We care about the 5th/6th numerical values (tilewidth and tileheight)
            parsedData = Regex.Matches(fileData.Dequeue(), @"[.\d]+"); // Dante really clutched up by knowing about this :D
            data = parsedData.Cast<Match>().Select(match => match.Value).ToList();

            bufferdTileSize = new Vector2(
                int.Parse(data[4]),     // Individual tile width
                int.Parse(data[5]));    // Individual tile height


            // Begin reading tile and geometry data
            while (fileData.Count != 0) {
                // Reading tile data
                if (fileData.Peek().Contains("<layer")) {
                    // Ignore tmx file formating data
                    fileData.Dequeue();
                    fileData.Dequeue();

                    // Read and collected relevant file data
                    List<int[]> tileRows = new();
                    while (!fileData.Peek().Contains("</data>")) {
                        // Get relevant numerical data
                        parsedData = Regex.Matches(fileData.Dequeue(), @"[.\d]+");
                        data = parsedData.Cast<Match>().Select(match => match.Value).ToList();

                        // Extract each tile ID from parsed data
                        int[] tileRow = new int[data.Count];
                        for (int i = 0; i < tileRow.Length; i++)
                            tileRow[i] = int.Parse(data[i]);

                        tileRows.Add(tileRow);
                    }

                    // Format all collectecd data
                    int[][] tiles = new int[tileRows.Count][];
                    for (int i = 0; i < tiles.Length; i++)
                        tiles[i] = tileRows[i];

                    // Store formated tile data
                    bufferedTiles.Add(tiles);
                }

                // Reading geometry data
                else if (fileData.Peek().Contains("<objectgroup")) {
                    // Ignore tmx formatting
                    fileData.Dequeue();

                    while (!fileData.Peek().Contains("</objectgroup>")) {
                        // Get relevant numerical data
                        parsedData = Regex.Matches(fileData.Dequeue(), @"[.\d]+");
                        data = parsedData.Cast<Match>().Select(match => match.Value).ToList();

                        // We care for the 2nd, 3rd, 4th, and 5th numeric values in the file
                       bufferedGeometry.Add(new Terrain(new Rectangle(
                            (int)float.Parse(data[1]),     // Collision's relative X position
                            (int)float.Parse(data[2]),     // Collision's relative Y position
                            (int)float.Parse(data[3]),     // Collision's width
                            (int)float.Parse(data[4])      // Collision's height
                            )));
                    }
                }

                else // If it isn't one of the above, most likely tmx file formatting data
                    fileData.Dequeue();
            }


            // Get sprite data
            string[] spriteFilePaths = Directory.GetFiles($"{path}sprites/"); // Gets paths to all images
            for (int ID = 0; ID < spriteFilePaths.Length; ID++)
                bufferedSprites.Add(
                    ID + 1,                                         // Relevant ID of the tile sprite
                    LoadTexture(spriteFilePaths[ID]                 // Relevant Texture2D 
                        .Remove(spriteFilePaths[ID].Length - 4)));


            return (bufferedTiles, bufferedGeometry,
                    bufferedSprites, bufferdTileSize);
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
