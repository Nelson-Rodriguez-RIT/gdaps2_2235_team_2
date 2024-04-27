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
        /// <summary>
        /// Different behaviors the boss can exhibit
        /// </summary>
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

        /// <summary>
        /// Attacks that are tied with a damage value
        /// </summary>
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
            
            //tie durations of behaviors to the behaviors
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

        /// <summary>
        /// Begin the boss
        /// </summary>
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

            //determine behavior based on phase
            if (health > maxHealth / 2)
            {
                PhaseOne(gt);
            }
            else if (health > 0)
            {
                //phase change
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
                //boss dies
                Boss = null;
            }

            base.Update(gt);
        }

        /// <summary>
        /// Behavior during phase 1 (and currently phase 2)
        /// </summary>
        /// <param name="gt"></param>
        private void PhaseOne(GameTime gt)
        {
            //if behavior is done, choose a new one
            if (cooldowns[(Behavior)currentBehavior] == 0)
            {
                SelectBehavior();
                actionHasBeenDone = false;
            }

            cooldowns.Update(gt);

            //based on current behavior, do actions
            switch (currentBehavior)
            {
                case Behavior.Forward:
                    //move forward
                    center.X += MoveSpeed * (float)gt.ElapsedGameTime.TotalSeconds
                        * (faceDirection == FaceDirection.Right ? 1 : -1);
                    break;
                case Behavior.Attack:
                    //spawn hitboxes
                    if (timer <= activeAnimation.AnimationLengthSeconds -0.5
                        && !actionHasBeenDone)
                    {
                        //arm hitbox
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

                        //shockwave hitbbox
                        Hitbox shockwave = new Hitbox(
                            0.4,
                            center,
                            new Point(51, 13),
                            GameManager.entities.GetAllOfType<Player>().Cast<IDamageable>().ToList(),
                            new Point(
                                faceDirection == FaceDirection.Right ? 37 : -37 - 51,
                                11));

                        //add subscribers
                        arm.targetEntered += this.DealDamage;
                        shockwave.targetEntered += this.DealDamage;

                        //if one hitbox is hit, the others can't be
                        arm.targetEntered += shockwave.AddToAlreadyHit;
                        shockwave.targetEntered += arm.AddToAlreadyHit;

                        actionHasBeenDone = true;
                    }
                    break;
                case Behavior.Barrage:
                    //shoot projectiles
                    if (timer <= activeAnimation.AnimationLengthSeconds - 0.5
                        && !actionHasBeenDone)
                    {
                        //spawn in an arc above the boss, moving away from the center of the boss
                        for (int i = 1; i < 8; i+=2)
                        {
                            //calculate angle
                            double angle = -i * 180 / 8f / 180 * Math.PI;

                            //calculate direction
                            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                            //spawn projectile
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
                    //add air and land after (they always have to come after jump)
                    if (!actionHasBeenDone && behaviorQueue.Count == 0)
                    {
                        behaviorQueue.Enqueue(Behavior.InAir);
                        behaviorQueue.Enqueue(Behavior.Land);
                        actionHasBeenDone = true;
                    }
                   
                    break;
                case Behavior.InAir:
                    //move to the player when jumping
                    if (!actionHasBeenDone)
                    {
                        center.X = Player.Location.X;
                        actionHasBeenDone = true;
                    }
                    
                    //indicator for dropping location
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
                    //create hitbox
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

            //if there is queued behavior, use that
            if (behaviorQueue.Count > 0)
            {
                currentBehavior = behaviorQueue.Dequeue();
                chosen = true;
            }

            //semi-randomly choose a new behavior
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
                            //higher chance to get claw attack if player is close
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
            //this is like switchanimation but for a boss
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
