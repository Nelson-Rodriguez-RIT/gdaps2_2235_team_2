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
    public class Platform
    {
        public Rectangle hitbox;
        public Texture2D texture;

        public Rectangle Hitbox
        { get { return hitbox; } }

        public Platform(Rectangle hitbox, Texture2D texture) 
        {
            this.hitbox = hitbox;
            this.texture = texture;
        }

        public void CheckCollision(GameObject obj) 
        {
            if (this.hitbox.Intersects(obj.Hitbox))
            {
                obj.hitbox.Y = this.hitbox.Y - obj.hitbox.Height;
            }
        }

        public void Draw(SpriteBatch sb) 
        {
            sb.Draw(
                texture,
                new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, hitbox.Height),
                Color.Red); 
        }
    }
}
