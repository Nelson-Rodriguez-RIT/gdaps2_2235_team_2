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
            Shoot,
            Jump,
            Land
        }

        private enum Phases
        {
            One,
            Two
        }

        private enum FaceDirection
        {
            Right,
            Left
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

        private const double AIFrequency = 10;
        private int frameTimer;
        private const float MoveSpeed = 12;

        FaceDirection faceDirection;

        private WidowBoss(Vector2 center) : base("../../../Content/WidowBoss", center)
        {
            

            foreach (Behavior behavior in Enum.GetValues(typeof(Behavior))) 
            {
                SwitchBehavior(behavior);
                properties[behavior.ToString()] = activeAnimation.AnimationLengthSeconds.ToString();

            }

            cooldowns = new AbilityCooldowns<Behavior>(properties);

            SwitchBehavior(Behavior.Idle);
            
        }

        public static void Start()
        {
            new WidowBoss(new Vector2(500, 400));
        }

        public override void Update(GameTime gt)
        {
            if (frameTimer > 0)
            {
                frameTimer--;
            }
            float xDifference = VectorMath.VectorDifference(center, Player.Location.ToVector2()).X;

            
            //Change the facing direction
            if (xDifference > 0)
            {
                faceDirection = FaceDirection.Right;
            }
            else if (xDifference < 0)
            {
                faceDirection = FaceDirection.Left;
            }

            activeAnimation.FaceDirection = (int)faceDirection;
            

            if (health > maxHealth / 2)
            {
                PhaseOne(gt);
            }
            else if (health > 0)
            {
                
            }
            else
            {
                //SwitchAnimation
            }

            base.Update(gt);
        }

        private void PhaseOne(GameTime gt)
        {
            cooldowns.Update(gt); 

            if (cooldowns[(Behavior)currentBehavior] == 0)
            {
                SelectBehavior();
            }

            switch (currentBehavior)
            {
                case Behavior.Forward:
                    center.X += MoveSpeed * (float)gt.ElapsedGameTime.TotalSeconds
                        * (faceDirection == FaceDirection.Right ? 1 : -1);
                    break;
                case Behavior.Attack:
                    if (frameTimer == activeAnimation.AnimationLength - 30)
                    {
                        Hitbox.activeHitboxes.Add(new Hitbox(
                            9 * 6, 
                            center, 
                            new Point(17, 29), 
                            GameManager.entities.GetAllOfType<Player>().Cast<IDamageable>().ToList(), 
                            new Point(34, -5)));

                        Hitbox.activeHitboxes.Add(new Hitbox(
                            9 * 6,
                            center,
                            new Point(51, 13),
                            GameManager.entities.GetAllOfType<Player>().Cast<IDamageable>().ToList(),
                            new Point(37, 11)));
                    }
                    break;

            }
        }

        private void SelectBehavior()
        {
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
                        if (VectorMath.VectorMagnitude(
                                VectorMath.VectorDifference(
                                    center, Player.Location.ToVector2())
                                ) 
                            < 100)
                        {
                            currentBehavior = Behavior.Attack;
                            
                        }
                        else
                        {
                            continue;
                        }
                        break;
                    case < 9:
                        currentBehavior = Behavior.Shoot;
                        break;
                }

                chosen = true;
            }

            SwitchBehavior(currentBehavior);

            if ((Behavior)currentBehavior == Behavior.Attack)
            {
                frameTimer = activeAnimation.AnimationLength;
            }
            
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
