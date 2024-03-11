using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Helpful_Stuff
{
    /// <summary>
    /// A data structure to hold ability cooldowns
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    internal class AbilityCooldowns<TEnum> where TEnum : Enum
    {
        /// <summary>
        /// Stores the name of the ability and its cooldown as the key, value is the time until it can be used again
        /// </summary>
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
