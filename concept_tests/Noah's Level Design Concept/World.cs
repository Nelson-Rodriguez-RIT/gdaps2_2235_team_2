using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Noah_s_Level_Design_Concept
{
    public class World
    {
        private bool active;
        private Texture2D asset;
        private Rectangle position;
        private Vector2 movementDirection;
        private float gravity = 9.8f;
        private Vector2 offset;

        public bool IsActive
        {
            get { return active; }
            set { active = value; }
        }
        public Rectangle Position
        {
            get { return position; } 
            set { position = value; }
        }
        public Vector2 Offset
        { get { return offset; } }

        public World(Texture2D asset, Rectangle postion, bool active)
        {
            this.asset = asset;
            this.position = postion;    
            this.active = active;
            this.offset = new Vector2 (-position.Width/2 , -position.Height/2);
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            if (active)
            {
                spriteBatch.Draw(
                    asset,
                    Camera.ApplyOffset(position),
                    Color.White);
            }
        }

        //want to make it where once the player reaches the middle of the screen, their character no longer moves
        //and the background just moves
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
