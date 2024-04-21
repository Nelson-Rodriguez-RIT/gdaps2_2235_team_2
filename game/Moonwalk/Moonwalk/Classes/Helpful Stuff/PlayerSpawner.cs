using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Entities;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Helpful_Stuff
{
    internal class PlayerSpawner
    {
        private static int oobDamage = 1;

        public static int OOB_Damage 
        {
            get { return oobDamage; }
        }

        public static void RespawnCounter()
        {
            oobDamage++;
        }
    }
}
