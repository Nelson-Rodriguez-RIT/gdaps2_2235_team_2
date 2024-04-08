using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Boss
{
    internal class BossFight
    {
        public static BossFight Boss = null;

        List<Rectangle> hitboxes;
        int health;
        Texture2D spritesheet;
        Enum currentPhase;

        public BossFight(int health, string directory)
        {
            
        }
    }
}
