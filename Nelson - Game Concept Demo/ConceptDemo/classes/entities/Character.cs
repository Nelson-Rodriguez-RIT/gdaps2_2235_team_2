using ConceptDemo.classes.entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConceptDemo.classes.entities
{
    /// <summary>
    /// A controllable character object with basic functionality
    /// </summary>
    internal class Character : Entity
    {
        // These might not need to be protected, probably would work fine as private too
        protected Collision collision;
        protected int horizontalSize;
        protected int verticalSize

        /// <summary>
        /// Creates a character entity at 0, 0
        /// </summary>
        public Character() : this(new Vector2(0, 0)) {}

        /// <summary>
        /// Creates a character entity at a specificed position
        /// </summary>
        /// <param name="absPosition"></param>
        public Character(Vector absPosition) : base(absPosition) {
            collision = new Collision(absPosition)
        }
    }
}
