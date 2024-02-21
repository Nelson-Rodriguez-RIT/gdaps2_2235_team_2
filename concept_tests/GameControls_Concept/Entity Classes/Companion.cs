using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
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

        //Animation
        private AnimationStates animationState;
        private int frame;
        private double timeCounter;
        private double fps = 12;
        private double timePerFrame;
        private Texture2D idleSheet;
        private Texture2D moveSheet;
        private Texture2D actionSheet;
        Dictionary<AnimationStates, int[]> spriteData;
        private ContentManager contentManager;
        private SpriteFont font;


        private enum AnimationStates
        {
            Idle,
            Move,
            Action
        }

        public Player Player 
        { 
            get { return player; } 
            set 
            { 
                if (player == null)
                player = value; 
            }
        }

        public Companion(LevelManager manager, ContentManager contentManager) 
            : base(true, manager)
        {
            state = State.Active; 
            animationState = AnimationStates.Idle;
            this.contentManager = contentManager;
            timePerFrame = 1 / fps;


            LoadSpriteSheets<AnimationStates>("../../../Content/spritesheets", contentManager);
        }

        public override void Update(GameTime gameTime)
        {
            Input();
            base.Update(gameTime);
            UpdateAnimation(gameTime);
            
        }

        public override void Draw(SpriteBatch sb)
        {
            frameCount = spriteData[animationState][2];
            //sb.DrawString(font, position.X + "  " + position.Y, new Vector2(300, 100), Color.White);
            //sb.DrawString(font, velocity.X + "  " + velocity.Y, new Vector2(500, 100), Color.White);

            switch (animationState)
            {
                case AnimationStates.Idle:
                    
                    DrawIdle(SpriteEffects.None, sb);
                    break;
            }
        }

        protected override void Input()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed
                && previousMS.LeftButton == ButtonState.Released)
            {
                if (player.Radius < 500)
                {
                    player.Impulse(GetDifferenceVector(player.Position) / 15);
                }
            }
        }

        public Vector2 GetDifferenceVector(Vector2 otherPosition)
        {
            Vector2 temp = new Vector2(
                position.X - otherPosition.X,
                position.Y - otherPosition.Y / 1.1f);

            return temp;
        }       

        /// <summary>
        /// Updates animation as necessary
        /// </summary>
        /// <param name="gameTime">Time information</param>
        public void UpdateAnimation(GameTime gameTime)
        {
            // Handle animation timing
            // - Add to the time counter
            // - Check if we have enough "time" to advance the frame

            // How much time has passed?  
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (timeCounter >= timePerFrame)
            {
                frame += 1;                     // Adjust the frame to the next image

                if (frame >= frameCount)     // Check the bounds - have we reached the end of walk cycle?
                    frame = 1;                  // Back to 1 (since 0 is the "standing" frame)

                timeCounter -= timePerFrame;    // Remove the time we "used" - don't reset to 0
                                                // This keeps the time passed 
            }
        }

        private void DrawIdle(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            int[] data = spriteData[AnimationStates.Idle];

            spriteBatch.Draw(
                idleSheet,                    // - The texture to draw
                position,                       // - The location to draw on the screen
                new Rectangle(                  // - The "source" rectangle
                    0,     //   - This rectangle specifies
                    frame * data[1],           //	   where "inside" the texture
                    data[0],             //     to get pixels (We don't want to
                    data[1]),           //     draw the whole thing)
                Color.White,                    // - The color
                0,                              // - Rotation (none currently)
                Vector2.Zero,                   // - Origin inside the image (top left)
                5.0f,                           // - Scale (100% - no change)
                flipSprite,                     // - Can be used to flip the image
                0);                             // - Layer depth (unused)
        }
    }

 
}
