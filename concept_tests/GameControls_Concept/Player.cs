using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GameControls_Concept
{
    enum PlayerStates
    {
        Default,
        Swinging
    }

    internal class Player : WASDControlledEntity
    {
        private MouseControlledEntity companion;
        private PlayerStates state;
        private PlayerStates prevState;
        protected float moveSpeed = 10000f;
        private SpriteFont font;

        public Player(Texture2D image, LevelManager manager, Vector2 position, MouseControlledEntity companion, SpriteFont font)
            : base(image, manager, position)
        {
            this.companion = companion;
            state = PlayerStates.Default;
            this.font = font;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            prevState = state;
        }

        public override void Input()
        {
            base.Input();

            if (mouseState.LeftButton == ButtonState.Pressed
                && previousMS.LeftButton == ButtonState.Released
                && state == PlayerStates.Default
                && position.Y > companion.Position.Y)
            {                
                state = PlayerStates.Swinging;
                physicsState = PhysicsState.Rotational;
                //companion can't move while player is swinging for now
                companion.state = State.Inactive;
            }
            
            else if (state == PlayerStates.Swinging
                && mouseState.LeftButton == ButtonState.Released
                && previousMS.LeftButton == ButtonState.Pressed)
            {
                state = PlayerStates.Default;
                physicsState = PhysicsState.Linear;
                //This determines the velocity the player will have after 
                //they stop swinging by converting the angular velocity
                //back to linear velocity.
                velocity = new Vector2(                                       // 3000: random number for downscaling (it was too big)
                    (float)(angVelocity * swingRadius * -Math.Sin((Math.PI / 180) * (theta)) / 3000),
                    (float)(angVelocity * swingRadius * Math.Cos((Math.PI / 180) * (theta))) / 3000);
                acceleration = new Vector2(
                    acceleration.X, gravity);
                companion.state = State.Active;
            }
            
            
            
        }

        public override void Draw(SpriteBatch sb)
        {
            string temp = acceleration.X + " - " + acceleration.Y;
            sb.DrawString(font, acceleration.X + "  " + acceleration.Y, new Vector2(700,100), Color.White);
            sb.DrawString(font, position.X + "  " + position.Y, new Vector2(300, 100), Color.White);
            sb.DrawString(font, velocity.X + "  " + velocity.Y, new Vector2(500, 100), Color.White);
            sb.DrawString(font, angAccel.ToString(), new Vector2(700, 200), Color.White);
            sb.DrawString(font, theta.ToString(), new Vector2(300, 200), Color.White);
            sb.DrawString(font, angVelocity.ToString(), new Vector2(500, 200), Color.White);

            base.Draw(sb);
        }

        public override void Movement(GameTime gameTime)
        {
            Vector2 oldPosition = position;

            if (state == PlayerStates.Swinging)
            {
                Swing(gameTime);             
            }
            else
            {
                //Update velocity using acceleration
                velocity = new Vector2(
                    velocity.X + (acceleration.X * (float)Math.Pow(
                        gameTime.ElapsedGameTime.TotalSeconds,
                        2)),
                    velocity.Y + (acceleration.Y * (float)Math.Pow(
                        gameTime.ElapsedGameTime.TotalSeconds,
                        2)));

                //Make sure speed is not over the maximum
                if (velocity.Y > terminalVelocity)
                {
                    velocity.Y = terminalVelocity;
                }
                else if (velocity.Y < -terminalVelocity)
                {
                    velocity.Y = -terminalVelocity;
                }

                //Update position using velocity
                position = CheckForPlatformCollision(
                    levelManager.Platforms);

                if (oldPosition.Y == position.Y)
                {
                    state = PlayerStates.Default;
                }
                if (oldPosition.X == position.X)
                {
                    state = PlayerStates.Default;
                }

                if (oldPosition.Y == position.Y)
                {
                    velocity.Y = 0;
                }
                if (oldPosition.X == position.X)
                {
                    velocity.X = 0;
                }
            }
        }

        /// <summary>
        /// Handles movement of the player while they are swinging.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Swing(GameTime gameTime)
        {
            if (prevState != PlayerStates.Swinging)
            {
                pivot = companion.Position;

                //Define the vector between the player and the companion
                Vector2 hypotenuse = new Vector2(
                    companion.Position.X - position.X,
                    companion.Position.Y - position.Y);

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

                //The rest of the code in this if statement is for determining the 
                //player's initial velocity when they start swinging. You need to 
                //get the component of the player's current velocity that is 
                //perpendicular to the radius. A diagram is very helpful for understanding.

                //Magnitude of the player's velocity
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
                angVelocity = (newVMag * 3000 *  -Math.Sign(hypotenuse.X)) / swingRadius;

                velocity = Vector2.Zero;
                acceleration = Vector2.Zero;
            }

            base.Swing(gameTime);

        }

        public override double RotationalMotionCollision(List<Collider> colliders)
        {
            //Scaling iterations based on velocity
            int maxIteration = CollisionAccuracy > 1 ? CollisionAccuracy : 1;

            // How many steps it can go before colliding into anything
            int peakIterations = maxIteration;

            // Shorten iterations based on current peakIteration TODO
            foreach (Collider collider in colliders) // Check each platform
            {
                for (int iteration = 0; iteration <= maxIteration; iteration++)
                { // Check how many steps it can go before colliding into this platform
                    Vector2 temp = new Vector2(
                    (float)(pivot.X + swingRadius * Math.Cos((Math.PI / 180) *
                    (theta + angVelocity / maxIteration * iteration))),
                    (float)(pivot.Y + swingRadius * Math.Sin((Math.PI / 180) *
                    (theta + angVelocity / maxIteration * iteration)))
                    );

                    if (new Rectangle( // Check for horizontal collision
                            (int)(temp.X - hitbox.Width / 2 + ((angVelocity) / maxIteration) * iteration),
                            (int)(temp.Y - hitbox.Height / 2 + (angVelocity) * iteration),
                            hitbox.Width,
                            hitbox.Height)
                            .Intersects(collider.Hitbox))
                        // We want the absolute minimum steps
                        peakIterations = iteration - 1 < peakIterations ? iteration - 1 : peakIterations;

                }
            }

            //Determine whether to stop swinging
            if (peakIterations < maxIteration)
            {
                state = PlayerStates.Default;
            }

            // Update position and relevant hitbox based on peakIteration
            return theta + (angVelocity / maxIteration) * peakIterations;
        }
    }
}
