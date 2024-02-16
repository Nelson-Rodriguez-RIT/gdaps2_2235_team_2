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
    internal class MouseControlledEntity
    {
        private Vector2 position;
        private Vector2 velocity;
        private Texture2D image;
        const float moveSpeed = 10;
        private State state;
        private KeyboardState keyboardState;
        private KeyboardState previousKB;

        public Vector2 Position
        {
            get { return position; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
        }

        public MouseControlledEntity(Texture2D image, bool active) 
        { 
            MouseState state = Mouse.GetState();
            position = state.Position.ToVector2();
            velocity = new Vector2(0, 0);
            this.image = image;
            if (active)
            {
                this.state = State.Active;
            }
            else
            {
                this.state = State.Inactive;
            }

        }

        public void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if (state == State.Active)
            {
                MouseState state = Mouse.GetState();
                velocity = state.Position.ToVector2() - position;
                position = new Vector2(
                    (velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed + position.X),
                    (velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed + position.Y));
            }

            if (!keyboardState.IsKeyDown(Keys.Space)
                && previousKB.IsKeyDown(Keys.Space))
            {
                Toggle();
            }

            previousKB = keyboardState;
        }


        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle
                ((int)position.X - (image.Width / 2), 
                (int)position.Y - (image.Height / 2), 
                image.Width,
                image.Height), 
                Color.White);
        }

        public void Toggle()
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
