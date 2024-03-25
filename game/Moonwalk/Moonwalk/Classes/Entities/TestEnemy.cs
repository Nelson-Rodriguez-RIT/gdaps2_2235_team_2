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
    internal class TestEnemy : Enemy, IDamageable, IJump
    {
        private enum Animations
        {
            StaticIdle,
            Wake,
            Move,
            Charge,
            Shoot,
            Dash,
            Damaged,
            Death
        }

        private enum Abilities
        {
            Jump
        }

        FaceDirection faceDirection;

        private AbilityCooldowns<Abilities> cooldowns;

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

        public override void Update(GameTime gameTime, StoredInput input)
        {
            base.Update(gameTime, input);
            cooldowns.Update(gameTime);
        }

        public TestEnemy(Vector2 position) : base(position, "../../../Content/Entities/TestEnemy")
        {
            health = int.Parse(properties["Health"]);
            damage = int.Parse(properties["Damage"]);
            SwitchAnimation(Animations.Move);
            gravity = 70f;
            acceleration = new Vector2(0, gravity);
            spriteScale = 1;
            maxXVelocity = 50;
            cooldowns = new(directory, 1.5);
        }

        public override void AI(Vector2 target)
        {
            double distance = VectorMath.VectorMagnitude(VectorMath.VectorDifference(vectorPosition, target));

            if (distance < 200) // range of aggrod
            {
                SwitchAnimation(Animations.Move, false);
                float xDifference = VectorMath.VectorDifference(vectorPosition, target).X;

                if (xDifference > 0)
                {
                    faceDirection = FaceDirection.Right;
                }
                else if (xDifference < 0)
                {
                    faceDirection = FaceDirection.Left;
                }

                //activeAnimation.FaceDirection = (int)faceDirection;

                acceleration.X = 60 * (faceDirection == FaceDirection.Right ? 1 : -1);
            }
            else if (activeAnimation.AnimationValue != (int)Animations.StaticIdle)
            {
                velocity.X = 0;
                acceleration.X = 0;
                SwitchAnimation(Animations.StaticIdle);
            }
        }

        public virtual void TakeDamage(int damage)
        {
            health -= damage;
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
                if (!CheckCollision())
                {
                    lastSafePosition = new Point(Position.X, Position.Y);      //Store old position in case we collide
                }

                //Cap velocity
                if (Math.Abs(velocity.Y) > maxYVelocity)
                {
                    velocity.Y = maxYVelocity * Math.Sign(velocity.Y);
                }

                vectorPosition.Y += velocity.Y * (time * iterationCounter / CollisionAccuracy);     // Increment position

                entity = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    entity.Width,
                    entity.Height);                      // Update hitbox location

                if (CheckCollision())                                                   // Check if there was a collision
                {
                    entity = new Rectangle(lastSafePosition, entity.Size);              // Revert hitbox position back to before collision
                    vectorPosition = lastSafePosition.ToVector2();                      // Revert position
                    velocity.Y = 0;
                    break;
                }

                iterationCounter++;
            }


            //Do the same thing but in the X direction
            iterationCounter = 1;

            while (!CheckCollision() && iterationCounter <= CollisionAccuracy)
            {
                if (!CheckCollision())
                {
                    lastSafePosition = new Point(Position.X, Position.Y);
                }

                //Cap velocity
                if (Math.Abs(velocity.X) > maxXVelocity)
                {
                    velocity.X = maxXVelocity * Math.Sign(velocity.X);
                }

                vectorPosition.X += velocity.X * (time * iterationCounter / CollisionAccuracy);

                entity = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    entity.Width,
                    entity.Height);

                if (CheckCollision())
                {
                    entity = new Rectangle(lastSafePosition, entity.Size);
                    vectorPosition = lastSafePosition.ToVector2();
                    velocity.X = 0;
                    
                    if (Grounded && cooldowns[Abilities.Jump] == 0)
                    {
                        //Jump!
                        velocity.Y = -42;
                        cooldowns.UseAbility(Abilities.Jump);
                    }

                    break;
                }
                iterationCounter++;

            }
        }

    }
}
