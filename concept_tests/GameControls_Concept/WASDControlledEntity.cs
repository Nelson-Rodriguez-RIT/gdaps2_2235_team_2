using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameControls_Concept
{
    
    /// <summary>
    /// An entity that can be controlled with the WASD keys
    /// </summary>
    internal class WASDControlledEntity : ControllableEntity
    {
        public WASDControlledEntity(LevelManager manager, Vector2 position) 
            : base(manager, position)
        {
            gravity = 10f;
            maxXVelocity = 70f;
            terminalVelocity = 1400f;

            acceleration = new Vector2 (0, gravity);
        }

        public override void Update(GameTime gameTime) 
        {
            Input();
            Movement(gameTime);

            //Todo: fix swinging so it uses velocity and acceleration
            //Update position using velocity                     

            hitbox = new Rectangle
                ((int)position.X - (image.Width / 2),
                (int)position.Y - (image.Height / 2),
                image.Width,
                image.Height);

            base.Update(gameTime);
        }

        /// <summary>
        /// Move the entity based on its acceleration and velocity.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Movement(GameTime gameTime)
        {
            //Update velocity using acceleration
            velocity = new Vector2(
                velocity.X + (acceleration.X * (float)Math.Pow(
                    gameTime.ElapsedGameTime.TotalSeconds,
                    2)),
                velocity.Y + (acceleration.Y * (float)Math.Pow(
                    gameTime.ElapsedGameTime.TotalSeconds,
                    2)));

        

            //Make sure speed is not over the maximum
            if (velocity.Y > terminalVelocity) 
            { 
                velocity.Y = terminalVelocity;
            }
            else if (velocity.Y < -terminalVelocity)
            {
                velocity.Y = -terminalVelocity;
            }

            Vector2 oldPosition = position;

            //Update position using velocity
            position = CheckForPlatformCollision(
                levelManager.Platforms);

            if (oldPosition.Y == position.Y)
            {
                velocity.Y = 0;
            }
            if (oldPosition.X == position.X)
            {
                velocity.X = 0;
            }
            
            //Slows down horizontal movement
            if (velocity.X > 0)
            {
                acceleration.X = -100000f;
            }
            else if (velocity.X < 0)
            {
                acceleration.X = 100000f;
            }
            else
            {
                acceleration.X = 0;
            }
            
        }
        
        /// <summary>
        /// Process input
        /// </summary>
        public override void Input()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                acceleration.X = -1400f;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.D))
            {
                acceleration.X = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                acceleration.X = 1400f;
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

            //Jump!
            if (
                keyboardState.IsKeyDown(Keys.Space))
            {
                velocity.Y = -15f;
            }
        }
    }
}
