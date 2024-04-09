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

    internal abstract class Enemy : Entity, IHostile, IDamageable, ISoft
    {
        //For Enemies specifically
        protected int health;                         // enemy health       
        protected int damage;


        /// <summary>
        /// Property to determine how many checks to do when checking for collision
        /// </summary>
        

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
            damage = int.Parse(properties["Damage"]);
            health = int.Parse(properties["Health"]);
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
        /// Updates movement of enemy
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime, StoredInput input)
        {
            AI();
            Movement(gameTime);
            base.Update(gameTime, input);

            if (health <= 0)
            {
                GameManager.DespawnEntity(this);
            }
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

            if (CheckCollision<ISolid>())           // If there is a collision, switch back to linear motion
            {
                vectorPosition = oldPosition;
                SetLinearVariables();

            }
        }

        #endregion

        public virtual void TakeDamage(int damage)
        {
            health -= damage;
        }

        public abstract void AI();
    }        
}
