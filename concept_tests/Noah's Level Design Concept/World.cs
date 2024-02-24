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
        private Vector2 movementDirection;
        private float gravity = 9.8f;
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

        //want to make it where once the player reaches the middle of the screen, their character no longer moves
        //and the background just moves
        public void Movement(Player player, int screenWidth, int screenHeight)
        {
            movementDirection = Vector2.Zero;
            keyboardState = Keyboard.GetState();
            if (player.hitbox.X == screenWidth / 2)
            {
                if (keyboardState.IsKeyDown(Keys.A) && previousKeyboardState.IsKeyDown(Keys.A)) //if player is moving left
                {
                    movementDirection += Vector2.UnitX * (int)player.Velocity;
                    this.position.X += (int)movementDirection.X;
                }
                if (keyboardState.IsKeyDown(Keys.D) && previousKeyboardState.IsKeyDown(Keys.D)) //if player is moving left)
                {
                    movementDirection += Vector2.UnitX * (int)player.Velocity;
                    this.position.X -= (int)movementDirection.X;
                }
                previousKeyboardState = keyboardState;
            }
        
        }
       
    }
}
