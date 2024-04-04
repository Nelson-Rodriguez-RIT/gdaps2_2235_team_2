using Microsoft.Xna.Framework;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Entities
{
    public class KeyObject //: Entity //IInteractible
    {
        /*
        //fields
        Rectangle hitbox;
        bool isColliding;
        bool isInteracted;


        public Rectangle Hitbox { get { return hitbox; } }


        public KeyObject(Rectangle hitbox, bool isInteracted)
        {
            this.hitbox = hitbox; //position/hitbox
            this.isColliding = false; //should not start by colliding with anything
            this.isInteracted = isInteracted; //if something is interacted
                                              //with before player reaches it (ie. open doors)
        }

        //methods

        public bool IsInteracted(Entity entity) 
        {
            if ((Player)entity != null && CheckCollision((Player)entity))
            {
                isInteracted = true;


            }
            return isInteracted;
        }

        public bool CheckCollision(Entity entity) 
        {
            if (this.hitbox.Intersects(((Player)entity).Hitbox))
            {
                isColliding = true; 
            }

            return isColliding;
        }
        */
    }
}
