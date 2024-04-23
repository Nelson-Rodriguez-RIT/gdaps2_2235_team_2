using Moonwalk.Interfaces;
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
    internal class WidowProjectile : Projectile
    {
        private bool hasShot = false;
        private bool shoot = false;
        private int particleSpawnCounter = 0;

        public WidowProjectile(Vector2 position, Vector2 direction, int damage) 
            : base(position, "", direction, 35f, damage, Color.Red)
        {
            acceleration = -velocity / 1f;
            spriteSheet = Loader.LoadTexture("particle");
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
               0.05, 3, 4));


        }

        public override void AI()
        {
            if (timer < 4 && !hasShot)
            {
                shoot = true;
                hasShot = true;
                acceleration = Vector2.Zero;
            }

            if (timer < 3  && shoot) 
            {
                shoot = false;
                velocity = speed * 2 * Vector2.Normalize(Player.Location.ToVector2() - this.hurtbox.Center.ToVector2());
            }
        }
    }
}
