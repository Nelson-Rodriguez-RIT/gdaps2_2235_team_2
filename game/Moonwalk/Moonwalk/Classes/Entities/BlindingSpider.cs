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
    //TO DO:
    //- edit adf in case the origin points don't work with the hitbox
    //- Update any more code within class
    //- Update location of spritesheet
    //- Maybe change so it only walks back and forth?
    //- figure out where the edit for the pixel size for each animation is

    internal class BlindingSpider : Enemy, IDamageable
    {
        private enum Animations
        {
            StaticIdle,
            BlindAttack,
            Damaged,
            Death,
            PrepBlindAttack,
            Walk
        }

        private enum Abilities
        {
            Blind, //this is something that can be implimented later, part of shoot
            Shoot
        }

        FaceDirection faceDirection;

        private AbilityCooldowns<Abilities> cooldowns;

        

        public override void Update(GameTime gameTime, StoredInput input)
        {
            base.Update(gameTime, input);
            cooldowns.Update(gameTime);
        }

        public BlindingSpider(Vector2 position) : base(position, "../../../Content/Entities/BlindingSpider")
        {
            health = int.Parse(properties["Health"]);
            damage = int.Parse(properties["Damage"]);
            SwitchAnimation(Animations.Walk);
            gravity = 70f;
            acceleration = new Vector2(0, gravity);
            spriteScale = 1;
            maxXVelocity = 50;
            cooldowns = new(properties);
        }

        /// <summary>
        /// AI for Blinding Spider Enemy
        /// </summary>
        public override void AI()
        {
            double distance = VectorMath.VectorMagnitude(VectorMath.VectorDifference(vectorPosition, Player.Location.ToVector2()));

            if (distance < 200) // range of aggro
            {
                SwitchAnimation(Animations.Walk, false);
                float xDifference = VectorMath.VectorDifference(vectorPosition, Player.Location.ToVector2()).X;

                //Change the facing direction
                if (xDifference > 0)
                {
                    faceDirection = FaceDirection.Right;
                }
                else if (xDifference < 0)
                {
                    faceDirection = FaceDirection.Left;
                }

                //activeAnimation.FaceDirection = (int)faceDirection;               

                if (cooldowns[Abilities.Shoot] == 0)
                {
                    //Shoot
                    GameManager.SpawnEntity<StandardProjectile>(new Object[] {
                        vectorPosition,
                    VectorMath.VectorDifference(vectorPosition, Player.Location.ToVector2()) });
                    cooldowns.UseAbility(Abilities.Shoot);
                }
            }
            else if (activeAnimation.AnimationValue != (int)Animations.StaticIdle)
            {
                //Deactivate the enemy if out of range
                velocity.X = 0;
                acceleration.X = 0;
                SwitchAnimation(Animations.StaticIdle);
            }
        }

        public override void Movement(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            int iterationCounter = 1;       // Number of collision checks we've done

            Point lastSafePosition = new Point(Position.X, Position.Y);        //Last point before a collision

            // Increase gravity towards height of the jump
            if (!Grounded)
            {
                acceleration.Y = gravity * (1 + (100 - Math.Abs(velocity.Y)) / 50);
            }

            velocity += acceleration * time;                                   //Update velocity

            //Vertical
            while (iterationCounter <= CollisionAccuracy)                      //Scaling number of checks
            {
                if (!CheckCollision<ISolid>())
                {
                    lastSafePosition = new Point(Position.X, Position.Y);      //Store old position in case we collide
                }

                //Cap velocity
                if (Math.Abs(velocity.Y) > maxYVelocity)
                {
                    velocity.Y = maxYVelocity * Math.Sign(velocity.Y);
                }

                vectorPosition.Y += velocity.Y * (time * iterationCounter / CollisionAccuracy);     // Increment position

                hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);                      // Update hitbox location

                if (CheckCollision<ISolid>())                                                   // Check if there was a collision
                {
                    hurtbox = new Rectangle(lastSafePosition, hurtbox.Size);              // Revert hitbox position back to before collision
                    vectorPosition = lastSafePosition.ToVector2();                      // Revert position
                    velocity.Y = 0;
                    break;
                }

                iterationCounter++;
            }


            //Do the same thing but in the X direction
            iterationCounter = 1;

            while (!CheckCollision<ISolid>() && iterationCounter <= CollisionAccuracy)
            {
                if (!CheckCollision<ISolid>())
                {
                    lastSafePosition = new Point(Position.X, Position.Y);
                }

                //Cap velocity
                if (Math.Abs(velocity.X) > maxXVelocity)
                {
                    velocity.X = maxXVelocity * Math.Sign(velocity.X);
                }

                vectorPosition.X += velocity.X * (time * iterationCounter / CollisionAccuracy);

                hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);

                /*if (CheckCollision())
                {
                    hurtbox = new Rectangle(lastSafePosition, hurtbox.Size);
                    vectorPosition = lastSafePosition.ToVector2();
                    velocity.X = 0;

                    if (Grounded && cooldowns[Abilities.Jump] == 0)
                    {
                        //Jump!
                        velocity.Y = -42;
                        cooldowns.UseAbility(Abilities.Jump);
                    }

                    break;
                }*/
                iterationCounter++;

            }
        }
    }
}
