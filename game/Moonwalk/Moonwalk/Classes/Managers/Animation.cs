using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Runtime.CompilerServices;
using System;


namespace Moonwalk.Classes.Managers {
    public enum AnimationStyle { 
        Horizontal,
        Vertical
    }
    
    internal class Animation {
        private const int AnimationFramerate = 60;

        // Represents the position the box used to take a sprite from the spritesheet
        // We need this in a Vector2 as we need decimal values to simulate animation
        private Vector2 spritesheetBoxPosition;

        private Rectangle spritesheetBox;

        // Dictates where spritesheetBox should be at the start of an animation
        private Vector2 defaultSpritesheetPosition;

        // How many sprites in this animation
        private int totalSprites;

        // How many frames each sprite of the animation gets
        private int framesPerSprite;

        // Width and height an individual sprite
        private Vector2 spriteSize;

        // Acts as an offset for where the sprite is placed. Useful for animation sets with different sizes
        private Vector2 origin;

        // Whether the animation sprites layed out horizontally or vertically on the spritesheet
        private AnimationStyle style;

        public int FaceDirection = 0; // 0 for right, 1 for left

        public int AnimationLength
        {
            get
            {
                return totalSprites * framesPerSprite;
            }
        }

        public int AnimationValue
        {
            get
            {
                if (style == AnimationStyle.Horizontal)
                    return spritesheetBox.Y;

                else return spritesheetBox.X;
            }
        }


        public Animation(
                // This information should be gathered from a file (please don't manually type it)
                Vector2 spriteSize,
                Vector2 origin,
                int totalSprites,
                int framesPerSprite,
                AnimationStyle style,
                int spaceTakenOnSpritesheet  // Used for defaultSpritesheetPosition
                )    {
            this.spriteSize = spriteSize;
            this.origin = origin;
            this.totalSprites = totalSprites;
            this.framesPerSprite = framesPerSprite;
            this.style = style;

            defaultSpritesheetPosition = style == AnimationStyle.Horizontal ?
                new Vector2(0, spaceTakenOnSpritesheet) : new Vector2(spaceTakenOnSpritesheet, 0);

            Reset();
        }


        public void UpdateAnimation(GameTime gameTime) {
            // This values progressivily increments the respective spritesheetBoxPosition so that each whole number
            // represent a sprite in an animation (i.e. 0 is the first sprite in the animation, 1 is the second, etc)
            float deltaTime = 
                (float) gameTime.ElapsedGameTime.TotalSeconds * 
                (AnimationFramerate / framesPerSprite);

            switch (style) {
                case AnimationStyle.Horizontal:
                    spritesheetBoxPosition.X += deltaTime;

                    if (spritesheetBoxPosition.X > totalSprites)
                        Reset();

                    spritesheetBox = new Rectangle(
                        (int)(Math.Floor(spritesheetBoxPosition.X) * spriteSize.X),
                        (int)spritesheetBoxPosition.Y,
                        (int)spriteSize.X,
                        (int)spriteSize.Y);
                    break;

                case AnimationStyle.Vertical:
                    spritesheetBoxPosition.Y += deltaTime;

                    if (spritesheetBoxPosition.Y > totalSprites)
                        Reset();

                    spritesheetBox = new Rectangle(
                        (int)spritesheetBoxPosition.X,
                        (int)(Math.Floor(spritesheetBoxPosition.Y) * spriteSize.Y),
                        (int)spriteSize.X,
                        (int)spriteSize.Y);
                    break;
            }

            spritesheetBox = new Rectangle(
                (int) (Math.Floor(spritesheetBoxPosition.X) *
                        (style == AnimationStyle.Horizontal ? spriteSize.X : 1)),
                (int) (Math.Floor(spritesheetBoxPosition.Y) * 
                        (style == AnimationStyle.Vertical ? spriteSize.Y : 1)),
                (int) spriteSize.X,
                (int) spriteSize.Y
                );
        }

        public void Draw(
                SpriteBatch batch,
                Vector2 scale,
                Texture2D spritesheet, 
                Vector2 position) {

            switch (FaceDirection)
            {
                case 0:
                    batch.Draw(
                spritesheet,
                new Rectangle(
                    (int)(position.X),    // X position
                    (int)(position.Y),    // Y position
                    (int)(spriteSize.X * scale.X),  // Width
                    (int)(spriteSize.Y * scale.Y)), // Height
                spritesheetBox, // Sprite from spritesheet
                Color.White,
                0f,
                origin,
                SpriteEffects.None,
                0);

                    break;
                case 1:

                    batch.Draw(
               spritesheet,
               new Rectangle(
                   (int)(position.X - (int)((spriteSize.X) * scale.X)),
                   (int)(position.Y),
                   (int)(spriteSize.X * scale.X),  // Width
                   (int)(spriteSize.Y * scale.Y)), // Height
               spritesheetBox,
               Color.White,
               0f,
               new Vector2(-24, 13),
               SpriteEffects.FlipHorizontally,
               0);
                    break;
            }             
        }

        public void Reset() {
            spritesheetBoxPosition = new Vector2(
                defaultSpritesheetPosition.X,
                defaultSpritesheetPosition.Y
            );
        }
    }
}
