using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Maps;
using System;
using System.Collections.Generic;
using Moonwalk.Classes.Entities;

namespace Moonwalk.Classes.Managers
{
    internal static class Map {
        // Root folder for all maps
        private const string RootDirectory = "../../../Content/Maps/";
        private static string activeMapName;

        // Contains IDs that dictate what tile, and where, to display
        internal static List<int[][]> tiles = null; // Data read from MDF

        // Contains collision elements
        internal static Assortment<Terrain> geometry = null; // Data read from MDF

        // Sprite sheet the map will use. Each tile correlates to an ID
        // Starts at ID 1 and increases as it goes right (and down)
        // NOTICE: Make sure to use the SAME sprite sheet you used in Tiled
        // any inconsistencies will cause the map to be displayed improperly
        internal static Texture2D spritesheet;

        // Sprite to use when map collision debugging is turned on (F2)
        // This sprite should consist of a single pixel (it is scaled as needed)
        private static Texture2D hitboxSprite;

        // The wdith and height of an individual tile (usually 16x16)
        internal static Vector2 tileSize;

        public static bool Loaded {
            get { return tiles != null; }
        }

        public static string LoadedMapName {
            get { return activeMapName; }
        }

        public static Assortment<Terrain> Geometry {
            get { return geometry; }
        }

        /// <summary>
        /// Loads a map (and relevant file data)
        /// </summary>
        /// <param name="mapRootFolderName">Name of the map</param>
        public static void LoadMap(string mapRootFolderName, bool resetState = false) {
            // Get and load data from file
            MapData bufferedData = Loader.LoadMap($"{RootDirectory}{mapRootFolderName}/");
            bufferedData.Load();

            activeMapName = mapRootFolderName;

            // Load hitbox sprite if it hasn't already
            if (hitboxSprite == null)
                hitboxSprite = Loader.LoadTexture("../../../Content/Maps/hitbox");

            //testing, clear later
            
            //Checkpoint first = new Checkpoint(new Rectangle(200, -30, 20, 20)); 
            //for testing in demoMap, uncomment ^^, and comment next line 
            Checkpoint first = new Checkpoint(new Rectangle(288, 400, 20, 20));
            Player.MostRecentCheckpoint = first;
            geometry.Add(first);

            //GUI.AddElement(new GUITextElement(new Vector2(50, 50),
            //    "Remember to remove tests from Map.LoadMap",
            //    "File",
            //    Color.White));
        }

        /// <summary>
        /// Unloads a map (and relevant file data)
        /// </summary>
        public static void UnloadMap() {
            tiles = null;
            geometry = null;
            spritesheet = null;
            tileSize = Vector2.Zero;
        }

        public static void Draw(SpriteBatch batch, bool drawhitboxes) {
            // Renders each layer on top of the previous ones
            // Order is based on how they appear in the MDF
            foreach (int[][] tiles in tiles)
                for (int row = 0; row < tiles.Length; row++)
                    for (int col = 0; col < tiles[row].Length; col++) {
                        if (tiles[row][col] == 0) // Ignore empty space (ID of 0)
                            continue;

                        batch.Draw(
                            spritesheet,                                                                // Sprite sheet
                            Camera.RelativePosition(new Vector2(col * tileSize.X, row * tileSize.Y)),   // Position
                            new Rectangle(                                                              // Sprite from sprite sheet
                                (int)(((tiles[row][col] - 1) % (spritesheet.Width / tileSize.X)) * tileSize.X),
                                (int)(Math.Floor((tiles[row][col] - 1) / (spritesheet.Width / tileSize.X)) * tileSize.Y),
                                (int)tileSize.X,
                                (int)tileSize.Y
                                ),
                            Color.White,            // Color
                            0f,                     // Rotation (unused)
                            Vector2.Zero,           // Origin (unused)
                            GameMain.ActiveScale,   // Image scale
                            SpriteEffects.None,     // Image flipping (unused)
                            0);                     // Layer (unused)
                    }

            // Draw collision if toggled (via F2)
            if (drawhitboxes)
                foreach (Terrain terrain in geometry) {
                    Vector2 position = Camera.RelativePosition(terrain.Hitbox);

                    batch.Draw(
                        hitboxSprite,
                        new Rectangle(
                            (int)(position.X),
                            (int)(position.Y),
                            (int)(terrain.Hitbox.Width * GameMain.ActiveScale.X),
                            (int)(terrain.Hitbox.Height * GameMain.ActiveScale.Y)
                            ),
                        Color.LightGreen
                        );
                }
        }
    }

    // These are used to buffer information gotten from a map's .mdf file
    // They aren't necessary but they keep the code a bit more clean
    internal class MapData {
        // Buffers for file data
        private List<int[][]> bufferedTiles;
        private Assortment<Terrain> bufferedGeometry;
        private Texture2D bufferedSpritesheet;
        Vector2 bufferedTileSize;

        public MapData(List<int[][]> tiles, Assortment<Terrain> geometry, Texture2D spritesheet, Vector2 tileSize) {
            bufferedTiles = tiles;
            bufferedGeometry = geometry;
            bufferedSpritesheet = spritesheet;
            bufferedTileSize = tileSize;
        }

        // For loading buffered data
        public void Load() {
            Map.tiles = bufferedTiles;
            Map.geometry = bufferedGeometry;
            Map.spritesheet = bufferedSpritesheet;
            Map.tileSize = bufferedTileSize;
        }

        // Also why don't they want us defining multiple classes for a single file?
        // Seems really useful for small, locally relevant data structures
    }
}
