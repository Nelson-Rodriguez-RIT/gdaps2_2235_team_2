using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.VisualBasic;

namespace GameControls_Concept
{
    enum PhysicsState
    {
        Linear,
        Rotational
    }
    internal class MovableEntity : Entity
    {
        protected float gravity;
        protected PhysicsState physicsState;

        //linear motion
        protected Vector2 velocity;
        protected Vector2 acceleration;
        protected float terminalVelocity;
        protected float maxXVelocity;

        //Rotational motion
        protected double theta;
        protected double angVelocity;
        protected double angAccel;
        protected float swingRadius;
        protected Vector2 pivot;


        public virtual Vector2 Velocity
        {
            get { return velocity; }
        }

        public virtual Vector2 Acceleration
        {
            get { return acceleration; }
        }

        /// <summary>
        /// Property to get the accuracy of collision checking based on speed
        /// </summary>
        protected int CollisionAccuracy
        {
            get
            {
                switch (physicsState)
                {
                    case PhysicsState.Linear:
                        return
                    (int)(
                        Math.Sqrt(
                            Math.Pow(velocity.X, 2) +
                            Math.Pow(Velocity.Y, 2))
                    );
                    case PhysicsState.Rotational:
                        return (int)(
                            Math.Abs(angVelocity / 10));
                    default:
                        return 0;
                }
                
            }
        }

        /// <summary>
        /// Constructor for an object that moves linearly
        /// </summary>
        /// <param name="image"></param>
        /// <param name="manager"></param>
        /// <param name="position"></param>
        public MovableEntity(LevelManager manager, Vector2 position)
            : base(manager, position)
        {
            physicsState = PhysicsState.Linear;
            gravity = 0;
            terminalVelocity = 0;
            maxXVelocity = 0;

            velocity = new Vector2(0, 0);
            acceleration = new Vector2(0, 0);
        }

        /// <summary>
        /// Constructor for an object that moves radially
        /// </summary>
        /// <param name="image"></param>
        /// <param name="manager"></param>
        /// <param name="position"></param>
        /// <param name="pivot">The center of the cirle the entity moves on</param>
        public MovableEntity(LevelManager manager, Vector2 position, Vector2 pivot)
            : base(manager, position)
        {
            velocity = new Vector2(0, 0);
            acceleration = new Vector2(0, 0);
            gravity = 0;

            physicsState = PhysicsState.Rotational;
            this.pivot = pivot;

            //Define the vector between the player and the companion
            Vector2 hypotenuse = new Vector2(
                pivot.X - position.X,
                pivot.Y - position.Y);

            //The magnitude of the previous vector,
            //or the radius of the circle on which the player will rotate
            swingRadius =
                    (float)(
                        Math.Sqrt(
                            Math.Pow(hypotenuse.X, 2) +
                            Math.Pow(hypotenuse.Y, 2)
                        )
                    );

            //Get the angle between the player and 0 degrees (right)
            theta = 180 - (float)((180 / Math.PI) * Math.Acos(
                    hypotenuse.X
                    /
                    swingRadius
                    ));
        }


        /// <summary>
        /// Movement for a swinging entity (enemy, obstacle or player, presumably)
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void Swing(GameTime gameTime)
        {
            //Determine the angular acceleration using the perpendicular component of gravity
            angAccel = gravity * 1000 * Math.Cos((Math.PI / 180) * theta);

            //Update velocity with acceleration and position with velocity
            angVelocity += angAccel * gameTime.ElapsedGameTime.TotalSeconds * gameTime.ElapsedGameTime.TotalSeconds;
            theta += angVelocity * gameTime.ElapsedGameTime.TotalSeconds;
            //Collision (doesn't work yet)
            //theta = RotationalMotionCollision(levelManager.Platforms);

            //Determine new position using the new angle
            Vector2 temp = new Vector2(
                    (float)(pivot.X + swingRadius * Math.Cos((Math.PI / 180) * (theta))),
                    (float)(pivot.Y + swingRadius * Math.Sin((Math.PI / 180) * (theta))
                    ));

            //update position
            position = temp;

            //Let me know if you guys have any questions about this, 
            //it's a lot of physics formulas that make more sense 
            //if I can explain it to you while showing you a 
            //picture - Dante :)
        }

        protected Vector2 CheckForPlatformCollision(
             List<Collider> platforms)               // List of platforms to check against     
        {
            //Scaling iterations based on velocity
            int maxIteration = CollisionAccuracy > 1 ? CollisionAccuracy : 1;

            // How many steps it can go before colliding into anything
            int peakXIteration = maxIteration;
            int peakYIteration = maxIteration;

            // Shorten iterations based on current peakIteration TODO
            foreach (Collider platform in platforms) // Check each platform
            {
                for (int iteration = 0; iteration <= maxIteration; iteration++)
                { // Check how many steps it can go before colliding into this platform
                    if (new Rectangle( // Check for horizontal collision
                            (int)(hitbox.X + ((velocity.X) / maxIteration) * iteration),
                            (int)hitbox.Y,
                            hitbox.Width,
                            hitbox.Height)
                            .Intersects(platform.Hitbox))
                        // We want the absolute minimum steps
                        peakXIteration = iteration - 1 < peakXIteration ? iteration - 1 : peakXIteration;

                    if (new Rectangle( // Check for vertical collision
                            (int)hitbox.X,
                            (int)(hitbox.Y + ((velocity.Y) / maxIteration) * iteration),
                            hitbox.Width,
                            hitbox.Height)
                            .Intersects(platform.Hitbox))
                        // We want the absolute minimum steps
                        peakYIteration = iteration - 1 < peakYIteration ? iteration - 1 : peakYIteration;
                }
            }

            // Update position and relevant hitbox based on peakIteration
            Vector2 temp = new Vector2(
                (hitbox.X + hitbox.Width / 2) + (velocity.X / maxIteration) * peakXIteration,
                (hitbox.Y + hitbox.Height / 2) + (velocity.Y / maxIteration) * peakYIteration);

                return temp;

        }

        protected virtual double RotationalMotionCollision(List<Collider> colliders)
        {
            
            //Scaling iterations based on velocity
            int maxIteration = CollisionAccuracy > 1 ? CollisionAccuracy : 1;

            // How many steps it can go before colliding into anything
            int peakIterations = maxIteration;

            // Shorten iterations based on current peakIteration TODO
            foreach (Collider collider in colliders) // Check each platform
            {
                for (int iteration = 0; iteration <= maxIteration; iteration++)
                { // Check how many steps it can go before colliding into this platform
                    Vector2 temp = new Vector2(
                    (float)(pivot.X + swingRadius * Math.Cos((Math.PI / 180) * 
                    (theta + angVelocity / maxIteration * iteration))), 
                    (float)(pivot.Y + swingRadius * Math.Sin((Math.PI / 180) *
                    (theta + angVelocity / maxIteration * iteration)))
                    );

                    if (new Rectangle( // Check for horizontal collision
                            (int)(temp.X - hitbox.Width / 2 + ((angVelocity) / maxIteration) * iteration + angAccel * iteration * iteration),
                            (int)(temp.Y - hitbox.Height / 2 + (angVelocity) / maxIteration * iteration + angAccel * iteration * iteration),
                            hitbox.Width,
                            hitbox.Height)
                            .Intersects(collider.Hitbox))
                        // We want the absolute minimum steps
                        peakIterations = iteration - 1 < peakIterations ? iteration - 1 : peakIterations;

                }
            }

            // Update position and relevant hitbox based on peakIteration
            
            return theta + (angVelocity / maxIteration) * peakIterations + angAccel * peakIterations * peakIterations;
            

        } 

        public virtual void Impulse(Vector2 impulse)
        {
            velocity = impulse;
        }
    }
}
