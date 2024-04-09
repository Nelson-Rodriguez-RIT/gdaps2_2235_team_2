using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Helpful_Stuff;
using System.IO;
using Moonwalk.Classes;
using Moonwalk.Classes.Boss;

namespace Moonwalk.Classes.Maps
{
    public delegate void StartBoss(Vector2 pos);
    internal class BossTrigger<T> : Terrain where T : BossFight
    {
        public event StartBoss StartBoss;
        public BossTrigger(Rectangle hitbox) : base(hitbox) 
        {
            collidable = false;
            OnCollision += WidowBoss.Start;
        }

    }
}
