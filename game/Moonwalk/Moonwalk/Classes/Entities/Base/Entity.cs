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
    
        protected enum PhysicsState
        {
            Linear,
            Rotational
        }

        // Contains the entity's sprite table and position
        protected Rectangle entity;

        protected float gravity;
        /// <summary>
        /// Determines whether or not an entity moves linearly or radially
        /// </summary>
        protected PhysicsState physicsState;

        //linear motion
        protected Vector2 vectorPosition;
        protected Vector2 velocity;
        protected Vector2 acceleration;
        protected int maxYVelocity;
        protected int maxXVelocity;

        //Rotational motion  
        protected double theta;
        protected double angVelocity;
        protected double angAccel;
        protected float swingRadius;
        protected Vector2 pivot;

        //Animation
        protected Texture2D spriteSheet;

        public virtual Rectangle Hitbox
        {
            get { return entity; }
        }

        public virtual Point Position
        {
            get { return entity.Location; }
        }

        public virtual Vector2 Velocity
        {
            get { return velocity; }
        }

        public virtual Vector2 Acceleration
        {
            get { return acceleration; }
        }

        // These rely on file data and only need to be loaded once
        protected static Dictionary<string, string> properties = null;
        protected static List<Animation> animations = null;
        protected static Texture2D spritesheet = null;

        // Currently displayed animation
        protected Animation activeAnimation; // DO NOT manually change this, use SwitchAnimation() instead

        protected int spriteScale;

        public Entity(Vector2 position, string directory) {
            physicsState = PhysicsState.Linear;
            vectorPosition = position;
            this.entity = new Rectangle(vectorPosition.ToPoint(), new Point(100, 100));
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            gravity = 0f;
            maxYVelocity = int.MaxValue;
            maxXVelocity = int.MaxValue;

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
                StoredInput input) {
            activeAnimation.UpdateAnimation(gameTime);
        }

        /// <summary>
        /// Switches the animation currently playing to another
        /// </summary>
        /// <param name="animation">The animation to switch to</param>
        protected void SwitchAnimation(Enum animationEnum) {
            activeAnimation = animations[Convert.ToInt32(animationEnum)];
            activeAnimation.Reset();
        }

        /// <summary>
        /// Switches the animation currently playing to another
        /// </summary>
        /// <param name="animation">The animation to switch to</param>
        protected void SwitchAnimation(Animation animation) {
            activeAnimation = animation;
            animation.Reset();
        }

        public virtual void Draw(SpriteBatch batch, Vector2 globalScale)
        {
            if (spriteScale == 0)
            {
                throw new Exception("You forgot to set the sprite scale");
            }

            //apply offset
            Vector2 temp = Camera.ApplyOffset(vectorPosition);

            activeAnimation.Draw(batch, globalScale * spriteScale, spritesheet, temp);
        }

        /// <summary>
        /// Move an entity linearly
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void LinearMotion(GameTime gameTime)
        {
            //Update velocity
            velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Update position
            vectorPosition += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Apply offset
            vectorPosition = Camera.ApplyOffset(vectorPosition);
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
            angVelocity += angAccel * gameTime.ElapsedGameTime.TotalSeconds /** gameTime.ElapsedGameTime.TotalSeconds*/;
            theta += angVelocity * gameTime.ElapsedGameTime.TotalSeconds;

            //Determine new position using the new angle
            Vector2 temp = new Vector2(
                    (float)(pivot.X + swingRadius * Math.Cos((Math.PI / 180) * (theta))),
                    (float)(pivot.Y + swingRadius * Math.Sin((Math.PI / 180) * (theta))
                    ));


            vectorPosition = temp;
        }

        /// <summary>
        /// To be called when an entity switches from linear motion to rotational
        /// </summary>
        /// <param name="centerOfCircle"></param>
        public void SetRotationalVariables(Vector2 centerOfCircle)
        {
            this.pivot = centerOfCircle;

            //Define the vector between the player and the companion
            Vector2 hypotenuse = new Vector2(
                pivot.X - vectorPosition.X,
                pivot.Y - vectorPosition.Y);

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

            //The rest of the code in this is for determining the 
            //entity's initial velocity when they start swinging. You need to 
            //get the component of the entity's current velocity that is 
            //perpendicular to the radius. A diagram is very helpful for understanding.

            //Magnitude of the entity's velocity
            double velocityMag = Math.Sqrt(
                Math.Pow(velocity.X, 2) +
                Math.Pow(velocity.Y, 2));

            //The length of the side opposite the angle we want
            double c = Math.Sqrt(
                Math.Pow(
                    hypotenuse.X -
                    velocity.X,
                    2) +
                Math.Pow(
                    hypotenuse.Y -
                    velocity.Y,
                    2));

            //Use law of cosines to get the angle
            double angleBetween = -90f + (180 / Math.PI) * Math.Acos(
                (swingRadius * swingRadius +
                velocityMag * velocityMag -
                c * c) /
                (2 * swingRadius * velocityMag));

            //Get the component of the velocity perpendicular to the radius
            double newVMag = velocityMag * Math.Cos(
                (Math.PI / 180) *
                angleBetween);

            //Set the initial angular velocity
            angVelocity = (newVMag * 3000 * -Math.Sign(hypotenuse.X)) / swingRadius;

        }
    }
}
