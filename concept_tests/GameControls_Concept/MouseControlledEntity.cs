using Microsoft.Xna.Framework;
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
       

        const float moveSpeed = 10f;
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
                mouseState = Mouse.GetState();

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
                    ((int)position.X - (image.Width / 2),
                    (int)position.Y - (image.Height / 2),
                    image.Width,
                    image.Height);
            }

            /* Temporarily disabled
            //Space to turn the following on and off
            if (!keyboardState.IsKeyDown(Keys.Space)
                && previousKB.IsKeyDown(Keys.Space))
            {
                Toggle();
            }
            */

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
