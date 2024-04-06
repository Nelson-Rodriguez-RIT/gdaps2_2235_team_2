using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonwalk.Classes;
using Moonwalk.Classes.Helpful_Stuff;

namespace Moonwalk.Classes
{
    public delegate void ClickHandler(Point mouse);
    /// <summary>
    /// Stores current and previous mouse states, and contains methods to update them
    /// </summary>
    internal class StoredInput
    {
        public static event ClickHandler UserClick;

        private MouseState currentMouse;
        private MouseState previousMouse;
        private KeyboardState currentKeyboard;
        private KeyboardState previousKeyboard;

        /// <summary>
        /// In the future this will store buffered inputs 
        /// </summary>
        private List<BufferedInput> buffered;
        bool hasUpdated = false;

        public List<BufferedInput> Buffered
        {
            get { return buffered; }
        }

        public MouseState CurrentMouse
        { get { return currentMouse; } }

        public MouseState PreviousMouse
        { get { return previousMouse; } }

        public KeyboardState CurrentKeyboard
        { get { return currentKeyboard; } }

        public KeyboardState PreviousKeyboard
        { get { return previousKeyboard; } }

        public StoredInput()
        {
            buffered = new List<BufferedInput>();
        }

        /// <summary>
        /// Updates the mouse and keyboard states
        /// </summary>
        /// <exception cref="Exception">This exception occurs if you did not update the previous states in the last update.</exception>
        public void Update()
        {
            if (!hasUpdated)
            {
                currentKeyboard = Keyboard.GetState();
                currentMouse = Mouse.GetState();

                //Update buffered inputs
                for (int i = 0; i < buffered.Count; i++)
                {
                    buffered[i].timer = buffered[i].timer - 1;

                    if (buffered[i].timer == 0)
                    {
                        buffered.RemoveAt(i);
                        i--;

                    }
                }

                hasUpdated = true;
            }
            else
            {
                throw new Exception("You forgot to call UpdatePrevious()");
            }

        }

        /// <summary>
        /// Updates the previous mouse and keyboard states
        /// </summary>
        public void UpdatePrevious()
        {
            previousMouse = currentMouse;
            previousKeyboard = currentKeyboard;
            hasUpdated = false;
        }

        public bool IsPressed(Keys key)
        {
            if (currentKeyboard.IsKeyDown(key))
            {
                return true;
            }

            return false;
        }

        public bool WasPressed(Keys key)
        {
            if (previousKeyboard.IsKeyDown(key))
            {
                return true;
            }

            return false;
        }

        public void Buffer(Keys key)
        {
            if (!buffered.Exists(item => item.Key == key))
            {
                buffered.Add(new BufferedInput(key));
            }

            
        }

        public void Click() {
            UserClick(currentMouse.Position);
        }
    }
}
