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
        protected static List<Animation> animations = null;
        protected static Texture2D spritesheet = null;

        // Currently displayed animation
        protected Animation activeAnimation; // DO NOT manually change this, use SwitchAnimation() instead

        // Entity's position
        protected Vector2 position;

        protected int spriteScale;

        public Entity(Vector2 position, string directory) {
            this.position = position;
            this.directory = directory;

            if (properties == null) { // Load data if it isn't already loaded
                (Dictionary<string, string> properties, List<Animation> animations,
                Texture2D spritesheet) bufferedData = Loader.LoadEntity(directory);

                properties = bufferedData.properties;
                animations = bufferedData.animations;
                spritesheet = bufferedData.spritesheet;
            }
        }
        

        public virtual void Update(
                GameTime gameTime, 
                KeyboardState kbState,
                MouseState msState) {
            activeAnimation.UpdateAnimation(gameTime);
        }

        public virtual void Draw(SpriteBatch batch, Vector2 globalScale) {
            activeAnimation.Draw(batch, globalScale * spriteScale, spritesheet, position);
        }

        protected void SwitchAnimation(Enum animationEnum) {
            activeAnimation = animations[Convert.ToInt32(animationEnum)];
            activeAnimation.Reset();
        }

        protected void SwitchAnimation(Animation animation) {
            activeAnimation = animation;
            animation.Reset();
        }

    }
}
