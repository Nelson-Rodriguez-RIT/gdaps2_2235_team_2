using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Entities.Base
{

    internal class StoredInput
    {
        private MouseState currentMouse;
        private MouseState previousMouse;
        private KeyboardState currentKeyboard;
        private KeyboardState previousKeyboard;
        /// <summary>
        /// In the future this will store buffered inputs 
        /// </summary>
        private List<Keys> buffered;
        bool hasUpdated = false;

        public MouseState CurrentMouse
        { get { return currentMouse; } }

        public MouseState PreviousMouse
        { get { return previousMouse; } }

        public KeyboardState CurrentKeyboard
        { get { return currentKeyboard; } }

        public KeyboardState PreviousKeyboard
        { get { return previousKeyboard; } }

        public void Update()
        {
            if (!hasUpdated)
            {
                currentKeyboard = Keyboard.GetState();
                currentMouse = Mouse.GetState();
                hasUpdated = true;
            }
            else
            {
                throw new Exception("You forgot to call UpdatePrevious()");
            }

        }

        public void UpdatePrevious()
        {
            previousMouse = currentMouse;
            previousKeyboard = currentKeyboard;
            hasUpdated = false;
        }


    }
}
