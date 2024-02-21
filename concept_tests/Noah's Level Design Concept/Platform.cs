using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Noah_s_Level_Design_Concept
{
    internal class Platform
    {
        public Rectangle hitbox;

        public Rectangle Hitbox
        { get { return hitbox; } }

        public Platform(Rectangle hitbox) 
        {
            this.hitbox = hitbox;
        }

        public void CheckCollision(GameObject obj) 
        {
            if (this.hitbox.Intersects(obj.Hitbox))
            {
                obj.hitbox.X = this.hitbox.X + obj.hitbox.Width;
                obj.hitbox.X = this.hitbox.Y + obj.hitbox.Height;
            }
        }
    }
}
