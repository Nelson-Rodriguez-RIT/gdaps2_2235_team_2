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
    /// <summary>
    /// Key Object that is able to be picked up and can unlock door terrain
    /// </summary>
    internal class KeyObject : Entity
    {
        //fields
        private bool isColliding;
        private Vector2 originalPos;
        private Player player;

        //constructor 
        public KeyObject(Vector2 position) :
            base (position, "../../../Content/Entities/Key", false)
        {
            //initialize position, hitbox, texture, and properties
            originalPos = position;
            isColliding = false;

            this.hurtbox = new Rectangle(
                (int)position.X, 
                (int)position.Y,
               int.Parse(properties["HitboxX"]),
                int.Parse(properties["HitboxY"])); //position/hitbox
            this.isColliding = false; //should not start by colliding with anything

            spriteSheet = Loader.LoadTexture("Entities/Key/keycard");
            spriteScale = 1;
        }

        //methods

        /// <summary>
        /// Check if player has collided with key object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>isColliding</returns>
        public bool CheckCollision(Entity entity) 
        {
            if (entity != null && this.hurtbox.Intersects(entity.Hitbox))
            {
                isColliding = true; 
            }

            return isColliding;
        }

        /// <summary>
        /// Updates Key's logic, including checking if player DNE, updating hitbox pos, and collision
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="input"></param>
        public override void Update(GameTime gameTime, StoredInput input)
        {
            if (player == null)
            {
                player = GameManager.entities.GetAllOfType<Player>()[0];
            }

            hurtbox = new Rectangle(
                (int)vectorPosition.X,
                (int)vectorPosition.Y,
                hurtbox.Width, 
                hurtbox.Height);

            if (CheckCollision(player)) 
            {
                this.hurtbox.X = player.Position.X - hurtbox.Height/4;
                this.hurtbox.Y = player.Position.Y - player.hurtbox.Height - hurtbox.Height/4;
            }
        }

        /// <summary>
        /// Draws the key
        /// </summary>
        /// <param name="batch"></param>
        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(
                spriteSheet, 
                Camera.RelativePosition(Position), 
                null,
                Color.White,
                0f,
                new Vector2 (0,0),
                new Vector2 (.25f, .25f),
                SpriteEffects.None,
                0
                );
        }

        /// <summary>
        /// Reset Key
        /// </summary>
        public void Reset()
        {
            GameManager.SpawnEntity<KeyObject>(new Object[] { originalPos });

            GameManager.DespawnEntity(this);
        }
    }
}
