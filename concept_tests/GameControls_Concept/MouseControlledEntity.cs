﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;

namespace GameControls_Concept
{
    enum State
    {
        Active,
        Inactive
    }
    /// <summary>
    /// Class for an entity that is controlled with the mouse
    /// </summary>
    internal class MouseControlledEntity : ControllableEntity
    {
       
        const float moveSpeed = 10;
        protected State state;

        public MouseControlledEntity(Texture2D image, bool active, LevelManager manager) 
            : base(image, manager, new Vector2(0, 0))
        { 
            MouseState state = Mouse.GetState();
            position = state.Position.ToVector2();

            if (active)
            {
                this.state = State.Active;
            }
            else
            {
                this.state = State.Inactive;
            }

        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if (state == State.Active) {
                MouseState state = Mouse.GetState();

                // Set velocity cap TODO
                velocity = new Vector2( // Moves this object's center towards the mouse cursor
                    (state.Position.X - image.Width / 2), 
                    (state.Position.Y - image.Height / 2)) 
                    - position;

                // Adjust velocity to with moveSpeed / gameTime
                velocity.X *= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity.Y *= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;


                /*
                 * I changed the CheckForPlatformCollision method to take an out PhysicsState
                 * so that it could set that state to grounded if there was a collision. However, 
                 * this creates a problem where we can't do the same for anything without that 
                 * PhysicsState field. This could be a reason to have the collision method be within 
                 * the entity class and not the platform, because the platform can't know how we want 
                 * it to interact with each different kind of entity. Lmk what you think we should do.
                 * - Dante
                /*
                position = Platform.CheckForPlatformCollision(
                    levelManager.Platforms,
                    hitbox,
                    velocity);
                */
                
                //Move hibox to the right spot
                hitbox = new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    image.Width,
                    image.Height);
                
            }

            //Space to turn the following on and off
            if (!keyboardState.IsKeyDown(Keys.Space)
                && previousKB.IsKeyDown(Keys.Space))
            {
                Toggle();
            }

            previousKB = keyboardState;
        }

        /// <summary>
        /// Toggles the state between inactive and active.
        /// </summary>
        public virtual void Toggle()
        {
            if (state == State.Active)
            {
                state = State.Inactive;
            }
            else
            {
                state = State.Active;
            }
        }
    }
}
