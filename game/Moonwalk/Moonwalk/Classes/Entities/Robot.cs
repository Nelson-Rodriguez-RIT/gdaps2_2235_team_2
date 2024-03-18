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

namespace Moonwalk.Classes.Entities
{
    public delegate List<IMovable> GetMovables();
    public delegate void SwingStart();
    public delegate void SwingStop();

    /// <summary>
    /// The player's trusty companion
    /// </summary>
    internal class Robot : PlayerControlled
    {
        protected enum Animations
        {
            Idle,
            Idle_Blink,
            Walk,
            Run,
            Crouch,
            Jump,
            Hurt,
            Death,
            Attack,
        }

        protected enum Abilities
        {
            Tether,
            Gravity
        }

        public event GetMovables getGravityPulseTargets;
        public event SwingStart swingStart;
        public event SwingStop swingStop;

        /// <summary>
        /// Cooldowns of each ability
        /// </summary>
        protected AbilityCooldowns<Abilities> cooldowns;

        //Change this to private later
        public Robot(Vector2 position) : base(position, "../../../Content/Entities/TestEntity", 10, 10)
        {
            physicsState = PhysicsState.Linear;
            SwitchAnimation(Animations.Idle);
            spriteScale = 2;
            
            
            cooldowns = new AbilityCooldowns<Abilities>(directory);
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            cooldowns.Update(gameTime);
            base.Update(gameTime, input);
        }

        public override void Input(StoredInput input)
        {
            //Velocity points towards the mouse cursor
            velocity = input.CurrentMouse.Position.ToVector2() - Camera.ApplyOffset(vectorPosition);

            //Gravity ability
            if (input.CurrentMouse.LeftButton == ButtonState.Pressed
                && input.PreviousMouse.LeftButton == ButtonState.Released
                && cooldowns.UseAbility(Abilities.Gravity))
            {
                //Get a list of movables from the game manager
                List<IMovable> movables = getGravityPulseTargets();

                //Make all entities move towards this
                foreach (IMovable movable in movables)
                {
                    //Check that entity is within range
                    if (Math.Sqrt(
                            Math.Pow(movable.Position.X - Position.X, 2) +
                            Math.Pow(movable.Position.Y - Position.Y, 2)
                            )
                        < 375)  
                    movable.Impulse(vectorPosition);
                }
            }

            //Tether ability - planning to have this be able to swing blocks and stuff too, maybe send back projectiles?
            if (input.CurrentMouse.RightButton == ButtonState.Pressed
                && input.PreviousMouse.RightButton == ButtonState.Released
                && cooldowns.UseAbility(Abilities.Tether))
            {

            }
        }

        
    }

}
