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
                velocity = new Vector2( // Moves this object's center towards the mouse cursor
                    state.Position.X - image.Width / 2, 
                    state.Position.Y - image.Height / 2
                    ) - position;

                int maxIteration = 20; // Increase this number to increase collision precision

                // How many steps it can go before colliding into anything
                int peakXIteration = maxIteration;
                int peakYIteration = maxIteration;

                foreach (Platform platform in levelManager.Platforms) // Check each platform
                    for (int iteration = 0; iteration <= maxIteration; iteration++) { // Check how many steps it can go before colliding into this platform
                        if (new Rectangle( // Check for horizontal collision
                                (int)((position.X + ((velocity.X * moveSpeed) / maxIteration) * iteration * (float)gameTime.ElapsedGameTime.TotalSeconds)),
                                (int)position.Y,
                                image.Width,
                                image.Height)
                                .Intersects(platform.Hitbox))
                            // We want the absolute minimum steps
                            peakXIteration = iteration - 1 < peakXIteration ? iteration - 1 : peakXIteration;

                        if (new Rectangle( // Check for vertical collision
                                (int) position.X,
                                (int)((position.Y + ((velocity.Y * moveSpeed) / maxIteration) * iteration * (float)gameTime.ElapsedGameTime.TotalSeconds)),
                                image.Width,
                                image.Height)
                                .Intersects(platform.Hitbox))
                            // We want the absolute minimum steps
                            peakYIteration = iteration - 1 < peakYIteration ? iteration - 1 : peakYIteration;
                    } 
                        

                // Update position and relevant hitbox based on peakIteration
                position += new Vector2(
                    (velocity.X / maxIteration) * peakXIteration * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds,
                    (velocity.Y / maxIteration) * peakYIteration * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

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
