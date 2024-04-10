using Microsoft.Xna.Framework;
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
    //- Update any more code within class
    //- Maybe change so it only walks back and forth?
    //- figure out how to get the flower to switch between attack and move animations
    //- figure out how to get the animations to loop

    internal class Flower : Enemy
    {
        private enum Animations
        {
            Move,
            Attack,
            Hit,
            Death
        }

        FaceDirection faceDirection;

        private bool inactive = true;
        private double timer;
        private AbilityCooldowns<Animations> cooldowns;

        public override void Update(GameTime gameTime, StoredInput input)
        {
            base.Update(gameTime, input);
            //cooldowns.Update(gameTime);
            if (timer > 0)
            {
                timer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (timer < 0)
            {
                timer = 0;
            }
        }

        public Flower(Vector2 position) : base(position, "../../../Content/Entities/FlowerEnemy")
        {
            health = int.Parse(properties["Health"]);
            damage = int.Parse(properties["Damage"]);
            SwitchAnimation(Animations.Move);
            gravity = 70f;
            acceleration = new Vector2(0, gravity);
            spriteScale = 1;
            maxXVelocity = 50;

        }

        /// <summary>
        /// AI for Flower Enemy
        /// </summary>
        public override void AI()
        {
            double distance = VectorMath.Magnitude(VectorMath.Difference(vectorPosition, Player.Location.ToVector2()));

            if (distance <= 200) // range of aggro
            {
                if (inactive)
                {
                    SwitchAnimation(Animations.Move, true);
                    timer = activeAnimation.AnimationLengthSeconds;
                    inactive = false;
                }

                if (timer > 0)
                {
                    if (activeAnimation.AnimationValue == (int)Animations.Move)
                        return;
                }
                else if (activeAnimation.AnimationValue == (int)Animations.Move)
                {
                    SwitchAnimation(Animations.Move, true);
                }




                float xDifference = VectorMath.Difference(vectorPosition, Player.Location.ToVector2()).X;

                //Change the facing direction
                if (xDifference > 0)
                {
                    faceDirection = FaceDirection.Right;
                }
                else if (xDifference < 0)
                {
                    faceDirection = FaceDirection.Left;
                }


                if (physicsState == PhysicsState.Linear)
                    //Enemy accelerates towards the player's x direction
                    acceleration.X = 60 * (faceDirection == FaceDirection.Right ? 1 : -1);

                if (distance <=10 && activeAnimation.AnimationValue != (int)Animations.Attack) //figure out how to get this to work
                {
                    velocity.X = 0;
                    acceleration.X = 0;

                    //Attack
                    SwitchAnimation(Animations.Attack);
                }
                else
                {
                    SwitchAnimation(Animations.Move);
                }
               
            }
            else
            {
                //Deactivate the enemy if out of range
                inactive = true;
                velocity.X = 0;
                acceleration.X = 0;
                SwitchAnimation(Animations.Move, true);
            }

            activeAnimation.FaceDirection = (int)faceDirection;
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

                if (CheckCollision<ISolid>())
                {
                    hurtbox = new Rectangle(lastSafePosition, hurtbox.Size);
                    vectorPosition = lastSafePosition.ToVector2();
                    velocity.X = 0;
                    break;
                }
                iterationCounter++;

            }
        }
    }
}
