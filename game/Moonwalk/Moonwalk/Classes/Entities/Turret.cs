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
            Idle,
            Charge,
            Shoot
        }

        enum Abilities
        {
            Shoot
        }

        private AbilityCooldowns<Abilities> cooldowns;
        private double timer;

        public Turret(Vector2 position) : base(position, "../../../Content/Entities/Turret")
        {
            gravity = 70f;
            double shootCooldown = 0;
            SwitchAnimation(Animations.Charge);
            shootCooldown += activeAnimation.AnimationLengthSeconds;
            
            properties["Shoot"] = shootCooldown.ToString();
            //timer = activeAnimation.AnimationLengthSeconds;
            SwitchAnimation(Animations.Charge, true);

            cooldowns = new AbilityCooldowns<Abilities>(properties);
            
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            timer -= gameTime.ElapsedGameTime.TotalSeconds;

            //slow down if moving
            if (velocity.X != 0)
            {
                acceleration.X = -Math.Sign(velocity.X) * 40;
            }

            //stop if slow enough
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
            //shoot at the player if cooldown is up
            if (cooldowns[Abilities.Shoot] == 0)
            {
                GameManager.SpawnEntity<StandardProjectile>(
                    new object[] { hurtbox.Center.ToVector2() + new Vector2(0, -15),
                VectorMath.Difference(hurtbox.Center.ToVector2() + new Vector2(0, -15), 
                Player.Location.ToVector2()),     //direction is from this to the player
                    Color.Red});

                cooldowns.UseAbility(Abilities.Shoot);
                
                timer = activeAnimation.AnimationLengthSeconds;
            }

            
        }

        public override void TakeDamage(int damage)
        {
            //can't be damaged
        }
    }
}
