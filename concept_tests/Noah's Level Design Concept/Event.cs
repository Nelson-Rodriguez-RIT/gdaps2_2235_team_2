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
    internal class Event : Platform
    {
        public Event(Rectangle rectangle) : base(rectangle)
        { }

        //if user collides with an event platform,
        //an event will play based on which one it is
    }
}
