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

namespace Moonwalk.Classes.Entities.Base
{
    /// <summary>
    /// An entity that is controlled by the player
    /// </summary>
    internal abstract class PlayerControlled : Entity, IControllable, ICollidable
    {
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

                        return      //Use the magnitude of the velocity to get the accuracy
                    (int)(
                        Math.Ceiling(
                            Math.Sqrt(
                                Math.Pow(velocity.X, 2) +
                                Math.Pow(Velocity.Y, 2))
                            / 4.0
                            )
                    );
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

        public PlayerControlled(Vector2 position, string directory, int width, int height) 
            : base(position, directory, width, height)
        {
            physicsState = PhysicsState.Linear;
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            Input(input);
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

        public abstract void Input(StoredInput input);

        /// <summary>
        /// Determines if the entity collided with terrain
        /// </summary>
        /// <returns>True if a collision occurred</returns>
        public virtual bool CheckCollision()
        {
            bool temp = Map.Geometry.Exists(terrain => terrain.Hitbox.Intersects(entity));

            if (temp)
            {
                Terrain intersectedTerrain = Map.Geometry.Find(terrain => terrain.Hitbox.Intersects(entity));
            }

            return temp;
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

        public virtual bool CheckCollision<T>(Rectangle rectangle, List<T> list) where T : Entity
        {
            bool temp = list.Exists(item => item.Hitbox.Intersects(entity));

            if (temp)
            {
                T intersected = list.Find(item => item.Hitbox.Intersects(entity));
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

        protected override void RotationalMotion(GameTime gt)
        {
            Vector2 oldPosition = new Vector2(vectorPosition.X, vectorPosition.Y);
            base.RotationalMotion(gt);

            if (CheckCollision())           // If there is a collision, switch back to linear motion
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

                LinearMotion(gt);
            }
        }

        
    }
}
