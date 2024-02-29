using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Moonwalk.Classes.Entities.Base {
    internal class Terrain {
        protected Rectangle hitbox;

        public Rectangle Hitbox {
            get { return hitbox; }
        }


        public Terrain(Rectangle hitbox) {
            this.hitbox = hitbox; 
        }
    }
}
