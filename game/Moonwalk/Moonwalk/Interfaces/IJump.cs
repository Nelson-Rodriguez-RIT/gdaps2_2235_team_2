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
    internal interface IJump
    {
        bool Grounded
        { get; }
    }
}
