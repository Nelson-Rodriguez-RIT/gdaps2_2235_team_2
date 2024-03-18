using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Interfaces
{
    public interface IMovable
    {
        /// <summary>
        /// Position of an object
        /// </summary>
        Point Position 
        { get; }

        /// <summary>
        /// Velocity of the object
        /// </summary>
        Vector2 Velocity 
        { get; }

        /// <summary>
        /// Acceleration of the object
        /// </summary>
        Vector2 Acceleration
        { get; }

        /// <summary>
        /// Handles movement of an entity
        /// </summary>
        void Movement(GameTime time);

        /// <summary>
        /// Apply an impulse to an entity
        /// </summary>
        /// <param name="destination"></param>
        void Impulse(Vector2 destination);

    }
}
