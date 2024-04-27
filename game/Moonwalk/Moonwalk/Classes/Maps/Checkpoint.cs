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
using Moonwalk.Classes.Entities;

namespace Moonwalk.Classes.Maps
{
    internal class Checkpoint : Terrain
    {
        public Checkpoint(Rectangle hitbox, bool starting = false) : base(hitbox)
        {
            collidable = false;
            //add subscriber
            OnCollision += SetRespawn;

            if (starting)
            {
                Player.MostRecentCheckpoint = this;
            }
        }

        /// <summary>
        /// sets the respawn point to this
        /// </summary>
        private void SetRespawn()
        {
            Player.MostRecentCheckpoint = this;
        }
    }
}
