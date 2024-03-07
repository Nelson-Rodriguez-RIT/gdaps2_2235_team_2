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
    internal abstract class PlayerControlled : Entity, IControllable, ICollidable
    {
        public int CollisionAccuracy
        {
            get
            {
                switch (physicsState)
                {
                    case PhysicsState.Linear:
                        return
                    (int)(
                        Math.Sqrt(
                            Math.Pow(velocity.X, 2) +
                            Math.Pow(Velocity.Y, 2))
                        / 2
                    );
                    case PhysicsState.Rotational:
                        return (int)(
                            Math.Abs(angVelocity / 10));
                    default:
                        return 1;
                }
            }
        }

        public PlayerControlled(Vector2 position, string directory) : base(position, directory)
        { 

        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            Input(input);
            base.Update(gameTime, input);
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

        public abstract void Input(StoredInput input);

        public virtual void CheckCollision()
        {
            switch (physicsState)
            {
                case PhysicsState.Linear:

                    break;
                case PhysicsState.Rotational:
                    break;
            }
        }
    }
}
