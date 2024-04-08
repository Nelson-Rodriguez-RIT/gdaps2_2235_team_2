using Microsoft.Xna.Framework;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Maps
{
    internal class Killzones
    {
        private Rectangle hitbox;

        public Killzones(Rectangle hitbox, Checkpoint checkpoint) 
        {
            this.hitbox = hitbox;
        
        }

    }
}
