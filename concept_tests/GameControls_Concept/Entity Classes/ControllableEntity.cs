﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GameControls_Concept
{
    internal abstract class ControllableEntity : MovableEntity
    {
        
        protected KeyboardState keyboardState;
        protected KeyboardState previousKB;
        protected MouseState mouseState;
        protected MouseState previousMS;

        
        public ControllableEntity(LevelManager manager, Vector2 position)
            : base(manager, position)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            previousKB = keyboardState;
            previousMS = mouseState;

            //base.Update(gameTime);
        }

        protected virtual void Input()
        {

        }
    }
}
