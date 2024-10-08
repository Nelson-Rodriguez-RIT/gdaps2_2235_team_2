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
       

        float moveSpeed = 10f;
        public State state;
        

        public MouseControlledEntity(bool active, LevelManager manager) 
            : base( manager, new Vector2(0, 0))
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
            mouseState = Mouse.GetState();


            if (state == State.Active) {

                // Set velocity cap TODO
                velocity = //new Vector2( // Moves this object's center towards the mouse cursor
                    /*
                    (state.Position.X + image.Width / 2), 
                    (state.Position.Y + image.Height / 2)) 
                    */
                    mouseState.Position.ToVector2()
                    - position;

                // Adjust velocity to with moveSpeed / gameTime
                velocity.X *= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity.Y *= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Update position using velocity
                position = CheckForPlatformCollision(
                    levelManager.Platforms);

                hitbox = new Rectangle
                    ((int)position.X - (spriteSheet.Width / 2),
                    (int)position.Y - (spriteSheet.Height / 2),
                    spriteSheet.Width,
                    spriteSheet.Height);

                if (Math.Abs(velocity.X) < 1)
                {
                    velocity.X = 0;
                }
                if (Math.Abs(velocity.Y) < 1)
                {
                    velocity.Y = 0;
                }
            }

            base.Update(gameTime);
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
