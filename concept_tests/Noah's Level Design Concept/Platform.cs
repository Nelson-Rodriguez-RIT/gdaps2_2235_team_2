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
        Rectangle hitbox;
        public Platform(Rectangle hitbox) 
        { 
            this.hitbox = hitbox;
        }

        public void CheckCollision(GameObject obj)
        {
            
        
        }

    }
}
