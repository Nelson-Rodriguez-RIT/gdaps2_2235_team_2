﻿using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Helpful_Stuff;
using System.IO;
using Moonwalk.Classes.Entities.Base;

namespace Moonwalk.Classes.Entities
{
    internal class StandardProjectile : Projectile
    {
        public StandardProjectile(Vector2 position, Vector2 direction, Color color)
            : base(position, "", direction, 50f, 1, color)
        {
            //Projectile will despawn after hitting something
            spriteSheet = Loader.LoadTexture("particle");
            hurtbox = new Rectangle(Position, new Point(5, 5));
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            base.Update(gameTime, input);

            Vector2 direction = Vector2.Normalize(velocity);

            //Trail effect
            Particle.Effects.Add(new Particle(0.1, this.color, ParticleEffects.Random, hurtbox.Center,
                new Vector2(
                    -(direction.X),
                    -(direction.Y)
                    ),
                1, 3)); 
        }

        public override void AI()
        {
            // no AI
        }

        
    }
}
