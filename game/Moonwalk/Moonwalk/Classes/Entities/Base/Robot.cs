﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Moonwalk.Classes.Entities.Base
{
    

    internal class Robot : PlayerControlled
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

        //Change this to private later
        public Robot(Vector2 position) : base(position, "../../../Content/Entities/TestEntity")
        {
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
            velocity = input.CurrentMouse.Position.ToVector2() - vectorPosition;
        }
    }
    
}
