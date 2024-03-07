using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;
using System;
using System.Collections.Generic;

namespace Moonwalk.Classes.Entities.Base
{
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
            
        }

    }
}
