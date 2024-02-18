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

        //Harmonic motion variables
        private float initialTheta;
        private float swingRadius;
        private double initialTime;
        private double period;

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
            }
            
            else if (state == PlayerStates.Swinging
                && mouseState.LeftButton == ButtonState.Released
                && previousMS.LeftButton == ButtonState.Pressed)
            {
                state = PlayerStates.Default;
                acceleration = new Vector2(
                    acceleration.X, gravity);
            }
            

            
        }

        public override void Draw(SpriteBatch sb)
        {
            string temp = acceleration.X + " - " + acceleration.Y;
            sb.DrawString(font, acceleration.X + "  " + acceleration.Y, new Vector2(700,100), Color.White);
            sb.DrawString(font, position.X + "  " + position.Y, new Vector2(300, 100), Color.White);
            sb.DrawString(font, velocity.X + "  " + velocity.Y, new Vector2(500, 100), Color.White);

            base.Draw(sb);
        }

        public override void Movement(GameTime gameTime)
        {
            Vector2 oldPosition = position;

            if (state == PlayerStates.Swinging)
            {

                state = PlayerStates.Swinging;

                Vector2 hypotenuse = new Vector2(
                companion.Position.X - position.X,
                companion.Position.Y - position.Y);

                if (prevState != PlayerStates.Swinging) 
                {

                    swingRadius =
                    (float)(
                        Math.Sqrt(
                            Math.Pow(hypotenuse.X, 2) +
                            Math.Pow(hypotenuse.Y, 2)
                        )
                    );

                    initialTheta = -90f + (float)((180 / Math.PI) * Math.Acos(
                    hypotenuse.X
                    /
                    swingRadius
                    ));

                    initialTime = gameTime.TotalGameTime.TotalSeconds;

                    
                    //Initial velocity calculations ( doesn't work yet )

                    //Magnitude of the velocity
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

                    //Set the velocity equal to that using its components
                    velocity = new Vector2(
                        (float)(newVMag * Math.Cos((Math.PI / 180) * Math.Abs(initialTheta))),
                        (float)(newVMag * Math.Sin((Math.PI / 180) * Math.Abs(initialTheta)))
                        );
                    
                    //Period of the oscillation
                    period = 2 * Math.PI * Math.Sqrt(swingRadius / gravity * 10);
                }
                
                //Use the pendulum equation to get the new angle
                float newTheta = (float)((Math.PI / 180) * initialTheta * Math.Cos( ((Math.PI / 180) *
                    Math.Sqrt(gravity * 10000 / swingRadius) * gameTime.TotalGameTime.TotalSeconds - initialTime)));

                float degrees = newTheta * (float)(180 / Math.PI); //for debug purposes

                //use the new angle to get the new position
                Vector2 temp = new Vector2(
                    (float)(companion.Position.X + swingRadius * Math.Sin(newTheta)),
                    (float)(companion.Position.Y + swingRadius * Math.Cos(newTheta))
                    );

                //Update velocity based on the new position
                velocity = temp - position;

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
    }
}
