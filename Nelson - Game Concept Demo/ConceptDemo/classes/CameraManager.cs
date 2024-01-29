using ConceptDemo.classes.entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConceptDemo.classes {
    internal class CameraManager {
        private Vector2 absPosition;

        public Vector2 AbsPosition { get { return absPosition; } }


        /// <summary>
        /// Create a camera object at 0, 0
        /// </summary>
        public CameraManager() : this(new Vector2(0, 0)) {}

        /// <summary>
        /// Create a camera object at a specified position
        /// </summary>
        /// <param name="initialPosition">Position to spawn camera at</param>
        public CameraManager(Vector2 initialPosition)
        {
            absPosition = new Vector2(initialPosition.X, initialPosition.Y);
        }


        /// <summary>
        /// Gets the position of a provided entity absPosition relative to the camera
        /// </summary>
        /// <returns>Position relative to the camera</returns>
        public virtual Vector2 GetRelativePosition(Vector2 entityPosition) {

            float distanceFromX = entityPosition.X - absPosition.X;
            float distanceFromY = entityPosition.Y - absPosition.Y;

            return new Vector2(distanceFromX, distanceFromY);
        }
    }
}
