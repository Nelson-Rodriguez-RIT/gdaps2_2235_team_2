using Microsoft.Xna.Framework;
using Moonwalk.Classes.Entities.Base;

namespace Moonwalk.Classes.Managers {
    internal static class Camera {
        // Global position shift for all calls of RelativePosition()
        // Make this a vector point to where you want the camera to focus on
        // i.e. making this the center of the window will center your target
        private static Vector2 globalOffset = Vector2.Zero;

        // This is where the camera will focus. Will automatically be set if
        // targetEntity is set, otherwise it will manually need to bet set
        private static Vector2 targetPosition = Vector2.Zero; // Required (if targetEntity isn't set)

        // This is what the camera will focus on
        // i.e you would set reference to the player to continously follow the player
        private static Entity targetEntity = null; // Optional (but recommended for most cases)

        public static Vector2 Target
        {
            get { return targetEntity.Position.ToVector2(); }
        }

        public static Vector2 GlobalOffset {
            get { return globalOffset; }
            set { globalOffset = value; }
        }


        private static void UpdateTargetPosition() {
            if (targetEntity == null) // Ignore empty references
                return;

            targetPosition = new Vector2(targetEntity.hurtbox.Center.X, targetEntity.hurtbox.Center.Y);
        }


        /// <summary>
        /// Set a position for the camera to focus on.
        /// </summary>
        public static void SetTarget(Vector2 target) {
            targetPosition = target;
            targetEntity = null; // Reset this to prevent unwanted changes to targetPosition
        }

        /// <summary>
        /// Set an entity for the camera to focus on
        /// </summary>
        public static void SetTarget(Entity target) {
            if (target == null) // Ignore empty references
                return;

            targetEntity = target;

            // Setup targetPosition
            UpdateTargetPosition();
        }

        /// <summary>
        /// Takes a provided vector and makes it relative to the target's position.
        /// This will also take sprite scalings into account.
        /// </summary>
        public static Vector2 RelativePosition(Vector2 position) {
            UpdateTargetPosition();
            return (position - targetPosition) * GameMain.ActiveScale + globalOffset;
        }

        /// <summary>
        /// Takes a provided point and makes it relative to the target's position.
        /// This will also take sprite scalings into account.
        /// </summary>
        public static Vector2 RelativePosition(Point position) {
            UpdateTargetPosition();
            return (new Vector2(position.X, position.Y) - targetPosition) * GameMain.ActiveScale + globalOffset;
        }

        /// <summary>
        /// Takes a provided rectangle and makes it relative to the target's position.
        /// This will also take sprite scalings into account.
        /// </summary>
        public static Rectangle RelativePosition(Rectangle position) {
            UpdateTargetPosition();
            Vector2 newPos = (new Vector2(position.X, position.Y) - targetPosition) * GameMain.ActiveScale + globalOffset;
            return new Rectangle(newPos.ToPoint(), position.Size);
        }

        // (x - offset) / scale + targetPosition = position

        /// <summary>
        /// Inverse of RelativePosition
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 WorldToScreen(Vector2 position)
        {
            UpdateTargetPosition();
            return ((position - globalOffset) / GameMain.ActiveScale) + targetPosition;
        }
    }


}
