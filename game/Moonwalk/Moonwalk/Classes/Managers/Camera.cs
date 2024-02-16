using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers
{
    /// <summary>
    /// Provides camera functionality
    /// </summary>
    internal sealed class Camera
    {
        private static Camera instance = null;

        // Entity for the camera to focus on
        private Rectangle target;

        private Camera() {}

        /// <summary>
        /// Gets Camera's singleton instance
        /// </summary>
        /// <returns>A Camera object</returns>
        public static Camera GetInstance() {
            if (instance == null)
                instance = new Camera();

            return instance;
        }
    }
}
