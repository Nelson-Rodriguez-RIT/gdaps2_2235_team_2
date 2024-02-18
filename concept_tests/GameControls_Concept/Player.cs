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
        protected float moveSpeed = 1000f;
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
        }

        public override void Input()
        {
            if (mouseState.LeftButton == ButtonState.Pressed
                && state == PlayerStates.Default)
            {
                state = PlayerStates.Swinging;
                
            }
            /*
            else if (state == PlayerStates.Swinging
                && mouseState.LeftButton == ButtonState.Released)
            {
                state = PlayerStates.Default;
                acceleration = new Vector2(
                    acceleration.X, gravity);
            }
            */

            base.Input();
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
            if (state == PlayerStates.Swinging)
            {
                //For now to make things simpler

                
                Vector2 hypotenuse = new Vector2(
                companion.Position.X - position.X,
                companion.Position.Y - position.Y);


                float radius =
                    (float)(
                        Math.Sqrt(
                            Math.Pow(hypotenuse.X, 2) +
                            Math.Pow(hypotenuse.Y, 2)
                        )
                    );

                //Todo: Add velocity and acceleration

                float theta = -90f + (float)((180 / Math.PI) * Math.Acos(                   
                    hypotenuse.X
                    / 
                    radius
                    ));

                float newTheta = 1f * (float)((Math.PI / 180) * theta * Math.Cos( ((Math.PI / 180) *
                    Math.Sqrt(gravity / radius) * gameTime.TotalGameTime.TotalSeconds)));

                float degrees = newTheta * (float)(180 / Math.PI);

                Vector2 temp = new Vector2(
                    (float)(companion.Position.X + radius * Math.Sin(newTheta)),
                    (float)(companion.Position.Y + radius * Math.Cos(newTheta))
                    );

                position = temp;


                /*
                float angVelocity = (float)(-theta * Math.Sin(
                    Math.Sqrt(gravity / radius) * gameTime.TotalGameTime.TotalSeconds)
                    * Math.Pow(Math.Sqrt(gravity / radius), 1));

                float angAccel = (float)(-theta * Math.Cos(
                    Math.Sqrt(gravity / radius) * gameTime.TotalGameTime.TotalSeconds)
                    * Math.Pow(Math.Sqrt(gravity / radius), 2));
                */

                
                /*
                //float pos = theta * hypotenuse;
                float linearAccel = angAccel * hypotenuse / 100;
                float linearVelocity = angVelocity * hypotenuse / 100;

                velocity = new Vector2(
                    linearVelocity * (float)Math.Cos(theta),
                    linearVelocity * (float)Math.Sin(theta));
                
                acceleration = new Vector2(
                    linearAccel * (float)Math.Cos(theta),
                    linearAccel * (float)Math.Sin(theta));
                */


            }
            else
            {
                base.Movement(gameTime);
            }

                
                        
        }
    }
}
