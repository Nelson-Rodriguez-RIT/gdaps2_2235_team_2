using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Helpful_Stuff
{
    internal class AbilityCooldowns<TEnum> where TEnum : Enum
    {
        private Dictionary<Tuple<TEnum, double>, double> cooldowns;

        public AbilityCooldowns(string directory) 
        { 
            //get cooldowns from file
            //string fileName = directory + "/cooldowns";

            cooldowns = new ();

            TEnum[] enumArray = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();

            for (int i = 0; i < enumArray.Length; i++)
            {
                cooldowns.Add(
                    new Tuple<TEnum, double>(
                        enumArray[i],
                        5),    //Placeholder, read cooldown from file
                    0);
            }
        }

        public void Update(GameTime gt)
        {
            double time = gt.ElapsedGameTime.TotalSeconds;

            foreach (KeyValuePair<Tuple<TEnum, double>, double> kv in cooldowns)
            {
                //Subtracts the elapsed time from the cooldowns
                cooldowns[kv.Key] = (kv.Value - time) >= 0 ? kv.Value - time : 0;
            }
        }
    }
}
