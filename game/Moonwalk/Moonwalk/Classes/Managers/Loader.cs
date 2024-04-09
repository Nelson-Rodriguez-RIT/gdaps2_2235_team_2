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
using Moonwalk.Classes.Maps;
using Moonwalk.Classes;
using System.Reflection;
using Moonwalk.Classes.Entities;

namespace Moonwalk.Classes.Managers
{
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
        public static MapData LoadMap(string path) {
            List<int[][]> bufferedTiles = new();
            Assortment<Terrain> bufferedGeometry = new Assortment<Terrain>(new List<Type>(){ typeof(Terrain), typeof(MapTrigger), typeof(OutOfBounds) });
            Texture2D bufferedSpritesheet;
            Vector2 bufferdTileSize = new Vector2();


            // MDF file reading //
            Queue<string> fileData = new();
            string data;
            string[] dataBlocks;
            string[] buffer;

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
                        dataBlocks = data.Split(',');            // Prepare data to be read
                        int[] row = new int[dataBlocks.Length];  // Properly size storage

                        for (int i = 0; i < row.Length - 1; i++) // Reading data
                            row[i] = int.Parse(dataBlocks[i]);

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
                    dataBlocks = data.Split("=");
                    buffer = dataBlocks[1].Split(",");

                    switch (dataBlocks[0].ToUpper()) {
                        // Information about the width and height of an individual tile
                        case "SIZE":
                            bufferdTileSize = new Vector2(
                                int.Parse(buffer[0]),    // Width
                                int.Parse(buffer[1])     // Height
                                );
                            break;

                        // Represents the most basic form of collision
                        // TMX to MDF converter looks at the name of the object layer
                        // when rewriting relevant information to file
                        case "TERRAIN":
                            bufferedGeometry.Add(new Terrain(new Rectangle(
                                int.Parse(buffer[0]),    // X
                                int.Parse(buffer[1]),    // Y
                                int.Parse(buffer[2]),    // Width
                                int.Parse(buffer[3])     // Height
                                )));
                            break;

                        // Loads a map upon collision
                        case "MAPTRIGGER":
                            bufferedGeometry.Add(
                                new MapTrigger(
                                    new Rectangle(
                                        int.Parse(buffer[1]),   // X
                                        int.Parse(buffer[2]),   // Y
                                        int.Parse(buffer[3]),   // Width
                                        int.Parse(buffer[4])    // Height
                                        ),
                                    buffer[0]                   // Map to load
                                    )
                                );
                            break;
                        case "CHECKPOINT":
                            bufferedGeometry.Add(
                                new Checkpoint(
                                    new Rectangle(
                                        int.Parse(buffer[1]),   // X
                                        int.Parse(buffer[2]),   // Y
                                        int.Parse(buffer[3]),   // Width
                                        int.Parse(buffer[4])    // Height
                                        )
                                    )
                                );
                            break;
                        case "OUTOFBOUNDS":
                            bufferedGeometry.Add(
                                new OutOfBounds(
                                    new Rectangle(
                                        int.Parse(buffer[0]),
                                        int.Parse(buffer[1]),
                                        int.Parse(buffer[2]),
                                        int.Parse(buffer[3])
                                        )
                                    )
                                );
                            break;
                        default: //for enemies
                            //Get the type of the entity
                            string typeString = dataBlocks[0][0].ToString().ToUpper() + dataBlocks[0].Substring(1);
                            Type type = Type.GetType( "Moonwalk.Classes.Entities." + typeString );
                            if (type == null ||
                                !type.IsAssignableTo(typeof(Entity)))
                            {
                                break;
                            }

                            //Get the spawnentity method and invoke it with the type we just defined
                            MethodInfo spawn = typeof(GameManager).GetMethod("SpawnEntity");
                            MethodInfo spawnGeneric = spawn.MakeGenericMethod(type);
                            spawnGeneric.Invoke(null, new Object[] 
                                { int.Parse(buffer[0]), int.Parse(buffer[1]) });


                            break;
                    }
                }
            }

            // Get sprite data
            bufferedSpritesheet = content.Load<Texture2D>($"{path}spritesheet");


            return new MapData(bufferedTiles, bufferedGeometry,
                    bufferedSpritesheet, bufferdTileSize);
        }
        
        /// <summary>
        /// Loads an entity's data from both their EDF and ADF files
        /// </summary>
        /// <param name="path">Path to related file contents</param>
        /// <param name="loadAnimations">Whether or not this entity has animations to be loaded</param>
        public static EntityData LoadEntity(string path, bool loadAnimations = true, bool loadProperties = true) {
            Dictionary<string, string> bufferedProperties = new();
            List<Animation> bufferedAnimations = new();
            Texture2D bufferedSpritesheet = null;

            Queue<string> fileData;
            string data;
            string[] dataBlocks;

            if (!loadProperties)
                goto animations; // Skips properties loading 

            // Entity Data File Reading //
            // Get and process properties file data
            fileData = new Queue<string>(LoadFile($"{path}/", ".edf"));
            while (fileData.Count != 0) {
                data = fileData.Dequeue();

                // Ignore comments and empty lines
                if (data == null || data[0] == '/')
                    continue;

                // Break data into its header and body components
                dataBlocks = data.Split('=');

                // Format and store data
                bufferedProperties.Add(dataBlocks[0], dataBlocks[1]);
            }


            // Animation Data File Reading //
            animations:
            if (!loadAnimations) // Used for entities not meant to have visible sprites
                return new EntityData(bufferedProperties, bufferedAnimations, bufferedSpritesheet);

            // Defaults
            AnimationStyle style = AnimationStyle.Horizontal;
            Vector2 defaultSpriteSize = Vector2.One;
            Vector2 defaultOrigin = Vector2.Zero;
            int defaultTotalSprites = 1;
            int defaultFramesPerSprite = 1;

            // This allows use to have custom Widths/Heights for each animation since this
            // will inform the Animation where it should get its data from. This will be
            // incremented by either a Width (for Vertical styles) or Height (for Horizontal styles)
            int spaceTakenOnSpritesheet = 0;

            // Get and process animation file data
            fileData = new Queue<string>(LoadFile($"{path}/", ".adf"));
            while (fileData.Count != 0) {
                data = fileData.Dequeue();

                // Ignore comments and empty lines
                if (data == "" || data[0] == '/')
                    continue;

                switch (data.First()) {
                    // Determines the direction, in terms of sprite sheet layout, where
                    // following sprites of the same animation are positioned
                    case '&':
                        style = data.Contains("Vertical") ? AnimationStyle.Vertical : AnimationStyle.Horizontal;
                        break;

                    // Sets default values. These are used in order to reduce the amount of repeative type
                    // and reduces the likelyhood that incorrect data will be used for animations
                    case '@':
                        dataBlocks = data.Remove(0, 1).Split(',');
                        string[] buffer;

                        buffer = dataBlocks[0].Split('x');
                        defaultSpriteSize = new Vector2(
                            int.Parse(buffer[0]),
                            int.Parse(buffer[1])
                            );

                        buffer = dataBlocks[1].Split(':');
                        defaultOrigin = new Vector2(
                            int.Parse(buffer[0]),
                            int.Parse(buffer[1])
                            );

                        defaultFramesPerSprite = int.Parse(dataBlocks[2]);
                        defaultTotalSprites = int.Parse(dataBlocks[3]);
                        break;

                    // Creates an animation out of the data provided on this line
                    // If the character # is found, default data will be used
                    default:
                        // Process file data
                        dataBlocks = data.Split(',');

                        buffer = dataBlocks[0] == "#" ? null : dataBlocks[0].Split('x');
                        Vector2 size = buffer == null ? defaultSpriteSize : 
                            new Vector2(int.Parse(buffer[0]), int.Parse(buffer[1]));

                        buffer = dataBlocks[1] == "#" ? null : dataBlocks[1].Split(':');
                        Vector2 origin = buffer == null ? defaultOrigin :
                            new Vector2(int.Parse(buffer[0]), int.Parse(buffer[1]));

                        int totalSprites = dataBlocks[2] == "#" 
                            ? defaultTotalSprites : int.Parse(dataBlocks[2]);

                        int framesPerSprite = dataBlocks[3] == "#"
                            ? defaultFramesPerSprite : int.Parse(dataBlocks[3]);

                        // Create animation
                        bufferedAnimations.Add(new Animation(
                            size, origin, totalSprites, framesPerSprite,
                            style, spaceTakenOnSpritesheet));


                        spaceTakenOnSpritesheet +=
                            (int)(style == AnimationStyle.Horizontal ? size.Y : size.X);
                        break;
                }
            }

            // Get sprite sheet
            bufferedSpritesheet = LoadTexture($"{path}/spritesheet");

            return new EntityData(bufferedProperties, bufferedAnimations, bufferedSpritesheet);
        }

        /// <summary>
        /// Loads an entity's data from both their EDF and ADF files
        /// </summary>
        /// <param name="path">Path to related file contents</param>
        /// <param name="loadAnimations">Whether or not this entity has animations to be loaded</param>
        public static Moonwalk.Classes.Boss.BossFight.BossData LoadBoss(string path, bool loadAnimations = true, bool loadProperties = true)
        {
            Dictionary<string, string> bufferedProperties = new();
            List<Animation> bufferedAnimations = new();
            Texture2D bufferedSpritesheet = null;

            Queue<string> fileData;
            string data;
            string[] dataBlocks;

            if (!loadProperties)
                goto animations; // Skips properties loading 

            // Entity Data File Reading //
            // Get and process properties file data
            fileData = new Queue<string>(LoadFile($"{path}/", ".edf"));
            while (fileData.Count != 0)
            {
                data = fileData.Dequeue();

                // Ignore comments and empty lines
                if (data == null || data[0] == '/')
                    continue;

                // Break data into its header and body components
                dataBlocks = data.Split('=');

                // Format and store data
                bufferedProperties.Add(dataBlocks[0], dataBlocks[1]);
            }


        // Animation Data File Reading //
        animations:
            if (!loadAnimations) // Used for entities not meant to have visible sprites
                return new Moonwalk.Classes.Boss.BossFight.BossData(bufferedProperties, bufferedAnimations, bufferedSpritesheet);

            // Defaults
            AnimationStyle style = AnimationStyle.Horizontal;
            Vector2 defaultSpriteSize = Vector2.One;
            Vector2 defaultOrigin = Vector2.Zero;
            int defaultTotalSprites = 1;
            int defaultFramesPerSprite = 1;

            // This allows use to have custom Widths/Heights for each animation since this
            // will inform the Animation where it should get its data from. This will be
            // incremented by either a Width (for Vertical styles) or Height (for Horizontal styles)
            int spaceTakenOnSpritesheet = 0;

            // Get and process animation file data
            fileData = new Queue<string>(LoadFile($"{path}/", ".adf"));
            while (fileData.Count != 0)
            {
                data = fileData.Dequeue();

                // Ignore comments and empty lines
                if (data == "" || data[0] == '/')
                    continue;

                switch (data.First())
                {
                    // Determines the direction, in terms of sprite sheet layout, where
                    // following sprites of the same animation are positioned
                    case '&':
                        style = data.Contains("Vertical") ? AnimationStyle.Vertical : AnimationStyle.Horizontal;
                        break;

                    // Sets default values. These are used in order to reduce the amount of repeative type
                    // and reduces the likelyhood that incorrect data will be used for animations
                    case '@':
                        dataBlocks = data.Remove(0, 1).Split(',');
                        string[] buffer;

                        buffer = dataBlocks[0].Split('x');
                        defaultSpriteSize = new Vector2(
                            int.Parse(buffer[0]),
                            int.Parse(buffer[1])
                            );

                        buffer = dataBlocks[1].Split(':');
                        defaultOrigin = new Vector2(
                            int.Parse(buffer[0]),
                            int.Parse(buffer[1])
                            );

                        defaultFramesPerSprite = int.Parse(dataBlocks[2]);
                        defaultTotalSprites = int.Parse(dataBlocks[3]);
                        break;

                    // Creates an animation out of the data provided on this line
                    // If the character # is found, default data will be used
                    default:
                        // Process file data
                        dataBlocks = data.Split(',');

                        buffer = dataBlocks[0] == "#" ? null : dataBlocks[0].Split('x');
                        Vector2 size = buffer == null ? defaultSpriteSize :
                            new Vector2(int.Parse(buffer[0]), int.Parse(buffer[1]));

                        buffer = dataBlocks[1] == "#" ? null : dataBlocks[1].Split(':');
                        Vector2 origin = buffer == null ? defaultOrigin :
                            new Vector2(int.Parse(buffer[0]), int.Parse(buffer[1]));

                        int totalSprites = dataBlocks[2] == "#"
                            ? defaultTotalSprites : int.Parse(dataBlocks[2]);

                        int framesPerSprite = dataBlocks[3] == "#"
                            ? defaultFramesPerSprite : int.Parse(dataBlocks[3]);

                        // Create animation
                        bufferedAnimations.Add(new Animation(
                            size, origin, totalSprites, framesPerSprite,
                            style, spaceTakenOnSpritesheet));


                        spaceTakenOnSpritesheet +=
                            (int)(style == AnimationStyle.Horizontal ? size.Y : size.X);
                        break;
                }
            }

            // Get sprite sheet
            bufferedSpritesheet = LoadTexture($"{path}/spritesheet");

            return new Moonwalk.Classes.Boss.BossFight.BossData(bufferedProperties, bufferedAnimations, bufferedSpritesheet);
        }

        /// <summary>
        /// Loads a font
        /// </summary>
        /// <param name="fontName">File path (without file extension)</param>
        public static SpriteFont LoadFont(string fontName) {
            return content.Load<SpriteFont>(fontName);
        }
    }
}
