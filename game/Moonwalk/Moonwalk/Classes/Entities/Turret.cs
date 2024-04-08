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
    internal class Turret : Enemy
    {
        enum Animations
        {
            Charge,
            Shoot
        }

        enum Abilities
        {
            Shoot
        }

        private AbilityCooldowns<Abilities> cooldowns;

        public Turret(Vector2 position) : base(position, "../../../Content/Entities/Turret")
        {
            gravity = 70f;
            SwitchAnimation(Animations.Charge);
            cooldowns = new AbilityCooldowns<Abilities>(properties);
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            //slow down if moving
            if (velocity.X != 0)
            {
                acceleration.X = -Math.Sign(velocity.X) * 40;
            }

            if (velocity.X != 0
                && Math.Abs(velocity.X) < 1f)
            {
                velocity.X = 0;
                acceleration.X = 0;
            }

            cooldowns.Update(gameTime);
            base.Update(gameTime, input);
        }

        public override void AI()
        {
            if (activeAnimation.AnimationValue != 2
                && cooldowns[Abilities.Shoot] == 0)
            {
                GameManager.SpawnEntity<StandardProjectile>
                (hurtbox.Center.ToVector2() + new Vector2(0, -3),
                new object[] { VectorMath.VectorDifference(vectorPosition, Player.Location.ToVector2())});

                cooldowns.UseAbility(Abilities.Shoot);
            }
            
        }

        public override void TakeDamage(int damage)
        {

        }
    }
}
