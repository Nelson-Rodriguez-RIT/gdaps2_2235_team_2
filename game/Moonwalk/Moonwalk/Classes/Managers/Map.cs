using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Entities.Base;
using System;
using System.Collections.Generic;

namespace Moonwalk.Classes.Managers {
    internal class Map {
        // Root folder for all maps
        private const string RootDirectory = "../../../Content/Maps/";

        // Contains IDs that dictate what tile, and where, to display
        private static List<int[][]> tilesSets = null; // Data read from MDF

        // Contains collision elements
        private static List<Terrain> geometry = null; // Data read from MDF

        // Sprite sheet the map will use. Each tile correlates to an ID
        // Starts at ID 1 and increases as it goes right (and down)
        // NOTICE: Make sure to use the SAME sprite sheet you used in Tiled
        // any inconsistencies will cause the map to be displayed improperly
        private static Texture2D spritesheet;

        // Sprite to use when map collision debugging is turned on (F2)
        // This sprite should consist of a single pixel (it is scaled as needed)
        protected static Texture2D hitboxSprite;

        // The wdith and height of an individual tile (usually 16x16)
        private static Vector2 tileSize;

        public static bool Loaded {
            get { return tilesSets != null; }
        }

        public static List<Terrain> Geometry {
            get { return geometry; }
        }


        /// <summary>
        /// Loads a map (and relevant file data)
        /// </summary>
        /// <param name="mapRootFolderName">Name of the map</param>
        public static void LoadMap(string mapRootFolderName) {
            // Get data from file
            (List<int[][]> tiles, List<Terrain> geometry,
                Texture2D spritesheet, Vector2 tileSize) 
                bufferedData = Loader.LoadMap($"{RootDirectory}{mapRootFolderName}/");

            // Store data
            tilesSets = bufferedData.tiles;
            geometry = bufferedData.geometry;
            spritesheet = bufferedData.spritesheet;
            tileSize = bufferedData.tileSize;

            // Load hitbox sprite if it hasn't already
            if (hitboxSprite == null)
                hitboxSprite = Loader.LoadTexture("../../../Content/Maps/hitbox");
        }

        /// <summary>
        /// Unloads a map (and relevant file data)
        /// </summary>
        public static void UnloadMap() {
            tilesSets = null;
            geometry = null;
            spritesheet = null;
            tileSize = Vector2.Zero;
        }


        public static void Draw(SpriteBatch batch, bool drawhitboxes) {
            // Renders each layer on top of the previous ones
            // Order is based on how they appear in the MDF
            foreach (int[][] tiles in tilesSets)
                for (int row = 0; row < tiles.Length; row++)
                    for (int col = 0; col < tiles[row].Length; col++) {
                        // 0's are empty space
                        if (tiles[row][col] == 0)
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
                            Color.White,                                                                // Color
                            0f,                                                                         // Rotation (unused)
                            Vector2.Zero,                                                               // Origin (unused)
                            GameMain.ActiveScale,                                                       // Image scale
                            SpriteEffects.None,                                                         // Image flipping (unused)
                            0);                                                                         // Layer (unused)
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
                        Color.White
                        );
                }
        }
    }
}
