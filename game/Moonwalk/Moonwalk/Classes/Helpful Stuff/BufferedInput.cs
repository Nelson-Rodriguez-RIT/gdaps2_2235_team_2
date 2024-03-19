using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Helpful_Stuff
{
    /// <summary>
    /// Stores a buffered input
    /// </summary>
    internal class BufferedInput
    {
        private Keys key;
        public int timer;

        public Keys Key 
        { 
            get { return key; } 
        }

        public BufferedInput(Keys key) 
        { 
            this.key = key;
            this.timer = 6;
        }

        public override string ToString()
        {
            return $"{key} - {timer} frames";
        }
    }
}
