using Moonwalk.Classes.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;
using System;
using System.Collections.Generic;

namespace Moonwalk.Classes.Entities {
    internal class TestEntity : Entity {
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
        }

        public override void Update(GameTime gameTime, KeyboardState kbState, MouseState msState) {
            if (kbState.IsKeyDown(Keys.D1))
                SwitchAnimation(Animations.Idle);

            if (kbState.IsKeyDown(Keys.D2))
                SwitchAnimation(Animations.Idle_Blink);

            if (kbState.IsKeyDown(Keys.D3))
                SwitchAnimation(Animations.Walk);

            if (kbState.IsKeyDown(Keys.D4))
                SwitchAnimation(Animations.Run);

            if (kbState.IsKeyDown(Keys.D5))
                SwitchAnimation(Animations.Crouch);

            if (kbState.IsKeyDown(Keys.D6))
                SwitchAnimation(Animations.Jump);

            if (kbState.IsKeyDown(Keys.D7))
                SwitchAnimation(Animations.Hurt);

            if (kbState.IsKeyDown(Keys.D8))
                SwitchAnimation(Animations.Death);

            if (kbState.IsKeyDown(Keys.D9))
                SwitchAnimation(Animations.Attack);


            base.Update(gameTime, kbState, msState);
        }
    }
}
