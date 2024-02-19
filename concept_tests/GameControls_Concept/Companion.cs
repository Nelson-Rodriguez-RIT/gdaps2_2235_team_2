using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameControls_Concept
{
    enum Abilities
    {
        Grapple, 
        Gravity
    }

    internal class Companion : MouseControlledEntity
    {
        private Player player;
        private Dictionary<Abilities, double> cooldowns;

        public Player Player 
        { 
            get { return player; } 
            set 
            { 
                if (player == null)
                player = value; 
            }
        }

        public Companion(LevelManager manager) 
            : base(true, manager)
        {
            state = State.Active;
        }

        public override void Update(GameTime gameTime)
        {
            Input();
            base.Update(gameTime);
            
        }

        public override void Input()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed
                && previousMS.LeftButton == ButtonState.Released)
            {
                if (player.Radius < 500 || true)
                {
                    player.Impulse(GetDifferenceVector(player.Position) / 15);
                }
            }
        }

        public Vector2 GetDifferenceVector(Vector2 otherPosition)
        {
            Vector2 temp = new Vector2(
                position.X - otherPosition.X,
                position.Y - otherPosition.Y);

            return temp;
        }
    }
}
