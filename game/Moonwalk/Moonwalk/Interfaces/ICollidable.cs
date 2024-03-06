using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Interfaces
{
    internal interface ICollidable : IMovable
    {
        void CheckCollision();
    }
}
