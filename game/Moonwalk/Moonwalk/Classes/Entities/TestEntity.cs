using Moonwalk.Classes.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;
using System;
using System.Collections.Generic;

namespace Moonwalk.Classes.Entities {
    internal class TestEntity : PlayerControlled {
        protected enum Animations {
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

        public TestEntity(Vector2 initalPosition) 
                : base(initalPosition, "../../../Content/Entities/TestEntity") {
            SwitchAnimation(Animations.Idle);
            spriteScale = 4;
            entity = new Rectangle((int)vectorPosition.X, (int)vectorPosition.Y, 300, 300);
        }

        public override void Update(GameTime gameTime, StoredInput input) {

            entity = new Rectangle((int)vectorPosition.X, (int)vectorPosition.Y, 300, 300);

            base.Update(gameTime, input);
        }

        public override void Input(StoredInput input)
        {
            if (input.CurrentKeyboard.IsKeyDown(Keys.D1))
                SwitchAnimation(Animations.Idle);

            if (input.CurrentKeyboard.IsKeyDown(Keys.D2))
                SwitchAnimation(Animations.Idle_Blink);

            if (input.CurrentKeyboard.IsKeyDown(Keys.D3))
                SwitchAnimation(Animations.Walk);

            if (input.CurrentKeyboard.IsKeyDown(Keys.D4))
                SwitchAnimation(Animations.Run);

            if (input.CurrentKeyboard.IsKeyDown(Keys.D5))
                SwitchAnimation(Animations.Crouch);

            if (input.CurrentKeyboard.IsKeyDown(Keys.D6))
                SwitchAnimation(Animations.Jump);

            if (input.CurrentKeyboard.IsKeyDown(Keys.D7))
                SwitchAnimation(Animations.Hurt);

            if (input.CurrentKeyboard.IsKeyDown(Keys.D8))
                SwitchAnimation(Animations.Death);

            if (input.CurrentKeyboard.IsKeyDown(Keys.D9))
                SwitchAnimation(Animations.Attack);

            if (input.IsPressed(Keys.D))
            {
                velocity.X = 40;
            }
            else if (input.IsPressed(Keys.A))
            {
                velocity.X = -40;
            }
            else if (input.IsPressed(Keys.W))
            {
                velocity.Y = -40;
            }
            else if (input.IsPressed(Keys.S))
            {
                velocity.Y = 40;
            }
            else
            {
                velocity = Vector2.Zero;
            }
        }
    }
}
