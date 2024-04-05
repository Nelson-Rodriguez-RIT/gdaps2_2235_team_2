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
    internal class TestProjectile : Projectile
    {
        public TestProjectile(Vector2 position, Vector2 direction)
            : base(position, "", direction, 50f, 1)
        {
            //Projectile will despawn after hitting something
            collisions = 1;
            spriteSheet = Loader.LoadTexture("particle");
            hurtbox = new Rectangle(Position, new Point(10, 10));
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            base.Update(gameTime, input);

            //Trail effect
            Particle.Effects.Add(new Particle(10, Color.SkyBlue, ParticleEffects.Random, hurtbox.Center, 3));
        }

        public override void AI()
        {
            //no change in motion
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(
                spriteSheet,
                Camera.RelativePosition(hurtbox.Center),
                null,
                Color.White,
                0f,
                new Vector2(0, 0),
                new Vector2(1f, 1),
                SpriteEffects.None,
                0
                );
        }
    }
}
