using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Moonwalk.Classes.Entities.Base
{
    internal abstract class PlayerControlled : Entity, IControllable
    {
        public PlayerControlled(Vector2 position, string directory) : base(position, directory)
        { 

        }

        public override void Update(GameTime gameTime, KeyboardState kbState, MouseState msState)
        {
            base.Update(gameTime, kbState, msState);
        }

        public virtual void Movement(GameTime time)
        {
            switch (physicsState)
            {
                case PhysicsState.Linear:
                    base.LinearMotion(time); 
                    break;
                case PhysicsState.Rotational:
                    base.RotationalMotion(time); 
                    break;
            }
            
        }

        public abstract void Input();
    }
}
