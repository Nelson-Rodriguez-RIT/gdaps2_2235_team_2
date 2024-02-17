using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GameControls_Concept
{
    internal class Platform : Entity
    {
        private Rectangle hitbox;
        private Texture2D image;



        public Platform(Rectangle rectangle, Texture2D image, LevelManager manager) : 
            base(image, manager, rectangle.Location.ToVector2())
        {
            hitbox = rectangle;
            this.image = image;
        }
        public Rectangle Hitbox 
        { 
            get { return hitbox; } 
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, hitbox, Color.White);
        }


       
    }
}
