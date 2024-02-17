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
    enum PhysicsState //to be added to
    {
        Grounded,
        Airborne
    }
    internal class MovableEntity : Entity
    {

        protected Vector2 velocity;
        protected Vector2 acceleration;

        protected float gravity;
        protected float terminalVelocity;
        protected float maxXVelocity;

        public virtual Vector2 Velocity
        {
            get { return velocity; }
        }

        public virtual Vector2 Acceleration
        {
            get { return acceleration; }
        }

        protected int CollisionAccuracy
        {
            get
            {
                return 
                    (int)(
                        Math.Sqrt(
                            Math.Pow(velocity.X, 2) + 
                            Math.Pow(Velocity.Y, 2))                        
                    );
            }
        }

        public MovableEntity(Texture2D image, LevelManager manager, Vector2 position) 
            : base(image, manager, position)
        {
            gravity = 0;
            terminalVelocity = 0;
            maxXVelocity = 0;

            velocity = new Vector2(0, 0);
            acceleration = new Vector2(0, 0);
        }

        public Vector2 CheckForPlatformCollision(
              List<Collider> platforms)               // List of platforms to check against     
        {     
            //Scaling iterations based on velocity
            int maxIteration = CollisionAccuracy > 1 ? CollisionAccuracy : 1; 


            // How many steps it can go before colliding into anything
            int peakXIteration = maxIteration;
            int peakYIteration = maxIteration;

            // Shorten iterations based on current peakIteration TODO

            foreach (Collider platform in platforms) // Check each platform
            {
                for (int iteration = 0; iteration <= maxIteration; iteration++)
                { // Check how many steps it can go before colliding into this platform
                    if (new Rectangle( // Check for horizontal collision
                            (int)(hitbox.X + ((velocity.X) / maxIteration) * iteration),
                            (int)hitbox.Y,
                            hitbox.Width,
                            hitbox.Height)
                            .Intersects(platform.Hitbox))
                        // We want the absolute minimum steps
                        peakXIteration = iteration - 1 < peakXIteration ? iteration - 1 : peakXIteration;

                    if (new Rectangle( // Check for vertical collision
                            (int)hitbox.X,
                            (int)(hitbox.Y + ((velocity.Y) / maxIteration) * iteration),
                            hitbox.Width,
                            hitbox.Height)
                            .Intersects(platform.Hitbox))
                        // We want the absolute minimum steps
                        peakYIteration = iteration - 1 < peakYIteration ? iteration - 1 : peakYIteration;
                }
            }

            // Update position and relevant hitbox based on peakIteration
            Vector2 temp = new Vector2(
                (hitbox.X + hitbox.Width / 2) + (velocity.X / maxIteration) * peakXIteration,
                (hitbox.Y + hitbox.Height / 2) + (velocity.Y / maxIteration) * peakYIteration);
            return temp;
        }
    }
}
