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

namespace Moonwalk.Classes.Entities
{
    /// <summary>
    /// Special enemy that hits enemies instead of players
    /// </summary>
    internal class PlayerProjectile : Projectile
    {
        public PlayerProjectile(Vector2 position, Vector2 direction)
            : base(position, "", direction, 60f, 1)
        {
            damage = 1;

            //Projectile will despawn after hitting something
            collisions = 1;
            spriteSheet = Loader.LoadTexture("particle");
            hurtbox = new Rectangle(Position, new Point(7, 7));
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            base.Update(gameTime, input);

            if (CheckCollision<IDamageable>(out IDamageable collision))
            {
                const int Knockback = 50;

                collision.TakeDamage(this.damage);
                collision.Impulse(new Vector2(
                    Knockback * Math.Sign(VectorMath.VectorDifference(vectorPosition, collision.Position.ToVector2()).X),
                    Knockback));
                collisions--;
            }

            //Trail  
            Particle.Effects.Add(new Particle(6, Color.White, ParticleEffects.Random, hurtbox.Center,
                new Vector2(
                    -velocity.X,
                    -velocity.Y
                    ),
                1, 20, 5));
        }

        public override void AI()
        {
            //No AI
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
