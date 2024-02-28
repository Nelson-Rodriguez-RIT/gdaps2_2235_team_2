using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;
using System;
using System.Collections.Generic;

namespace Moonwalk.Classes.Entities.Base
{
    /// <summary>
    /// Contains universal functionality for all entities
    /// </summary>
    internal abstract class Entity {
        // The path the directory containing this entity's file related data in Content
        protected string directory;

        // These rely on file data and only need to be loaded once
        protected static Dictionary<string, string> properties = null;
        protected static Dictionary<int, (int totalSprites, int framesPerSprite)> animationMetadata = null;
        protected static Texture2D spritesheet = null;
        protected static Vector2 spriteSize;

        // Holds Animations enum values for the current animation
        protected int activeAnimation;
        
        // Subsection of the sprite sheet to draw
        protected Vector2 spritePosition;

        // Entity's position
        protected Vector2 position;


        public Entity(
                Vector2 position, 
                string directory,
                int activeAnimation = 0) {
            this.position = position;
            this.directory = directory;
            this.activeAnimation = activeAnimation;

            if (properties == null) { // Load data if it isn't already loaded
                (Dictionary<string, string> properties,
                Dictionary<int, (int totalSprites, int framesPerSprite)> animationMetadata,
                Texture2D spritesheet, Vector2 spriteSize) 
                    bufferedData = Loader.LoadEntity(directory);

                properties = bufferedData.properties;
                animationMetadata = bufferedData.animationMetadata;
                spritesheet = bufferedData.spritesheet;
                spriteSize = bufferedData.spriteSize;
            }
        }
        

        public virtual void Update(
                GameTime gt, 
                KeyboardState kbState,
                MouseState msState) {
            // Determines how much to increment the frame by. Based on 60 FPS
            float deltaTime =
                (float)gt.ElapsedGameTime.TotalSeconds *
                (60 / animationMetadata[activeAnimation].framesPerSprite);

            spritePosition.Y = (int) activeAnimation;
            spritePosition.X =
                // Updates this value, potentially resetting it to 0 if it trys to point 
                // to a sprite that does not exist (basically restarting the animation)
                (spritePosition.X += deltaTime) < animationMetadata[activeAnimation].totalSprites ?
                    spritePosition.X : 0;
        }

        public virtual void Draw(SpriteBatch sb, Vector2 globalScale) {
            sb.Draw(
                spritesheet,
                new Rectangle( // Position
                    (int)position.X,
                    (int)position.Y,
                    (int)(spriteSize.X * globalScale.X),
                    (int)(spriteSize.Y * globalScale.Y)),
                new Rectangle( // Specific sprite from spritesheet
                    (int)(Math.Floor(spritePosition.X) * spriteSize.X),
                    (int)(spritePosition.Y * spriteSize.Y),
                    (int)spriteSize.X,
                    (int)spriteSize.Y),
                Color.White);
        }
    }
}
