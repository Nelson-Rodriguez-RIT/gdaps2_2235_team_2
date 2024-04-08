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
            Backward,
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
            Left,
            Right
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
        private double AITimer;

        Behavior currentBehavior;
        FaceDirection faceDirection;

        public WidowBoss() : base("../../Content/WidowBoss")
        {
            cooldowns = new AbilityCooldowns<Behavior>(properties);

        }

        public override void Update(GameTime gt)
        {
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

            switch (health)
            {
                case > 50:
                    PhaseOne(gt);
                    break;
                case > 0:
                    break;
                default:
                    break;
            }

            base.Update(gt);
        }

        private void PhaseOne(GameTime gt)
        {
            cooldowns.Update(gt); 

            if (cooldowns[currentBehavior] == 0)
            {
                SelectBehavior();
            }

            switch (currentBehavior)
            {
                case Behavior.Idle:
                    break;

            }
        }

        private void SelectBehavior()
        {
            bool chosen = false;

            if (currentBehavior == Behavior.Jump)
            {
                currentBehavior = Behavior.Land;
            }

            while (!chosen)
            {
                int random = rng.Next(0, 10);

                switch (random)
                {
                    case < 1:
                        currentBehavior = Behavior.Idle;
                        break;
                    case < 3:
                        currentBehavior = Behavior.Forward;
                        break;
                    case < 5:
                        currentBehavior = Behavior.Backward;
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
            
        }
    }
}
