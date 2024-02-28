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
        private string directory;

        private List<int[][]> _tiles;
        private List<Rectangle> geometry;

        private Dictionary<int, Texture2D> sprites;
        private Vector2 tileSize;


        public Map() {
            // In the middle of refactoring this class - Nelson
        }


        public void Draw(SpriteBatch batch, Vector2 globalScale) {

        }

        
    }
}
