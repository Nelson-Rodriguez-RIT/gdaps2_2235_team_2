using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dante_Test
{
    enum EggStates
    {
        Held,
        Thrown,
        Broken
    }

    internal sealed class Egg
    {
        private Vector2 position;
        private Vector2 velocity;
        private Vector2 acceleration;
        private EggStates state;
        private Player player;

        public Vector2 Position
        {
            get { return position; }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
        }
        public Vector2 Acceleration
        {
            get { return acceleration; }
        }
        public bool Proximity
        {
            get
            {
                if (Math.Sqrt(
                    Math.Pow(position.X - player.Position.X, 2) +
                    Math.Pow(position.Y - player.Position.Y, 2))
                    <= 200)
                {
                    return true;
                }

                return false;
            }
        }

        public Egg(Player player) 
        { 
            state = EggStates.Thrown;
            position = new Vector2(200, 200);
            velocity = new Vector2(0, 0);
            acceleration = new Vector2(0, 2f);
            this.player = player;
        }

        public void Update()
        {
            
            switch (state)
            {
                case EggStates.Thrown:
                    Movement();
                    Pickup();
                    break;
                case EggStates.Held: 
                    position = new Vector2(player.Position.X, player.Position.Y);
                    Throw();
                    break;
                case EggStates.Broken:
                    break;
            }
        }

        public void Movement()
        {
            if (velocity.X > 0)
            {
                acceleration.X = -0.1f;
            }
            else if (velocity.X < 0)
            {
                acceleration.X = 0.1f;
            }
            else
            {
                acceleration.X = 0;
            }

            velocity += acceleration;
            position += velocity;
        }

        public void Throw()
        {
            if (Mouse.GetState().RightButton != ButtonState.Pressed)
            {
                state = EggStates.Thrown;
            }

            velocity = new Vector2(player.Velocity.X, -60);
        }

        public void Pickup()
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed && Proximity) 
            {
                state = EggStates.Held;
            }
        }
    }
}
