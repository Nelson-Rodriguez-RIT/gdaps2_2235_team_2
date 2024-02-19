using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GameControls_Concept
{
    internal abstract class Entity
    {
        protected Vector2 position;
        protected Texture2D image;
        protected Rectangle hitbox;
        protected LevelManager levelManager;

        public virtual Rectangle Hitbox
        {
            get { return hitbox; }
        }

        public virtual Vector2 Position
        {
            get { return position; }
        }

        public Entity(LevelManager manager, Vector2 position)
        {
            this.position = position;          
            this.image = image;

            hitbox = new Rectangle
                ((int)position.X - (50),
                (int)position.Y - (50),
                100,
                100);
            this.levelManager = manager;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(image,
                hitbox,
                Color.White);
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public virtual void LoadSprite(Texture2D texture)
        {
            image = texture;
        }
       
    }
}
