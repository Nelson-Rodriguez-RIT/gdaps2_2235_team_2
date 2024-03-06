using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Interfaces
{
    internal interface IMovable
    {

        Vector2 Velocity 
        { get; }

        Vector2 Acceleration
        { get; }

        /// <summary>
        /// Handles movement of an entity
        /// </summary>
        void Movement();

    }
}
