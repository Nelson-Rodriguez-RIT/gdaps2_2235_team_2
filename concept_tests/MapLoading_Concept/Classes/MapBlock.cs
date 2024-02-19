using MapLoading_Concept.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MapLoading_Concept.Classes {
    internal class MapBlock {
        private string name;

        private Texture2D background;
        private Texture2D foreground;

        private Vector2 position;

        private List<Rectangle> geometry;



        public MapBlock(
                string name,
                Vector2 position,
                Texture2D background,
                Texture2D foreground) {
            this.position = position;
            this.name = name;
            this.background = background;
            this.foreground = foreground;

            geometry = new List<Rectangle>();
        }


        public void Add(Rectangle element) {
            geometry.Add(element);
        }

        public void Draw(SpriteBatch sb) {
            sb.Draw(
                background,
                position,
                Color.White);

            sb.Draw(
                foreground,
                position,
                Color.White);

        }
    }
}
