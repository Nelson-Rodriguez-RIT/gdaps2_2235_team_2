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

        private Texture2D hitboxSprite = null;

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
        protected static Dictionary<string, string> properties = null;
        protected static List<Animation> animations = null;
        protected static Texture2D spritesheet = null;

        // Currently displayed animation
        protected Animation activeAnimation; // DO NOT manually change this, use SwitchAnimation() instead

        protected int spriteScale;

        public Entity(Vector2 position, string directory, int width, int height) {
            physicsState = PhysicsState.Linear;
            vectorPosition = position;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            gravity = 0f;
            maxYVelocity = int.MaxValue;
            maxXVelocity = int.MaxValue;

            this.entity = new Rectangle(
                (int)vectorPosition.X,
                (int)vectorPosition.Y,
                width,
                height);

            this.directory = directory;

            if (properties == null) { // Load data if it isn't already loaded
                (Dictionary<string, string> properties, List<Animation> animations,
                Texture2D spritesheet) bufferedData = Loader.LoadEntity(directory);

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

        
        public void DrawHitbox(SpriteBatch batch, Vector2 globalScale, GraphicsDevice graphics) {
            batch.Draw(
                hitboxSprite,
                new Rectangle(
                    (int)((hitbox.X + Position.X) * globalScale.X),
                    (int)((hitbox.Y + Position.Y) * globalScale.Y),
                    (int)(hitbox.Width * globalScale.X),
                    (int)(hitbox.Height * globalScale.Y)
                    ),
                Color.White
                );
            // // Doesn't work at the moment, but ill try to get it working later
            // This uses projection (omg Math Graphical Sim. actually has a purpose :O )
            // onto a 3D pane to create a box (this avoids having us to make predrawn boxes)
            // https://stackoverflow.com/questions/23305577/draw-rectangle-in-monogame
            /*
            VertexPositionColor[] vertexPositions = new[] { 
                // + new Vector3(Position.X, Position.Y, 0)
                new VertexPositionColor(new Vector3(0, 0, 1), Color.Orange),
                new VertexPositionColor(new Vector3(2, 0, 1), Color.Orange),
                new VertexPositionColor(new Vector3(2, 2, 1), Color.Orange),
                new VertexPositionColor(new Vector3(0, 2, 1), Color.Orange)
            };

            // We draw this like it is a shader (done via BasicEffect)
            BasicEffect projection = new BasicEffect(graphics);
            projection.World = Matrix.CreateOrthographicOffCenter(
                0,                          // X cord of 0, 0, 0
                graphics.Viewport.Width,    // Window width
                graphics.Viewport.Height,   // Window height
                0,                          // Y cord of 0, 0, 0
                0,                          // Z cord of 0, 0, 0
                1                           // Depth
                );

            // MonoGame has no docs for these some I'm not too sure what they do
            EffectTechnique box = projection.Techniques[0];
            box.Passes[0].Apply(); // Draws the box (shader)

            graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertexPositions, 0, 3);
            */
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
            velocity = (destination - vectorPosition) / 4f;
        }
    }
}
