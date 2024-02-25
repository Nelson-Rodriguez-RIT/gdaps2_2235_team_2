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

        public Rectangle Hitbox
        { get { return position; } }

        public Platform(Rectangle position, Texture2D texture) 
        {
            this.position = position;
            this.texture = texture;
        }

        public void CheckCollision(GameObject obj) 
        {
            if (this.position.Intersects(obj.Position))
            {
                obj.position.Y = this.position.Y - obj.position.Height;
            }
        }

        public void Draw(SpriteBatch sb, Camera camera) 
        {
            sb.Draw(
                texture,
                new Rectangle(
                        this.position.X - camera.CameraX,
                        this.position.Y - camera.CameraY,
                        this.position.Width,
                        this.position.Height),
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
