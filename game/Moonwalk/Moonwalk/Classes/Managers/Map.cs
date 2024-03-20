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

        private static List<int[][]> tilesSets;
        private static List<Terrain> geometry;

        //private static Dictionary<int, Texture2D> sprites;
        private static Texture2D spritesheet;

        protected static Texture2D hitboxSprite;

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

            tilesSets = bufferedData.tiles;
            geometry = bufferedData.geometry;

            //REMOVE THIS LATER (for testing purposes) - Dante
            geometry.Add(new Terrain(new Rectangle(0, 500, 1000, 100)));

            spritesheet = bufferedData.spritesheet;
            tileSize = bufferedData.tileSize;

            if (hitboxSprite == null)
                hitboxSprite = Loader.LoadTexture("../../../Content/Maps/hitbox");
        }


        public static void Draw(SpriteBatch batch, bool drawhitboxes)
        {
            foreach (int[][] tiles in tilesSets) // This is for rendering several layers
                for (int row = 0; row < tiles.Length; row++)
                    for (int col = 0; col < tiles[row].Length; col++)
                    {
                        // 0's are empty space
                        if (tiles[row][col] == 0)
                            continue;

                        int id = tiles[row][col] - 1;
                        int spriteX = (int)(id % (spritesheet.Width / tileSize.X));

                        int spriteY = (int)(Math.Floor(id / (spritesheet.Height / tileSize.Y)) - Math.Floor(tileSize.X / 16));

                        Rectangle sprite = new Rectangle(
                                (int)(spriteX * tileSize.X),
                                (int)(spriteY * tileSize.Y),
                                (int)tileSize.X,
                                (int)tileSize.Y
                                );

                        // Draw the relevant map tile
                        batch.Draw(
                            //sprites[tiles[row][col]],  // Uses tile ID to get a specific sprite
                            spritesheet,
                            Camera.ApplyOffset(new Vector2(col * tileSize.X, row * tileSize.Y)),  // Position, with relevant offseets
                            //new Vector2(col * tileSize.X, row * tileSize.Y),
                            //null,           // Unused since we don't plan using sprite sheets for map tiles
                            sprite,
                            Color.White,    // Color
                            0f,             // Rotation
                            //Camera.VectorTarget * globalScale,   // Origin
                            Vector2.Zero,
                            GameMain.ActiveScale,    // Image scale
                            SpriteEffects.None, // Image flipping
                            0);             // Layer
                    }

            if (drawhitboxes)
                foreach (Terrain terrain in geometry) {
                    Vector2 position = Camera.ApplyOffset(new Vector2(
                    terrain.Hitbox.X,
                    terrain.Hitbox.Y
                    ));

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
