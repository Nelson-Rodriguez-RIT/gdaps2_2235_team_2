using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Boss;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Maps;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

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

        // X and Y represent origin offsets
        internal protected Rectangle hurtbox; // Internal used for GUIRobotDebugElement

        protected float gravity;
        /// <summary>
        /// Determines whether or not an entity moves linearly or radially
        /// </summary>
        protected PhysicsState physicsState;

        //linear motion
        internal protected Vector2 vectorPosition; // Internal used for GUIRobotDebugElement
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

        protected static Texture2D hitboxSprite = null;

        public virtual Rectangle Hitbox
        {
            get { return hurtbox; }
        }

        public virtual Point Position
        {
            get { return hurtbox.Location; }
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
        internal protected Dictionary<string, string> properties = null;
        internal protected List<Animation> animations = null;
        internal protected Texture2D spritesheet = null;

        // Currently displayed animation
        protected Animation activeAnimation; // Do NOT manually change this, use SwitchAnimation() instead

        protected float spriteScale;

        public Entity(Vector2 position, string directory, bool loadAnimations = true, bool loadProperties = true) {
            // File data setup
            this.directory = directory;
            EntityData bufferedData = Loader.LoadEntity(directory, loadAnimations, loadProperties);
            bufferedData.Load(this);

            spriteScale = 1;

            // Physics set up
            physicsState = PhysicsState.Linear;
            vectorPosition = position;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            gravity = 0f;
            maxYVelocity = int.MaxValue;
            maxXVelocity = int.MaxValue;

            if (loadProperties) 
            {  
                hurtbox = new Rectangle(
                    int.Parse(properties["HitboxXOrigin"]),
                    int.Parse(properties["HitboxYOrigin"]),
                    int.Parse(properties["HitboxX"]),
                    int.Parse(properties["HitboxY"])
                    );
            }
            if (hitboxSprite == null)
                hitboxSprite = Loader.LoadTexture("../../../Content/Entities/hitbox");
        }

        
        public virtual void Update(
                GameTime gameTime, 
                StoredInput input) 
        {
            if (activeAnimation  != null)
            activeAnimation.UpdateAnimation(gameTime);

            acceleration.Y = gravity;
        }

        /// <summary>
        /// Switches the animation currently playing to another
        /// </summary>
        /// <param name="animation">The animation to switch to</param>
        protected virtual void SwitchAnimation(Enum animationEnum, bool resetAnimation = true) {
            activeAnimation = animations[Convert.ToInt32(animationEnum)];
            if (resetAnimation)
                activeAnimation.Reset();
        }

        public virtual void Draw(SpriteBatch batch)
        {
            if (spriteScale == 0)
            {
                throw new Exception("You forgot to set the sprite scale");
            }

            //apply offset
            Vector2 temp = Camera.RelativePosition(Position);

            activeAnimation.Draw(batch, GameMain.ActiveScale, spritesheet, temp);
        }

        
        public void DrawHitbox(SpriteBatch batch) {
            Vector2 position = Camera.RelativePosition(
                new Vector2(
                    hurtbox.X,
                    hurtbox.Y
                    )
                );
            
            batch.Draw(
                hitboxSprite,
                new Rectangle(
                    (int)(position.X),
                    (int)(position.Y),
                    (int)(hurtbox.Width * GameMain.ActiveScale.X),
                    (int)(hurtbox.Height * GameMain.ActiveScale.Y)
                    ),
                Color.Blue
                );
            
        }

        #region Collision

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

                        return (int)(VectorMath.VectorMagnitude(velocity) / 4f);  //Use the magnitude of the velocity to get the accuracy

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

        public virtual bool CheckCollision<T>(out T thing)
        {
            bool isColliding = false;
            thing = default(T);

            Type type = typeof(T);

            List<T> list = null;

            try
            {
                list = GameManager.entities.GetAllOfType<T>();
                list.AddRange(Map.Geometry.GetAllOfType<T>());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            if (type == typeof(ISolid))
            {
                foreach (ISolid solid in list)
                {
                    if (solid.Hitbox.Intersects(hurtbox))
                    {
                        if (solid.Collidable)
                            isColliding = true;

                        thing = (T)solid;
                        solid.Collide();
                        break;
                    }
                }
            }
            else if (type == typeof(ISoft))
            {
                foreach (ISoft soft in list)
                {
                    if (soft.Hitbox.Intersects(hurtbox))
                    {
                        isColliding = true;
                        thing = (T)soft;
                        break;
                    }
                }
            }
            else if (type == typeof(IDamageable))
            {
                foreach (IDamageable damageable in list)
                {
                    if (damageable.Hitbox.Intersects(hurtbox))
                    {
                        isColliding = true;
                        thing = (T)damageable;
                        break;
                    }
                }
            }
            else if (type == typeof(IHostile))
            {
                foreach (IHostile hostile in list)
                {
                    if (hostile.Hitbox.Intersects(hurtbox))
                    {
                        isColliding = true;
                        thing = (T)hostile;
                        break;
                    }
                }
            }
            else
            {
                foreach (T item in list)
                {
                    if (((ICollidable)item).Hitbox.Intersects(hurtbox))
                    {
                        isColliding = true;
                        thing = item;
                        break;
                    }
                }
            }

            return isColliding;
        }

        public virtual bool CheckCollision<T>()
        {
            bool isColliding = false;


            Type type = typeof(T);

            List<T> list = null;

            try
            {
                list = GameManager.entities.GetAllOfType<T>();
                list.AddRange(Map.Geometry.GetAllOfType<T>());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            if (type == typeof(ISolid))
            {
                foreach (ISolid solid in list)
                {
                    if (solid.Hitbox.Intersects(hurtbox))
                    {
                        if (solid.Collidable)
                            isColliding = true;


                        solid.Collide();
                        break;
                    }
                }
            }
            else if (type == typeof(ISoft))
            {
                foreach (ISoft soft in list)
                {
                    if (soft.Hitbox.Intersects(hurtbox))
                    {
                        isColliding = true;

                        break;
                    }
                }
            }
            else if (type == typeof(IDamageable))
            {
                foreach (IDamageable damageable in list)
                {
                    if (damageable.Hitbox.Intersects(hurtbox))
                    {
                        isColliding = true;

                        break;
                    }
                }
            }
            else if (type == typeof(IHostile))
            {
                foreach (IHostile hostile in list)
                {
                    if (hostile.Hitbox.Intersects(hurtbox))
                    {
                        isColliding = true;

                        break;
                    }
                }
            }
            else
            {
                foreach (T item in list)
                {
                    if (((ICollidable)item).Hitbox.Intersects(hurtbox))
                    {
                        isColliding = true;
                        break;
                    }
                }
            }

            return isColliding;
        }

        public virtual bool CheckCollision<T>(Rectangle rect)
        {
            bool isColliding = false;


            Type type = typeof(T);

            List<T> list = null;

            try
            {
                list = GameManager.entities.GetAllOfType<T>();
                list.AddRange(Map.Geometry.GetAllOfType<T>());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            if (type == typeof(ISolid))
            {
                foreach (ISolid solid in list)
                {
                    if (solid.Hitbox.Intersects(rect))
                    {
                        if (solid.Collidable)
                            isColliding = true;


                        solid.Collide();
                        break;
                    }
                }
            }
            if (type == typeof(ISoft))
            {
                foreach (ISoft soft in list)
                {
                    if (soft.Hitbox.Intersects(rect))
                    {
                        isColliding = true;

                        break;
                    }
                }
            }
            if (type == typeof(IDamageable))
            {
                foreach (IDamageable damageable in list)
                {
                    if (damageable.Hitbox.Intersects(rect))
                    {
                        isColliding = true;

                        break;
                    }
                }
            }
            if (type == typeof(IHostile))
            {
                foreach (IHostile hostile in list)
                {
                    if (hostile.Hitbox.Intersects(rect))
                    {
                        isColliding = true;

                        break;
                    }
                }
            }
            else
            {
                foreach (T item in list)
                {
                    if (((ICollidable)item).Hitbox.Intersects(rect))
                    {
                        isColliding = true;
                        break;
                    }
                }
            }

            return isColliding;
        }

        #endregion

        #region Movement

        public virtual void Movement(GameTime time)
        {
            switch (physicsState)
            {
                case PhysicsState.Linear:
                    LinearMotion(time);
                    break;
                case PhysicsState.Rotational:
                    RotationalMotion(time);
                    break;
            }

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

            hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);
        }

        /// <summary>
        /// Movement for a swinging entity (enemy, obstacle or player, presumably)
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void RotationalMotion(GameTime gameTime)
        {
            //Determine the angular acceleration using the perpendicular component of gravity
            angAccel = gravity * 10 * Math.Cos((Math.PI / 180) * theta);

            //Update velocity with acceleration and position with velocity
            angVelocity += angAccel * gameTime.ElapsedGameTime.TotalSeconds;
            theta += angVelocity * gameTime.ElapsedGameTime.TotalSeconds;

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
        }

        /// <summary>
        /// To be called when an entity switches from linear motion to rotational
        /// </summary>
        /// <param name="centerOfCircle"></param>
        public void SetRotationalVariables(Vector2 centerOfCircle)
        {
            this.physicsState = PhysicsState.Rotational;
            this.pivot = centerOfCircle;

            //Define the vector between the entity and the pivot
            Vector2 hypotenuse = VectorMath.VectorDifference(vectorPosition, pivot);

            //The magnitude of the previous vector,
            //or the radius of the circle on which the player will rotate
            swingRadius =
                (float)VectorMath.VectorMagnitude(hypotenuse);
                    

            //Get the angle between the player and 0 degrees (right)
            theta = //vectorPosition.Y > pivot.Y ? -1 : 1 *
                (180 - (float)((180 / Math.PI) * Math.Acos(
                    hypotenuse.X
                    /
                    swingRadius
                    )));

            if (vectorPosition.Y < pivot.Y)
            {
                theta += 2 * (180 - theta);
            }

            //The rest of the code in this is for determining the 
            //entity's initial velocity when they start swinging. You need to 
            //get the component of the entity's current velocity that is 
            //perpendicular to the radius. A diagram is very helpful for understanding.

            //Magnitude of the entity's velocity
            double velocityMag = VectorMath.VectorMagnitude(velocity);

            if (velocityMag <= 0)
            {
                angVelocity = 0;
                return;
            }

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
            angVelocity = (newVMag * 550 * Math.Sign(pivot.Y - vectorPosition.Y)) * Math.Sign(velocity.X) / swingRadius;
                                // 550: upscaling number
        }

        public void SetLinearVariables()
        {
            if (Robot.Tether == this)
            {
                Robot.Tether = null;
            }

            physicsState = PhysicsState.Linear;
            //This determines the velocity the player will have after 
            //they stop swinging by converting the angular velocity
            //back to linear velocity.
            velocity = new Vector2(                                       // 400: random number for downscaling (it was too big)
                (float)(angVelocity * swingRadius * -Math.Sin((Math.PI / 180) * (theta)) / 400),
                (float)(angVelocity * swingRadius * Math.Cos((Math.PI / 180) * (theta))) / 400);
            acceleration = new Vector2(
                acceleration.X, gravity);
        }

        public virtual void Impulse(Vector2 destination)
        {
            velocity = (destination);
        }

        #endregion
    }

    internal class EntityData {
        private Dictionary<string, string> properties;
        private List<Animation> animations;
        private Texture2D spritesheet;

        public EntityData(Dictionary<string, string> properties, 
                List<Animation> animations, Texture2D spritesheet) {
            this.properties = properties;
            this.animations = animations;
            this.spritesheet = spritesheet;
        }

        public void Load(Entity entity) {
            entity.properties = properties;
            entity.animations = animations;
            entity.spritesheet = spritesheet;
        }

        
    }
}
