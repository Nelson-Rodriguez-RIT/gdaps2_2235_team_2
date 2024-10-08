﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Managers;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Moonwalk.Classes.Helpful_Stuff
{
    public delegate void TargetEntered(List<IDamageable> list);

    internal class Hitbox
    {
        public static Texture2D hitboxSprite;
        public static List<Hitbox> activeHitboxes = new List<Hitbox>();
        public static bool drawHitbox = false;

        private Rectangle box;
        private Entity source;
        private Vector2 sourcePos;
        private double duration;  
        private Point offset;
        private List<IDamageable> targets;
        private List<IDamageable> alreadyHit;

        public event TargetEntered targetEntered;


        #region Constructors
        public Hitbox(double duration, Entity source, Point size, List<IDamageable> targets, Point offset) 
        {
            if (hitboxSprite == null)
                hitboxSprite = Loader.LoadTexture("../../../Content/Entities/hitbox");

            this.duration = duration;
            this.source = source;
            this.offset = offset;
            box = new Rectangle(source.Position + offset, size);
            activeHitboxes.Add(this);
            this.targets = targets;
            alreadyHit = new List<IDamageable>();

        }

        public Hitbox(double duration, Rectangle hitbox, Vector2 ownerPosition, List<IDamageable> targets, int direction = 0) {
            if (hitboxSprite == null)
                hitboxSprite = Loader.LoadTexture("../../../Content/Entities/hitbox");

            this.duration = duration;
            sourcePos = new Vector2(hitbox.X, hitbox.Y) + ownerPosition;
            box = (direction == 0) ? hitbox : new Rectangle(hitbox.X * -1, hitbox.Y, hitbox.Width, hitbox.Height);
            this.targets = targets;

            this.source = null;
            this.offset = Point.Zero;

            activeHitboxes.Add(this);
            alreadyHit = new List<IDamageable>();
        }

        public Hitbox(double duration, Vector2 sourceVector, Point size, List<IDamageable> targets, Point offset)
        {
            if (hitboxSprite == null)
                hitboxSprite = Loader.LoadTexture("../../../Content/Entities/hitbox");

            this.duration = duration;
            this.source = null;
            sourcePos = sourceVector;
            this.offset = offset;
            box = new Rectangle(sourcePos.ToPoint() + offset, size);
            activeHitboxes.Add(this);
            this.targets = targets;
            alreadyHit = new List<IDamageable>();

        }
        #endregion

        public void Update(GameTime gameTime)
        {
            // update position based on the source
            if (source != null)
            {
                sourcePos = source.Position.ToVector2();
            }

            //update rectangle location
            box = new Rectangle(sourcePos.ToPoint() + offset, box.Size);

            //Get a list of things colliding with this
            List<IDamageable> collisions = CheckCollision();

            for (int i = 0; i < collisions.Count; i++)
            {
                //remove items already collided with
                if (alreadyHit.Contains(collisions[i]))
                {
                    collisions.RemoveAt(i);
                    i--;
                }
            }

            //raise event for each collision
            if (collisions.Count > 0)
                targetEntered(collisions);

            //add those collisions to the things already collided with
            foreach (IDamageable target in collisions)
            {
                alreadyHit.Add(target);
            }

            duration -= gameTime.ElapsedGameTime.TotalSeconds;

            if (duration <= 0)
            {
                activeHitboxes.Remove(this);
            }
        }

        public virtual List<IDamageable> CheckCollision()
        {
            List<IDamageable> temp = targets.FindAll(item => item.Hitbox.Intersects(box));

            return temp;
        }

        public virtual void AddToAlreadyHit(List<IDamageable> collisions)
        {
            foreach (IDamageable target in collisions)
            {
                alreadyHit.Add(target);
            }
        }

        public void DrawHitbox(SpriteBatch batch)
        {
            Vector2 position = Camera.RelativePosition(
                new Vector2(
                    box.X,
                    box.Y
                    )
                );

            batch.Draw(
                hitboxSprite,
                new Rectangle(
                    (int)(position.X),
                    (int)(position.Y),
                    (int)(box.Width * GameMain.ActiveScale.X),
                    (int)(box.Height * GameMain.ActiveScale.Y)
                    ),
                Color.Red
                );

        }
    }
}
