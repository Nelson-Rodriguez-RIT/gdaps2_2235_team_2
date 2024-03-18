using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Interfaces
{
    internal interface IMovable
    {
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

    }
}
