using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Entities;

namespace Moonwalk.Classes.Maps
{
    internal class EnemySpawners
    {
        public static void SpawnFlowerEnemy(List<FlowerEnemy> type, Rectangle hitbox)
        {
            GameManager.SpawnEntity<FlowerEnemy>(new Vector2(hitbox.X, hitbox.Y));
        }
    }
}
