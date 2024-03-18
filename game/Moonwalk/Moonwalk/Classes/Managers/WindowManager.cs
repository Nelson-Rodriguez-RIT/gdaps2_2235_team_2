using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Moonwalk.Classes.Managers {
    internal class WindowManager {
        private static WindowManager instance = null;

        private int defaultWidth;
        private int defaultHeight;

        private int width;
        private int height;

        public Vector2 WindowRatioScale {
            get {
                return new Vector2(
                    ((float) width) / defaultWidth,
                    ((float) height) / defaultHeight);
            }
        }

        /// <summary>
        /// Access the windowmanager instance so we can get the center
        /// </summary>
        public static WindowManager Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Center of the window
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return new Vector2(
                    width / 2,
                    height / 2
                    );
            }
        }

        private WindowManager(int initialWidth, int initialHeight) {
            defaultWidth = initialWidth;
            defaultHeight = initialHeight;

            width = initialWidth;
            height = initialHeight;
        }


        public static WindowManager GetInstance(int initialWidth, int initialHeight) {
            if (instance == null)
                instance = new WindowManager(initialWidth, initialHeight);

            return instance;
        }

        public void Update(int nextWidth, int nextHeight) {
            width = nextWidth;
            height = nextHeight;
        }

        public void Reset() {
            width = defaultWidth;
            height = defaultHeight;
        }
    }
}
