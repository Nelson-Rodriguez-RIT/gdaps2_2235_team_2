using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Interfaces
{
    /// <summary>
    /// Interface to be used for things that stuff collides with like terrain but is not terrain
    /// </summary>
    public interface ISolid
    {
        /// <summary>
        /// Hitbox of the entity
        /// </summary>
        Rectangle Hitbox { get; }

        bool Collidable { get; }

        void Collide();
    }
}
