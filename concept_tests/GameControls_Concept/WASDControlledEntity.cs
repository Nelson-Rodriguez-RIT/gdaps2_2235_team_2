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
    enum PhysicsState //to be added to
    {
        Grounded,
        Airborne
    }
    /// <summary>
    /// An entity that can be controlled with the WASD keys
    /// </summary>
    internal class WASDControlledEntity : ControllableEntity
    {
        
        protected PhysicsState physicsState;

       
        public WASDControlledEntity(Texture2D image, LevelManager manager, Vector2 position) 
            : base(image, manager, position)
        {
            gravity = 1400f;
            maxXVelocity = 70f;
            terminalVelocity = 1400f;

            acceleration = new Vector2 (0, gravity);
            physicsState = PhysicsState.Airborne;
        }

        public override void Update(GameTime gameTime) 
        {
            Input();
            Movement(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// Move the entity based on its acceleration and velocity.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Movement(GameTime gameTime)
        {

            switch (physicsState)
            {
                //Sets vertical velocity to 0 if grounded
                case PhysicsState.Grounded:
                    velocity = new Vector2(
                        velocity.X + (acceleration.X * (float)Math.Pow(
                            gameTime.ElapsedGameTime.TotalSeconds,
                            2)),
                        0);
                    break;
                
                    //Free fall
                case PhysicsState.Airborne:
                    //Update velocity using acceleration
                    velocity = new Vector2(
                        velocity.X + (acceleration.X * (float)Math.Pow(
                            gameTime.ElapsedGameTime.TotalSeconds,
                            2)),
                        velocity.Y + (acceleration.Y * (float)Math.Pow(
                            gameTime.ElapsedGameTime.TotalSeconds,
                            2)));
                    break;
            }

            //Make sure speed is not over the maximum
            if (velocity.Y > terminalVelocity) 
            { 
                velocity.Y = terminalVelocity;
            }
            else if (velocity.Y < -terminalVelocity)
            {
                velocity.Y = -terminalVelocity;
            }


            //Update position using velocity
            position = Platform.CheckForPlatformCollision(
                levelManager.Platforms,
                hitbox,
                velocity,
                out physicsState);

            
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
        public virtual void Input()
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
            if (physicsState == PhysicsState.Grounded &&
                keyboardState.IsKeyDown(Keys.Space))
            {
                physicsState = PhysicsState.Airborne;
                velocity.Y = -15f;
            }
        }
    }
}
