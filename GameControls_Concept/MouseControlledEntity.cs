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
        private Rectangle hitbox;
        private LevelManager levelManager;
        
        public Rectangle Hitbox
        {
            get { return hitbox; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
        }

        public MouseControlledEntity(Texture2D image, bool active, LevelManager manager) 
        { 
            MouseState state = Mouse.GetState();
            position = state.Position.ToVector2();
            velocity = new Vector2(0, 0);
            this.image = image;
            hitbox = new Rectangle
                ((int)position.X - (image.Width / 2),
                (int)position.Y - (image.Height / 2),
                image.Width,
                image.Height);

            if (active)
            {
                this.state = State.Active;
            }
            else
            {
                this.state = State.Inactive;
            }

            levelManager = manager;
        }

        public virtual void MoveInParts(GameTime gameTime) 
        {
            int iterations = (int)(Math.Max(velocity.X, velocity.Y) / 10) + 1;           

            for (int i = 0; i < iterations; i++)
            {
                Vector2 lastPosition = new Vector2(position.X, position.Y);

                position = new Vector2(
                    ((velocity.X / iterations) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed + position.X),
                    ((velocity.Y / iterations) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed + position.Y));
                hitbox = new Rectangle
                    ((int)position.X - (image.Width / 2),
                    (int)position.Y - (image.Height / 2),
                    image.Width,
                    image.Height);            

                if (CheckPlatformCollision(levelManager))
                {
                    position = lastPosition;
                    break;
                }
            }
            
        }

        public virtual void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if (state == State.Active)
            {
                MouseState state = Mouse.GetState();
                velocity = state.Position.ToVector2() - position;

                Vector2 lastPosition = new Vector2(position.X, position.Y);

                position = new Vector2(
                    ((velocity.X) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed + position.X),
                    ((velocity.Y) * (float)gameTime.ElapsedGameTime.TotalSeconds * moveSpeed + position.Y));
                hitbox = new Rectangle
                    ((int)position.X - (image.Width / 2),
                    (int)position.Y - (image.Height / 2),
                    image.Width,
                    image.Height);

                if (CheckPlatformCollision(levelManager))
                {
                    position = lastPosition;
                    MoveInParts(gameTime);
                }
            }

            if (!keyboardState.IsKeyDown(Keys.Space)
                && previousKB.IsKeyDown(Keys.Space))
            {
                Toggle();
            }

            previousKB = keyboardState;
        }


        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(image, 
                hitbox, 
                Color.White);
        }

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

        /// <summary>
        /// Checks if the entity is colliding with a platform
        /// </summary>
        /// <param name="manager">The level manager</param>
        /// <returns>A boolean value.</returns>
        public virtual bool CheckPlatformCollision(LevelManager manager)
        {
            bool intersects = false;

            foreach (Platform platform in manager.Platforms)
            {
                if (hitbox.Intersects(platform.Hitbox))
                {
                    intersects = true;
                }
            }
            /*
            if (manager.Platforms.Exists(platform => platform.Hitbox.Intersects(hitbox)))
            {
                intersects = true;
            }
            */

            return intersects;
        }
    }
}
