﻿using Microsoft.Xna.Framework;
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
    internal class KeyObject : Entity
    {
        
        //fields
        private bool isColliding = false;
        private bool isInteracted = false;
        private Vector2 originalPos;

        private Player player;

        public bool IsColliding { get { return isColliding; } }

        public bool IsInteracted { get { return isInteracted; } }   


        public KeyObject(Vector2 position) :
            base (position, "../../../Content/Entities/Key", false)
        {
            originalPos = position;

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

        public bool CheckInteraction(Entity entity) 
        {
            if (player != null && CheckCollision(player))
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
            if (player == null)
            {
                player = GameManager.entities.GetAllOfType<Player>()[0];
            }

            hurtbox = new Rectangle(
                (int)vectorPosition.X,
                (int)vectorPosition.Y,
                hurtbox.Width, 
                hurtbox.Height);

            if (CheckInteraction(player)) 
            {
                this.hurtbox.X = player.Position.X - hurtbox.Height/4;
                this.hurtbox.Y = player.Position.Y - player.hurtbox.Height - hurtbox.Height/4;
            }
        }

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

        public void Reset()
        {

            KeyObject newKey = GameManager.SpawnEntity<KeyObject>(new Object[] { originalPos });

            GameManager.DespawnEntity(this);
        }
    }
}
