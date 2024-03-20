﻿using System;
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
            /*
            Idle,
            Idle_Blink,
            Walk,
            Run,
            Crouch,
            Jump,
            Hurt,
            Death,
            Attack,
            */

            Idle,
            TransitionToMove,
            Move
        }      

        //Change this to private later
        public Robot(Vector2 position, Object[] args) : base(position, "../../../Content/Entities/Robot")
        {
            physicsState = PhysicsState.Linear;
            SwitchAnimation(Animations.Idle);
            spriteScale = 2;
        
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {           
            base.Update(gameTime, input);

            if (velocity.X != 0
                && Math.Abs(velocity.X) < 0.5f)
            //&& Math.Sign(acceleration.X) != Math.Sign(velocity.X))
            {
                velocity.X = 0;
                acceleration.X = 0;
            }

            if (velocity.Y > 0 || velocity.X > 0)
            {
                SwitchAnimation(Animations.Move);
            }
            else
            {
                SwitchAnimation(Animations.Idle);
            }

            
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
