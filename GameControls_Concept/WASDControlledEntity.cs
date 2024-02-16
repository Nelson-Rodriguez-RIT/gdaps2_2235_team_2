using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameControls_Concept
{
    internal class WASDControlledEntity : ControllableEntity
    {
        protected Vector2 acceleration;
        protected const float Gravity = 70000f;
        protected const float TerminalVelocity = 900f;
        protected const float MaxXVelocity = 100f;

        public Vector2 Acceleration
        {
            get { return acceleration; } 
        }

        public WASDControlledEntity(Texture2D image, LevelManager manager, Vector2 position) 
            : base(image, manager, position)
        { 
            acceleration = new Vector2 (0, Gravity);
        }

        public override void Update(GameTime gameTime) 
        {
            Input();
            Movement(gameTime);
            base.Update(gameTime);
        }

        public void Movement(GameTime gameTime)
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
            if (velocity.Y > TerminalVelocity) 
            { 
                velocity.Y = TerminalVelocity;
            }

            //Update position using velocity
            position = new Vector2(
                position.X + (velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds),
                position.Y + (velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds));

            //Slows down horizontal movement
            if (velocity.X > 0)
            {
                acceleration.X = -20000f;
            }
            else if (velocity.X < 0)
            {
                acceleration.X = 20000f;
            }
            else
            {
                acceleration.X = 0;
            }
        }
        
        public virtual void Input()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                acceleration.X = -140000f;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.D))
            {
                acceleration.X = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                acceleration.X = 140000f;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.A))
            {
                acceleration.X = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D)
                && Keyboard.GetState().IsKeyDown(Keys.A))
            {
                acceleration.X = 0;
            }
        }
    }
}
