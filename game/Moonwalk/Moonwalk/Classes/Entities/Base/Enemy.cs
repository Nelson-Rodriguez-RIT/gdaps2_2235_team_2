using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Managers;

namespace Moonwalk.Classes.Entities.Base
{
    
    internal class Enemy : Entity, ICollidable
    {
        //Fields
        bool active;                        //whether the enemy is there

        /*
        // Animation
        int frame;              // The current animation frame
        double timeCounter;     // The amount of time that has passed
        double fps;             // The speed of the animation
        double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image) //CHANGE FOR SPECIFICS - maybe as parameters taken in (frameNum, rectOffsetY, rectHeight, rectWidth)
        const int WalkFrameCount = 3;       // The number of frames in the animation
        const int EnemyRectOffsetY = 116;   // How far down in the image are the frames?
        const int EnemyRectHeight = 72;     // The height of a single frame
        const int EnemyRectWidth = 44;      // The width of a single frame
        */

        //For Enemies specifically
        int health;                         // enemy health
        int speed;                          // Enemy speed of movement
        bool isLeft;                        // Whether enemy is facing left - can be changed
        int count;                          // count for how far enemy has moved
        int countMax;                       // maximum of enemy movement in one direction

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

        /// <summary>
        /// Parameterized Constructor of moving enemy
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public Enemy(string directory, Vector2 position, int speed, bool isLeft, int health, int countMax,
            int width, int height)
            : base(position, directory)
        {
            active = true;
            this.speed = speed;
            this.isLeft = isLeft;
            count = 0;
            this.health = health;
            this.countMax = countMax;
        }

        /*
        /// <summary>
        /// Parameterized Constructor of still enemy
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public Enemy(Texture2D asset, Rectangle position, bool isLeft, int health)
            : base(asset, position, health)
        {
            active = true;
            this.position = position;
            this.asset = asset;            
            this.isLeft = isLeft;
            this.health = health;
            spritePosition = new Microsoft.Xna.Framework.Vector2(position.X, position.Y);
        }
        */

        /// <summary>
        /// Returns the health of the enemy
        /// </summary>
        public int Health { get { return health; } set { health = value; } }

        /// <summary>
        /// Checks for collision between collectibles and player
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool CheckPlayerCollision(Player check) //will check if player collides with this enemy
        {
            if (active)
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

        // All of this is unnecessary as it is done in Animation. Thanks Nelson! - Dante
        /*
        /// <summary>
        /// Updates enemies' animation as necessary
        /// </summary>
        /// <param name="gameTime">Time information</param>
        public void UpdateAnimation(GameTime gameTime)
        {
            // How much time has passed?  
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (timeCounter >= timePerFrame)
            {
                frame += 1;                     // Adjust the frame to the next image

                if (frame > WalkFrameCount)     // Check the bounds - have we reached the end of walk cycle?
                    frame = 1;                  // Back to 1 (since 0 is the "standing" frame)

                timeCounter -= timePerFrame;    // Remove the time we "used" - don't reset to 0
                                                // This keeps the time passed 
            }
        }

        /// <summary>
        /// Draws specific spriteBatch for Collectibles
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch) //needs a parent class
        {
            if (active)
            {
                //If enemy should be facing left
                if (isLeft) //Sprite stuff in here will change
                {
                    spriteBatch.Draw(
                asset,                          // - The texture to draw
                spritePosition,                 // - The location to draw on the screen
                new Rectangle(                  // - The "source" rectangle
                    0,                          //   - This rectangle specifies
                    EnemyRectOffsetY,           //	   where "inside" the texture
                    EnemyRectWidth,             //     to get pixels (We don't want to
                    EnemyRectHeight),           //     draw the whole thing)
                Color.White,                    // - The color
                0,                              // - Rotation (none currently)
                spritePosition.Zero,            // - Origin inside the image (top left)
                spritePosition.One,             // - Scale (100% - no change)
                SpriteEffects.FlipHorizontally, // - Can be used to flip the image
                0);                             // - Layer depth (unused)
                }

                //If enemy should be facing right
                if (!isLeft) //Sprite stuff in here will change
                {
                    spriteBatch.Draw(
                asset,
                spritePosition,
                new Rectangle( 
                    0, 
                    EnemyRectOffsetY,
                    EnemyRectWidth, 
                    EnemyRectHeight),
                Color.White,
                0,
                spritePosition.Zero,
                spritePosition.One, //might change for scaling
                SpriteEffects.None,
                0);
                }

                //if enemy has a speed of 0
                if (speed == 0) //Sprite stuff in here will change
                {
                    spriteBatch.Draw(asset, position, Color.White);
                }
            }
        }
        */

        /// <summary>
        /// Updates movement of enemy
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime, StoredInput input) //needs a parent class
        {
            //updates enemy's movement if speed is greater than 0 or not equal to null
            if (speed > 0)
            {
                //changes position of enemy
                if (isLeft)
                {
                    vectorPosition.Y -= speed;
                    count -= speed;
                }

                else
                {
                    vectorPosition.Y += speed;
                    count += speed;
                }

                //changes direction of enemy
                if (count < (- countMax))
                {
                    isLeft = true;
                }

                else if (count > countMax)
                {
                    isLeft = false;
                }
            }
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
