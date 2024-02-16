using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GameControls_Concept
{
    internal class Platform
    {
        private Rectangle hitbox;
        private Texture2D image;

        public Platform(Rectangle rectangle, Texture2D image) 
        {
            hitbox = rectangle;
            this.image = image;
        }
        public Rectangle Hitbox 
        { 
            get { return hitbox; } 
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, hitbox, Color.White);
        }

        public static Vector2 CheckForPlatformCollision(
                Object inputState,                      // Relevant input
                List<Platform> platforms,               // List of platforms to check against
                Rectangle entity,                       // Entity hitbox
                Vector2 velocity) {   // Entity velocity
            if (inputState is MouseState) {
                // Set maxIteraiton based on velocity TODO
                int maxIteration = 20; // Increase this number to increase collision precision

                // How many steps it can go before colliding into anything
                int peakXIteration = maxIteration;
                int peakYIteration = maxIteration;

                // Shorten iterations based on current peakIteration TODO

                foreach (Platform platform in platforms) // Check each platform
                    for (int iteration = 0; iteration <= maxIteration; iteration++) { // Check how many steps it can go before colliding into this platform
                        if (new Rectangle( // Check for horizontal collision
                                (int)(entity.X + ((velocity.X) / maxIteration) * iteration),
                                (int) entity.Y,
                                entity.Width,
                                entity.Height)
                                .Intersects(platform.Hitbox))
                            // We want the absolute minimum steps
                            peakXIteration = iteration - 1 < peakXIteration ? iteration - 1 : peakXIteration;

                        if (new Rectangle( // Check for vertical collision
                                (int) entity.X,
                                (int)(entity.Y + ((velocity.Y) / maxIteration) * iteration),
                                entity.Width,
                                entity.Height)
                                .Intersects(platform.Hitbox))
                            // We want the absolute minimum steps
                            peakYIteration = iteration - 1 < peakYIteration ? iteration - 1 : peakYIteration;
                    }

                // Update position and relevant hitbox based on peakIteration
                return new Vector2(
                    entity.X + (velocity.X / maxIteration) * peakXIteration,
                    entity.Y + (velocity.Y / maxIteration) * peakYIteration);
            } 

            else if (inputState is KeyboardState) {
                return Vector2.Zero;
            }

            return Vector2.Zero;
        }
    }
}
