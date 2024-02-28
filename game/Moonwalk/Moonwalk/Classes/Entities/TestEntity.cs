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
            // Just to test the animations
            activeAnimation = (int) Animations.Idle;
        }
    }
}
