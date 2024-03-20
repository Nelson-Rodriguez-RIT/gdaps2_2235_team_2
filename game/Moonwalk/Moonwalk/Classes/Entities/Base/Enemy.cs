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

namespace Moonwalk.Classes.Entities.Base
{
    
    internal abstract class Enemy : Entity, IHostile
    {
        //For Enemies specifically
        int health;                         // enemy health       
        int damage;

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

        public int Damage
        {
            get { return damage; }
        }

        /// <summary>
        /// Parameterized Constructor of moving enemy
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public Enemy(string directory, Vector2 position, int health, int damage)
            : base(position, directory)
        {
            this.health = health;
            this.damage = damage;
        }

        /// <summary>
        /// Returns the health of the enemy
        /// </summary>
        public int Health 
        {
            get 
            { 
                return health; 
            } 
            set 
            { 
                health = value; 
            } 
        }

        /// <summary>
        /// Checks for collision between collectibles and player
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool CheckPlayerCollision(Player check) //will check if player collides with this enemy
        {

                if (entity.Intersects(check.Hitbox))
                {
                    return true;
                }

                else
                {
                    return false;
                }
            

        }

        /// <summary>
        /// Determines if the entity collided with terrain
        /// </summary>
        /// <returns>True if a collision occurred</returns>
        public virtual bool CheckCollision()
        {
            // checks if there is a terrain that collides with this
            bool collision = Map.Geometry.Exists(terrain => terrain.Hitbox.Intersects(entity));

            if (collision)
            {
                // for testing purposes
                Terrain intersectedTerrain = Map.Geometry.Find(terrain => terrain.Hitbox.Intersects(entity));
            }           

            return collision;
        }       

        /// <summary>
        /// Updates movement of enemy
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime, StoredInput input)
        {
            Movement(gameTime);
            base.Update(gameTime, input);
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
                    break;
                }
                iterationCounter++;

            }
        }


    }        
}
