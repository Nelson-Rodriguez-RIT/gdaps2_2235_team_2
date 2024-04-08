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
        private Dictionary<TEnum, double> cooldowns;
        private Dictionary<TEnum, double> maxCooldowns;

        public double this[TEnum ability]
        {
            get
            {
                return cooldowns[ability];
            }
        }

        public AbilityCooldowns(Dictionary<string, string> properties) 
        { 
            //get cooldowns from file
            //string fileName = directory + "/cooldowns";

            cooldowns = new Dictionary<TEnum, double>();
            maxCooldowns = new Dictionary<TEnum, double>();

            TEnum[] enumArray = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
            string[] abilityNames = Enum.GetNames(typeof(TEnum));

            for (int i = 0; i < enumArray.Length; i++)
            {
                cooldowns.Add(enumArray[i], 0);

                if (properties.ContainsKey(abilityNames[i]))
                {
                    maxCooldowns.Add(enumArray[i], double.Parse(properties[abilityNames[i]]));
                }
                else
                {
                    throw new Exception(abilityNames[i] + " was not given a cooldown");
                }
                
            }
        }

        public void Update(GameTime gt)
        {
            double time = gt.ElapsedGameTime.TotalSeconds;

            foreach (KeyValuePair<TEnum, double> kv in cooldowns)
            {
                //Subtracts the elapsed time from the cooldowns
                cooldowns[kv.Key] = (kv.Value - time) >= 0 ? kv.Value - time : 0;
            }
        }

        public bool UseAbility(TEnum ability)
        {
            if (cooldowns[ability] == 0)
            {
                cooldowns[ability] = maxCooldowns[ability];
                return true;
            }

            return false;
        }
    }
}
