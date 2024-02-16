using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileIO_Concept.Content.Entity {


    internal class Entity {
        // Content grabbed from file
        private static Dictionary<DataHeader, string> _data;
        private static Texture2D _sprite = null;

        // Needed in case only part of the data is loaded
        private static bool dataLoadedSuccess;

        private Rectangle entity;
        
        public static Texture2D LoadedSprite {
            set {
                if (_sprite == null)
                    _sprite = value;
            }
        }

        public Entity(Vector2 position) {
            entity = new Rectangle(
                (int) position.X, 
                (int) position.Y, 
                _sprite.Width / 4,
                _sprite.Height / 4);
        }

        public static void Load(string classRootPath) {
            string dataPath = $"{classRootPath}data.txt";

            // For data reading
            StreamReader file = null;
            string fileDataRaw;
            string[] fileDataFormatted;

            // For data sorting
            Dictionary<string, string> dataBuffer = new Dictionary<string, string>();

            int line = 0; // For error logging purposes

            try {
                if (!File.Exists(dataPath)) // For error logging purposes
                    throw new FileNotFoundException();

                file = new StreamReader(dataPath);

                // Read file content
                // Error checking should be done here
                while ((fileDataRaw = file.ReadLine()) != null) {
                    line++;

                    if (fileDataRaw[0] == '/') // Ignore comments
                        continue;

                    // Check for improper line formatting
                    if (!fileDataRaw.Contains('=') ||
                        // Check for any file format errors that would prevent
                        // proper data formatting before this next line
                        (fileDataFormatted = fileDataRaw.Split('=')).Length != 2)
                            throw new MalformedLineException();

                    // Prepare data for sorting via enums
                    dataBuffer.Add(fileDataFormatted[0], fileDataFormatted[1]);
                }

                // Check to see if the right amount of data was loaded
                if (dataBuffer.Count != Enum.GetNames(typeof(DataHeader)).Length)
                    throw new FileLoadException();

                // Store read data via its respective enum
                foreach (DataHeader header in Enum.GetValues(typeof(DataHeader)))
                    _data.Add(
                        header,
                        dataBuffer[header.ToString()]);


                dataLoadedSuccess = true;
            }
            // Add debug console call for FileNotFoundException
            catch {

                dataLoadedSuccess = false;
            }
            finally {
                file?.Close(); // Closes file if successfully opened
            }
        }


        public void Draw(SpriteBatch sb) {
            sb.Draw(
                _sprite,
                entity,
                Color.White);
        }

        private enum DataHeader {
            value_int,
            value_string
        }
    }
    
}
