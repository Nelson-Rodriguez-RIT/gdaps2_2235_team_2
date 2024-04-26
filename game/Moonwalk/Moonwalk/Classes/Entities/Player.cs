using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Managers;
using Moonwalk.Classes.Maps;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Moonwalk.Classes.Entities {
    public delegate Vector2 GetRobotPosition();

    /// <summary>
    /// The player controlled character
    /// </summary>
    internal class Player : PlayerControlled, IDamageable, ISoft
    {
        private static Point location;
        public static Checkpoint MostRecentCheckpoint;
        private static GUIElement playerStatusElement = null;
        private static Rectangle loadRange;

        /// <summary>
        /// Range in which entities load
        /// </summary>
        public static Rectangle LoadRange
        {
            get { return loadRange; }
        }

        /// <summary>
        /// Location of the player publically available
        /// </summary>
        public static Point Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>
        /// Center of the player's hitbox
        /// </summary>
        public Vector2 Center {
            get { return new Vector2(
                location.X + int.Parse(properties["HitboxX"]) / 2, 
                location.Y + int.Parse(properties["HitboxY"]) / 2); 
            }
        }

        /// <summary>
        /// The different animations the player has
        /// </summary>
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

        /// <summary>
        /// Different abilities with cooldowns
        /// </summary>
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
        protected internal static int prevHealth = 8;
        const int meleeDmg = 2;

        //Events
        public event GetRobotPosition GetRobotPosition;

        private Animations animation;
        private FaceDirection faceDirection;

        //Timers
        private float animationTimer;
        private double iFrames;

        private float swingChange;
        private float maxAngVelocity;

        protected internal float rangedAttackCooldownTimer;
        protected internal float rangedAttackCharge;

        //Dev mode
        private bool GodMode = false;
        private bool Spawning = false;
        private GUIGroup spawnMenu;
        private GUITextField textField;
        private Vector2 SpawnLocation;

        /// <summary>
        /// Health of the player
        /// </summary>
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        /// <summary>
        /// Cooldowns of the player's abilities
        /// </summary>
        public AbilityCooldowns<Abilities> Cooldowns
        {
            get { return cooldowns; }
        }

        public Player(Point? initialPosition = null) : base(new Vector2(((Point)initialPosition).X, ((Point)initialPosition).Y), "../../../Content/Entities/Player") {
            //Set initial checkpoint
            if (MostRecentCheckpoint == null) {
                MostRecentCheckpoint = (Checkpoint)Map.Geometry.First();
                Respawn();
            }

            //Set initial values for fields
            hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);

            gravity = 70f;
            acceleration = new Vector2(0, gravity);
            maxXVelocity = 40;
            maxYVelocity = 60;
            maxAngVelocity = 380;
            health = int.Parse(properties["MaxHealth"]);

            //default idle animation
            SwitchAnimation(Animations.Idle);
            spriteScale = 1;


            cooldowns = new AbilityCooldowns<Abilities>(properties);

            //Player UI
            if (playerStatusElement != null)
                GUI.RemoveElement(playerStatusElement);

            playerStatusElement = new GUIPlayerStatusElement(this);
            GUI.AddElement(playerStatusElement);

        }

        public Player(Vector2? initialPosition = null) : base((Vector2)initialPosition, "../../../Content/Entities/Player")
        {
            //Set initial checkpoint
            if (MostRecentCheckpoint == null)
            {
                MostRecentCheckpoint = (Checkpoint)Map.Geometry.First();
                Respawn();
            }

            //Set initial values for fields
            hurtbox = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    hurtbox.Width,
                    hurtbox.Height);

            gravity = 70f;
            acceleration = new Vector2(0, gravity);
            maxXVelocity = 40;
            maxYVelocity = 60;
            maxAngVelocity = 380;
            health = int.Parse(properties["MaxHealth"]);

            SwitchAnimation(Animations.Idle);
            spriteScale = 1;


            cooldowns = new AbilityCooldowns<Abilities>(properties);

            //Player UI
            if (playerStatusElement != null)
                GUI.RemoveElement(playerStatusElement);

            playerStatusElement = new GUIPlayerStatusElement(this);
            GUI.AddElement(playerStatusElement);

        }

        /// <summary>
        /// Constructor that respawns at the last checkpoint
        /// </summary>
        public Player() : base(MostRecentCheckpoint.Hitbox.Location.ToVector2(), "../../../Content/Entities/Player") {
            //Set initial checkpoint
            if (MostRecentCheckpoint == null) {
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
            maxAngVelocity = 380;
            health = int.Parse(properties["MaxHealth"]);

            SwitchAnimation(Animations.Idle);
            spriteScale = 1;


            cooldowns = new AbilityCooldowns<Abilities>(properties);

            //Player UI
            if (playerStatusElement != null)
                GUI.RemoveElement(playerStatusElement);

            playerStatusElement = new GUIPlayerStatusElement(this);
            GUI.AddElement(playerStatusElement);

        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            //Change publically available position
            location = this.Hitbox.Center;

            //update loading range for entities ( a bit outside the view range)
            loadRange = 
            
            new Rectangle(
            new Point(
                location.X - 50, 
                location.Y - 50) 
            - (Camera.GlobalOffset / 2).ToPoint(), 
            (Camera.GlobalOffset * 2).ToPoint() + new Point(100, 100));
            

            //Rectangle temp = Camera.RelativePosition(loadRange);

            //dev mode logic
            #region Godmode

            //press f5 to enter
            if (input.IsPressed(Keys.F5)
                && !input.WasPressed(Keys.F5))
            {
                GodMode = !GodMode;

                Robot robot = Robot.Respawn();

                GetRobotPosition += robot.GetPosition;
            }

            const int GodModeSpeed = 5;

            if (GodMode)
            {
                //spawn entities by clicking and typing
                if (input.CurrentMouse.LeftButton == ButtonState.Pressed
                    && input.PreviousMouse.LeftButton == ButtonState.Released)
                {
                    Spawning = true;
                    //location to spawn is mouse location
                    SpawnLocation = Camera.WorldToScreen(input.CurrentMouse.Position.ToVector2() / 2) + Camera.GlobalOffset / 2;

                    //add menu to gui
                    spawnMenu = new GUIGroup(input.CurrentMouse.Position.ToVector2());

                    object[] args = new object[] { new Vector2(0, 0), "MonogramRegular", Color.White };

                    textField = (GUITextField)spawnMenu.Add(args);

                    //read spawn history from file
                    try
                    {
                        string[] lines = File.ReadAllLines("../../../Content/GUI/History.txt");

                        for (int i = 0; i < lines.Length; i++)
                        {
                            spawnMenu.Add(new object[] { 
                                new Vector2(0, 50 * (i + 1)), 
                                (i + 1) + " - " + lines[i], 
                                "MonogramRegular", 
                                Color.White });
                        }
                    }
                    catch
                    {

                    }

                    

                    GUI.AddElement(spawnMenu);
                }

                if (Spawning)
                {
                    //text field logic (key inputs)
                    textField.Input(input);

                    Keys key = Keys.Escape;

                    if (input.IsPressed(Keys.Enter))
                    {
                        key = Keys.Enter;
                        Spawning = false;
                    }
                    else if (input.IsPressed(Keys.D1))
                    {
                        key = Keys.D1;
                        Spawning = false;
                    }
                    else if(input.IsPressed(Keys.D2))
                    {
                        key = Keys.D2;
                        Spawning = false;
                    }
                    else if(input.IsPressed(Keys.D3))
                    {
                        key = Keys.D3;
                        Spawning = false;
                    }
                    

                    string typeName = null;

                    //enter or shortcut pressed
                    switch (key)
                    {
                        case Keys.Enter:
                            typeName = textField.Text;
                            break;
                        case Keys.D1:
                        case Keys.D2:
                        case Keys.D3:
                            typeName = ((GUITextElement)spawnMenu.Elements[
                                int.Parse(key.ToString().Substring(1))]).Text.Substring(4);
                            break;

                        
                    }

                   
                    // if done spawning, spawn the entity and remove GUI
                    if (!Spawning)
                    {
                        GUI.RemoveElement(spawnMenu);
                        this.spawnMenu = null;
                        

                        Type type = Type.GetType("Moonwalk.Classes.Entities." + typeName);

                        if (type != null &&
                            type.IsAssignableTo(typeof(Entity))
                            && type != typeof(Player)
                            && type != typeof(Robot)
                            && type != typeof(Projectile))
                        {
                            //Get the spawnentity method and invoke it with the type we just defined
                            MethodInfo spawn = typeof(GameManager).GetMethod("SpawnEntity");
                            MethodInfo spawnGeneric = spawn.MakeGenericMethod(type);

                            //arguments for the spawning
                            object[] args = new object[] {SpawnLocation};

                            // This is a bit convoluted but if you ask me I can explain why we need an array 
                            // which contains an array which contains the paramaters for this - Dante
                            spawnGeneric.Invoke(null, new object[] { args });

                            StreamWriter writer = null;

                            //update spawn historyy in the file
                            try
                            {
                                List<string> lines = File.ReadAllLines("../../../Content/GUI/History.txt").ToList();

                                writer = new StreamWriter("../../../Content/GUI/History.txt");

                                List<string> lines2 = new List<string>();


                                try
                                {
                                    if (lines.Contains(typeName))
                                    {
                                        lines2.Add(typeName);

                                        for (int i = 0; i < lines.Count; i++)
                                        {
                                            if (lines[i] != typeName)
                                            {
                                                lines2.Add(lines[i]);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        lines2.Add(typeName);

                                        for (int i = 0; i < 2; i++)
                                        {
                                            lines2.Add(lines[i]);
                                        }
                                    }
                                }
                                catch (ArgumentOutOfRangeException e)
                                {

                                }


                                foreach (string line in lines2)
                                {
                                    writer.WriteLine(line);
                                }
                            }
                            catch (FileNotFoundException e)
                            {

                            }
                            finally
                            {
                                if (writer != null)
                                    writer.Close();
                            }


                        }
                    }

                        

                    return;

                }
                else
                {
                    //basic 4 way movement in dev mode
                    if (input.IsPressed(Keys.A))
                    {
                        vectorPosition.X -= GodModeSpeed;
                    }
                    if (input.IsPressed(Keys.D))
                    {
                        vectorPosition.X += GodModeSpeed;
                    }
                    if (input.IsPressed(Keys.W))
                    {
                        vectorPosition.Y -= GodModeSpeed;
                    }
                    if (input.IsPressed(Keys.S))
                    {
                        vectorPosition.Y += GodModeSpeed;
                    }

                    hurtbox = new Rectangle(
                        (int)Math.Round(vectorPosition.X),
                        (int)Math.Round(vectorPosition.Y),
                        hurtbox.Width,
                        hurtbox.Height);

                    return;
                }
            }

            #endregion

            //Decrease Iframes if the timer is running
            iFrames = iFrames > 0 ? iFrames - gameTime.ElapsedGameTime.TotalSeconds : 0;

            //decrease cooldowns
            if (physicsState == PhysicsState.Linear)
            cooldowns.Update(gameTime);

            //update animations
            ChangeAnimation(input, gameTime);

            base.Update(gameTime, input);

            //Fine tuning movement
            int sign = 0;

            //Slow down if not pressing anything
            if (!input.IsPressed(Keys.D) &&
                !input.IsPressed(Keys.A)
                && velocity.X != 0)
            {
                sign = Math.Sign(velocity.X);
                acceleration.X = -Math.Sign(velocity.X) * 80;
            }

            // if velocity changes sign in this update, set it to 0
            if (sign != Math.Sign(velocity.X + acceleration.X * gameTime.ElapsedGameTime.TotalSeconds)
                && sign != 0)
            {
                acceleration.X = 0;
                velocity.X = 0;
            }

            // if velocity is small enough, set it to 0
            if (velocity.X != 0
                && Math.Abs(velocity.X) < 1f)
            {
                velocity.X = 0;
                acceleration.X = 0;
            }

          
            //respawn the player if R is pressed
            if (input.IsPressed(Keys.R)
                && !input.WasPressed(Keys.R)) 
            {
                Respawn();
                Player.prevHealth = health;
            }


            // player dies
            if (health <= 0)
            {
                Respawn();
                Player.prevHealth = health;
                //GUI.RemoveElement(playerStatusElement);
            }

            // update ranged attack cooldown
            rangedAttackCooldownTimer = (float)(rangedAttackCooldownTimer < 0 ? 
                0 : rangedAttackCooldownTimer - gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void Movement(GameTime gt)
        {
            if (!GodMode)
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
                    }

                    //Knock the player back
                    Impulse(new Vector2(
                        -45 * Math.Sign(VectorMath.Difference(vectorPosition, collision.Position.ToVector2()).X),
                        -35));

                }
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

                if (Grounded)
                {
                    SetLinearVariables();
                    Robot.Tether = null;
                }
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

            //change swing speed by pressing A and D
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

                    //remove any buffered jumps from the list
                    BufferedInput buffer = input.Buffered.Find(item => item.Key == Keys.Space);
                    input.Buffered.Remove(buffer);
                }   
                else
                {
                    //can buffer a jump, if they hit the ground while it is buffered they will immediately jump
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

                //if entity is grounded and within range of the ability (player is prioritized)
                if (VectorMath.Magnitude(
                        VectorMath.Difference(vectorPosition, robotPos)
                        )
                    < 125
                    && !Grounded)
                {
                    // set tether and use cooldowns
                    Robot.Tether = this;
                    cooldowns.UseAbility(Abilities.Tether);
                    SetRotationalVariables(robotPos);
                }
                else
                {
                    //do the same thing but for entities other than the player
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
                        }                      
                    }


                }
            }
            else if (input.CurrentMouse.RightButton == ButtonState.Released
                && input.PreviousMouse.RightButton == ButtonState.Pressed
                && Robot.Tether != null)
            {
                    //end tether if RMB is released
                    Robot.Tether.SetLinearVariables();
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
                
                if (rangedAttackCharge == float.Parse(properties["RangeChargeToMax"]))
                {
                    Particle.Effects.Add(new Particle(
                        0.05, 
                        Color.White,
                        ParticleEffects.Random,
                        (hurtbox.Center.ToVector2() + new Vector2(
                            faceDirection == FaceDirection.Left ? -hurtbox.Width - 4: hurtbox.Width + 4,
                            -1)).ToPoint(),
                        new Vector2(
                            faceDirection == FaceDirection.Left ? -2 : 2,
                            new Random().Next(-1, 2)),
                        4,
                        3));;
                }
                

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

            // What? why does this exist
            //batch.Draw(hitboxSprite, Camera.RelativePosition(loadRange), Color.White);
            
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

            if( GodMode)
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

                if (CheckCollision<ISolid>(out thing, true)
                    || (thing != null && thing is OutOfBounds))          // Check if there was a collision
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

                if (CheckCollision<ISolid>(out thing, true)
                    || (thing != null && thing is OutOfBounds))
                {
                    hurtbox = new Rectangle(lastSafePosition, hurtbox.Size);
                    vectorPosition = lastSafePosition.ToVector2();
                    velocity.X = 0;
                    break;
                }
                iterationCounter++;

            }
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

            //spawn hitbox based on facing direction
            if (faceDirection == FaceDirection.Right) 
            {
                attack = new Hitbox(
                0.333333,
                this,
                new Point(
                    14,
                    20),
                GameManager.entities.GetAllOfType<IDamageable>(),
                new Point(
                    10, -2));
            }
            else
            {
                attack = new Hitbox(
                0.333333,
                this,
                new Point(
                    14,
                    20),
                GameManager.entities.GetAllOfType<IDamageable>(),
                new Point(
                    -16, -2));
            }

            
            // add subscribers to events
            attack.targetEntered += this.DealDamage;
        }

        /// <summary>
        /// Deal damage to everything in a list of damageables
        /// </summary>
        /// <param name="list"></param>
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

            //draw gravity effect using particles
            const int Particles = 72;
            double slice = 360 / Particles;

            //circle around the robot
            for (int i = 0; i < Particles; i++)
            {
                Vector2 pos = new Vector2(
                    (float)Math.Sin(slice * i) * Range,
                    (float)Math.Cos(slice * i) * Range);

                pos.Normalize();

                Point location = robotPos.ToPoint() + (pos * Range).ToPoint();

                Particle.Effects.Add(
                    new Particle(
                        0.5,
                        Color.Lime,
                        ParticleEffects.Random,
                        location));
            }
        }
        
        public void TakeDamage(int damage)
        {
            //check if player is immune to damage
            if (!GodMode && iFrames <= 0)
            {
                // if not, apply effects
                Health -= damage;
                //knockback
                Impulse(new Vector2(0, 20 /*damage * -40 */)); //for oob purposes
                //make player invulnerable temporarily
                iFrames = 1;
                prevHealth = Health;
            }
            
        }

        public override void SetLinearVariables()
        {
            base.SetLinearVariables();
            //player jumps a bit extra
            velocity *= 1.5f;
        }

        public static void Respawn(Vector2? location = null) {
            //despawn player
            GameManager.entities[typeof(Player)].Clear();

            //respawn player. Same spot if checkpoint is null, otherwise last visited checkpoint
            Player player = GameManager.SpawnEntity<Player>(location == null ? MostRecentCheckpoint.Hitbox.Location : location);

            //update static location
            Player.location = location == null ?
                MostRecentCheckpoint.Hitbox.Location : 
                new Point((int)((Vector2)location).X, (int)((Vector2)location).Y);
            Camera.SetTarget(player);

            //respawn the robot and add event subscribers
            Robot robot = Robot.Respawn();
            player.GetRobotPosition += robot.GetPosition;

            //reset pickups
            foreach (Entity entity in GameManager.entities)
            {
                if (entity is KeyObject)
                {
                    ((KeyObject)entity).Reset();
                }
            }

        }

        /// <summary>
        /// Method that runs when player hits out-of-bounds area.
        /// Player takes damage according to their previous health, and respawns themselves.
        /// </summary>
        public static void HitBarrier()
        {
            List<Player> list = (List<Player>)GameManager.entities[typeof(Player)];
            GameManager.entities[typeof(Player)].Clear();
            Player player = GameManager.SpawnPlayer(8) ;

            Camera.SetTarget(player);
            location = player.Hitbox.Center;

            Robot robot = Robot.Respawn();

            player.GetRobotPosition += robot.GetPosition;

            foreach (Entity entity in GameManager.entities)
            {
                if (entity is KeyObject)
                {
                    ((KeyObject)entity).Reset();
                }
            }

            if (Player.prevHealth == 8) player.TakeDamage(PlayerSpawner.OOB_Damage);
            else player.TakeDamage(PlayerSpawner.OOB_Damage + (player.health - Player.prevHealth));
            //PlayerSpawner.RespawnCounter();
        }

    }

    /// <summary>
    /// GUI for the player's health and cooldowns
    /// </summary>
    internal class GUIPlayerStatusElement : GUIElement {
        const int Size = 2;

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

        public GUIPlayerStatusElement(Player player) {
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

            // Draw health guage
            batch.Draw(
                healthBar,
                new Rectangle(
                    (10) * Size,
                    (10) * Size,
                    (healthBar.Width) * Size,
                    (healthBar.Height) * Size
                    ), 
                Color.White);

            // Draw health ticks
            for (int tick = 0; tick < player.health; tick++)
                batch.Draw(
                    healthTick,
                    new Rectangle(
                        (10 * Size + (8 * Size) + ((tick * Size) + (Size) - 1) + (tick * (3 * Size))),
                        (10 * Size + (3 * Size)),
                        (healthTick.Width) * Size,
                        (healthTick.Height) * Size
                        ),
                    Color.White
                    );

            // Draw ranged weapon charge //
            batch.Draw( // Charge Bar
                chargeBar,
                new Rectangle(
                    (10 + healthBar.Width + 2) * Size,
                    (10 + 2) * Size,
                    (chargeBar.Width) * Size,
                    (chargeBar.Height) * Size
                    ),
                Color.White
                );

            batch.Draw( // Charge Cooldown Tick
                chargeCooldownTick,
                 new Rectangle(
                    (10 + healthBar.Width + 2 + 1) * Size,
                    (10 + 2 + 1) * Size,
                    (int)(40 * (player.Cooldowns[Player.Abilities.Shoot] / player.Cooldowns[Player.Abilities.Shoot, true])) * Size,
                    (chargeCooldownTick.Height) * Size
                    ),
                Color.White
                );

            batch.Draw( // Charge Tick
                chargeTick,
                new Rectangle(
                    (10 + healthBar.Width + 2 + 1) * Size,
                    (10 + 2 + 1) * Size,
                    (int)(40 * (player.rangedAttackCharge / float.Parse(player.properties["RangeChargeToMax"]))) * Size,
                    (chargeTick.Height) * Size
                    ),
                Color.White
                );

            // Abilities will only be displayed if they are on cooldowns
            int abilitiesOnCooldown = 0;
            foreach (Player.Abilities ability in Enum.GetValues(typeof(Player.Abilities)))
                if (player.Cooldowns[ability] != 0)
                    abilitiesOnCooldown++;

            if (abilitiesOnCooldown == 0) return;

            int offset = 3 * Size; // An odd amount of abilities on cooldown requires different positioning
            if (abilitiesOnCooldown % 2 == 0)
                offset += (gravIcon.Width / 2) * Size;

            for (int index = 0; index < abilitiesOnCooldown; index++)
                offset -= (gravIcon.Width - 3) * Size;

            //Draw ability cooldowns
            if (player.Cooldowns[Player.Abilities.Gravity] != 0) {
                batch.Draw(
                gravIcon,
                new Rectangle(
                    (int)(WindowManager.Instance.Center.X / 2 + 25 - gravIcon.Width / 2 + offset) * Size,
                    (int)(WindowManager.Instance.Center.Y / 2 + 30) * Size,
                    gravIcon.Width * Size,
                    gravIcon.Height * Size
                    ),
                Color.White);

                batch.Draw(
                    cooldownTick,
                    new Rectangle(
                        (int)(WindowManager.Instance.Center.X / 2 + 25 - gravIcon.Width / 2 + offset) * Size,
                        (int)(WindowManager.Instance.Center.Y / 2 + 30 + gravIcon.Height -
                            gravIcon.Height * (player.Cooldowns[Player.Abilities.Gravity] / player.Cooldowns[Player.Abilities.Gravity, true])) * Size,
                        gravIcon.Width * Size,
                        (int)(gravIcon.Height * (player.Cooldowns[Player.Abilities.Gravity] / player.Cooldowns[Player.Abilities.Gravity, true])) * Size),
                    Color.White);
                offset += gravIcon.Width + 3;
            }
            
            if (player.Cooldowns[Player.Abilities.Tether] != 0) {
                batch.Draw(
                tetherIcon,
                new Rectangle(
                    (int)(WindowManager.Instance.Center.X / 2 + 25 - gravIcon.Width / 2 + offset) * Size,
                    (int)(WindowManager.Instance.Center.Y / 2 + 30) * Size,
                    gravIcon.Width * Size,
                    gravIcon.Height * Size
                    ),
                Color.White);


                batch.Draw(
                    cooldownTick,
                    new Rectangle(
                        (int)(WindowManager.Instance.Center.X / 2 + 25 - gravIcon.Width / 2 + offset) * Size,
                        (int)(WindowManager.Instance.Center.Y / 2 + 30 + gravIcon.Height -
                            gravIcon.Height * (player.Cooldowns[Player.Abilities.Tether] / player.Cooldowns[Player.Abilities.Tether, true])) * Size,
                        gravIcon.Width * Size,
                        (int)(gravIcon.Height * (player.Cooldowns[Player.Abilities.Tether] / player.Cooldowns[Player.Abilities.Tether, true])) * Size),
                    Color.White);
                offset += gravIcon.Width + 3;
            }
        }
    }
}
