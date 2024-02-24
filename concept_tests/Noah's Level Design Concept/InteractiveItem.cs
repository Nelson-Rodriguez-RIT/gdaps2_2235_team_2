using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noah_s_Level_Design_Concept
{
    public abstract class InteractiveItem
    {
        //baseline for all interactive items
        //all interactibles need assets, boundary boxes, and states
        enum InteractibleStates
        { 
            Idle,
            Interacted,
            Powering,
            Held
        }
        InteractibleStates state;
        protected Texture2D asset;
        protected Rectangle hitbox;
        protected bool interactedWith = false;
        protected bool canBePickedUp = false;
        protected bool canPower = false;

        public bool InteractedWith
        {
            get { return interactedWith; }
            set { interactedWith = value; }
        }

        public InteractiveItem (Texture2D asset, Rectangle hitbox, bool canBePickedUp, bool canPower)
        {
            state = InteractibleStates.Idle;
            this.asset = asset;
            this.hitbox = hitbox;
            this.canBePickedUp = canBePickedUp;
            this.canPower = canPower;
        }

        public void Update(GameTime gameTime)
        {
            switch (state)
            {
                case InteractibleStates.Idle:
                    //once interacted with, if item can be picked up, goes into held state,
                    //otherwise is simply interacted with.
                    if (interactedWith)
                    {   
                        if (!canBePickedUp) //doors or vents
                        {
                            state = InteractibleStates.Interacted;
                        }
                        else //items or batteries
                        {
                            state = InteractibleStates.Held;
                        }
                    }
                    break;

                case InteractibleStates.Interacted:
                    //once entered, the state should go back to idle so the player can re-interact.
                    state = InteractibleStates.Idle;
                    break;

                case InteractibleStates.Held:
                    //if being held, the player could either place directly into powerslot if 
                    //item is a battery, or if something like a box, item will return to idle when dropped
                    


                    break; 
                case InteractibleStates.Powering:

                    break;
            }
        }

    }
}
