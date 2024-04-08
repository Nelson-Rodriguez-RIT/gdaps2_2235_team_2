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
    internal abstract class Projectile : Entity, IHostile
    {
        protected int damage;
        protected int collisions;
        protected double timer;

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

        public double Timer
        {
            get { return timer; }
        }

        public int Damage
        {
            get { return damage; }
        }

        public Projectile(Vector2 position, string directory, Vector2 direction, float speed, int damage, int collisions = 1, double timer = 5) 
            : base(position, directory, false, false) 
        { 
            velocity = Vector2.Normalize(direction) * speed;
            this.damage = damage;
            this.collisions = collisions;
            this.timer = timer;
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            AI();
            Movement(gameTime);
            base.Update(gameTime, input);

            timer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (timer <= 0 || collisions == 0) 
            { 
                GameManager.DespawnEntity(this);
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

            foreach (ISolid solid in GameManager.entities.GetAllOfType<ISolid>())
            {
                if (solid.Hitbox.Intersects(hurtbox))
                {
                    return true;
                }
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

        protected override void LinearMotion(GameTime gt)
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

                hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);

                if (CheckCollision())
                {
                    hurtbox = new Rectangle(lastSafePosition, hurtbox.Size);
                    vectorPosition = lastSafePosition.ToVector2();
                    velocity.X = -velocity.X;
                    collisions--;
                    break;
                }
                iterationCounter++;

            }
        }

        protected override void RotationalMotion(GameTime gt)
        {
            Vector2 oldPosition = new Vector2(vectorPosition.X, vectorPosition.Y);
            double oldTheta = theta;

            //Determine the angular acceleration using the perpendicular component of gravity
            angAccel = gravity * 10 * Math.Cos((Math.PI / 180) * theta);

            //Update velocity with acceleration and position with velocity
            angVelocity += angAccel * gt.ElapsedGameTime.TotalSeconds;

            theta += angVelocity * gt.ElapsedGameTime.TotalSeconds;

            //Determine new position using the new angle
            Vector2 temp = new Vector2(
                    (float)(pivot.X + swingRadius * Math.Cos((Math.PI / 180) * (theta))),
                    (float)(pivot.Y + swingRadius * Math.Sin((Math.PI / 180) * (theta))
                    ));


            vectorPosition = temp;

            //Update position
            hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);

            if (CheckCollision())           // If there is a collision, switch back to linear motion
            {
                collisions--;
                vectorPosition = oldPosition;
                SetLinearVariables();

            }
        }

        public abstract void AI();

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(
                spriteSheet,
                Camera.RelativePosition(hurtbox.Center),
                null,
                Color.White,
                0f,
                new Vector2(0, 0),
                new Vector2(1f, 1),
                SpriteEffects.None,
                0
                );
        }
    }
}
