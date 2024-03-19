using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers
{
    internal class Map
    {
        private const string RootDirectory = "../../../Content/Maps/";

        private static List<int[][]> tiles;
        private static List<Terrain> geometry;

        //private static Dictionary<int, Texture2D> sprites;
        private static Texture2D spritesheet;

        private static Vector2 tileSize;

        public static List<Terrain> Geometry 
        { 
            get
            {
                return geometry;
            } 
        }

        public static void LoadMap(string mapRootFolderName)
        {
            (List<int[][]> tiles, List<Terrain> geometry,
                Texture2D spritesheet, Vector2 tileSize) bufferedData
                = Loader.LoadMap($"{RootDirectory}{mapRootFolderName}/");

            tiles = bufferedData.tiles;
            geometry = bufferedData.geometry;

            //REMOVE THIS LATER (for testing purposes) - Dante
            geometry.Add(new Terrain(new Rectangle(0, 500, 1000, 100)));

            spritesheet = bufferedData.spritesheet;
            tileSize = bufferedData.tileSize;
        }


        public static void Draw(SpriteBatch batch)
        {
            foreach (int[][] tiles in tiles) // This is for rendering several layers
                for (int row = 0; row < tiles.Length; row++)
                    for (int col = 0; col < tiles[row].Length; col++)
                    {
                        // 0's are empty space
                        if (tiles[row][col] == 0)
                            continue;

                        Rectangle sprite = new Rectangle(
                                (int)((tiles[row][col] % (spritesheet.Width / tileSize.X) - 1) * tileSize.X),
                                (int)(Math.Floor(tiles[row][col] / (spritesheet.Height / tileSize.Y) - 1) * tileSize.Y),
                                (int)tileSize.X,
                                (int)tileSize.Y
                                );

                        // Draw the relevant map tile
                        batch.Draw(
                            //sprites[tiles[row][col]],  // Uses tile ID to get a specific sprite
                            spritesheet,
                            Camera.ApplyOffset(new Vector2(col * tileSize.X, row * tileSize.Y)) * GameMain.ActiveScale,  // Position, with relevant offseets
                            //new Vector2(col * tileSize.X, row * tileSize.Y),
                            //null,           // Unused since we don't plan using sprite sheets for map tiles
                            sprite,
                            Color.White,    // Color
                            0f,             // Rotation
                            //Camera.VectorTarget * globalScale,   // Origin
                            Vector2.Zero,
                            GameMain.ActiveScale,    // Image scale, DO NOT USE THIS
                            SpriteEffects.None, // Image flipping
                            0);             // Layer
                    }
        }


    }
}
