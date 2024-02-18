using FileIO_Concept.Classes;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileIO_Concept.Content.Entity {

    // IMPORTANT ! ! !
    // Anything labled as required is needed for my FileIO system to work

    internal class Entity {
        protected static Dictionary<string, string> _data; // Required 


        protected static Texture2D sprite;

        protected Rectangle entity;

        public static string SpritePath { 
            get { 
                if (_data != null)
                    return _data["sprite_load_path"];

                return "<DATA NOT LOADED>";
            } 
        }
        public static Texture2D Sprite {
            get { return sprite; }
            set {
                if (sprite == null)
                    sprite = value;
            }
        }

        public Entity(Vector2 position) {
            entity = new Rectangle(
                (int) position.X, 
                (int) position.Y, 
                sprite.Width / 4,
                sprite.Height / 4);
        }

        public static void Load() { // Required 
            // Ensure data isn't already loaded
            if (_data != null)
                return;

            // Get file data
            FileGroup _file = new(
                "../../../Content/Entity/",
                "data.dat",
                new Func<List<string>, Object>(Ruleset));

            // Polymorphism my beloved
            _data = (Dictionary<string, string>)_file.Data;
        }

        protected static Object Ruleset(List<string> dataBlock) { // Required, but you can custimize it
            // Best practice would be to include checks for
            // improper file formatting, but this is a demo
            // so ill ignored them for now :)
            Dictionary<string, string> bufferedData = new();
            foreach (string line in dataBlock) { // Go through file data and apply necceesary logic
                if (line[0] == '/') // Ignore comments
                    continue;

                string[] formatedLine = line.Split('=');
                bufferedData.Add(
                    formatedLine[0],    // Header
                    formatedLine[1]);   // Body
            }

            return bufferedData;
        }

        public void Draw(SpriteBatch sb) {
            sb.Draw(
                sprite,
                entity,
                Color.White);
        }

    }
    
}
