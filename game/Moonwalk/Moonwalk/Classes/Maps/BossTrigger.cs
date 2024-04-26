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
using Moonwalk.Classes.Entities;

namespace Moonwalk.Classes.Maps
{ 
    internal class BossTrigger<T> : Terrain where T : BossFight
    {
        
        public BossTrigger(Rectangle hitbox) : base(hitbox) 
        {
            collidable = false;

            //add boss spawning subscribers to the event
            if (typeof(T).IsAssignableTo(typeof(WidowBoss)))
            {
                OnCollision += WidowBoss.Start;
            }
            else
            {
                OnCollision += SpawnBloodKing;
            }
        }

        public void SpawnBloodKing()
        {
            new BloodKing(new Vector2(528, 559), (Player)GameManager.entities[typeof(Player)][0]);
        }

    }
}
