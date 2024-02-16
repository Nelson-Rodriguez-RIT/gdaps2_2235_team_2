using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dante_Test
{
    internal class Companion
    {
        public Vector2 position;
        public MouseState mouse;

        public Companion() 
        { 
        }

        public void Update(MouseState state)
        {

            mouse = state;
                //Vector2 newLocation = mouse.Position.ToVector2();
                position = new Vector2(mouse.X, mouse.Y);
            
            
        }
    }
}
