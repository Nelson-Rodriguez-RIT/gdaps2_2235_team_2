using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileIO_Concept.Content.Entity {


    internal class Entity {
        protected const string _FilePath = $"Content/Entity/";

        // Content grabbed from file
        private static Dictionary<DataHeader, string> _data = null;
        private static Texture2D _sprite = null;

        private Rectangle entity;
        
        public static Texture2D LoadedSprite {
            set {
                if (_sprite == null)
                    _sprite = value;
            }
        }

        public static Dictionary<DataHeader, string> LoadedData {
            set {
                if (_data == null)
                    _data = value;
            }
        }

        public static Type DataHeaders
        {
            get { return typeof(DataHeader); }
        }

        public static string FilePath {
            get { return _FilePath; }
        }


        public Entity(Vector2 position) {
            entity = new Rectangle(
                (int) position.X, 
                (int) position.Y, 
                _sprite.Width / 4,
                _sprite.Height / 4);
        }




        public void Draw(SpriteBatch sb) {
            sb.Draw(
                _sprite,
                entity,
                Color.White);
        }

        enum DataHeader
        {
        value_int,
        value_string
        }
    }
    
}
