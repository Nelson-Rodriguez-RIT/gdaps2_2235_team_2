using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Noah_s_Level_Design_Concept
{
    public class Player : GameObject
    {
        KeyboardState keyboardState;
        KeyboardState previousKeyboardState;
        private int gravity = 10;
        private Vector2 acceleration;
        private Vector2 movementDirection;
        private float velocity = 5;
        private bool movingLeft = false;
        private bool movingRight = false;

        public bool MovingLeft
        { get { return movingLeft; } }
        public bool MovingRight
        { get { return movingRight; } }
        public float Velocity
        { get { return velocity; } }

        public Player(Texture2D asset, Rectangle position): base(asset, position)
        {
            this.acceleration = new Vector2(0, gravity);
            this.position = position;
        }

        public override void Update(GameTime gameTime)
        {
            movingLeft = false;
            movingRight = false;
            //movement capabilities using vectors
            keyboardState = Keyboard.GetState();
            movementDirection = Vector2.Zero;

            //if player moves to the right, screen should move left at same pace
            if (keyboardState.IsKeyDown(Keys.A) && previousKeyboardState.IsKeyDown(Keys.A))
            {
                movementDirection += Vector2.UnitX * velocity;
                this.position.X -= (int)(movementDirection.X);
                movingLeft = true;
            }
            if (keyboardState.IsKeyDown(Keys.D) && previousKeyboardState.IsKeyDown(Keys.D))
            {
                movementDirection += Vector2.UnitX * velocity;
                this.position.X += (int)(movementDirection.X);
                movingRight = true;
            }
            this.position.Y += (int)this.acceleration.Y;

            previousKeyboardState = keyboardState;
        }

        public override void Draw(SpriteBatch sb) 
        {
            sb.Draw(
                    asset,
                    position,
                    new Rectangle(0,0,32,32),
                    Color.White);
        }
    }
}
