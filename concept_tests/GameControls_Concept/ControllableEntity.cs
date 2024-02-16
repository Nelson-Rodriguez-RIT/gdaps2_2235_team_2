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
    internal abstract class ControllableEntity
    {
        protected Vector2 position;
        protected Vector2 velocity;
        protected Texture2D image;
        protected KeyboardState keyboardState;
        protected KeyboardState previousKB;
        protected MouseState mouseState;
        protected MouseState previousMS;
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

        public virtual Vector2 Velocity
        {
            get { return velocity; }
        }

        public ControllableEntity(Texture2D image, LevelManager manager, Vector2 position)
        {
            this.position = position;
            velocity = new Vector2(0, 0);
            this.image = image;
            hitbox = new Rectangle
                ((int)position.X - (image.Width / 2),
                (int)position.Y - (image.Height / 2),
                image.Width,
                image.Height);
            this.levelManager = manager;
        }

        public virtual void Update(GameTime gameTime)
        {
            previousKB = keyboardState;
            previousMS = mouseState;

            hitbox = new Rectangle
                ((int)position.X - (image.Width / 2),
                (int)position.Y - (image.Height / 2),
                image.Width,
                image.Height);
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(image,
                hitbox,
                Color.White);
        }
    }
}
