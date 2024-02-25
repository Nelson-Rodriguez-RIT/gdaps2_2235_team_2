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
        public Rectangle position;
        public Texture2D texture;
        public Vector2 movementDirection;
        private Vector2 offset;

        public Rectangle Hitbox
        { get { return position; } }
        public Vector2 Offset
        { get { return offset; } }

        public Platform(Rectangle position, Texture2D texture) 
        {
            this.position = position;
            this.texture = texture;
            this.offset = new Vector2(-position.Width / 2, -position.Height / 2);
        }

        public void CheckCollision(GameObject obj) 
        {
            if (this.position.Intersects(obj.Position))
            {
                obj.position.Y = this.position.Y - obj.position.Height;
            }
        }

        public void Draw(SpriteBatch sb) 
        {
            sb.Draw(
                texture,
                Camera.ApplyOffset(position, offset),
                Color.Red); 
        }
        /*
        public void Movement(Player player)
        {
            movementDirection = Vector2.Zero;

            if (player.MovingLeft) //if player is moving left
            {
                movementDirection += Vector2.UnitX * 6;
                this.position.X += (int)movementDirection.X;
            }
            if (player.MovingRight) //if player is moving left)
            {
                movementDirection += Vector2.UnitX * 6;
                this.position.X -= (int)movementDirection.X;
            }
        }
        */
    }
}
