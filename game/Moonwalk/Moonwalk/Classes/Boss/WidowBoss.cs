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
            Land,
            Barrage,
            Death,
            InAir
        }

        private enum Attacks
        {
            Collide,
            Claw,
            Projectile,
            Slam
        }

        /// <summary>
        /// What behavior to do next
        /// </summary>
        private Queue<Behavior> behaviorQueue;

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
        private bool actionHasBeenDone = false;
        bool phaseChange = false;
        

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

            behaviorQueue = new Queue<Behavior>();

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
                if (!phaseChange)
                {
                    //Particle effects
                    Particle.Effects.Add(new Particle(
                        1,
                        Color.Red,
                        ParticleEffects.Random,
                        center.ToPoint(),
                        0.1,
                        60,
                        40
                        ));

                    //phase change attacks
                    behaviorQueue.Enqueue(Behavior.Jump);
                    behaviorQueue.Enqueue(Behavior.InAir);
                    behaviorQueue.Enqueue(Behavior.Land);
                    behaviorQueue.Enqueue(Behavior.Barrage);
                    behaviorQueue.Enqueue(Behavior.Jump);
                    behaviorQueue.Enqueue(Behavior.InAir);
                    behaviorQueue.Enqueue(Behavior.Land);
                    behaviorQueue.Enqueue(Behavior.Barrage);
                    behaviorQueue.Enqueue(Behavior.Jump);
                    behaviorQueue.Enqueue(Behavior.InAir);
                    behaviorQueue.Enqueue(Behavior.Land);
                    behaviorQueue.Enqueue(Behavior.Barrage);
                    phaseChange = true;
                }
                
                PhaseOne(gt);
            }
            else
            {
                Boss = null;
            }

            base.Update(gt);
        }

        private void PhaseOne(GameTime gt)
        {
            
            if (cooldowns[(Behavior)currentBehavior] == 0)
            {
                SelectBehavior();
                actionHasBeenDone = false;
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
                        && !actionHasBeenDone)
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

                        actionHasBeenDone = true;
                    }
                    break;
                case Behavior.Barrage:
                    if (timer <= activeAnimation.AnimationLengthSeconds - 0.5
                        && !actionHasBeenDone)
                    {
                        for (int i = 1; i < 8; i+=2)
                        {
                            double angle = -i * 180 / 8f / 180 * Math.PI;

                            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                            GameManager.SpawnEntity<WidowProjectile>(
                                new object[]
                                {
                                    center + direction * 30f,
                                    direction,
                                    attackDamage[Attacks.Projectile]
                                });
                        }

                        actionHasBeenDone = true;
                    }
                    break;

                case Behavior.Jump:
                    if (!actionHasBeenDone && behaviorQueue.Count == 0)
                    {
                        behaviorQueue.Enqueue(Behavior.InAir);
                        behaviorQueue.Enqueue(Behavior.Land);
                        actionHasBeenDone = true;
                    }
                   
                    break;
                case Behavior.InAir:
                    if (!actionHasBeenDone)
                    {
                        center.X = Player.Location.X;
                        actionHasBeenDone = true;
                    }
                    

                    if (timer < 1.5 && timer > 0.5)
                    {
                        for (int i = -80; i < 81; i+=4)
                        {
                            if (rng.Next(0, 7) == 0)
                            Particle.Effects.Add(
                                new Particle(rng.NextDouble(), Color.Red, ParticleEffects.Random,
                                    new Point(
                                        (int)center.X + i,
                                        (int)center.Y + 23),
                                    new Vector2(0, -0.5f),
                                    0.15));
                        }
                        
                    }

                    break;
                case Behavior.Land:
                    if (timer < activeAnimation.AnimationLengthSeconds - 0.1
                        && !actionHasBeenDone)
                    {
                        //Create hitbox for the slam 
                        Hitbox slam = new Hitbox(
                            0.2,
                            center,
                            new Point(
                                133,
                                12),
                            GameManager.entities.GetAllOfType<Player>().Cast<IDamageable>().ToList(),
                            new Point(
                                faceDirection == FaceDirection.Right ? -69 : 69 - 133,
                                13));
                        actionHasBeenDone = true;

                        slam.targetEntered += this.DealDamage;
                    }
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

            if (behaviorQueue.Count > 0)
            {
                currentBehavior = behaviorQueue.Dequeue();
                chosen = true;
            }

            while (!chosen)
            {
                int random = rng.Next(0, 14);

                switch (random)
                {
                    case < 2:
                        currentBehavior = Behavior.Idle;
                        break;
                    case < 8:
                        currentBehavior = Behavior.Forward;
                        break;
                    case < 10:
                        currentBehavior = Behavior.Attack;
                        currentAttack = Attacks.Claw;
                        break;
                    case < 11:
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
                    case < 13:
                        currentBehavior = Behavior.Barrage;
                        break;
                    case < 14:
                        currentBehavior = Behavior.Jump;
                        currentAttack = Attacks.Slam;

                        
                        break;
                }

                chosen = true;
            }

            SwitchBehavior(currentBehavior);

 
        }

        protected override void SwitchBehavior(Enum animationEnum, bool resetAnimation = true)
        {
            currentBehavior = animationEnum;
            actionHasBeenDone = false;
           

            base.SwitchBehavior(animationEnum, resetAnimation);

            timer = activeAnimation.AnimationLengthSeconds;
            activeAnimation.FaceDirection = (int)faceDirection;

            if (cooldowns != null)
            cooldowns.UseAbility((Behavior)animationEnum);
        }

        
    }
}
