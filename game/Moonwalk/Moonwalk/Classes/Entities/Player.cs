using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Managers;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;

namespace Moonwalk.Classes.Entities
{
    /// <summary>
    /// The player controlled character
    /// </summary>
    internal class Player : PlayerControlled, IJump
    {
        protected enum Animations
        {
            Idle,
            Idle_Blink,
            Walk,
            Run,
            Crouch,
            Jump,
            Hurt,
            Death,
            Attack,
        }

        /// <summary>
        /// Determines if the entity is grounded or not
        /// </summary>
        public bool Grounded
        {
            get
            {
                if (CheckCollision(new Rectangle(entity.X, entity.Y + 5, entity.Width, entity.Height)))
                {
                    return true;
                }

                return false;
            }
        }

        //Make private later
        public Player(Vector2 position) : base(position, "../../../Content/Entities/TestEntity")
        {
            gravity = 50f;
            acceleration = new Vector2(0, gravity);
            maxXVelocity = 60;
            maxYVelocity = 100;           

            SwitchAnimation(Animations.Idle);
            spriteScale = 4;
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            base.Update(gameTime, input);          
        }

        public override void Input(StoredInput input)
        {
            //Horizontal movement
            if (input.IsPressed(Keys.A) &&
                !input.IsPressed(Keys.D))
            {
                // acceleration is higher if the player is moving in the opposite direction for smoother movement
                acceleration.X = velocity.X > 0 ? -maxXVelocity * 2.5f : -maxXVelocity * 2;
            }
            else if (input.IsPressed(Keys.D) &&
                !input.IsPressed(Keys.A))
            {
                // acceleration is higher if the player is moving in the opposite direction for smoother movement
                acceleration.X = velocity.X < 0 ? maxXVelocity * 2.5f : maxXVelocity * 2;
            }

            //Slow down if not pressing anything
            if (!input.IsPressed(Keys.D) &&
                !input.IsPressed(Keys.A)
                && velocity.X != 0)
            {
                acceleration.X = -Math.Sign(velocity.X) * 20;

            }

            

            //Jump (doesn't work yet, needs check for grounded)
            if (input.IsPressed(Keys.Space)
                && !input.WasPressed(Keys.Space) 
                && Grounded)
            {
                velocity.Y = -60;
            }

        }

        public override void Draw(SpriteBatch batch, Vector2 globalScale)
        {
            base.Draw(batch, globalScale);

            batch.DrawString(GameManager.font, 
                $"{vectorPosition.Y} - {velocity.Y} - {acceleration.Y} \n {Position.Y}",
                new Vector2(400, 50),
                Color.White);
        }

    }
}
