using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers
{
    /// <summary>
    /// Handles loading levels from file and level geometry
    /// </summary>
    internal sealed class Level
    {
        private static Level instance = null;


        private Level() {}

        /// <summary>
        /// Gets Level's singleton instance
        /// </summary>
        /// <returns>A Level object</returns>
        public static Level GetInstance() {
            if (instance == null)
                instance = new Level();

            return instance;
        }
    }
}
