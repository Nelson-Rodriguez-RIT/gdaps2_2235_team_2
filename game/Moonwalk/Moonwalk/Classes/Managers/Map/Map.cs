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

        private List<int[][]> _tiles;
        private List<Rectangle> _geometry;
        private Dictionary<int, Texture2D> _sprites;

        public Map(
                string rootDirectory,
                Vector2 position,
                bool loadImmediatly = true) {
            this.rootDirectory = rootDirectory;

            if (loadImmediatly)
                Load();
        }


        public void Load() {
            // Get tile data
            Object[] data = new Object[2];
            data = (Object[]) Loader.ProcessFileData(
                rootDirectory,
                "tmx",
                new Func<List<List<string>>, Object>(DecodeFileRules));

            _tiles = (List<int[][]>) data[0];
            _geometry = (List<Rectangle>) data[1];

            // Get sprites and their associated IDs
            _sprites = (Dictionary<int, Texture2D>) Loader.ProcessSpriteData(
                $"{rootDirectory}/sprites",
                new Func<ContentManager, List<string>, Object>(FormatSpriteRules));
        }

        private Object DecodeFileRules(List<List<string>> fileData) {
            Object[] bufferedData = new Object[2];

            // Data parsing fields
            Queue<string> dataQueue;
            MatchCollection rawData;
            List<string> data;
            string header;
            string body;

            foreach (List<string> file in fileData) { // Unneccesary foreach loop fyi
                bufferedData[0] = new List<int[][]>();
                bufferedData[1] = new List<Rectangle>();

                dataQueue = new Queue<string>(file);

                // First line contains tmx file formating information (irrelevant)
                dataQueue.Dequeue();

                // Get general map information
                rawData = Regex.Matches(dataQueue.Dequeue(), @"[.\d]+");
                data = rawData.Cast<Match>().Select(match => match.Value).ToList(); // dark magic


                dataQueue.Dequeue(); // Third line contains irrelevant data

                // Parse file data
                while(dataQueue.Count != 0) {
                    header = dataQueue.Dequeue();

                    // Get tile information
                    if (header.Contains("<layer")) {
                        dataQueue.Dequeue(); // Ignore tmx file formatting

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

                        ((List<int[][]>) bufferedData[0]).Add(tiles);
                        dataQueue.Dequeue();
                    }

                    // Get geometry information
                    else if (header.Contains("<objectgroup")) {
                        // I will do this later (probably ;P) - Nelson
                    }

                }
            }

            //You can return this instead - Dante
            Tuple<List<int[][]>, List<Rectangle>> changeThisName = new 
                Tuple<List<int[][]>, List<Rectangle>>
                (
                    (List<int[][]>)bufferedData[0],
                    (List<Rectangle>)bufferedData[1]
                );

            return bufferedData;
        }

        // Can this be changed to return a Dictionary<int, Texture2D>? - Dante
        private Object FormatSpriteRules(ContentManager _content, List<string> paths) {
            Dictionary<int, Texture2D> bufferedData = new();

            foreach (string path in paths) {
                string filename = path
                        .Remove(0, ($"{rootDirectory}/sprites\\\\").Length - 1)
                        .Split('.')[0];

                bufferedData.Add(
                    int.Parse(filename),
                    _content.Load<Texture2D>($"{rootDirectory}/sprites\\\\{filename}"));
            }
                

            return bufferedData;
        }

        public void Draw(SpriteBatch sb) {
            foreach (int[][] tiles in _tiles)
                for (int row = 0; row < tiles.Length; row++)
                    for (int col = 0; col < tiles[row].Length; col++) {
                        if (tiles[row][col] == 0)
                            continue;

                        sb.Draw(
                            _sprites[tiles[row][col]],
                            new Vector2(col * 25, row * 25),
                            Color.White);
                    }
                        
        }
    }
}
