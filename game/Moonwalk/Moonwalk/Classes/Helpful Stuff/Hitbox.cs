using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Managers;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Moonwalk.Classes.Helpful_Stuff
{
    internal class Hitbox
    {
        public static Texture2D hitboxImage;
        public static List<Hitbox> activeHitboxes = new List<Hitbox>();

        private Rectangle box;
        private Entity source;
        private int frameDuration;
        private Point offset;

        /// <summary>
        /// Things that the hitbox can hit, e.g. players, enemies
        /// </summary>
        Type target;

        public Hitbox(int duration, Entity source, Point size, Type target) 
        { 
            frameDuration = duration;
            this.target = target;
            this.source = source;
            box = new Rectangle(source.Position + offset, size);
            activeHitboxes.Add(this);
        }

        public void Update(GameTime gameTime)
        {
            
            box = new Rectangle(source.Position + offset, box.Size);

            //get targets and check collision

            frameDuration--;

            if (frameDuration == 0)
            {
                activeHitboxes.Remove(this);
            }
        }

        
    }
}
