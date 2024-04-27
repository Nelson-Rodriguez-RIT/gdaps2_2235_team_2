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
    internal class OOB_Properties
    {
        //damage that oob areas do
        private static double damage = 1;

        //return that damage
        public static int Damage 
        {
            get { return (int)damage; }
            set { damage = value; }
        }
    }
}
