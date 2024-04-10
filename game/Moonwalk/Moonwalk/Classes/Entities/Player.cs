﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Maps;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Moonwalk.Classes.Entities
{
    public delegate Vector2 GetRobotPosition();
    public delegate void ToggleBotLock();

    /// <summary>
    /// The player controlled character
    /// </summary>
    internal class Player : PlayerControlled, IDamageable, ISoft
    {
        public static Point Location;
        public static Checkpoint MostRecentCheckpoint;
        private bool godMode = false;
        private static GUIElement playerStatusElement = null;

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

        public enum Abilities
        {
            Tether,
            Gravity,
            Shoot
        }

        /// <summary>
        /// Cooldowns of each ability
        /// </summary>
        protected AbilityCooldowns<Abilities> cooldowns;

        protected internal int health;
        const int meleeDmg = 2;

        //Events
        public event GetRobotPosition GetRobotPosition;
        public event ToggleBotLock ToggleBotLock;

        private Animations animation;
        private FaceDirection faceDirection;

        //Timers
        private float animationTimer;
        private double iFrames;

        private float swingChange;
        private float maxAngVelocity;

        protected internal float rangedAttackCooldownTimer;
        protected internal float rangedAttackCharge;

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public AbilityCooldowns<Abilities> Cooldowns
        {
            get { return cooldowns; }
        }

        //Make private later
        public Player() : base(MostRecentCheckpoint.Hitbox.Location.ToVector2(), "../../../Content/Entities/Player")
        {
            if (MostRecentCheckpoint == null)
            {
                MostRecentCheckpoint = (Checkpoint)Map.Geometry.First();
                Respawn();
            }

            hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);

            gravity = 70f;
            acceleration = new Vector2(0, gravity);
            maxXVelocity = 40;
            maxYVelocity = 60;
            maxAngVelocity = 350;
            health = int.Parse(properties["MaxHealth"]);

            SwitchAnimation(Animations.Idle);
            spriteScale = 1;


            cooldowns = new AbilityCooldowns<Abilities>(properties);

            if (playerStatusElement != null)
                GUI.RemoveElement(playerStatusElement);

            playerStatusElement = new GUIPlayerStatusElement(new Vector2(10, 10), this);
            GUI.AddElement(playerStatusElement);
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {

            //Change publically available position
            Location = this.Hitbox.Center;

            //Decrease Iframes if the timer is running
            iFrames = iFrames > 0 ? iFrames - gameTime.ElapsedGameTime.TotalSeconds : 0;

            if (physicsState == PhysicsState.Linear)
            cooldowns.Update(gameTime);

            ChangeAnimation(input, gameTime);

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

          

            if (input.IsPressed(Keys.R)
                && !input.WasPressed(Keys.R)) 
            {
                Respawn();
            }

            if(input.IsPressed(Keys.F5)
                && !input.WasPressed(Keys.F5))
            {
                godMode = !godMode;
            }

           

            if (health <= 0)
            {
                Respawn();
                GUI.RemoveElement(playerStatusElement);
            }

            rangedAttackCooldownTimer = (float)(rangedAttackCooldownTimer < 0 ? 
                0 : rangedAttackCooldownTimer - gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void Movement(GameTime gt)
        {
            base.Movement(gt);
            
            //Check if player hits an enemy or projectile
            IHostile collision = null;

            if ((CheckCollision<IHostile>(out collision))
                && iFrames <= 0
                && collision is not PlayerProjectile)
            {
                TakeDamage(collision.Damage);

                //Make the player invincible for a short time
                iFrames = 1;

                //Stop tether if swinging
                if (physicsState == PhysicsState.Rotational)
                {
                    SetLinearVariables();
                    ToggleBotLock();
                }

                //Knock the player back
                Impulse(new Vector2(
                    -45 * Math.Sign(VectorMath.Difference(vectorPosition, collision.Position.ToVector2()).X),
                    -35));

            }
            
        }

   

        protected override void RotationalMotion(GameTime gt)
        {
            Vector2 oldPosition = new Vector2(vectorPosition.X, vectorPosition.Y);
            double oldTheta = theta;

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

            if (CheckCollision<ISolid>(true))        
            {
                vectorPosition = oldPosition;
                //physicsState = PhysicsState.Linear;
                /*
                //This determines the velocity the entity will have after 
                //it stops swinging by converting the angular velocity
                //back to linear velocity.
                velocity = new Vector2(                                       // 3000: random number for downscaling (it was too big)
                    (float)(angVelocity * swingRadius * -Math.Sin((Math.PI / 180) * (theta)) / 3000),
                    (float)(angVelocity * swingRadius * Math.Cos((Math.PI / 180) * (theta))) / 3000);
                acceleration = new Vector2(
                    acceleration.X, gravity);
                */
                //ToggleBotLock();
                theta = oldTheta;
                angVelocity = 0;
            }
        }

        public override void Input(StoredInput input, GameTime gameTime)
        {
            
            //Horizontal movement
            if (input.IsPressed(Keys.A) &&
                !input.IsPressed(Keys.D))
            {
                // acceleration is higher if the player is moving in the opposite direction for smoother movement
                acceleration.X = velocity.X > 0 ? -maxXVelocity * 5f : -maxXVelocity * 2f;
                faceDirection = FaceDirection.Left;
            }
            else if (input.IsPressed(Keys.D) &&
                !input.IsPressed(Keys.A))
            {
                // acceleration is higher if the player is moving in the opposite direction for smoother movement
                acceleration.X = velocity.X < 0 ? maxXVelocity * 5f : maxXVelocity * 2f;
                faceDirection = FaceDirection.Right;
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
                && GetRobotPosition != null
                && cooldowns.UseAbility(Abilities.Gravity))
            {
                Vector2 robotPos = GetRobotPosition();
                GravityAbility(robotPos);
            }

            //Tether ability - planning to have this be able to swing blocks and stuff too, maybe send back projectiles?
            if (physicsState == PhysicsState.Linear
                && input.CurrentMouse.RightButton == ButtonState.Pressed
                && input.PreviousMouse.RightButton == ButtonState.Released
                && cooldowns[Abilities.Tether] == 0
                && !Robot.Locked)
            {
                Vector2 robotPos = GetRobotPosition();

                if (VectorMath.Magnitude(
                        VectorMath.Difference(vectorPosition, robotPos)
                        )
                    < 125
                    && !Grounded)
                {
                    Robot.Tether = this;
                    cooldowns.UseAbility(Abilities.Tether);
                    SetRotationalVariables(robotPos);
                    ToggleBotLock();
                }
                else
                {
                    List<ICollidable> list = GameManager.entities.GetAllOfType<ICollidable>();
                    list.Remove(list.Find(item => item is Robot));
                    list.Remove(list.Find(item => item is Player));

                    if (list.Count > 0)
                    {
                        if(list.Exists(item => (VectorMath.Magnitude(
                                VectorMath.Difference(
                                    item.Position.ToVector2(),
                                    robotPos)) < 125)))
                        {
                            Robot.Tether = list.MinBy(item =>
                            VectorMath.Magnitude(
                                VectorMath.Difference(
                                    item.Position.ToVector2(),
                                    robotPos)
                                )
                            );

                            Robot.Tether.SetRotationalVariables(robotPos);
                            cooldowns.UseAbility(Abilities.Tether);
                            ToggleBotLock();
                        }                      
                    }


                }
            }
            else if (input.CurrentMouse.RightButton == ButtonState.Released
                && input.PreviousMouse.RightButton == ButtonState.Pressed
                && Robot.Tether != null)
            {

                    Robot.Tether.SetLinearVariables();
                    ToggleBotLock();
            }

            // if E is pressed, play melee attack animation
            if (input.IsPressed(Keys.E) &&
                    !input.WasPressed(Keys.E) &&
                    activeAnimation.AnimationValue != (int)(Animations.Attack)) {
                SwitchAnimation(Animations.Attack);
                Attack();
                animationTimer = activeAnimation.AnimationLength;
            }

            // if F is pressed, play ranged attack animation
            if (input.IsPressed(Keys.LeftShift) && 
                    activeAnimation.AnimationValue != (int)(Animations.Shoot) &&
                    cooldowns[Abilities.Shoot] == 0) {
                // Player can hold shift to charge up an attack
                rangedAttackCharge = (rangedAttackCharge < float.Parse(properties["RangeChargeToMax"]) ?
                    rangedAttackCharge + (float)gameTime.ElapsedGameTime.TotalSeconds :
                    float.Parse(properties["RangeChargeToMax"])) ;

                
            }
            else if (input.WasPressed(Keys.LeftShift) &&
                    activeAnimation.AnimationValue != (int)(Animations.Shoot) &&
                    cooldowns[Abilities.Shoot] == 0) {
                SwitchAnimation(Animations.Shoot);

                float projectileSpeed = 1.5f * (rangedAttackCharge / float.Parse(properties["RangeChargeToMax"]));
                GameManager.SpawnEntity<PlayerProjectile>(
                    new object[]
                    {
                        hurtbox.Center.ToVector2() + new Vector2(
                        faceDirection == FaceDirection.Left ? -hurtbox.Width : hurtbox.Width,
                        -4),
                        faceDirection == FaceDirection.Left ? new Vector2(-1, 0) : new Vector2(1, 0),
                        (int)Math.Ceiling(float.Parse(properties["RangeRampupMax"]) * (rangedAttackCharge / float.Parse(properties["RangeChargeToMax"]))),
                        projectileSpeed < 1 ? 1: projectileSpeed
                    });

                animationTimer = activeAnimation.AnimationLength;
                rangedAttackCharge = 0;
                cooldowns.UseAbility(Abilities.Shoot);
            }

            activeAnimation.FaceDirection = (int)faceDirection;
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            
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

            if( godMode)
            {
                acceleration.Y = gravity * 0.5f;
            }
            


            velocity += acceleration * time;                                   //Update velocity
            Vector2 tempVelocity = new Vector2(velocity.X, velocity.Y);        //For if the player is airborne

            

            //Vertical
            while (iterationCounter <= CollisionAccuracy)                      //Scaling number of checks
            {
                ISolid thing = null;

                if (!CheckCollision<ISolid>(out thing))
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

                if (CheckCollision<ISolid>(out thing))          // Check if there was a collision
                {
                    hurtbox = new Rectangle(lastSafePosition, hurtbox.Size);    // Revert hitbox position back to before collision
                    vectorPosition = lastSafePosition.ToVector2();                      // Revert position
                    velocity.Y = 0;
                    break;
                }

                iterationCounter++;
            }


            //Do the same thing but in the X direction
            iterationCounter = 1;

            while (!CheckCollision<ISolid>() && iterationCounter <= CollisionAccuracy)
            {
                ISolid thing = null;

                if (!CheckCollision<ISolid>(out thing))
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

                if (CheckCollision<ISolid>(out thing))
                {
                    hurtbox = new Rectangle(lastSafePosition, hurtbox.Size);
                    vectorPosition = lastSafePosition.ToVector2();
                    velocity.X = 0;
                    break;
                }
                iterationCounter++;

            }

            CheckCollision<ISolid>(true);
        }

        private void ChangeAnimation(StoredInput input, GameTime gameTime)
        {
            
            // For animations that play until they are done, don't change the animation until then
            if (animationTimer > 0)
            {
                animationTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds * 60;
                return;
            }

            // If not moving, play idle
            SwitchAnimation(Animations.Idle, false);

            // If moving horizontally, play run
            if (velocity.X != 0) {
                SwitchAnimation(Animations.Run, false);
            }

            // If in the air, play airborne animation
            if (!Grounded) {
                SwitchAnimation(Animations.Jump);
            }

            // Update the face direction in the animation class
            activeAnimation.FaceDirection = (int)faceDirection;
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
                    Knockback * Math.Sign(VectorMath.Difference(vectorPosition, item.Position.ToVector2()).X),
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
                    Vector2 difference = VectorMath.Difference(movable.Position.ToVector2(), robotPos);
                    difference.Normalize();
                    movable.Impulse(difference * 
                        (movable is Player ? GravityStrength : 60)
                        );
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
            if (!godMode)
            Health -= damage;
        }

        public static void Respawn()
        {
            var temp = Map.geometry;
            GameManager.entities[typeof(Player)].Clear();
            Player player = GameManager.SpawnEntity<Player>();
            Camera.SetTarget(player);
            Player.Location = player.Hitbox.Center;

            GameManager.entities[typeof(Robot)].Clear();
            Robot robot = GameManager.SpawnEntity<Robot>();

            player.GetRobotPosition += robot.GetPosition;
            player.ToggleBotLock += robot.ToggleLock;

            /*
            while (GUI.GUIElementList.Exists(item => item is GUIPlayerStatusElement))
            {
                GUIElement remove = GUI.GUIElementList.Find(item => item is GUIPlayerStatusElement);
                GUI.GUIElementList.Remove(remove);
            }
            */
        }
    }

    internal class GUIPlayerStatusElement : GUIElement {
        const int Size = 2;

        protected Vector2 position;
        protected Player player;

        // Health Bar
        protected Texture2D healthBar;
        protected Texture2D healthTick;

        // Abilities
        protected Texture2D gravIcon;
        protected Texture2D tetherIcon;
        protected Texture2D cooldownTick;

        // Ranged Weapon Charge
        protected Texture2D chargeBar;
        protected Texture2D chargeTick;
        protected Texture2D chargeCooldownTick;

        public GUIPlayerStatusElement(Vector2 position, Player player) {
            this.position = position;
            this.player = player;

            healthBar = GUI.GetTexture("HealthBar");
            healthTick = GUI.GetTexture("HealthTick");

            gravIcon = GUI.GetTexture("GravIcon");
            tetherIcon = GUI.GetTexture("SwingIcon");
            cooldownTick = Loader.LoadTexture("../../../Content/Entities/hitbox");

            chargeBar = GUI.GetTexture("ChargeBar");
            chargeTick = GUI.GetTexture("ChargeTick");
            chargeCooldownTick = GUI.GetTexture("ChargeCooldownTick");
        }

        public override void Draw(SpriteBatch batch) {

            const int Spacing = 15;

            Rectangle healthRect = new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    48 * Size,
                    12 * Size
                    );

            // Draw health guage
            batch.Draw(
                healthBar, 
                healthRect, 
                Color.White);

            // Draw health ticks
            for (int tick = 0; tick < player.health; tick++)
                batch.Draw(
                    healthTick,
                    new Rectangle(
                        (int)(position.X + (8 * Size) + ((tick * Size) + (Size) - 1) + (tick * (3 * Size))),
                        (int)(position.Y + (3 * Size)),
                        3 * Size,
                        6 * Size
                        ),
                    Color.White
                    );

            Rectangle gravRect = new Rectangle(
                    healthRect.X + healthRect.Width + Spacing,
                    (int)position.Y - 2,
                    16 * Size,
                    16 * Size);


            //Draw ability cooldowns
            batch.Draw(
                gravIcon,
                gravRect,
                Color.White);

            batch.Draw(
                cooldownTick,
                new Rectangle(
                    gravRect.X,
                    (int)
                    (gravRect.Y + gravRect.Height - 
                    gravRect.Height *
                        player.Cooldowns[Player.Abilities.Gravity] / player.Cooldowns[Player.Abilities.Gravity, true]),
                    gravRect.Width,
                    (int)(gravRect.Height *
                        player.Cooldowns[Player.Abilities.Gravity] / player.Cooldowns[Player.Abilities.Gravity, true])),
                Color.White);

            batch.Draw(
                tetherIcon,
                new Rectangle(
                    healthRect.X + healthRect.Width + Spacing * 2 + 16 * Size,
                    (int)position.Y - 2,
                    16 * Size,
                    16 * Size),
                Color.White);

            Rectangle tetherRect = new Rectangle(
                    healthRect.X + healthRect.Width + Spacing * 2 + 16 * Size,
                    (int)position.Y - 2,
                    16 * Size,
                    16 * Size);

            batch.Draw(
                cooldownTick,
                new Rectangle(
                    tetherRect.X,
                    (int)
                    (tetherRect.Y + tetherRect.Height -
                    tetherRect.Height *
                        player.Cooldowns[Player.Abilities.Tether] / player.Cooldowns[Player.Abilities.Tether, true]),
                    tetherRect.Width,
                    (int)(tetherRect.Height *
                        player.Cooldowns[Player.Abilities.Tether] / player.Cooldowns[Player.Abilities.Tether, true])),
                Color.White);


            // Draw ranged weapon charge //
            batch.Draw( // Charge Var
                chargeBar,
                new Rectangle(
                    (int)position.X * Size,
                    (int)(position.Y + 20) * Size,
                    42 * Size,
                    8 * Size
                    ),
                Color.White
                );

            batch.Draw( // Charge Cooldown Tick
                chargeCooldownTick,
                 new Rectangle(
                    (int)(position.X + 1) * Size,
                    (int)(position.Y + 21) * Size,
                    (int)(40 * (player.Cooldowns[Player.Abilities.Shoot] / player.Cooldowns[Player.Abilities.Shoot, true])) * Size,
                    6 * Size
                    ),
                Color.White
                );

            batch.Draw( // Charge Tick
                chargeTick,
                new Rectangle(
                    (int)(position.X + 1) * Size,
                    (int)(position.Y + 21) * Size,
                    (int)(40 * (player.rangedAttackCharge / float.Parse(player.properties["RangeChargeToMax"]))) * Size,
                    6 * Size
                    ),
                Color.White
                );
        }
    }
}
