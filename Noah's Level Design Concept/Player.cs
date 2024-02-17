using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using System.Xml.Serialization;

namespace Noah_s_Level_Design_Concept
{
    internal enum PlayerState
    {
        Attack,
        Dead,
        DoorIn,
        DoorOut,
        Fall,
        Ground,
        Hit,
        Idle,
        Jump,
        Run
    }
    internal class Player
    {
        Vector2 playerLocation;
        PlayerState state;

        Texture2D attackSpriteSheet;
        Texture2D idleSpriteSheet;

        public int frame;
        double timeCounter;
        double fps;
        double timePerFrame;

        const int AttackFrameCount = 3;
        const int IdleFrameCount = 11;
        const int PlayerRectOffsetY = 0;
        const int PlayerRectHeight = 58;
        const int PlayerRectWidth = 78;

        public float Xpos
        {
            get { return this.playerLocation.X; }
            set { this.playerLocation.X = value; }
        }
        public float Ypos
        {
            get { return this.playerLocation.Y; }
            set { this.playerLocation.Y = value; }
        }
        public PlayerState State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        public Player(Texture2D attackSpritesheet, Texture2D idleSpriteSheet, Vector2 playerLocation, PlayerState startingState)
        {
            this.attackSpriteSheet = attackSpritesheet;
            this.idleSpriteSheet = idleSpriteSheet;
            this.playerLocation = playerLocation;
            this.state = startingState;

            fps = 10;
            timePerFrame = 1 / fps;
        }

        public void UpdateAnimation(GameTime gametime) 
        {
            timeCounter += gametime.ElapsedGameTime.TotalSeconds;
            
            if (timeCounter >= timePerFrame) 
            {
                switch (State)
                {
                    case PlayerState.Attack:
                        frame += 1;
                        if (frame >= AttackFrameCount)
                        { 
                            frame = 1;
                            State = PlayerState.Idle;
                        }
                        
                        break;
                    case PlayerState.Idle:
                        frame += 1;
                        if (frame >= IdleFrameCount)
                        { frame = 1; }
                        break;
                }
                timeCounter -= timePerFrame;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (State)
            {
                case PlayerState.Attack:
                    DrawAttacking(SpriteEffects.None, spriteBatch);
                    break;
                case PlayerState.Idle:
                    DrawIdle(SpriteEffects.None, spriteBatch);
                    break;
            }
        }

        private void DrawAttacking(SpriteEffects flipSprite, SpriteBatch attackSprites)
        {
            attackSprites.Draw(
                attackSpriteSheet,
                playerLocation,
                new Rectangle(
                    frame * PlayerRectWidth,
                    PlayerRectOffsetY,
                    PlayerRectWidth,
                    PlayerRectHeight),
                Color.White,
                0,
                Vector2.Zero,
                3.0f,
                flipSprite,
                0);
        }
        private void DrawIdle(SpriteEffects flipSprite, SpriteBatch idleSprites)
        {
            idleSprites.Draw(
                idleSpriteSheet,
                playerLocation,
                new Rectangle(
                    frame * PlayerRectWidth,
                    PlayerRectOffsetY,
                    PlayerRectWidth,
                    PlayerRectHeight),
                Color.White,
                0,
                Vector2.Zero,
                3.0f,
                flipSprite,
                0);
        }
    }
}
