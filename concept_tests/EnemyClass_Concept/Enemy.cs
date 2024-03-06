using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorkForEnemyClass
{
    internal class Enemy
    {
        //Fields
        bool active;
        Texture2D asset;
        Rectangle position;
        Color color;
        Vector2 spritePosition;


        // Animation
        int frame;              // The current animation frame
        double timeCounter;     // The amount of time that has passed
        double fps;             // The speed of the animation
        double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image) //CHANGE FOR SPECIFICS
        const int WalkFrameCount = 3;       // The number of frames in the animation
        const int EnemyRectOffsetY = 116;   // How far down in the image are the frames?
        const int EnemyRectHeight = 72;     // The height of a single frame
        const int EnemyRectWidth = 44;      // The width of a single frame

        //For Enemies specifically
        int health;
        int speed;
        bool isLeft;
        int count;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public Enemy(Texture2D asset, Rectangle position, Color color, int speed, bool isLeft, int health) //figure out how to get this to work
            : base(asset, position, color) //may get rid of levelScore & totalScore
        {
            active = true;
            this.position = position;
            this.color = color;
            this.asset = asset;
            this.speed = speed;
            this.isLeft = isLeft;
            count = 0;
            this.health = health;
            spritePosition = new Microsoft.Xna.Framework.Vector2(position.X, position.Y);
        }

        /// <summary>
        /// Returns the health of the enemy
        /// </summary>
        public int Health { get { return health; } set { health = value; } }

        /// <summary>
        /// Checks for collision between collectibles and player
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool CheckCollision(Player check) //will check if player collides with this enemy
        {
            if (active)
            {
                if (position.Intersects(check.Position))
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
        /// Updates enemies' animation as necessary
        /// </summary>
        /// <param name="gameTime">Time information</param>
        public void UpdateAnimation(GameTime gameTime)
        {
            // Handle animation timing
            // - Add to the time counter
            // - Check if we have enough "time" to advance the frame

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
                spritePosition.Zero,                   // - Origin inside the image (top left)
                spritePosition.One,                    // - Scale (100% - no change)
                SpriteEffects.FlipHorizontally, // - Can be used to flip the image
                0);                             // - Layer depth (unused)
                }

                if (!isLeft) //Sprite stuff in here will change
                {
                    spriteBatch.Draw(
                asset,
                spritePosition, //figure out how to fix the issue with the Vector2's
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

                if (speed == 0) //Sprite stuff in here will change
                {
                    spriteBatch.Draw(asset, position, color);
                }
            }
        }

        /// <summary>
        /// Updates movement of enemy
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime) //needs a parent class
        {
            if (speed > 0)
            {
                if (isLeft)
                {
                    position.Y -= speed;
                    count -= speed;
                }
                else
                {
                    position.Y += speed;
                    count += speed;
                }

                if (count < 200)
                {
                    isLeft = true;
                }

                else if (count > 200)
                {
                    isLeft = false;
                }
            }
        }
    }
}
}