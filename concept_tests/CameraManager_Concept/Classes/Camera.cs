using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CameraManager_Concept.Classes {
    internal static class Camera {

        // What the camera is focusing on
        private static Rectangle rectTarget;        // i.e. the player
        private static Vector2 vectorTarget;        // Invisable map geometry for a boss fight

        private static bool focusStatic;

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


        public static Rectangle ApplyOffset(Rectangle position) {
            //if (focusStatic)
            //    return new Rectangle(
            //       position.X + (int) (vectorTarget.X / 2),
            //       position.Y + (int) (vectorTarget.Y / 2),
            //       position.Width,
            //       position.Height);
            //else
            // Doesn't want to work. Ill figure it out soon - Nelson
                return new Rectangle(
                        position.X - rectTarget.X + (rectTarget.X / 2) + (rectTarget.Width / 2),
                        position.Y - rectTarget.Y + (rectTarget.Y / 2) + (rectTarget.Height / 2),
                        position.Width,
                        position.Height);
        }

        public static Rectangle ApplyOffset(Rectangle position, Vector2 offset) {
            if (focusStatic)
                return new Rectangle(
                  (position.X + (int)offset.X) + (int)(vectorTarget.X / 2),
                  (position.Y + (int)offset.Y) + (int)(vectorTarget.Y / 2),
                   position.Width,
                   position.Height);
            else
                return new Rectangle(
                (position.X + (int) offset.X) + (rectTarget.X / 2) + (rectTarget.Width / 2),
                (position.Y + (int) offset.Y) + (rectTarget.Y / 2) + (rectTarget.Height / 2),
                position.Width,
                position.Height
                );
        }


        public static Vector2 ApplyOffset(Vector2 position) {
            if (focusStatic)
                return new Vector2(
                    position.X + (vectorTarget.X / 2),
                    position.Y + (vectorTarget.Y / 2));
            else
                return new Vector2(
                    position.X + (rectTarget.X / 2),
                    position.Y + (rectTarget.Y / 2));
        }

        public static Vector2 ApplyOffset(Vector2 position, Vector2 offset) {
            if (focusStatic)
                return new Vector2(
                    (position.X + offset.X) + (vectorTarget.X / 2),
                    (position.Y + offset.Y) + (vectorTarget.Y / 2));
            else
                return new Vector2(
                    (position.X + offset.X) + (rectTarget.X / 2),
                    (position.Y + offset.Y) + (rectTarget.Y / 2));

        }
    }


}
