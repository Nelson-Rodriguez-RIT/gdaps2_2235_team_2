using FileIO_Concept.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapLoading_Concept.Classes {
    enum Maps {
        Test,
    }

    internal static class Map {
        private static List<MapBlock> _data = null;

        private static Maps activeMap;

        private static ContentManager content = null;


        public static ContentManager Content {
            set {
                if (content == null) // Only should set it once
                    content = value;
            }
        }


        public static void Load(Maps map) {
            if (_data == null)
                _data = new List<MapBlock>();

            activeMap = map;

            // Get file data
            FileGroup _file = new(
                $"../../../Content/Maps/{activeMap.ToString()}/",
                "map.dat",
                new Func<Queue<string>, Object>(Ruleset));

            // Polymorphism my beloved
            _data = (List<MapBlock>)_file.Data;
        }
        
        /* 
            File Format

            [<BlockName>]
            <x, y>
            <background_image_path>
            <foreground_image_path>
            <x, y, width, height>
        */

        private static Object Ruleset(Queue<string> dataBlock) {
            List<MapBlock> bufferedData = new();

            string line;
            string[] formattedLine;

            // Allows use to get the position in a timly manner
            // Probably could simplify this
            Func<string, Vector2> getPosition = (cords) => {
                string[] formattedCords = cords.Split(',');
                return new Vector2(
                    int.Parse(formattedCords[0]),
                    int.Parse(formattedCords[1]));
            };

            while (dataBlock.Count > 0) {
                // Set up Map block
                MapBlock block = new(
                    dataBlock.Dequeue(),                // Name
                    getPosition(dataBlock.Dequeue()),   // Position
                    content.Load<Texture2D>(            // Background image
                        $"../../../Content/Maps/{activeMap.ToString()}/{dataBlock.Dequeue()}"),
                    content.Load<Texture2D>(            // Foreground image
                        $"../../../Content/Maps/{activeMap.ToString()}/{dataBlock.Dequeue()}"));

                // Add collision related geometry
                while (
                        (line = dataBlock.Dequeue()) != "" &&
                        dataBlock.Count > 0) {
                    formattedLine = line.Split(',');
                    block.Add(new Rectangle(
                        int.Parse(formattedLine[0]),    // x
                        int.Parse(formattedLine[1]),    // y
                        int.Parse(formattedLine[2]),    // width
                        int.Parse(formattedLine[3])));  // height
                }

                bufferedData.Add(block);
            }

            return bufferedData;
        }

        public static void Draw(SpriteBatch sb) {
            if (_data != null)
                foreach (MapBlock block in _data)
                    block.Draw(sb);
        }
    }
}
