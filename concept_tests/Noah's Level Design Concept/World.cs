using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Noah_s_Level_Design_Concept
{
    public class World
    {
        private bool active;
        private Texture2D asset;
        private Rectangle position;
        private float gravity = 9.8;
        KeyboardState keyboardState;
        KeyboardState previousKeyboardState;

        public bool IsActive
        {
            get { return active; }
            set { active = value; }
        }
        public Rectangle Position
        {
            get { return position; } 
            set { position = value; }
        }

        public World(Texture2D asset, Rectangle postion, bool active)
        {
            this.asset = asset;
            this.position = postion;    
            this.active = active;
        }

        public void Update(GameTime gameTime) 
        {
            //movement capabilities using vectors
            keyboardState = Keyboard.GetState();   
            Vector2 movementDirection = Vector2.Zero;
            float velocity = 6;
            //if player moves to the right, screen should move left at same pace
            if (keyboardState.IsKeyDown(Keys.A))
            {
                movementDirection += Vector2.UnitX * velocity;
                this.position.X += (int)(movementDirection.X);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                movementDirection -= Vector2.UnitX * velocity;
                this.position.X += (int)(movementDirection.X);
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                //iT DONT WORK
                movementDirection.Y -= velocity - gravity * (float)Math.Pow(gameTime.ElapsedGameTime.TotalSeconds, 2f);
                
            }
            
            previousKeyboardState = keyboardState;
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            if (active)
            {
                spriteBatch.Draw(
                    asset,
                    position,
                    Color.White);
            }
        }

        private bool SingleKeyPress(Keys key, KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key))
            {
                return true;
            }
            else { return false; }
        }
    }
}
