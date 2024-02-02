using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dante_Test
{
    internal class Player
    {
        public Vector2 position;
        private Vector2 velocity;
        public Vector2 acceleration;

        public Player(float x, float y) 
        { 
            position = new Vector2(x, y);
            velocity = new Vector2(0f, 0f);
            acceleration = new Vector2(0f, 1f);
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
                velocity.Y = -30f;
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

            if (velocity.X < 10 && velocity.X > -10) 
            {
                velocity.X += acceleration.X;
            }

            
            
        }
    }
}
