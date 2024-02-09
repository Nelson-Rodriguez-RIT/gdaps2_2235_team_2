using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dante_Test
{
        internal sealed class Player
    {
        private Vector2 position;
        private Vector2 velocity;
        private Vector2 acceleration;

        public Vector2 Position
        {
            get { return position; }
        }
        public Vector2 Velocity
        {
            get { return velocity;}
        }
        public Vector2 Acceleration
        {
            get { return acceleration;}
        }

        public Player(float x, float y) 
        { 
            position = new Vector2(x, y);
            velocity = new Vector2(0f, 0f);
            acceleration = new Vector2(0f, 1.5f);
        }

        public void VerticalMovement()
        {
            velocity.Y += acceleration.Y;

            position.Y += velocity.Y;
        }

        public void Jump()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                velocity.Y = -40f;
                VerticalMovement();
            }
          
        }

        public void HorizontalMovement()
        {
            position.X += velocity.X;

            
            if (velocity.X > 0f)
            {
                velocity.X -= 0.5f;
            }
            if (velocity.X < 0f)
            {
                velocity.X += 0.5f;
            }
            

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                acceleration.X = -1.5f;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.D))
            {
                acceleration.X = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                acceleration.X = 1.5f;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.A))
            {
                acceleration.X = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D)
                && Keyboard.GetState().IsKeyDown(Keys.A))
            {
                acceleration.X = 0;
            }

            if (velocity.X < 20 && velocity.X > -20) 
            {
                velocity.X += acceleration.X;
            }

            
            
        }
    }
}
