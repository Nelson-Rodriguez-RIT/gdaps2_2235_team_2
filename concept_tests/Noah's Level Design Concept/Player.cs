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
        public Vector2 position;

        public Player(Texture2D asset, Rectangle hitbox, Vector2 position): base(asset, hitbox)
        {
            this.position = position;
        }

        public override void Update(GameTime gameTime)
        {}

        public override void Draw(SpriteBatch sb) 
        {
            sb.Draw(
                    asset,
                    hitbox,
                    new Rectangle(0,0,32,32),
                    Color.White);
        }
    }
}
