using Moonwalk.Classes.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Moonwalk.Classes.Managers {
    internal static class Loader {
        // Used to load content such as PNGs from file
        // More useful to have it here so we can load files only
        // when we need too, instead of all at once on start up
        private static ContentManager content = null;

        public static ContentManager Content {
            set {
                if (content == null)
                    content = value;
            }
        }


        /// <summary>
        /// Reads file content
        /// </summary>
        /// <param name="path">File to read</param>
        /// <returns>A List containing each line of the file</returns>
        public static List<string> LoadFile(string path) {
            StreamReader reader = new StreamReader(path); ;
            List<string> data = new();
            string incomingData;

            // Get all file contents
            while ((incomingData = reader.ReadLine()) != null)
                data.Add(incomingData);


            return data;
        }

        /// <summary>
        /// Looks for a specific file type and reads its content
        /// </summary>
        /// <param name="directory">Directory to search</param>
        /// <param name="fileType">File type to look for</param>
        /// <returns>A List containing each line of the file</returns>
        public static List<string> LoadFile(string directory, string fileType) {
            string[] paths = Directory.GetFiles(directory);
            string relevantPath = null;

            // Get the first path to have the relevant file ending
            foreach (string path in paths)
                if (path.Contains(fileType)) {
                    relevantPath = path;
                    break;
                }

            if (relevantPath == null)
                return null;

            // Load relevant data
            return LoadFile(relevantPath);
        }

        /// <summary>
        /// Loads texture data from file
        /// </summary>
        /// <param name="path">Texture file to load</param>
        public static Texture2D LoadTexture(string path) {
            // StackTrace	"   at Microsoft.Xna.Framework.Content.ContentManager.OpenStream(String assetName)\r\n   at Microsoft.Xna.Framework.Content.ContentManager.ReadAsset[T](String assetName, Action`1 recordDisposableObject)\r\n   at Microsoft.Xna.Framework.Content.ContentManager.Load[T](String assetName)\r\n   at Moonwalk.Classes.Managers.Loader.LoadTexture(String path) in Z:\\IGMProfile\\Documents\\GitHub\\gdaps2_2235_team_2\\game\\Moonwalk\\Moonwalk\\Classes\\Managers\\Loader.cs:line 55\r\n   at Moonwalk.Classes.Managers.Loader.LoadMap(String path) in Z:\\IGMProfile\\Documents\\GitHub\\gdaps2_2235_team_2\\game\\Moonwalk\\Moonwalk\\Classes\\Managers\\Loader.cs:line 126\r\n   at Moonwalk.Classes.Managers.Map.LoadMap(String mapRootFolderName) in Z:\\IGMProfile\\Documents\\GitHub\\gdaps2_2235_team_2\\game\\Moonwalk\\Moonwalk\\Classes\\Managers\\Map.cs:line 36\r\n   at Moonwalk.Classes.Managers.GameManager.Transition(GameState nextState) in Z:\\IGMProfile\\Documents\\GitHub\\gdaps2_2235_team_2\\game\\Moonwalk\\Moonwalk\\Classes\\Managers\\GameManager.cs:line 183\r\n   at Moonwalk.Classes.Managers.GameManager..ctor(ContentManager content, GraphicsDevice graphics) in Z:\\IGMProfile\\Documents\\GitHub\\gdaps2_2235_team_2\\game\\Moonwalk\\Moonwalk\\Classes\\Managers\\GameManager.cs:line 67\r\n   at Moonwalk.Classes.Managers.GameManager.GetInstance(ContentManager content, GraphicsDevice graphics) in Z:\\IGMProfile\\Documents\\GitHub\\gdaps2_2235_team_2\\game\\Moonwalk\\Moonwalk\\Classes\\Managers\\GameManager.cs:line 77\r\n   at Moonwalk.GameMain.LoadContent() in Z:\\IGMProfile\\Documents\\GitHub\\gdaps2_2235_team_2\\game\\Moonwalk\\Moonwalk\\GameMain.cs:line 52\r\n   at Moonwalk.GameMain.Initialize() in Z:\\IGMProfile\\Documents\\GitHub\\gdaps2_2235_team_2\\game\\Moonwalk\\Moonwalk\\GameMain.cs:line 45\r\n   at Microsoft.Xna.Framework.Game.DoInitialize()\r\n   at Microsoft.Xna.Framework.Game.Run(GameRunBehavior runBehavior)\r\n   at Program.<Main>$(String[] args) in Z:\\IGMProfile\\Documents\\GitHub\\gdaps2_2235_team_2\\game\\Moonwalk\\Moonwalk\\Program.cs:line 3"	string
            if (path == "../../../Content/Maps/Demo/sprites/Thumb")
                return null; // I haven't got a clue why it tries to search for this

            return content.Load<Texture2D>(path);
        }

        /// <summary>
        /// Loads a map's MDF and sprite sheet
        /// </summary>
        /// <param name="path">Should (usually) be a valid map's name</param>
        public static MapDataFile LoadMap(string path) {
            List<int[][]> bufferedTiles = new();
            List<Terrain> bufferedGeometry = new();
            Texture2D bufferedSpritesheet;
            Vector2 bufferdTileSize = new Vector2();


            // MDF file reading //
            Queue<string> fileData = new();
            string data;
            string[] dataBlock;

            // Begin reading mdf file data
            fileData = new Queue<string>(LoadFile(path, ".mdf"));
            while (fileData.Count != 0) { // Peek() or Dequeue() will throw exceptions if Queue is empty :(
                data = fileData.Dequeue();

                // Tile data uses a keyword system (which differs from the typical Header=Body sytem)
                // To add on to this system, add an else if statement. If you need to add on to the Header=Body
                // format, add a case to the switch statement inside the else block
                if (data == "TILESTART") {
                    List<int[]> tileRows = new(); // New layer
                    while ((data = fileData.Dequeue()) != "TILEEND") { // Continue untill TILEEND keyword
                        dataBlock = data.Split(',');            // Prepare data to be read
                        int[] row = new int[dataBlock.Length];  // Properly size storage

                        for (int i = 0; i < row.Length - 1; i++) // Reading data
                            row[i] = int.Parse(dataBlock[i]);

                        tileRows.Add(row);
                    }

                    // Convert list into a jagged array
                    int[][] formattedTileRows = new int[tileRows.Count][];
                    for (int i = 0; i < formattedTileRows.Length; i++)
                        formattedTileRows[i] = tileRows[i];

                    bufferedTiles.Add(formattedTileRows);
                }

                // Header=Body
                // FYI if you want to add new types of collision (or really any type of data)
                // via object layers, do it here.
                else {
                    dataBlock = data.Split("=");

                    switch(dataBlock[0].ToUpper()) {
                        // Information about the width and height of an individual tile
                        case "SIZE":
                            bufferdTileSize = new Vector2(
                                int.Parse(dataBlock[0]),    // Width
                                int.Parse(dataBlock[1])     // Height
                                );
                            break;

                        // Represents the most basic form of collision
                        // TMX to MDF converter looks at the name of the object layer
                        // when rewriting relevant information to file
                        case "TERRAIN":
                            bufferedGeometry.Add(new Terrain(new Rectangle(
                                int.Parse(dataBlock[0]),    // X
                                int.Parse(dataBlock[1]),    // Y
                                int.Parse(dataBlock[2]),    // Width
                                int.Parse(dataBlock[3])     // Height
                                )));
                            break;
                    }
                }
            }

            // Get sprite data
            bufferedSpritesheet = content.Load<Texture2D>($"{path}spritesheet");


            return new MapDataFile(bufferedTiles, bufferedGeometry,
                    bufferedSpritesheet, bufferdTileSize);
        }
        
        public static (
                Dictionary<string, string> properties,
                List<Animation> animations,
                Texture2D spritesheet)
                LoadEntity(string path, bool loadAnimations = true) {
            Dictionary<string, string> bufferedProperties = new();
            List<Animation> bufferedAnimations = new();
            Texture2D bufferedSpritesheet = null;

            Queue<string> fileData;
            string[] splitData;

            // Get entity properties
            fileData = new Queue<string>(LoadFile($"{path}/", ".edf"));

            while (fileData.Count != 0) {
                if (fileData.Peek()[0] == '/' || // Ignore comments or (likely) empty lines
                        fileData.Peek()[0] == ' ') {
                    fileData.Dequeue();
                    continue;
                }

                // Break data into its header and body components
                splitData = fileData.Dequeue().Split('=');

                // Format and store data
                bufferedProperties.Add(splitData[0], splitData[1]);
            }


            if (loadAnimations) {
                // Get animation data
                fileData = new Queue<string>(LoadFile($"{path}/", ".adf"));

                AnimationStyle style = AnimationStyle.Horizontal;

                // Set up default fields with placeholder values
                // Prevents crashes and allows us to compile :D
                Vector2 defaultSpriteSize = Vector2.One;
                Vector2 defaultOrigin = Vector2.Zero;
                int defaultTotalSprites = 1;
                int defaultFramesPerSprite = 1;

                // This allows use to have custom Widths/Heights for each animation since this
                // will inform the Animation where it should get its data from. This will be
                // incremented by either a Width (for Vertical styles) or Height (for Horizontal styles)
                int spaceTakenOnSpritesheet = 0;

                while (fileData.Count != 0) {
                    if (fileData.Peek().Length == 0) // Skip empty lines
                        fileData.Dequeue();

                    switch (fileData.Peek()[0]) {
                        case '/': // Ignore comments or (likely) empty lines
                        case ' ':
                            fileData.Dequeue();
                            break;

                        case '&': // Set AnimationStyle
                            if (fileData.Dequeue().Contains("Vertical"))
                                style = AnimationStyle.Vertical;
                            break;

                        case '@': // Set default values
                            splitData = fileData.Dequeue().Split(",");
                            splitData[0] = splitData[0].Remove(0, 1); // Removes line identifier

                            defaultSpriteSize = new Vector2(
                                int.Parse(splitData[0].Split('x')[0]),
                                int.Parse(splitData[0].Split('x')[1])
                                );

                            defaultOrigin = new Vector2(
                                int.Parse(splitData[1].Split(':')[0]),
                                int.Parse(splitData[1].Split(':')[1])
                                );

                            defaultTotalSprites = int.Parse(splitData[2]);

                            defaultFramesPerSprite = int.Parse(splitData[3]);
                            break;

                        default: // Create an animation using file data
                            splitData = fileData.Dequeue().Split(',');

                            // If a # is detected, default values are used instead
                            bufferedAnimations.Add(new Animation(
                                splitData[0][0] == '#' ? defaultSpriteSize :
                                    new Vector2(
                                        int.Parse(splitData[0].Split('x')[0]),
                                        int.Parse(splitData[0].Split('x')[1])),
                                splitData[1][0] == '#' ? defaultOrigin :
                                    new Vector2(
                                        int.Parse(splitData[1].Split(':')[0]),
                                        int.Parse(splitData[1].Split(':')[1])),
                                splitData[2][0] == '#' ? defaultTotalSprites :
                                    int.Parse(splitData[2]),
                                splitData[3][0] == '#' ? defaultFramesPerSprite :
                                    int.Parse(splitData[3]),
                                style,
                                spaceTakenOnSpritesheet));

                            spaceTakenOnSpritesheet += style == AnimationStyle.Horizontal ?
                                (splitData[0][0] == '#' ? (int)defaultSpriteSize.Y : int.Parse(splitData[0].Split('x')[1])) :
                                (splitData[0][0] == '#' ? (int)defaultSpriteSize.X : int.Parse(splitData[0].Split('x')[0]));
                            break;
                    }
                }


                // Get sprite sheet
                bufferedSpritesheet = LoadTexture($"{path}/spritesheet");
            }


            return (bufferedProperties, bufferedAnimations, bufferedSpritesheet);
        }
    }
}
