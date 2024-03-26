using Microsoft.Xna.Framework.Graphics;
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
    public class Door : Platform
    { 
        private bool isOpen;
        private Vector2 offset;


        public bool IsOpen
        { get { return isOpen; } set { isOpen = value; } }
        public Rectangle Hitbox
        { get { return position; } }
        public Vector2 Offset
        { get { return offset; } }


        public Door(bool isOpen, Rectangle position, Texture2D texture) : base(position, texture)
        {
            this.position = position;
            this.texture = texture;
            this.offset = new Vector2(-position.Width / 2, -position.Height / 2);
        }

        public void CheckCollision(GameObject obj)
        {
            if (obj.position.Intersects(this.position))
            {
                obj.position.X = this.position.X - obj.position.Width;
            }
        }

        //if player hits interact button, makes the door have no hitbox?
        public void OpenDoors(GameObject @object) 
        {
        


        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(
                texture,
                Camera.ApplyOffset(position),
                Color.Blue);
        }
    }
}
