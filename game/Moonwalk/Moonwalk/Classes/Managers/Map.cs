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

        private static Dictionary<int, Texture2D> sprites;
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
                Dictionary<int, Texture2D> sprites, Vector2 tileSize) bufferedData
                = Loader.LoadMap($"{RootDirectory}{mapRootFolderName}/");

            tiles = bufferedData.tiles;
            geometry = bufferedData.geometry;

            sprites = bufferedData.sprites;
            tileSize = bufferedData.tileSize;
        }


        public static void Draw(SpriteBatch batch, Vector2 globalScale)
        {
            foreach (int[][] tiles in tiles) // This is for rendering several layers
                for (int row = 0; row < tiles.Length; row++)
                    for (int col = 0; col < tiles[row].Length; col++)
                    {
                        // 0's are empty space
                        if (tiles[row][col] == 0)
                            continue;

                        // Draw the relevant map tile
                        batch.Draw(
                            sprites[tiles[row][col]],  // Uses tile ID to get a specific sprite
                            Camera.ApplyOffset(new Vector2(col * tileSize.X, row * tileSize.Y)) * globalScale,  // Position, with relevant offseets
                            null,           // Unused since we don't plan using sprite sheets for map tiles
                            Color.White,    // Color
                            0f,             // Rotation
                            Vector2.Zero,   // Origin
                            globalScale,    // Image scale, affected by the initial default scale and the window size ration
                            SpriteEffects.None, // Image flipping
                            0);             // Layer
                    }
        }


    }
}
