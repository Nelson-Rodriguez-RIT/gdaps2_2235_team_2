using ConceptDemo.classes.entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConceptDemo.classes {
    /// <summary>
    /// Used for entities to track collision boxes and interactions
    /// </summary>
    internal class Collision {
        /// <summary>
        /// The absPosition of an entity
        /// </summary>
        private Vector2 position;

        private int horizontalSize;
        private int verticalSize;

        public Vector Position { get { return position; } }
        public int HorizontalSize { get { return horizontalSize; } }
        public int verticalSize { get { return verticalSize; } }


        /// <summary>
        /// Create collision for an entity
        /// </summary>
        /// <param name="position">Reference to absolute position of the entity</param>
        /// <param name="horizontalSize">Horizontal length of the entity</param>
        /// <param name="verticalSize">Vertical length of the entity</param>
        public Collision(Vector2 position, int horizontalSize, int verticalSize) {
            this.position = position;

            this.horizontalSize = horizontalSize;
            this.verticalSize = verticalSize;
        }


        /// <summary>
        /// Uses AABB detection to find if you entities are collided with one another
        /// </summary>
        /// <param name="foreignCollision">Collision of the foreignCollision</param>
        /// <returns>Whether or not the two entities are colliding</returns>
        public bool CheckForCollision(Collision foreignCollision) {
            return (foreignCollision.Position.X <= position.X + horizontalSize && 
                    foreignCollision.Position.Y <= position.Y + verticalSize &&
                    foreignCollision.Position.X + foreignCollision.HorizontalSize >= position.X &&
                    foreignCollision.Position.Y + foreignCollision.VerticalSizeSize >= position.Y)
        }
    }
}
