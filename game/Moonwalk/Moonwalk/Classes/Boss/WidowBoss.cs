using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Managers;
using Moonwalk.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using Moonwalk.Classes.Entities;

namespace Moonwalk.Classes.Boss
{
    internal class WidowBoss : BossFight
    {
        private enum Abilities
        {
            Attack,
            Shoot,
            Jump,
            Land,
            Attack2
        }
        private enum Behavior
        {
            Idle,
            Forward,
            Attack,
            Barrage,
            Jump,
            Land
        }

        private enum Attacks
        {
            Collide,
            Claw,
            Projectile,
            Slam
        }



        

        /// <summary>
        /// How long an ability lasts
        /// </summary>
        private AbilityCooldowns<Abilities> durations;
        /// <summary>
        /// How long a behavior lasts
        /// </summary>
        private AbilityCooldowns<Behavior> cooldowns;
        /// <summary>
        /// The behavior the boss can use in this phase
        /// </summary>
        private List<Behavior> currentPhase;

        private double timer;
        private float MoveSpeed;

        

        private WidowBoss() : base("../../../Content/WidowBoss")
        {
            

            foreach (Behavior behavior in Enum.GetValues(typeof(Behavior))) 
            {
                SwitchBehavior(behavior);
                properties[behavior.ToString()] = activeAnimation.AnimationLengthSeconds.ToString();

            }

            MoveSpeed = float.Parse(properties["MoveSpeed"]);
            cooldowns = new AbilityCooldowns<Behavior>(properties);
            
            //Add the damage of each different attack
            foreach (string attack in Enum.GetNames(typeof(Attacks)))
            {
                attackDamage.Add(Enum.Parse<Attacks>(attack), int.Parse(properties[attack]));
            }

            SwitchBehavior(Behavior.Idle);
            
        }

        public static void Start()
        {
            new WidowBoss();
        }

        public override void Update(GameTime gt)
        {
            CheckCollision();

            if (timer > 0)
            {
                timer -= gt.ElapsedGameTime.TotalSeconds;
            }

            if (health > maxHealth / 2)
            {
                PhaseOne(gt);
            }
            else if (health > 0)
            {
                SwitchBehavior(Behavior.Jump);
            }
            else
            {
                //SwitchAnimation
            }

            base.Update(gt);
        }

        private void PhaseOne(GameTime gt)
        {
            
            if (cooldowns[(Behavior)currentBehavior] == 0)
            {
                SelectBehavior();
            }

            cooldowns.Update(gt);


            switch (currentBehavior)
            {
                case Behavior.Forward:
                    center.X += MoveSpeed * (float)gt.ElapsedGameTime.TotalSeconds
                        * (faceDirection == FaceDirection.Right ? 1 : -1);
                    break;
                case Behavior.Attack:
                    if (timer <= activeAnimation.AnimationLengthSeconds -0.5
                        && timer > activeAnimation.AnimationLengthSeconds - 0.515)
                    {
                        Hitbox arm = new Hitbox(
                            0.4,
                            center,
                            new Point(
                                17,
                                29),
                            GameManager.entities.GetAllOfType<Player>().Cast<IDamageable>().ToList(),
                            new Point(
                                faceDirection == FaceDirection.Right ? 34 : -34 - 17,
                                -5));

                        Hitbox shockwave = new Hitbox(
                            0.4,
                            center,
                            new Point(51, 13),
                            GameManager.entities.GetAllOfType<Player>().Cast<IDamageable>().ToList(),
                            new Point(
                                faceDirection == FaceDirection.Right ? 37 : -37 - 51,
                                11));

                        arm.targetEntered += this.DealDamage;
                        shockwave.targetEntered += this.DealDamage;

                        //if one hitbox is hit, the others can't be
                        arm.targetEntered += shockwave.AddToAlreadyHit;
                        shockwave.targetEntered += arm.AddToAlreadyHit;
                    }
                    break;
                case Behavior.Barrage:

                    break;

            }
        }
        
        private void SelectBehavior()
        {
            float xDifference = VectorMath.Difference(center, Player.Location.ToVector2()).X;

            //Change the facing direction
            if (xDifference > 0)
            {
                faceDirection = FaceDirection.Right;
            }
            else if (xDifference < 0)
            {
                faceDirection = FaceDirection.Left;
            }

            

            bool chosen = false;

            if ((Behavior)currentBehavior == Behavior.Jump)
            {
                currentBehavior = Behavior.Land;
            }

            while (!chosen)
            {
                int random = rng.Next(0, 8);

                switch (random)
                {
                    case < 2:
                        currentBehavior = Behavior.Idle;
                        break;
                    case < 4:
                        currentBehavior = Behavior.Forward;
                        break;
                    case < 8:
                        if (VectorMath.Magnitude(
                                VectorMath.Difference(
                                    center, Player.Location.ToVector2())
                                ) 
                            < 100)
                        {
                            
                            currentBehavior = Behavior.Attack;
                            currentAttack = Attacks.Claw;

                        }
                        else
                        {
                            continue;
                        }
                        break;
                    case < 9:
                        currentBehavior = Behavior.Barrage;
                        break;
                }

                chosen = true;
            }

            SwitchBehavior(currentBehavior);

            if ((Behavior)currentBehavior == Behavior.Attack)
            {
                timer = activeAnimation.AnimationLengthSeconds;
            }

            activeAnimation.FaceDirection = (int)faceDirection;

        }

        protected override void SwitchBehavior(Enum animationEnum, bool resetAnimation = true)
        {
            currentBehavior = animationEnum;

            base.SwitchBehavior(animationEnum, resetAnimation);

            if (cooldowns != null)
            cooldowns.UseAbility((Behavior)animationEnum);
        }

        
    }
}
