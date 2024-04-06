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
using Moonwalk.Classes.Maps;

namespace Moonwalk.Classes.Entities.Base
{

    internal abstract class Enemy : Entity, IHostile
    {
        //For Enemies specifically
        protected int health;                         // enemy health       
        protected int damage;


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
        public Enemy(Vector2 position, string directory)
            : base(position, directory)
        {

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
        /// Determines if the entity collided with terrain
        /// </summary>
        /// <returns>True if a collision occurred</returns>
        public virtual bool CheckCollision()
        {
            // checks if there is a terrain that collides with this
            bool collision = Map.Geometry.ToList().Exists(terrain => terrain.Hitbox.Intersects(new Rectangle(
                    hurtbox.X,
                    hurtbox.Y,
                    hurtbox.Width,
                    hurtbox.Height
                    )));

            if (collision)
            {
                // for testing purposes
                Terrain intersectedTerrain = Map.Geometry.ToList().Find(terrain => terrain.Hitbox.Intersects(new Rectangle(
                    hurtbox.X,
                    hurtbox.Y,
                    hurtbox.Width,
                    hurtbox.Height
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
            bool temp = Map.Geometry.ToList().Exists(terrain => terrain.Hitbox.Intersects(rectangle));

            if (temp)
            {
                //for debugging
            }

            return temp;
        }

        /// <summary>
        /// Updates movement of enemy
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime, StoredInput input)
        {
            AI();
            Movement(gameTime);
            base.Update(gameTime, input);
        }

        public virtual void Movement(GameTime time)
        {
            switch (physicsState)
            {
                case PhysicsState.Linear:
                    LinearMotion(time);
                    break;
                case PhysicsState.Rotational:
                    RotationalMotion(time);
                    break;
            }

        }

        private void LinearMotion(GameTime gt)
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

                hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);                      // Update hitbox location

                if (CheckCollision())                                                   // Check if there was a collision
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

                hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);

                if (CheckCollision())
                {
                    hurtbox = new Rectangle(lastSafePosition, hurtbox.Size);
                    vectorPosition = lastSafePosition.ToVector2();
                    velocity.X = 0;
                    break;
                }
                iterationCounter++;

            }
        }

        public abstract void AI();
    }        
}
