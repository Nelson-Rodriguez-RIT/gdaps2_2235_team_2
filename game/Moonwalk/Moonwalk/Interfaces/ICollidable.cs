using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Interfaces
{
    public interface ICollidable : IMovable
    {
        /// <summary>
        /// The accuracy with which to check (Higher means more checks)
        /// </summary>
        int CollisionAccuracy
        { get; }

        /// <summary>
        /// Hitbox of the entity
        /// </summary>
        Rectangle Hitbox {  get; }

        /// <summary>
        /// Checks if the object has collided with another
        /// </summary>
        bool CheckCollision();
    }
}
