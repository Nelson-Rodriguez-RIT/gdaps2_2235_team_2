using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GameControls_Concept
{
    internal class Collider : Entity
    {

        public Collider(Rectangle rectangle, LevelManager manager) : 
            base(manager, rectangle.Location.ToVector2())
        {
            hitbox = rectangle;
        }


       
    }
}
