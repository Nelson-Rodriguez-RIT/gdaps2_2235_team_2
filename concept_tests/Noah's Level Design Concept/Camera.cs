using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noah_s_Level_Design_Concept
{
    public class Camera
    {
        private Rectangle position;
        private Vector2 movementDirection;

        public int CameraX
        { get { return position.X; } }
        public int CameraY
        { get { return position.Y; } }

        public Camera(Rectangle position)
        {
            this.position = position;
        }

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
        
    }
}
