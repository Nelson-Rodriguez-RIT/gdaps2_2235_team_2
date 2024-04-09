using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Interfaces
{
    /*
     * This might seem useless but the reason I made it is becasue stuff like boxes, if they move into 
     * another entity, stop them from moving at all. This way Enemies and the Player can still collide 
     * and solids won't mess stuff up. - Dante
     */

    /// <summary>
    /// Solids collide with softs but softs don't collide with each other
    /// </summary>
    public interface ISoft
    {
        /// <summary>
        /// Hitbox of the entity
        /// </summary>
        Rectangle Hitbox { get; }
    }
}
