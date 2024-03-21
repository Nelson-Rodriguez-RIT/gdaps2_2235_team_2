using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Helpful_Stuff;
using System.IO;

namespace Moonwalk.Classes.Entities.Base
{
    internal abstract class Projectile : Entity, IHostile
    {
        protected int damage;
        protected int collisions;

        /// <summary>
        /// Property to determine how many checks to do when checking for collision
        /// </summary>
        public int CollisionAccuracy
        {
            get
            {
                switch (physicsState)
                {
                    case PhysicsState.Linear:

                        // Min accuracy is 1
                        if (velocity.X == 0 &&
                            velocity.Y == 0)
                        {
                            return 1;
                        }

                        return (int)(VectorMath.VectorMagnitude(velocity) / 4f);  //Use the magnitude of the velocity to get the accuracy

                    case PhysicsState.Rotational:
                        if (angVelocity == 0)
                        {
                            return 1;
                        }
                        return (int)(
                            Math.Abs(angVelocity / 10));
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// Number of collisions before despawn
        /// </summary>
        public int Collisions
        {
            get { return collisions; }
        }

        public int Damage
        {
            get { return damage; }
        }

        public Projectile(Vector2 position, string directory, Vector2 initialVelocity) : base(position, directory) 
        { 
            velocity = initialVelocity;
        }

        /// <summary>
        /// Determines if the entity collided with terrain
        /// </summary>
        /// <returns>True if a collision occurred</returns>
        public virtual bool CheckCollision()
        {
            // checks if there is a terrain that collides with this
            bool collision = Map.Geometry.Exists(terrain => terrain.Hitbox.Intersects(new Rectangle(
                    hitbox.X + (int)Position.X,
                    hitbox.Y + (int)Position.Y,
                    hitbox.Width,
                    hitbox.Height
                    )));

            if (collision)
            {
                // for testing purposes
                Terrain intersectedTerrain = Map.Geometry.Find(terrain => terrain.Hitbox.Intersects(new Rectangle(
                    hitbox.X + (int)Position.X,
                    hitbox.Y + (int)Position.Y,
                    hitbox.Width,
                    hitbox.Height
                    )));
            }

            return collision;
        }

        /// <summary>
        /// Determines if the entity collided with terrain
        /// </summary>
        /// <returns>True if a collision occurred</returns>
        public virtual bool CheckCollision(Rectangle rectangle)
        {
            bool temp = Map.Geometry.Exists(terrain => terrain.Hitbox.Intersects(rectangle));

            if (temp)
            {
                //for debugging
            }

            return temp;
        }

        public virtual void Movement(GameTime gt)
        {
            float time = (float)gt.ElapsedGameTime.TotalSeconds;

            int iterationCounter = 1;       // Number of collision checks we've done

            Point lastSafePosition = new Point(Position.X, Position.Y);        //Last point before a collision

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
                    velocity.Y = -velocity.Y;
                    collisions--;
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
                    velocity.X = -velocity.X;
                    collisions--;
                    break;
                }
                iterationCounter++;

            }
        }

        public abstract void AI(Vector2 target);
    }
}
