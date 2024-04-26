using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Entities.Base;
using System.Linq;
using System.Collections.Generic;
using Moonwalk.Classes.Entities;
using Moonwalk.Classes.Helpful_Stuff;
using System;

namespace Moonwalk.Classes.Maps
{
    internal class Door : Terrain
    {

        public Door(Rectangle hitbox) : base(hitbox) 
        {
            collidable = true;
            OnCollision += this.CheckUnlock;
        }

        private void CheckUnlock()
        {
            //Find a key that is close to the door
            KeyObject key = (KeyObject)((List<Entity>)GameManager.entities[typeof(Entity)]).Find
                (key => key is KeyObject
                && Math.Abs(VectorMath.Difference(
                    hitbox.Center.ToVector2(),
                    key.Hitbox.Center.ToVector2()).X)
                < 100);

            if (key != null)
            {
                //if there is a key, make this terrain uncollidable
                collidable = false;
                GameManager.DespawnEntity(key);
            }
        }

        public override void Collide()
        {
            base.Collide();
        }

    }
}
