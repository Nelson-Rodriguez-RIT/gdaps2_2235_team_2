using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Entities;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Moonwalk.Classes.Helpful_Stuff
{
    internal class PlayerSpawner
    {
        private static double oobDamage = 1;
        private static int times;

        public static int OOB_Damage 
        {
            get { return (int)oobDamage; }
            set { oobDamage = value; }
        }

        public static int Times
        { get { return times; } }

        public static void RespawnCounter()
        {
            times++;
            oobDamage ++;
            Debug.WriteLine(oobDamage);
        }
    }
}
