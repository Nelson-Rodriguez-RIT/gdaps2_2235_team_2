using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DynamicWindowSizing_Concept.Classes {
    internal class WindowManager {
        private static int defaultWidth;
        private static int defaultHeight;

        private static int width;
        private static int height;

        public static Vector2 WindowRatioScale {
            get {
                return new Vector2(
                    ((float) width) / defaultWidth,
                    ((float) height) / defaultHeight);
            }
        }


        public WindowManager(int initialWidth, int initialHeight) {
            defaultWidth = initialWidth;
            defaultHeight = initialHeight;

            width = initialWidth;
            height = initialHeight;
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
