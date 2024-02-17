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
    internal class MovableEntity : Entity
    {
        protected Vector2 velocity;

        public virtual Vector2 Velocity
        {
            get { return velocity; }
        }

        public MovableEntity(Texture2D image, LevelManager manager, Vector2 position) 
            : base(image, manager, position)
        {
            velocity = new Vector2(0, 0);
        }

        public override void Update(GameTime gameTime)
        {
            hitbox = new Rectangle
                ((int)position.X - (image.Width / 2),
                (int)position.Y - (image.Height / 2),
                image.Width,
                image.Height);
        }
    }
}
