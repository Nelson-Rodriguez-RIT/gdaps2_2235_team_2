 using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;
using System.Net.Http.Headers;
using Moonwalk.Classes.Maps;

namespace Moonwalk.Classes.Entities.Base
{
    /// <summary>
    /// An entity that is controlled by the player
    /// </summary>
    internal abstract class PlayerControlled : Entity, IControllable, ICollidable
    {

        public PlayerControlled(Vector2 position, string directory) 
            : base(position, directory)
        {
            physicsState = PhysicsState.Linear;
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            Input(input);
            Movement(gameTime);
            base.Update(gameTime, input);
        }

        #region Movement       

        protected override void LinearMotion(GameTime gt)
        {
            float time = (float)gt.ElapsedGameTime.TotalSeconds;

            int iterationCounter = 1;       // Number of collision checks we've done

            Point lastSafePosition = new Point(Position.X, Position.Y);        //Last point before a collision

            velocity += acceleration * time;                                   //Update velocity

            //Vertical
            while (iterationCounter <= CollisionAccuracy)                      //Scaling number of checks
            {
                if (!CheckCollision<Terrain>())
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


                if (CheckCollision<Terrain>())                                                   // Check if there was a collision
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

            while (!CheckCollision<Terrain>() && iterationCounter <= CollisionAccuracy)
            {
                if (!CheckCollision<Terrain>())
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

                if (CheckCollision<Terrain>())
                {
                    hurtbox = new Rectangle(lastSafePosition, hurtbox.Size);
                    vectorPosition = lastSafePosition.ToVector2();
                    velocity.X = 0;
                    break;
                }
                iterationCounter++;

            }


        }

        protected override void RotationalMotion(GameTime gt)
        {
            Vector2 oldPosition = new Vector2(vectorPosition.X, vectorPosition.Y);
            base.RotationalMotion(gt);

            if (CheckCollision<Terrain>())           // If there is a collision, switch back to linear motion
            {
                vectorPosition = oldPosition;
                physicsState = PhysicsState.Linear;

                //This determines the velocity the entity will have after 
                //it stops swinging by converting the angular velocity
                //back to linear velocity.
                velocity = new Vector2(                                       // 3000: random number for downscaling (it was too big)
                    (float)(angVelocity * swingRadius * -Math.Sin((Math.PI / 180) * (theta)) / 3000),
                    (float)(angVelocity * swingRadius * Math.Cos((Math.PI / 180) * (theta))) / 3000);
                acceleration = new Vector2(
                    acceleration.X, gravity);

                angVelocity = 0;
            }
        }


        #endregion


        public abstract void Input(StoredInput input);


    }
}
