using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers
{
    /// <summary>
    /// Maintains the player's HUD
    /// </summary>
    internal sealed class GUI
    {
        private static GUI instance = null;


        private GUI() {}

        /// <summary>
        /// Gets GUI's singleton instance
        /// </summary>
        /// <returns>A GUI object</returns>
        public static GUI GetInstance() {
            if (instance == null)
                instance = new GUI();

            return instance;
        }
    }
}
