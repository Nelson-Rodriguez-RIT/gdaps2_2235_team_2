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
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace Moonwalk.Classes.Entities
{

    /// <summary>
    /// The player's trusty companion
    /// </summary>
    internal class Robot : PlayerControlled
    {
        protected enum Animations
        {
            /*
            Idle,
            Idle_Blink,
            Walk,
            Run,
            Crouch,
            Jump,
            Hurt,
            Death,
            Attack,
            */

            Idle,
            TransitionToMove,
            Move
        }

        private bool locked;
        const float MoveSpeed = 40f;
        internal Point mousepos; // Internal used for GUIRobotDebugElement

        //Change this to private later
        public Robot(Vector2 position) : base(position, "../../../Content/Entities/Robot")
        {
            physicsState = PhysicsState.Linear;
            SwitchAnimation(Animations.Idle);
            spriteScale = 1;

            //GUI.AddElement(new GUIRobotDebugElement(new Vector2(400, 50), "File", this)); // Debug
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            Input(input);

            if (!locked)
            //Movement(gameTime);

            activeAnimation.UpdateAnimation(gameTime);

            if (velocity.X != 0
                && Math.Abs(velocity.X) < 0.5f)
            //&& Math.Sign(acceleration.X) != Math.Sign(velocity.X))
            {
                velocity.X = 0;
            }

            if (velocity.Y != 0
                && Math.Abs(velocity.Y) < 0.5f)
            //&& Math.Sign(acceleration.X) != Math.Sign(velocity.X))
            {
                velocity.Y = 0;
            }

            //Flip the sprite
            if (velocity.X < 0)
            {
                activeAnimation.FaceDirection = (int)FaceDirection.Left;
            }
            else if (velocity.X > 0) 
            {
                activeAnimation.FaceDirection = (int)FaceDirection.Right;
            }



            if (velocity.Y > 0 || velocity.X > 0)
            {
                SwitchAnimation(Animations.Move, false);
            }
            else
            {
                SwitchAnimation(Animations.Idle, false);
            }

            
        }

        public override void Input(StoredInput input)
        {
            mousepos = input.CurrentMouse.Position;
            Vector2 pos = (Camera.RelativePosition(mousepos.ToVector2()) + Camera.Target * GameMain.ActiveScale - Camera.GlobalOffset);
            //pos.Normalize();

            //vectorPosition = Camera.RelativePosition(pos) ;

            vectorPosition = pos;

        }

        public Vector2 GetPosition()
        {
            return this.vectorPosition;
        }

        public void ToggleLock()
        {
            locked = !locked;
        }

        public override void Draw(SpriteBatch batch)
        {
            if (spriteScale == 0)
            {
                throw new Exception("You forgot to set the sprite scale");
            }

            //apply offset
            Vector2 temp = (vectorPosition);

            activeAnimation.Draw(batch, GameMain.ActiveScale, spritesheet, temp);

            batch.DrawString(GUI.GetFont("File"),
                $"Hitbox: {hurtbox.X} - {hurtbox.Y} ",
                new Vector2(400, 50),
                Color.White);

            batch.DrawString(GUI.GetFont("File"),
                $"Drawing: {Math.Round(temp.Y)} - {Math.Round(temp.X)} ",
                new Vector2(400, 75),
                Color.White);


            batch.DrawString(GUI.GetFont("File"),
                $"Position: {(mousepos.Y)} - {(mousepos.X)} ",
                new Vector2(400, 100),
                Color.White);
        }
    }


    #region GUIElements
    internal class GUIRobotDebugElement : GUIElement {
        private Vector2 position;
        private SpriteFont font;
        private Robot target;

        public GUIRobotDebugElement(Vector2 position, string fontName, Robot target) {
            this.position = position;
            font = GUI.GetFont(fontName);
            this.target = target;
        }

        public override void Draw(SpriteBatch batch) {
            batch.DrawString(
                font,
                    $"Hitbox: {target.hurtbox.X} - {target.hurtbox.Y}\n" +
                    $"Drawing: {Math.Round(target.vectorPosition.Y)} - {Math.Round(target.vectorPosition.X)}\n" +
                    $"Position: {target.mousepos.Y} - {target.mousepos.X}",
                position,
                Color.White
                );
        }
    }
    #endregion
}
