using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;

namespace Moonwalk.Classes.Managers {
    internal static class Camera {
        private static Vector2 globalOffset;

        // What the camera is focusing on
        private static Rectangle rectTarget;        // i.e. the player
        private static Vector2 vectorTarget;        // Invisable map geometry for a boss fight

        private static bool focusStatic;            // What ever is the most recent target set
                                                    // will be the focused Camera target

        public static Rectangle RectTarget {
            get { return rectTarget; }
            set {
                focusStatic = false;
                rectTarget = value; }
        }

        public static Vector2 VectorTarget {
            get { return vectorTarget; }
            set {
                focusStatic = true;
                vectorTarget = value; 
            }
        }

        public static Vector2 GlobalOffset {
            get { return globalOffset; }
            set { globalOffset = value; }
        }


        public static Rectangle ApplyOffset(Rectangle position) {
            return ApplyOffset(position, Vector2.Zero);
        }

        public static Rectangle ApplyOffset(Rectangle position, Vector2 offset) {
            if (focusStatic)
                return new Rectangle(
                   (position.X + (int)offset.X) - (int)vectorTarget.X + (int)globalOffset.X,
                   (position.Y + (int)offset.Y) - (int)vectorTarget.Y + (int)globalOffset.Y,
                   position.Width,
                   position.Height);
            else
                return new Rectangle(
                   (position.X + (int)offset.X) - rectTarget.X + (int)globalOffset.X,
                   (position.Y + (int)offset.Y) - rectTarget.Y + (int)globalOffset.Y,
                   position.Width,
                   position.Height);
        }


        public static Vector2 ApplyOffset(Vector2 position) {
            return ApplyOffset(position, Vector2.Zero);
        }

        public static Vector2 ApplyOffset(Vector2 position, Vector2 offset) {
            if (focusStatic)
                return new Vector2(
                    (position.X + offset.X) - vectorTarget.X + globalOffset.X,
                    (position.Y + offset.Y) - vectorTarget.Y + globalOffset.Y);
            else
                return new Vector2(
                    (position.X + offset.X) - rectTarget.X + globalOffset.X,
                    (position.Y + offset.Y) - rectTarget.Y + globalOffset.Y);

        }
    }


}
