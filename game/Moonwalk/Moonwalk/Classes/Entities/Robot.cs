using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Managers;
using Moonwalk.Interfaces;

namespace Moonwalk.Classes.Entities
{

    /// <summary>
    /// The player's trusty companion
    /// </summary>
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
        public Robot(Vector2 position) : base(position, "../../../Content/Entities/TestEntity", 10, 10)
        {
            physicsState = PhysicsState.Linear;
            SwitchAnimation(Animations.Idle);
            spriteScale = 2;
        
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {           
            base.Update(gameTime, input);
        }

        public override void Input(StoredInput input)
        {
            //Velocity points towards the mouse cursor
            velocity = input.CurrentMouse.Position.ToVector2() - Camera.ApplyOffset(vectorPosition);         
        }

        public Vector2 GetPosition()
        {
            return this.vectorPosition;
        }
    }

}
