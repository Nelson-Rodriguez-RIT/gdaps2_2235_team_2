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
        KeyboardState _kbState;
        KeyboardState _prevkbState;
        public Player(Texture2D asset, Rectangle hitbox): base(asset, hitbox)
        {}

        public void Update(GameTime gameTime)
        {
            _kbState = Keyboard.GetState();
            Vector2 movementDirection = Vector2.Zero;
            if (_kbState.IsKeyDown(Keys.S))
            {
                movementDirection += Vector2.UnitY * 3;
                this.hitbox.Y += (int)(movementDirection.Y);
            }
            if (_kbState.IsKeyDown(Keys.W))
            {
                movementDirection -= Vector2.UnitY * 3;
                this.hitbox.Y += (int)(movementDirection.Y);
            }
            if (_kbState.IsKeyDown(Keys.A))
            {
                movementDirection -= Vector2.UnitX * 3;
                this.hitbox.X += (int)(movementDirection.X);
            }
            if (_kbState.IsKeyDown(Keys.D))
            {
                movementDirection += Vector2.UnitX * 3;
                this.hitbox.X += (int)(movementDirection.X);
            }
            _prevkbState = _kbState;
        }

        public void Draw(SpriteBatch sb) 
        {
            sb.Draw(
                    asset,
                    hitbox,
                    new Rectangle(0,0,32,32),
                    Color.White);
        }
    }
}
