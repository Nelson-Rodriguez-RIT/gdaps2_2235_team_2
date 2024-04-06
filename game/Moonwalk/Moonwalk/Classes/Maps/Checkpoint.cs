using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Helpful_Stuff;
using System.IO;
using Moonwalk.Classes.Entities.Base;

namespace Moonwalk.Classes.Maps
{
    internal class Checkpoint : Terrain
    {

        private int priority;
        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public Checkpoint(Rectangle hitbox) : base(hitbox)
        {
            priority = 0;
            collidable = false;
        }

        
    }
}
