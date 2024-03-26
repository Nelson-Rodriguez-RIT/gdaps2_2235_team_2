using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noah_s_Level_Design_Concept
{
    public abstract class Item : GameObject
    {
        private bool isHeld;
        private bool exists = false;

        public Item(Rectangle position, Texture2D asset) : base (asset, position) 
        {
            exists = true;  
        }

        public void CheckCollision(GameObject obj)
        { }
    }
}
