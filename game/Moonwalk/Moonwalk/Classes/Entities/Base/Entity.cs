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

        protected enum FaceDirection
        {
            Right,
            Left
        }

        // Contains the entity's sprite table and position
        protected Rectangle entity;

        // X and Y represent origin offsets
        protected Rectangle hitbox;

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

        private static Texture2D hitboxSprite = null;

        public virtual Rectangle Hitbox
        {
            get { return hitbox; }
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
        protected Dictionary<string, string> properties = null;
        protected List<Animation> animations = null;
        protected Texture2D spritesheet = null;

        // Currently displayed animation
        protected Animation activeAnimation; // DO NOT manually change this, use SwitchAnimation() instead

        protected int spriteScale;

        public Entity(Vector2 position, string directory, bool loadAnimations = true) {
            physicsState = PhysicsState.Linear;
            vectorPosition = position;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            gravity = 0f;
            maxYVelocity = int.MaxValue;
            maxXVelocity = int.MaxValue;

            this.directory = directory;

            if (properties == null) { // Load data if it isn't already loaded
                (Dictionary<string, string> properties, List<Animation> animations,
                Texture2D spritesheet) bufferedData = Loader.LoadEntity(directory, loadAnimations);

                properties = bufferedData.properties;
                animations = bufferedData.animations;
                spritesheet = bufferedData.spritesheet;
            }

            hitbox = new Rectangle(
                int.Parse(properties["HitboxXOrigin"]),
                int.Parse(properties["HitboxYOrigin"]),
                int.Parse(properties["HitboxX"]),
                int.Parse(properties["HitboxY"])
                );

            if (hitboxSprite == null)
                hitboxSprite = Loader.LoadTexture("../../../Content/Entities/hitbox");
        }

        

        public virtual void Update(
                GameTime gameTime, 
                StoredInput input) 
        {
            activeAnimation.UpdateAnimation(gameTime);
        }

        /// <summary>
        /// Switches the animation currently playing to another
        /// </summary>
        /// <param name="animation">The animation to switch to</param>
        protected void SwitchAnimation(Enum animationEnum, bool resetAnimation = true) {
            activeAnimation = animations[Convert.ToInt32(animationEnum)];
            if (resetAnimation)
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

        public virtual void Draw(SpriteBatch batch)
        {
            if (spriteScale == 0)
            {
                throw new Exception("You forgot to set the sprite scale");
            }

            //apply offset
            Vector2 temp = Camera.ApplyOffset(vectorPosition);

            activeAnimation.Draw(batch, GameMain.ActiveScale, spritesheet, temp);
        }

        
        public void DrawHitbox(SpriteBatch batch) {
            Vector2 position = Camera.ApplyOffset(new Vector2(
                hitbox.X + Position.X,
                hitbox.Y + Position.Y
                ));

            batch.Draw(
                hitboxSprite,
                new Rectangle(
                    (int)(position.X),
                    (int)(position.Y),
                    (int)(hitbox.Width * GameMain.ActiveScale.X),
                    (int)(hitbox.Height * GameMain.ActiveScale.Y)
                    ),
                Color.White
                );
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

            entity = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    entity.Width,
                    entity.Height);
        }

        /// <summary>
        /// Movement for a swinging entity (enemy, obstacle or player, presumably)
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void RotationalMotion(GameTime gameTime)
        {
            //Determine the angular acceleration using the perpendicular component of gravity
            angAccel = gravity * 10 * Math.Cos((Math.PI / 180) * theta);
            //angAccel -= Math.Sign(angAccel * 5);

            //Update velocity with acceleration and position with velocity
            angVelocity += angAccel * gameTime.ElapsedGameTime.TotalSeconds /** gameTime.ElapsedGameTime.TotalSeconds*/;
            theta += angVelocity * gameTime.ElapsedGameTime.TotalSeconds;

            //Determine new position using the new angle
            Vector2 temp = new Vector2(
                    (float)(pivot.X + swingRadius * Math.Cos((Math.PI / 180) * (theta))),
                    (float)(pivot.Y + swingRadius * Math.Sin((Math.PI / 180) * (theta))
                    ));


            vectorPosition = temp;

            entity = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    entity.Width,
                    entity.Height);
        }

        /// <summary>
        /// To be called when an entity switches from linear motion to rotational
        /// </summary>
        /// <param name="centerOfCircle"></param>
        public void SetRotationalVariables(Vector2 centerOfCircle)
        {
            this.physicsState = PhysicsState.Rotational;
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
            angVelocity = (newVMag *1000 * -Math.Sign(hypotenuse.X)) / swingRadius;

        }

        public void SetLinearVariables()
        {
            physicsState = PhysicsState.Linear;
            //This determines the velocity the player will have after 
            //they stop swinging by converting the angular velocity
            //back to linear velocity.
            velocity = new Vector2(                                       // 3000: random number for downscaling (it was too big)
                (float)(angVelocity * swingRadius * -Math.Sin((Math.PI / 180) * (theta)) / 600),
                (float)(angVelocity * swingRadius * Math.Cos((Math.PI / 180) * (theta))) / 600);
            acceleration = new Vector2(
                acceleration.X, gravity);
        }

        public void Impulse(Vector2 destination)
        {
            velocity = (destination);
        }
    }
}
