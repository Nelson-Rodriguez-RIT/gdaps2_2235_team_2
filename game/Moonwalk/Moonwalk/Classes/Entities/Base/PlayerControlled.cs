using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;
using System.Net.Http.Headers;

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
                        if (velocity.X == 0 &&
                            velocity.Y == 0)
                        {
                            return 1;
                        }

                        return
                    (int)(
                        Math.Sqrt(
                            Math.Pow(velocity.X, 2) +
                            Math.Pow(Velocity.Y, 2))
                        / 20
                    );
                    case PhysicsState.Rotational:
                        if (angVelocity == 0)
                        {
                            return 1;
                        }
                        return (int)(
                            Math.Abs(angVelocity / 10));
                    default:
                        return 0;
                }
            }
        }

        public PlayerControlled(Vector2 position, string directory) : base(position, directory)
        { 

        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            Input(input);
            Movement(gameTime);
            base.Update(gameTime, input);
        }

        public virtual void Movement(GameTime time)
        {
            switch (physicsState)
            {
                case PhysicsState.Linear:
                    LinearMotion(time);
                    break;
                case PhysicsState.Rotational:
                    RotationalMotion(time); 
                    break;
            }
            
        }

        public abstract void Input(StoredInput input);

        /// <summary>
        /// Determines if the entity collided with terrain
        /// </summary>
        /// <returns>True if a collision occurred</returns>
        public virtual bool CheckCollision()
        {
            bool temp = Map.Geometry.Exists(terrain => terrain.Hitbox.Intersects(entity));

            return temp;
        }

        protected override void LinearMotion(GameTime gt) 
        {
            float time = (float)gt.ElapsedGameTime.TotalSeconds;

            int iterationCounter = 1;       // Number of collision checks we've done

            Point lastSafePosition = new Point(Position.X, Position.Y);

            //Vertical
            while (iterationCounter <= CollisionAccuracy)
            {
                if (!CheckCollision())
                {
                    lastSafePosition = new Point(Position.X, Position.Y);      //Store old position in case we collide
                }

                velocity.Y += acceleration.Y * (time * iterationCounter / CollisionAccuracy);
                vectorPosition.Y += velocity.Y * (time * iterationCounter / CollisionAccuracy);

                entity = new Rectangle(vectorPosition.ToPoint(), entity.Size);

                if (CheckCollision())
                {
                    entity = new Rectangle(lastSafePosition, entity.Size);
                    vectorPosition = lastSafePosition.ToVector2();
                    velocity.Y = 0;
                    break;
                }

                iterationCounter++;              
            }

            
            //Horizontal
            iterationCounter = 1;

            while (!CheckCollision() && iterationCounter <= CollisionAccuracy)
            {
                if (!CheckCollision())
                    lastSafePosition = new Point(Position.X, Position.Y);      //Store old position in case we collide

                velocity.X += acceleration.X * (time * iterationCounter / CollisionAccuracy);
                vectorPosition.X += velocity.X * (time * iterationCounter / CollisionAccuracy);

                entity = new Rectangle(vectorPosition.ToPoint(), entity.Size);

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

        protected override void RotationalMotion(GameTime gt)
        {
            Vector2 oldPosition = new Vector2(vectorPosition.X, vectorPosition.Y);
            base.RotationalMotion(gt);

            if (CheckCollision())
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

                LinearMotion(gt);
            }
        }
    }
}
