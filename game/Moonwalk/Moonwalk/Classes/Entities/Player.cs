﻿using Microsoft.Xna.Framework;
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
    public delegate Vector2 GetRobotPosition();
    public delegate void ToggleBotLock();

    /// <summary>
    /// The player controlled character
    /// </summary>
    internal class Player : PlayerControlled, IJump, IDamageable
    {
        public static Point Location;
        protected enum Animations
        {
            Idle,
            Run,
            Attack,
            Shoot,
            Hit,
            Death,
            Jump
        }

        protected enum Abilities
        {
            Tether,
            Gravity
        }

        /// <summary>
        /// Cooldowns of each ability
        /// </summary>
        protected AbilityCooldowns<Abilities> cooldowns;

        int health;
        const int meleeDmg = 2;

        //Events
        public event GetRobotPosition GetRobotPosition;
        public event ToggleBotLock ToggleBotLock;

        private Animations animation;
        private FaceDirection faceDirection;

        //Timers
        private int animationTimer;
        private double iFrames;

        private float swingChange;
        private float maxAngVelocity;

        /// <summary>
        /// Determines if the entity is grounded or not
        /// </summary>
        public bool Grounded
        {
            get
            {
                if (CheckCollision(new Rectangle(
                        hurtbox.X,
                        hurtbox.Y + 5,
                        hurtbox.Width,
                        hurtbox.Height
                        )))
                {
                    return true;
                }

                return false;
            }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        //Make private later
        public Player(Vector2 position) : base(position, "../../../Content/Entities/Player")
        {
            gravity = 70f;
            acceleration = new Vector2(0, gravity);
            maxXVelocity = 40;
            maxYVelocity = 60;
            maxAngVelocity = 350;
            health = 3;

            SwitchAnimation(Animations.Idle);
            spriteScale = 1;

            cooldowns = new AbilityCooldowns<Abilities>(directory, 5);
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            //Decrease Iframes if the timer is running
            iFrames = iFrames > 0 ? iFrames - gameTime.ElapsedGameTime.TotalSeconds : 0;

            cooldowns.Update(gameTime);

            ChangeAnimation(input);

            base.Update(gameTime, input);

            int sign = 0;

            //Slow down if not pressing anything
            if (!input.IsPressed(Keys.D) &&
                !input.IsPressed(Keys.A)
                && velocity.X != 0)
            {
                sign = Math.Sign(velocity.X);
                acceleration.X = -Math.Sign(velocity.X) * 80;
            }

            if (sign != Math.Sign(velocity.X + acceleration.X * gameTime.ElapsedGameTime.TotalSeconds)
                && sign != 0)
            {
                acceleration.X = 0;
                velocity.X = 0;
            }

            if (velocity.X != 0
                && Math.Abs(velocity.X) < 1f)
            {
                velocity.X = 0;
                acceleration.X = 0;
            }

            //Change publically available position
            Location = this.Position;
        }

        public override void Movement(GameTime gt)
        {
            base.Movement(gt);
            
            //Check if player hits an enemy or projectile
            IHostile collision = null;

            if ((collision = HostileCollision()) != null
                && iFrames <= 0
                && collision is not PlayerProjectile)
            {
                TakeDamage(collision.Damage);

                //Make the player invincible for a short time
                iFrames = 1;

                //Stop tether if swinging
                if (physicsState == PhysicsState.Rotational)
                {
                    physicsState = PhysicsState.Linear;
                }

                //Knock the player back
                Impulse(new Vector2(
                    -45 * Math.Sign(VectorMath.VectorDifference(vectorPosition, collision.Position.ToVector2()).X),
                    -35));

            }

            if (physicsState == PhysicsState.Rotational)
            {
                //number of links to make
                const int Links = 10;

                //Distance between each link
                int slice = (int)(swingRadius / Links);

                for (int i = 1; i < Links; i++)
                {
                    Vector2 thing = Vector2.Normalize(VectorMath.VectorDifference(hurtbox.Center.ToVector2(), pivot)) 
                        * slice * i; //increment every iteration

                    Particle.Effects.Add(new Particle
                        (1, 
                        Color.SkyBlue, 
                        ParticleEffects.None,
                        (hurtbox.Center.ToVector2() + thing)
                            .ToPoint(),
                        0, 
                        3, 
                        4)
                        );
                }
            }
            
        }

        protected override void RotationalMotion(GameTime gt)
        {
            Vector2 oldPosition = new Vector2(vectorPosition.X, vectorPosition.Y);

            //Determine the angular acceleration using the perpendicular component of gravity
            angAccel = gravity * 10 * Math.Cos((Math.PI / 180) * theta);
            angAccel += swingChange;

            //Update velocity with acceleration and position with velocity
            angVelocity += angAccel * gt.ElapsedGameTime.TotalSeconds;

            if (Math.Abs(angVelocity) > maxAngVelocity)
            {
                angVelocity = Math.Sign(angVelocity) * maxAngVelocity;
            }

            theta += angVelocity * gt.ElapsedGameTime.TotalSeconds;

            //Determine new position using the new angle
            Vector2 temp = new Vector2(
                    (float)(pivot.X + swingRadius * Math.Cos((Math.PI / 180) * (theta))),
                    (float)(pivot.Y + swingRadius * Math.Sin((Math.PI / 180) * (theta))
                    ));


            vectorPosition = temp;

            //Update position
            hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);

            if (CheckCollision())           // If there is a collision, switch back to linear motion
            {
                vectorPosition = oldPosition;
                physicsState = PhysicsState.Linear;

                //This determines the velocity the entity will have after 
                //it stops swinging by converting the angular velocity
                //back to linear velocity.
                velocity = new Vector2(                                       // 3000: random number for downscaling (it was too big)
                    (float)(angVelocity * swingRadius * -Math.Sin((Math.PI / 180) * (theta)) / 3000),
                    (float)(angVelocity * swingRadius * Math.Cos((Math.PI / 180) * (theta))) / 3000);
                acceleration = new Vector2(
                    acceleration.X, gravity);

                ToggleBotLock();
                LinearMotion(gt);
            }
        }

        public override void Input(StoredInput input)
        {
            
            //Horizontal movement
            if (input.IsPressed(Keys.A) &&
                !input.IsPressed(Keys.D))
            {
                // acceleration is higher if the player is moving in the opposite direction for smoother movement
                acceleration.X = velocity.X > 0 ? -maxXVelocity * 5f : -maxXVelocity * 2f;
            }
            else if (input.IsPressed(Keys.D) &&
                !input.IsPressed(Keys.A))
            {
                // acceleration is higher if the player is moving in the opposite direction for smoother movement
                acceleration.X = velocity.X < 0 ? maxXVelocity * 5f : maxXVelocity * 2f;
            }

            if (physicsState == PhysicsState.Rotational)
            {
                if (input.IsPressed(Keys.A) &&
                !input.IsPressed(Keys.D))
                {
                    swingChange = 50;
                }
                else if (input.IsPressed(Keys.D) &&
                    !input.IsPressed(Keys.A))
                {
                    swingChange = -50;
                }
            }

            //Jump 
            if ((input.IsPressed(Keys.Space)
                && !input.WasPressed(Keys.Space))
                || input.Buffered.Exists(item => item.Key == Keys.Space))
            {
                if (Grounded) 
                {
                    velocity.Y = -50;

                    BufferedInput buffer = input.Buffered.Find(item => item.Key == Keys.Space);
                    input.Buffered.Remove(buffer);
                }   
                else
                {
                    input.Buffer(Keys.Space);
                }
            }

                //Robot abilities:

            //Gravity ability
            if (input.CurrentMouse.LeftButton == ButtonState.Pressed
                && input.PreviousMouse.LeftButton == ButtonState.Released
                && cooldowns.UseAbility(Abilities.Gravity))
            {
                Vector2 robotPos = GetRobotPosition();
                GravityAbility(robotPos);
            }

            //Tether ability - planning to have this be able to swing blocks and stuff too, maybe send back projectiles?
            if (physicsState == PhysicsState.Linear
                && input.CurrentMouse.RightButton == ButtonState.Pressed
                && input.PreviousMouse.RightButton == ButtonState.Released
                && cooldowns[Abilities.Tether] == 0)
            {
                Vector2 robotPos = GetRobotPosition();

                if (VectorMath.VectorMagnitude(
                        VectorMath.VectorDifference(vectorPosition, robotPos)
                        )
                    < 125)
                {
                    cooldowns.UseAbility(Abilities.Tether);
                    SetRotationalVariables(robotPos);
                    ToggleBotLock();
                }
                

            }
            else if (input.CurrentMouse.RightButton == ButtonState.Released
                && input.PreviousMouse.RightButton == ButtonState.Pressed)
            {
                if (physicsState == PhysicsState.Rotational)
                {
                    SetLinearVariables();
                    ToggleBotLock();
                }  
            }

        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);

            /*
            batch.DrawString(GUI.GetFont("File"), 
                $"{Math.Round(vectorPosition.X)} - {Math.Round(vectorPosition.Y)} ",
                new Vector2(400, 50),
                Color.White);
            */

            Vector2 temp = Camera.WorldToScreen(Camera.RelativePosition(new Vector2(0,0)));
                //Camera.RelativePosition((Camera.RelativePosition((Vector2.Zero)) + vectorPosition * GameMain.ActiveScale - Camera.GlobalOffset));
            //testing
            //Continue here future Dante (converting coordinates)
            batch.Draw(hitboxSprite, new Rectangle(
                (int)temp.X,
                (int)temp.Y,
                20, 20), Color.White);
            
        }

        protected IHostile HostileCollision()
        {
            List<IHostile> list = GameManager.entities.GetAllOfType<IHostile>();

            IHostile collision = list.Find(enemy => 
            new Rectangle(
                enemy.Position.X,
                enemy.Position.Y,
                enemy.Hitbox.Width,
                enemy.Hitbox.Height)
            .Intersects(
                new Rectangle(
                    Position.X,
                    Position.Y,
                    Hitbox.Width,
                    Hitbox.Height)
                )
            );

            return collision;
        }

        /// <summary>
        /// Use this to see if an attack hits an enemy
        /// </summary>
        /// <param name="hitbox"></param>
        /// <returns></returns>
        protected IDamageable[] EnemyCollision(Rectangle hitbox)
        {
            List<IDamageable> list = GameManager.entities.GetAllOfType<IDamageable>();
            list.Remove(this);

            IDamageable[] collisions = list.FindAll(damageable =>
            new Rectangle(
                damageable.Position.X,
                damageable.Position.Y,
                damageable.Hitbox.Width,
                damageable.Hitbox.Height)
            .Intersects(new Rectangle(
                    hitbox.X,
                    hitbox.Y,
                    hitbox.Width,
                    hitbox.Height
                    )))
                .ToArray();

            return collisions;
        }

        protected override void LinearMotion(GameTime gt)
        {
            float time = (float)gt.ElapsedGameTime.TotalSeconds;

            int iterationCounter = 1;       // Number of collision checks we've done

            Point lastSafePosition = new Point(Position.X, Position.Y);        //Last point before a collision

            if (!Grounded)
            {
                acceleration.Y = gravity * (1 + (100 - Math.Abs(velocity.Y)) / 50);
            }
            


            velocity += acceleration * time;                                   //Update velocity
            Vector2 tempVelocity = new Vector2(velocity.X, velocity.Y);        //For if the player is airborne

            

            //Vertical
            while (iterationCounter <= CollisionAccuracy)                      //Scaling number of checks
            {
                if (!CheckCollision())
                {
                    lastSafePosition = new Point(Position.X, Position.Y);      //Store old position in case we collide
                }

                //Cap velocity
                if (Math.Abs(velocity.Y) > maxYVelocity)
                {
                    velocity.Y = maxYVelocity * Math.Sign(tempVelocity.Y);
                }

                vectorPosition.Y += tempVelocity.Y * (time * iterationCounter / CollisionAccuracy);     // Increment position

                hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);                      // Update hitbox location

                if (CheckCollision())                                                   // Check if there was a collision
                {
                    hurtbox = new Rectangle(lastSafePosition, hurtbox.Size);              // Revert hitbox position back to before collision
                    vectorPosition = lastSafePosition.ToVector2();                      // Revert position
                    velocity.Y = 0;
                    break;
                }

                iterationCounter++;
            }


            //Do the same thing but in the X direction
            iterationCounter = 1;

            while (!CheckCollision() && iterationCounter <= CollisionAccuracy)
            {
                if (!CheckCollision())
                {
                    lastSafePosition = new Point(Position.X, Position.Y);
                }

                //Cap velocity
                if (Math.Abs(velocity.X) > maxXVelocity)
                {
                    velocity.X = maxXVelocity * Math.Sign(velocity.X);
                }

                if (!Grounded)
                {
                    tempVelocity.X = velocity.X / 1.2f;
                }

                vectorPosition.X += tempVelocity.X * (time * iterationCounter / CollisionAccuracy);

                hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);

                if (CheckCollision())
                {
                    hurtbox = new Rectangle(lastSafePosition, hurtbox.Size);
                    vectorPosition = lastSafePosition.ToVector2();
                    velocity.X = 0;
                    break;
                }
                iterationCounter++;

            }
        }

        private void ChangeAnimation(StoredInput input)
        {
            
            // For animations that play until they are done, don't change the animation until then
            if (animationTimer > 0)
            {
                animationTimer--;
                return;
            }         

            // Change facing direciton of the player
            if (velocity.X < 0
                && faceDirection != FaceDirection.Left)
            {
                faceDirection = FaceDirection.Left;
            }
            else if (velocity.X > 0
                && faceDirection != FaceDirection.Right)
            {
                faceDirection = FaceDirection.Right;
            }

            // Update the face direction in the animation class
            activeAnimation.FaceDirection = (int)faceDirection;

            // If not moving, play idle
            if (velocity.X == 0 && velocity.Y == 0)
            {
                SwitchAnimation(Animations.Idle, false);
            }

            // If moving horizontally, play run
            if (velocity.X != 0)
            {                
                SwitchAnimation(Animations.Run, false);
            }

            // If in the air, play airborne animation
            if (!Grounded)
            {
                SwitchAnimation(Animations.Jump);
            }

            // if E is pressed, play melee attack animation
            if (input.IsPressed(Keys.E) && 
                !input.WasPressed(Keys.E))
            {
                SwitchAnimation(Animations.Attack);
                Attack();
                animationTimer = activeAnimation.AnimationLength;
            }

            // if F is pressed, play ranged attack animation
            if (input.IsPressed(Keys.F) &&
                activeAnimation.AnimationValue != (int)(Animations.Shoot))
            {
                SwitchAnimation(Animations.Shoot, 
                    true
                    );
                GameManager.SpawnEntity<PlayerProjectile>(
                    hurtbox.Center.ToVector2() + new Vector2(
                        faceDirection == FaceDirection.Left ? -hurtbox.Width : hurtbox.Width, 
                        -4),
                    new object[]
                    {
                        faceDirection == FaceDirection.Left ? new Vector2(-1, 0) : new Vector2(1, 0)
                    });

                animationTimer = activeAnimation.AnimationLength;
            }
        }

        private void Attack()
        {
            Hitbox attack = null;

            if (faceDirection == FaceDirection.Right) 
            {
                attack = new Hitbox(
                20,
                this,
                new Point(
                    14,
                    20),
                typeof(IDamageable),
                GameManager.entities.GetAllOfType<IDamageable>(),
                new Point(
                    10, -2));
            }
            else
            {
                attack = new Hitbox(
                20,
                this,
                new Point(
                    14,
                    20),
                typeof(IDamageable),
                GameManager.entities.GetAllOfType<IDamageable>(),
                new Point(
                    -16, -2));
            }

            

            attack.targetEntered += this.DealDamage;
        }

        private void DealDamage(List<IDamageable> list)
        {
            const int Knockback = 50;

            foreach (IDamageable item in list)
            {
                item.TakeDamage(meleeDmg);
                item.Impulse(new Vector2(
                    Knockback * Math.Sign(VectorMath.VectorDifference(vectorPosition, item.Position.ToVector2()).X),
                    Knockback));
            }
        }

        
        private void GravityAbility(Vector2 robotPos)
        {
            const int Range = 120;
            // Get a list of movables from the game manager
            List<IMovable> movables = GameManager.entities.GetAllOfType<IMovable>();

            // Make all entities move towards this
            foreach (IMovable movable in movables)
            {
                // Check that entity is within range
                if (Math.Sqrt(
                        Math.Pow(movable.Position.X - robotPos.X, 2) +
                        Math.Pow(movable.Position.Y - robotPos.Y, 2)
                        )
                    < Range
                    && movable is not Robot)
                {
                    const int GravityStrength = 100;

                    // Apply the impulse towards the robot
                    Vector2 difference = VectorMath.VectorDifference(vectorPosition, robotPos);
                    difference.Normalize();
                    movable.Impulse(difference * GravityStrength);
                }

            }

            //draw gravity effect
            const int Particles = 72;
            double slice = 360 / Particles;

            for (int i = 0; i < Particles; i++)
            {
                Vector2 pos = new Vector2(
                    (float)Math.Sin(slice * i) * Range,
                    (float)Math.Cos(slice * i) * Range);

                pos.Normalize();

                Point location = robotPos.ToPoint() + (pos * Range).ToPoint();

                Particle.Effects.Add(
                    new Particle(
                        30,
                        Color.Lime,
                        ParticleEffects.Random,
                        location));
            }
        }
        
        public void TakeDamage(int damage)
        {
            Health -= damage;
        }
        
    }
}
