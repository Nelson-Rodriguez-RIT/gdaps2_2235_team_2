using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonwalk.Classes.Managers;

namespace Moonwalk.Classes.Entities
{
    internal class KeyObject : Entity, IInteractible
    {
        
        //fields
        private bool isColliding;
        private bool isInteracted;

        public KeyObject(Vector2 position) :
            base (position, "../../../Content/Entities/Key", false, false)
        {
            //this.hurtbox = new Rectangle((int)position.X, (int)position.Y, 16, 16); //position/hitbox
            this.isColliding = false; //should not start by colliding with anything

            spriteSheet = Loader.LoadTexture("Entities/Key/key");
            spriteScale = 1;
        }

        //methods

        public bool IsInteracted(Entity entity) 
        {
            if ((Player)entity != null && CheckCollision((Player)entity))
            {
                isInteracted = true;


            }
            return isInteracted;
        }

        public bool CheckCollision(Entity entity) 
        {
            if (this.hurtbox.Intersects(((Player)entity).Hitbox))
            {
                isColliding = true; 
            }

            return isColliding;
        }

        public override void Update(GameTime gameTime, StoredInput input)
        {
            
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(spriteSheet, Camera.RelativePosition(Position), Color.White);
        }

    }
}
