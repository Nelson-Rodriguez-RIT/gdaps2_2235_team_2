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
        private Companion companion;
        private PlayerStates state;
        private PlayerStates prevState;
        private SpriteFont font;

        public Player(LevelManager manager, Vector2 position, Companion companion, SpriteFont font)
            : base(manager, position)
        {
            this.companion = companion;
            state = PlayerStates.Default;
            this.font = font;
            this.companion.Player = this;
        }

        public double Radius
        {
            get
            {
                //Define the vector between the player and the companion
                Vector2 hypotenuse = new Vector2(
                    companion.Position.X - position.X,
                    companion.Position.Y - position.Y);

                //The magnitude of the previous vector,
                //or the radius of the circle on which the player will rotate
                return
                        (
                            Math.Sqrt(
                                Math.Pow(hypotenuse.X, 2) +
                                Math.Pow(hypotenuse.Y, 2)
                            )
                        );

            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            prevState = state;
        }

        protected override void Input()
        {
            base.Input();

            if (mouseState.RightButton == ButtonState.Pressed
                && previousMS.RightButton == ButtonState.Released
                && physicsState == PhysicsState.Linear
                && position.Y > companion.Position.Y)
            {
                state = PlayerStates.Swinging;
                physicsState = PhysicsState.Rotational;
                //companion can't move while player is swinging for now
                companion.state = State.Inactive;
            }

            else if (physicsState == PhysicsState.Rotational
                && mouseState.RightButton == ButtonState.Released
                && previousMS.RightButton == ButtonState.Pressed)
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
            /*
            sb.DrawString(font, acceleration.X + "  " + acceleration.Y, new Vector2(700,100), Color.White);
            sb.DrawString(font, position.X + "  " + position.Y, new Vector2(300, 100), Color.White);
            sb.DrawString(font, velocity.X + "  " + velocity.Y, new Vector2(500, 100), Color.White);
            sb.DrawString(font, angAccel.ToString(), new Vector2(700, 200), Color.White);
            sb.DrawString(font, theta.ToString(), new Vector2(300, 200), Color.White);
            sb.DrawString(font, angVelocity.ToString(), new Vector2(500, 200), Color.White);
            */

            base.Draw(sb);
        }

        protected override void Movement(GameTime gameTime)
        {
            Vector2 oldPosition = position;

            if (physicsState == PhysicsState.Rotational)
            {
                Swing(gameTime);             
            }
            else
            {

                velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Make sure speed is not over the maximum
                if (velocity.Y > terminalVelocity)
                {
                    velocity.Y = terminalVelocity;
                }
                else if (velocity.Y < -terminalVelocity)
                {
                    velocity.Y = -terminalVelocity;
                }

                if (velocity.X > maxXVelocity)
                {
                    velocity.X = maxXVelocity;
                }
                else if (velocity.X < -maxXVelocity)
                {
                    velocity.X = -maxXVelocity;
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
        protected override void Swing(GameTime gameTime)
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

                
            }

            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;

            base.Swing(gameTime);

        }

        
    }
}
