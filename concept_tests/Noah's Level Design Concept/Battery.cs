using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noah_s_Level_Design_Concept
{
    internal class Battery : InteractiveItem
    {
        //this is an interactive item the player picks up and can insert/take out of things
        //needs a boundary box, an asset, a state
        protected bool canPower;
        protected bool isPowering;

        public Battery(Texture2D asset, Rectangle hitbox, bool canBePickedUp, bool canPower) : 
            base(asset, hitbox, canBePickedUp, canPower)
        {
            
        
        }
    }
}
