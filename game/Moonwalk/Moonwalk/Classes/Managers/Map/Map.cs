using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers.Map {
    internal class Map {
        private string rootDirectory;

        // List of each layer, with their individual tiles, found inside a tml file
        private List<int[][]> _tiles;

        // The size of each tile
        private Vector2 _tileSize;

        // List of geometry found inside a tml file
        private List<Rectangle> _geometry;

        // Sprite file names are turned into IDs. IDs are based on how they appear in the tml file.
        // To convert from editor to file, take its ID and add 1 (since 0 are used for empty tiles)
        private Dictionary<int, Texture2D> _sprites;

        /// <summary>
        /// A map instance with its own sprites, layers, and geometry. All of which is gathered from file
        /// </summary>
        /// <param name="rootDirectory">Root directory that contains this map's related sprites and tml file</param>
        public Map(
                string rootDirectory,
                Vector2 position,
                bool loadImmediatly = true) {
            this.rootDirectory = rootDirectory;

            if (loadImmediatly)
                Load();
        }


        /// <summary>
        /// Prepares to read data from file and store read data in a proper field
        /// </summary>
        public void Load() {
            // Get tile and geometry data
            Tuple<List<int[][]>, List<Rectangle>> data = new(
              new List<int[][]>(),    // Stores tile IDs
              new List<Rectangle>()); // Stores geometry (i.e. walls, floors, or any sort of collision)

            data = (Tuple<List<int[][]>, List<Rectangle>>) Loader.ProcessFileData(
                rootDirectory,
                "tmx",  // We argeed on using the program "Tiled" which provide use with tml files
                new Func<List<List<string>>, Object>(FormatFileRules));

            // Store tile and geometry data
            _tiles = data.Item1;
            _geometry = data.Item2;


            // Get sprites and their associated IDs
            _sprites = (Dictionary<int, Texture2D>) Loader.ProcessSpriteData(
                $"{rootDirectory}/sprites",
                new Func<ContentManager, List<string>, Object>(FormatSpriteRules));
        }

        /// <summary>
        /// How to interpret and store file read data
        /// </summary>
        /// <param name="fileData">Raw data read from file</param>
        /// <returns>A relevant file data structure that was already predetermined in Load()</returns>
        private Object FormatFileRules(List<List<string>> fileData) {
            Object[] bufferedData = new Object[2];      // Will be converted to a Tuple when returned
            bufferedData[0] = new List<int[][]>();      // Tile data
            bufferedData[1] = new List<Rectangle>();    // Geometry data

            Queue<string> dataQueue;    // A queue containing each line of file data
            MatchCollection rawData;    // Used to extract number values
            List<string> data;          // Used to store extracted number values

            string header;              // Header is data related to tml formatting, and will not contain tile or geometry data
            string body;                // Body is data directly related to tile layout or geometry

            List<string> file = fileData[0]; // Should only contain 1 file to begin with


            // Start parsing data
            dataQueue = new Queue<string>(file);

            // First line contains tmx file formating information (irrelevant)
            dataQueue.Dequeue();

            // Get general map information
            // Specifically we want tilewidth and tileheight
            rawData = Regex.Matches(dataQueue.Dequeue(), @"[.\d]+");
            data = rawData.Cast<Match>().Select(match => match.Value).ToList();

            // This data we want is the 4th and 5th numerical values for that line of data
            _tileSize = new Vector2(
                int.Parse(data[4]),     // Tile width
                int.Parse(data[5]));    // Tile height


            dataQueue.Dequeue(); // Third line contains irrelevant data
            

            // Begin reading relevant data
            while (dataQueue.Count != 0) { // Parse header
                header = dataQueue.Dequeue();

                // Get tile information
                if (header.Contains("<layer")) {
                    dataQueue.Dequeue(); // Ignore tmx specific file formatting

                    // Begin reading tile data
                    List<int[]> tilesList = new();
                    while (!(body = dataQueue.Dequeue()).Contains("</data>")) {
                        rawData = Regex.Matches(body, @"[.\d]+"); // Gets only the numbers
                        data = rawData.Cast<Match>().Select(match => match.Value).ToList();

                        // Format data to be stored in an int[]
                        int[] tileRow = new int[data.Count];
                        for (int i = 0; i < tileRow.Length; i++)
                            tileRow[i] = int.Parse(data[i]);

                        tilesList.Add(tileRow);
                    }

                    // Format data to an int[][]
                    int[][] tiles = new int[tilesList.Count][];
                    for (int i = 0; i < tiles.Length; i++)
                        tiles[i] = tilesList[i];

                    ((List<int[][]>)bufferedData[0]).Add(tiles);
                    dataQueue.Dequeue();
                }

                // Get geometry information
                else if (header.Contains("<objectgroup")) {

                    List<Rectangle> geometry = new();
                    while (!(body = dataQueue.Dequeue()).Contains("</objectgroup>")) {
                        rawData = Regex.Matches(body, @"[.\d]+");
                        data = rawData.Cast<Match>().Select(match => match.Value).ToList();

                        // We care for the 2nd, 3rd, 4th, and 5th numeric values in the file
                        geometry.Add(new Rectangle(
                            int.Parse(data[2]),     // Collision's relative X position
                            int.Parse(data[3]),     // Collision's relative Y position
                            int.Parse(data[4]),     // Collision's width
                            int.Parse(data[5])      // Collision's height
                            ));
                    }

                    bufferedData[1] = geometry;
                }

            }

            return new Tuple<List<int[][]>, List<Rectangle>>(
                (List<int[][]>) bufferedData[0],
                (List<Rectangle>) bufferedData[1]);
        }

        // Can this be changed to return a Dictionary<int, Texture2D>? - Dante

        // We could for the moment since our current use case is only this, but my initial
        // thoughts of the Loader class is to contain universal functionality,
        // regardless of the specific use case, hence the object return type - Nelson

        /// <summary>
        /// How to interpret and store sprite data
        /// </summary>
        /// <param name="paths">File paths to sprite images</param>
        /// <returns>A relevant sprite data structure that was already predetermined in Load()</returns>
        private Object FormatSpriteRules(ContentManager _content, List<string> paths) {
            Dictionary<int, Texture2D> bufferedData = new();

            foreach (string path in paths) { // For each image found...
                string filename = path  // Uses the file name (without the png) as the ID
                        .Remove(0, ($"{rootDirectory}/sprites\\\\").Length - 1)
                        .Split('.')[0];

                bufferedData.Add(   // Stores Texture2Ds with their associated ID
                    int.Parse(filename),
                    _content.Load<Texture2D>($"{rootDirectory}/sprites\\\\{filename}"));
            }
                
            return bufferedData;
        }

        /// <summary>
        /// Draw this maps tiles
        /// </summary>
        public void Draw(SpriteBatch sb, Vector2 globalScale) {
            foreach (int[][] tiles in _tiles) // This is for rendering several layers
                for (int row = 0; row < tiles.Length; row++)
                    for (int col = 0; col < tiles[row].Length; col++) {
                        // 0's are empty space
                        if (tiles[row][col] == 0)
                            continue;

                        // Draw the relevant map tile
                        sb.Draw(
                            _sprites[tiles[row][col]],  // Uses tile ID to get a specific sprite
                            Camera.ApplyOffset(new Vector2(col * _tileSize.X, row * _tileSize.Y)) * globalScale,  // Position, with relevant offseets
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
