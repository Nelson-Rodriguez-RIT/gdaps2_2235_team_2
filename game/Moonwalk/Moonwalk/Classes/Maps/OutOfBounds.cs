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
    /// <summary>
    /// Terrain that respawns the player when entered
    /// </summary>
    internal class OutOfBounds : Terrain
    {
        public OutOfBounds(Rectangle hitbox) : base(hitbox) 
        {
            collidable = false;
            OnCollision += Collide;
        }

        public override void Collide()
        {
            //call player method to respawn and take damage
            Player.HitBarrier();
        }

    }
}
