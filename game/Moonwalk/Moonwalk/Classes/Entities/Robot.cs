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
            Idle,
            TransitionToMove,
            Move
        }

        private static bool locked;
        const float MoveSpeed = 40f;
        protected internal Point mousepos; // Internal used for GUIRobotDebugElement
        public static ICollidable Tether;

        public static bool Locked
        {
            get { return locked; }
        }

        //Change this to private later
        public Robot() : base(Player.MostRecentCheckpoint.Hitbox.Location.ToVector2(), "../../../Content/Entities/Robot")
        {
            physicsState = PhysicsState.Linear;
            SwitchAnimation(Animations.Idle);
            spriteScale = 1;

            
            GUI.AddElement(new GUIRobotDebugElement(new Vector2(400, 50), "File", this)); // Debug
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            Input(input, gameTime);

            if (!locked)
            Movement(gameTime);

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

            if (Tether != null)
            {
                //number of links to make
                const int Links = 10;
                double radius = VectorMath.Magnitude(VectorMath.Difference(Tether.Hitbox.Center.ToVector2(), hurtbox.Center.ToVector2()));

                //Distance between each link
                int slice = (int)(radius / Links);

                for (int i = 1; i <= Links; i++)
                {
                    Vector2 thing = Vector2.Normalize(VectorMath.Difference(Tether.Hitbox.Center.ToVector2(), hurtbox.Center.ToVector2()))
                        * slice * i; //increment every iteration

                    Particle.Effects.Add(new Particle
                        (1,
                        Color.SkyBlue,
                        ParticleEffects.None,
                        (Tether.Hitbox.Center.ToVector2() + thing)
                            .ToPoint(),
                        0,
                        3,
                        4)
                        );
                }
            }
            
        }

        public override void Input(StoredInput input, GameTime gameTime)
        {
            mousepos = input.CurrentMouse.Position;

            
            Vector2 pos = Camera.WorldToScreen(mousepos.ToVector2() / 2);


            if (!locked)
            velocity = (pos + Camera.GlobalOffset / 2) - vectorPosition - (hurtbox.Center - hurtbox.Location).ToVector2();

            if (VectorMath.Magnitude(velocity) < 5)
            {
                vectorPosition = (pos + Camera.GlobalOffset / 2) - (hurtbox.Center - hurtbox.Location).ToVector2();
            }


        }

        public Vector2 GetPosition()
        {
            return this.hurtbox.Center.ToVector2();
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
            Vector2 temp = Camera.RelativePosition(vectorPosition);

            activeAnimation.Draw(batch, GameMain.ActiveScale, spritesheet, temp);
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
