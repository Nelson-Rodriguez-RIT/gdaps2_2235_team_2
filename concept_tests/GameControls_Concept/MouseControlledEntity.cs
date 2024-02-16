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
                    (state.Position.X - image.Width / 2) * moveSpeed * (float) gameTime.ElapsedGameTime.TotalSeconds, 
                    (state.Position.Y - image.Height / 2) * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds) 
                    - position;

                
                position = Platform.CheckForPlatformCollision(
                    state,
                    levelManager.Platforms,
                    hitbox,
                    velocity);

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
