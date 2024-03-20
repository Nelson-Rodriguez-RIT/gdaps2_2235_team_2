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
    public delegate List<IMovable> OnGravityAbilityUsed();
    public delegate Vector2 GetRobotPosition();
    //public delegate List<IDamageable> OnAttack();

    /// <summary>
    /// The player controlled character
    /// </summary>
    internal class Player : PlayerControlled, IJump
    {
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

        public event OnGravityAbilityUsed OnGravityAbilityUsed;
        public event GetRobotPosition GetRobotPosition;

        private Animations animation;
        private FaceDirection faceDirection;

        private int animationTimer;
        private double jumpTimer;

        /// <summary>
        /// Determines if the entity is grounded or not
        /// </summary>
        public bool Grounded
        {
            get
            {
                if (CheckCollision(new Rectangle(
                        hitbox.X + (int)Position.X,
                        hitbox.Y + (int)Position.Y + 5,
                        hitbox.Width,
                        hitbox.Height
                        )))
                {
                    return true;
                }

                return false;
            }
        }

        //Make private later
        public Player(Vector2 position, Object[] args) : base(position, "../../../Content/Entities/Player")
        {
            gravity = 70f;
            acceleration = new Vector2(0, gravity);
            maxXVelocity = 40;
            maxYVelocity = 60;           

            SwitchAnimation(Animations.Idle);
            spriteScale = 1;

            cooldowns = new AbilityCooldowns<Abilities>(directory);
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {

            if (!Grounded)
            {
                jumpTimer += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                jumpTimer = 0;
            }

            cooldowns.Update(gameTime);
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
                //&& Math.Sign(acceleration.X) != Math.Sign(velocity.X))
            {
                velocity.X = 0;
                acceleration.X = 0;
            }

                animationTimer--;
            ChangeAnimation(input);
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
                //UseAbility(Abilities.Tether))
            {
                Vector2 robotPos = GetRobotPosition();

                if (VectorMath.VectorMagnitude(
                        VectorMath.VectorDifference(vectorPosition, robotPos)
                        )
                    < 125)
                {
                    cooldowns.UseAbility(Abilities.Tether);
                    SetRotationalVariables(robotPos);
                }
                

            }
            else if (input.CurrentMouse.RightButton == ButtonState.Released
                && input.PreviousMouse.RightButton == ButtonState.Pressed)
            {
                if (physicsState == PhysicsState.Rotational)
                    SetLinearVariables();
            }

        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);

            batch.DrawString(GameManager.font, 
                $"{Math.Round(vectorPosition.Y)} - {Math.Round(velocity.Y)} - {Math.Round(acceleration.Y)} \n {Math.Round(velocity.X)}",
                new Vector2(400, 50),
                Color.White);
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

                entity = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    entity.Width,
                    entity.Height);                      // Update hitbox location

                if (CheckCollision())                                                   // Check if there was a collision
                {
                    entity = new Rectangle(lastSafePosition, entity.Size);              // Revert hitbox position back to before collision
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

                entity = new Rectangle(
                    (int)Math.Round(vectorPosition.X),
                    (int)Math.Round(vectorPosition.Y),
                    entity.Width,
                    entity.Height);

                if (CheckCollision())
                {
                    entity = new Rectangle(lastSafePosition, entity.Size);
                    vectorPosition = lastSafePosition.ToVector2();
                    velocity.X = 0;
                    break;
                }
                iterationCounter++;

            }
        }

        private void ChangeAnimation(StoredInput input)
        {
            //For animations that play until they are done
            if (animationTimer > 0)
            {
                return;
            }
            else
            {
                animationTimer = 0;
            }

            //Change facing direciton of the player
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

            activeAnimation.FaceDirection = (int)faceDirection;

            if (velocity.X == 0 && velocity.Y == 0)
            {
                SwitchAnimation(Animations.Idle, false);
            }

            if (velocity.X != 0)
            {                
                SwitchAnimation(Animations.Run, false);
            }

            if (!Grounded)
            {
                SwitchAnimation(Animations.Jump);
            }

            if (input.IsPressed(Keys.E) && 
                !input.WasPressed(Keys.E))
            {
                SwitchAnimation(Animations.Attack, false);
                animationTimer = activeAnimation.AnimationLength;
            }

            if (input.IsPressed(Keys.F))
            {
                SwitchAnimation(Animations.Shoot, false);
                animationTimer = activeAnimation.AnimationLength;
            }
        }

        private void Attack()
        {
            //List<Enemy> enemies = 
        }

        
        private void GravityAbility(Vector2 robotPos)
        {
            
            //Get a list of movables from the game manager
            List<IMovable> movables = OnGravityAbilityUsed();

            //Make all entities move towards this
            foreach (IMovable movable in movables)
            {
                //Check that entity is within range
                if (Math.Sqrt(
                        Math.Pow(movable.Position.X - robotPos.X, 2) +
                        Math.Pow(movable.Position.Y - robotPos.Y, 2)
                        )
                    < 150)
                {
                    Vector2 difference = VectorMath.VectorDifference(vectorPosition, robotPos);
                    movable.Impulse(new Vector2(
                        difference.X,
                        difference.Y));
                }

            }

        }
        
    }
}
