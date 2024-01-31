using ConceptDemo.classes.entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using System.Linq;

namespace ConceptDemo.classes.entities
{
    /// <summary>
    /// Contains methods that gives all Entity derived classes universal functionality
    /// </summary>
    internal class Entity
    {
        protected Dictionary<string, List<string>> metadata;
        protected Dictionary<string, Texture2D> textures;

        private bool deleteQueued;

        protected Vector2 absPosition;

        public bool DeleteQueued { get { return deleteQueued;} }
        public Vector2 AbsPosition { get { return absPosition; } }
        

        public Entity(Dictionary<string, List<string>> metadata, Dictionary<string, Texture2D> textures,
            Vector2 initialPosition)
        {
            this.metadata = metadata;
            this.textures = textures;

            deleteQueued = false;

            absPosition = initialPosition;
        }


        /// <summary>
        /// Updates the gameplay several times a frame
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime) {
        }

        /// <summary>
        /// Updates the graphics several times a frame
        /// </summary>
        /// <param name="gameTime">DeltaTime (normalized) variable</param>
        public void Draw(GameTime gameTime) {


        }


        // Teleport()
        // MoveTo()
        // Collision()

    }
}
