using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GameControls_Concept
{
    internal abstract class Entity
    {
        protected Vector2 position;
        protected Texture2D image;
        protected Rectangle hitbox;
        protected LevelManager levelManager;

        public virtual Rectangle Hitbox
        {
            get { return hitbox; }
        }

        public virtual Vector2 Position
        {
            get { return position; }
        }

        public Entity(Texture2D image, LevelManager manager, Vector2 position)
        {
            this.position = position;          
            this.image = image;

            hitbox = new Rectangle
                ((int)position.X - (image.Width / 2),
                (int)position.Y - (image.Height / 2),
                image.Width,
                image.Height);
            this.levelManager = manager;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(image,
                hitbox,
                Color.White);
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        /*
        public static Vector2 CheckForPlatformCollision(
               List<Platform> platforms,               // List of platforms to check against
               Rectangle entity,                       // Entity hitbox
               Vector2 velocity,                       // Entity velocity
               out PhysicsState state) {               // State of the entity 
                                                       // Set maxIteraiton based on velocity TODO
            int maxIteration = 20; // Increase this number to increase collision precision

            // How many steps it can go before colliding into anything
            int peakXIteration = maxIteration;
            int peakYIteration = maxIteration;

            // Shorten iterations based on current peakIteration TODO

            foreach (Platform platform in platforms) // Check each platform
            {
                for (int iteration = 0; iteration <= maxIteration; iteration++) { // Check how many steps it can go before colliding into this platform
                    if (new Rectangle( // Check for horizontal collision
                            (int)(entity.X + ((velocity.X) / maxIteration) * iteration),
                            (int)entity.Y,
                            entity.Width,
                            entity.Height)
                            .Intersects(platform.Hitbox))
                        // We want the absolute minimum steps
                        peakXIteration = iteration - 1 < peakXIteration ? iteration - 1 : peakXIteration;

                    if (new Rectangle( // Check for vertical collision
                            (int)entity.X,
                            (int)(entity.Y + ((velocity.Y) / maxIteration) * iteration),
                            entity.Width,
                            entity.Height)
                            .Intersects(platform.Hitbox))
                        // We want the absolute minimum steps
                        peakYIteration = iteration - 1 < peakYIteration ? iteration - 1 : peakYIteration;
                }
            }

            //If there is a collision in the y direction, ground the entity
            if (peakYIteration < maxIteration) {
                state = PhysicsState.Grounded;
            }
            else {
                state = PhysicsState.Airborne;
            }

            // Update position and relevant hitbox based on peakIteration
            Vector2 temp = new Vector2(
                (entity.X + entity.Width / 2) + (velocity.X / maxIteration) * peakXIteration,
                (entity.Y + entity.Height / 2) + (velocity.Y / maxIteration) * peakYIteration);
            return temp;
        }
        */
    }
}
