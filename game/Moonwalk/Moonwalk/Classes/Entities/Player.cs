﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Managers;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Moonwalk.Classes.Entities
{
    public delegate List<IMovable> OnGravityAbilityUsed();
    public delegate Vector2 GetRobotPosition();
    //public delegate List<IDamageable> OnAttack();

    /// <summary>
    /// The player controlled character
    /// </summary>
    internal class Player : PlayerControlled, IJump
    {
        protected enum Animations
        {
            Idle,
            Run,
            Attack,
            Shoot,
            Hit,
            Death,
            Jump
        }

        protected enum Abilities
        {
            Tether,
            Gravity
        }

        /// <summary>
        /// Cooldowns of each ability
        /// </summary>
        protected AbilityCooldowns<Abilities> cooldowns;

        public event OnGravityAbilityUsed OnGravityAbilityUsed;
        public event GetRobotPosition GetRobotPosition;

        private Animations animation;
        private FaceDirection faceDirection;

        private int animationTimer;

        /// <summary>
        /// Determines if the entity is grounded or not
        /// </summary>
        public bool Grounded
        {
            get
            {
                if (CheckCollision(new Rectangle(
                        hitbox.X + (int)Position.X,
                        hitbox.Y + (int)Position.Y + 5,
                        hitbox.Width,
                        hitbox.Height
                        )))
                {
                    return true;
                }

                return false;
            }
        }

        //Make private later
        public Player(Vector2 position) : base(position, "../../../Content/Entities/Player", 10, 10)
        {
            gravity = 50f;
            acceleration = new Vector2(0, gravity);
            maxXVelocity = 45;
            maxYVelocity = 70;           

            SwitchAnimation(Animations.Idle);
            spriteScale = 1;

            cooldowns = new AbilityCooldowns<Abilities>(directory);
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            cooldowns.Update(gameTime);
            base.Update(gameTime, input);
            animationTimer --;
            ChangeAnimation(input);
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

            if (velocity.X != 0 
                && Math.Abs(velocity.X) < 0.5f 
                && Math.Sign(acceleration.X) != Math.Sign(velocity.X))
            {
                velocity.X = 0;
                acceleration.X = 0;
            }

            
            

  


            //Jump 
            if ((input.IsPressed(Keys.Space)
                && !input.WasPressed(Keys.Space))
                || input.Buffered.Exists(item => item.Key == Keys.Space))
            {
                if (Grounded) 
                {
                    velocity.Y = -60;

                    BufferedInput buffer = input.Buffered.Find(item => item.Key == Keys.Space);
                    input.Buffered.Remove(buffer);
                }   
                else
                {
                    input.Buffer(Keys.Space);
                }
            }

                //Robot abilities:

            //Gravity ability
            if (input.CurrentMouse.LeftButton == ButtonState.Pressed
                && input.PreviousMouse.LeftButton == ButtonState.Released
                && cooldowns.UseAbility(Abilities.Gravity))
            {
                //GravityAbility();
            }

            //Tether ability - planning to have this be able to swing blocks and stuff too, maybe send back projectiles?
            if (physicsState == PhysicsState.Linear
                && input.CurrentMouse.RightButton == ButtonState.Pressed
                && input.PreviousMouse.RightButton == ButtonState.Released
                && cooldowns.UseAbility(Abilities.Tether))
            {

                SetRotationalVariables(GetRobotPosition());

            }
            else if (input.CurrentMouse.RightButton == ButtonState.Released
                && input.PreviousMouse.RightButton == ButtonState.Pressed)
            {
                if (physicsState == PhysicsState.Rotational)
                    SetLinearVariables();
            }

        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);

            batch.DrawString(GameManager.font, 
                $"{vectorPosition.Y} - {velocity.Y} - {acceleration.Y} \n {Position.Y}",
                new Vector2(400, 50),
                Color.White);
        }

        private void ChangeAnimation(StoredInput input)
        {
            if (animationTimer > 0)
            {
                return;
            }
            else
            {
                animationTimer = 0;
            }

            if (velocity.X < 0)
            {
                faceDirection = FaceDirection.Left;
            }
            else if (velocity.X > 0)
            {
                faceDirection = FaceDirection.Right;
            }

            activeAnimation.FaceDirection = (int)faceDirection;

            if (velocity.X == 0 && velocity.Y == 0)
            {
                SwitchAnimation(Animations.Idle, false);
            }

            if (velocity.X != 0)
            {                
                SwitchAnimation(Animations.Run, false);
            }

            if (!Grounded)
            {
                SwitchAnimation(Animations.Jump);
            }

            if (input.IsPressed(Keys.E))
            {
                SwitchAnimation(Animations.Attack, false);
                animationTimer = activeAnimation.AnimationLength;
            }

            if (input.IsPressed(Keys.F))
            {
                SwitchAnimation(Animations.Shoot, false);
                animationTimer = activeAnimation.AnimationLength;
            }
        }

        private void Attack()
        {
            //List<Enemy> enemies = 
        }

        
        private void GravityAbility()
        {
            
            //Get a list of movables from the game manager
            List<IMovable> movables = OnGravityAbilityUsed();

            //Make all entities move towards this
            foreach (IMovable movable in movables)
            {
                //Check that entity is within range
                if (Math.Sqrt(
                        Math.Pow(movable.Position.X - Position.X, 2) +
                        Math.Pow(movable.Position.Y - Position.Y, 2)
                        )
                    < 375)
                    movable.Impulse(GetRobotPosition());
            }
            
        }
        
    }
}
