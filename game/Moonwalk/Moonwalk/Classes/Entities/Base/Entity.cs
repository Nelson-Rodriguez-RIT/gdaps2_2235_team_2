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
<<<<<<< Updated upstream
    internal abstract class Entity {
        // The path the directory containing this entity's file related data in Content
        protected string directory;
=======
    internal abstract class Entity
    {
        protected enum PhysicsState
        {
            Linear,
            Rotational
        }

        // Contains the entity's sprite table and position
        protected Rectangle entity;
        protected Rectangle hitbox;

        protected float gravity;
        protected PhysicsState physicsState;

        //linear motion
        protected Vector2 position;
        protected Vector2 velocity;
        protected Vector2 acceleration;
        protected float terminalVelocity;
        protected float maxXVelocity;

        //Rotational motion  *** This stuff can be moved to the player if we want since we don't have any other rotating things***
        protected double theta;
        protected double angVelocity;
        protected double angAccel;
        protected float swingRadius;
        protected Vector2 pivot;

        //Animation
        protected Texture2D spriteSheet;

        public virtual Vector2 Position
        {
            get { return position; }
        }

        public virtual Vector2 Velocity
        {
            get { return velocity; }
        }

        public virtual Vector2 Acceleration
        {
            get { return acceleration; }
        }
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
=======
        public abstract void Draw(SpriteBatch sb, Vector2 globalScale);

        protected virtual void LinearMotion(GameTime gameTime)
        {
            //Add collision later
            velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Movement for a swinging entity (enemy, obstacle or player, presumably)
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void RotationalMotion(GameTime gameTime)
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

            /*
            //Check for collision
            foreach (Collider collider in levelManager.Platforms)
            {
                if (new Rectangle(
                (int)temp.X - hitbox.Width / 2,
                (int)temp.Y - hitbox.Height / 2,
                hitbox.Width,
                hitbox.Height)
                    .Intersects(collider.Hitbox))
                {
                    physicsState = PhysicsState.Linear;
                    //Convert back to linear motion
                    velocity = new Vector2(                                       // 3000: random number for downscaling (it was too big)
                    (float)(angVelocity * swingRadius * -Math.Sin((Math.PI / 180) * (theta)) / 3000),
                    (float)(angVelocity * swingRadius * Math.Cos((Math.PI / 180) * (theta))) / 3000);
                    acceleration = new Vector2(
                        acceleration.X, gravity);
                    return;
                }
            }
            */ //Collision checking


            //update position
            position = temp;
        }
>>>>>>> Stashed changes
    }
}
