using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Interfaces
{
    /// <summary>
    /// An entity that can jump
    /// </summary>
    public interface IJump : ICollidable
    {
        bool Grounded
        { get; }
    }
}
