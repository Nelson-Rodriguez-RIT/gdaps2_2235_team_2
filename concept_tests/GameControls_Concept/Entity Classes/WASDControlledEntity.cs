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
            gravity = 30f;
            maxXVelocity = 20f;
            terminalVelocity = 100f;

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
        protected virtual void Movement(GameTime gameTime)
        {

            //Update velocity using acceleration
            velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Make sure speed is not over the maximum
            if (velocity.Y > terminalVelocity) 
            { 
                velocity.Y = terminalVelocity;
            }
            else if (velocity.Y < -terminalVelocity)
            {
                velocity.Y = -terminalVelocity;
            }

            if (velocity.X > maxXVelocity)
            {
                velocity.X = maxXVelocity;
            }
            else if (velocity.X < -maxXVelocity)
            {
                velocity.X = -maxXVelocity;
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
                       
        }
        
        /// <summary>
        /// Process input
        /// </summary>
        protected override void Input()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.A) 
                )
            {
                if (previousKB.IsKeyUp(Keys.A))
                velocity.X += -3f;

                 acceleration.X = -40f;

                
            }           

            if (keyboardState.IsKeyDown(Keys.D)
                )
            {
                if (previousKB.IsKeyUp(Keys.D))
                    velocity.X += 3f;

                acceleration.X = 40f;
            }

            

            if (keyboardState.IsKeyUp(Keys.D)
                && keyboardState.IsKeyUp(Keys.A))
            {
                
                acceleration.X = -velocity.X / 
                    (Math.Abs(velocity.Y) < 0.5 ? 0.2f : 0.5f);



                if (Math.Abs(velocity.X) < 0.2)
                {
                    velocity.X = 0;
                    acceleration.X = 0;
                }
            }

            //If the player is accelerating opposite their motion, 
            //double the acceleration for smoother movement
            if (Math.Sign(acceleration.X) != Math.Sign(velocity.X))
            {
                acceleration.X *= 2f;
            }

            //Jump!
            if (
                keyboardState.IsKeyDown(Keys.Space) && (Math.Abs(velocity.Y) < 0.5)
                )
            {
                velocity.Y = -15f;
            }
        }
    }
}
