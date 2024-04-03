using Microsoft.Xna.Framework;
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
        private int frameDuration;
        private Point offset;
        private List<IDamageable> targets;
        private List<IDamageable> alreadyHit;

        public event TargetEntered targetEntered;

        /// <summary>
        /// Things that the hitbox can hit, e.g. players, enemies
        /// </summary>
        Type target;

        public Hitbox(int duration, Entity source, Point size, Type target, List<IDamageable> targets, Point offset) 
        {
            if (hitboxSprite == null)
                hitboxSprite = Loader.LoadTexture("../../../Content/Entities/hitbox");

            frameDuration = duration;
            this.target = target;
            this.source = source;
            this.offset = offset;
            box = new Rectangle(source.Position + offset, size);
            activeHitboxes.Add(this);
            this.targets = targets;
            alreadyHit = new List<IDamageable>();

        }

        public void Update(GameTime gameTime)
        {
            
            box = new Rectangle(source.Position + offset, box.Size);

            List<IDamageable> collisions = CheckCollision();

            for (int i = 0; i < collisions.Count; i++)
            {
                if (alreadyHit.Contains(collisions[i]))
                {
                    collisions.RemoveAt(i);
                    i--;
                }
            }

            targetEntered(collisions);

            foreach (IDamageable target in collisions)
            {
                alreadyHit.Add(target);
            }

            frameDuration--;

            if (frameDuration == 0)
            {
                activeHitboxes.Remove(this);
            }
        }

        public virtual List<IDamageable> CheckCollision()
        {
            List<IDamageable> temp = targets.FindAll(item => item.Hitbox.Intersects(box));

            return temp;
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
