using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Interfaces
{
    public interface IHostile : ICollidable
    {
        /// <summary>
        /// Amount of damage this deals
        /// </summary>
        int Damage { get; }

        /// <summary>
        /// Controls the entity's movement
        /// </summary>
        void AI();
    }
}
