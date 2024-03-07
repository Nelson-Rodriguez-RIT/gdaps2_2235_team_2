using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Managers;
using System;
using System.Collections.Generic;

namespace Moonwalk.Classes.Entities
{
    /// <summary>
    /// The player controlled character
    /// </summary>
    internal class Player : PlayerControlled
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

        //Make private later
        public Player(Vector2 position) : base(position, "../../../Content/Entities/TestEntity")
        {
            gravity = 10f;
            acceleration = new Vector2(0, gravity);
            maxXVelocity = 40;
            maxYVelocity = 40;
            physicsState = PhysicsState.Linear;

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
                acceleration.X = -10f;
            }
            else if (input.IsPressed(Keys.D) &&
                !input.IsPressed(Keys.A))
            {
                acceleration.X = 10f;
            }

            //Slow down if not pressing anything
            if (!input.IsPressed(Keys.D) &&
                !input.IsPressed(Keys.A))
            {
                acceleration.X = -Math.Sign(velocity.X) * 5;
            }

            //Jump (doesn't work yet)
            if (input.IsPressed(Keys.Space))
            {
                velocity.Y = 20;
            }

        }

    }
}
