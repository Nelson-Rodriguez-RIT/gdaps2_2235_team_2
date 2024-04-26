using Microsoft.Xna.Framework;
using Moonwalk.Interfaces;

namespace Moonwalk.Classes.Maps {

    /// <summary>
    /// Class for collision and map geometry
    /// </summary>
    internal class Terrain : ISolid {
        public delegate void OnCollisionHandler();
        public event OnCollisionHandler OnCollision;

        protected bool collidable = true;

        protected Rectangle hitbox;

        public Rectangle Hitbox {
            get { return hitbox; }
        }

        /// <summary>
        /// if this terrain stops entity movement
        /// </summary>
        public bool Collidable { get { return collidable; } }

        public Terrain(Rectangle hitbox) {
            this.hitbox = hitbox;
        }

        /// <summary>
        /// Raises collision event
        /// </summary>
        public virtual void Collide() {
            if (OnCollision != null)
            {
                OnCollision();
            }
        }


        public override string ToString() {
            return hitbox.X + " - " + hitbox.Y + " - " + hitbox.Width + " - " + hitbox.Height;
        }
    }
}
