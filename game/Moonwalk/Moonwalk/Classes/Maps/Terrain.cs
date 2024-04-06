﻿using Microsoft.Xna.Framework;

namespace Moonwalk.Classes.Maps {


    internal class Terrain {
        public delegate void OnCollisionHandler();
        public event OnCollisionHandler OnCollision;

        protected bool collidable = true;

        protected Rectangle hitbox;

        public Rectangle Hitbox {
            get { return hitbox; }
        }

        public bool Collidable { get { return collidable; } }

        public Terrain(Rectangle hitbox) {
            this.hitbox = hitbox;
        }

        public virtual void Collide() {
            OnCollision();
        }


        public override string ToString() {
            return hitbox.X + " - " + hitbox.Y + " - " + hitbox.Width + " - " + hitbox.Height;
        }
    }
}
