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
    public abstract class GameObject
    {
        public Texture2D asset;
        public Rectangle hitbox;
        public Vector2 position;

        public Rectangle Hitbox
        { 
            get { return hitbox; } 
        }

        public GameObject(Texture2D asset, Rectangle hitbox) 
        {
            this.asset = asset;
            this.hitbox = hitbox;
            this.position = position;
        }
        public void Update(GameTime gameTime){ }

        public void Draw(SpriteBatch sb) { }
    }
}
