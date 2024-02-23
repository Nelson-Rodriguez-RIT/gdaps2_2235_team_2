using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Numerics;

namespace GameControls_Concept
{
    public delegate void OnPlayerEnterDelegate();

    internal class EventTrigger
    {
        private Rectangle area;
        public event OnPlayerEnterDelegate onPlayerEnter;
        private bool eventHappened;

        public EventTrigger(Rectangle area)
        {
            this.area = area;
            eventHappened = false;
        }

        public void Update(Player player)
        {
            //triggers the event if the player enters for the first time
            if (area.Contains(player.Position) && !eventHappened)
            {
                onPlayerEnter();
                eventHappened = false;
            }
        }

        public void LoadEvent()
        {
            /*
             * Maybe this method can be used to load the specific event 
             * (instructions for the player, cutscenes, idk) from a file
             * OR the name of the method which could then be used to get
             * whatever we want to happen from a static class that holds
             * all of our game events.
            */
        }
    }
}
