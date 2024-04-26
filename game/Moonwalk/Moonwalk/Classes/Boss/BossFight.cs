using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Entities;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Managers;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Boss
{
    internal abstract class BossFight
    {
        protected enum FaceDirection
        {
            Right,
            Left
        }

        //lots and lots of fields
        protected FaceDirection faceDirection;

        public static BossFight Boss = null;
        public static bool DrawHitboxes = false;
        private static Texture2D hitboxSprite = null;

        protected int maxHealth;
        protected int health;
        protected Random rng;
        protected Vector2 cameraTarget;
        protected Vector2 center;
        protected Animation activeAnimation;
        protected Enum currentBehavior;
        protected Enum currentAttack;

        internal protected Dictionary<string, string> properties = null;
        internal protected List<Animation> animations = null;
        internal protected Texture2D spritesheet = null;
        internal protected Dictionary<string, List<Rectangle>> hitboxData = null;
        protected List<Rectangle> hitboxes;

        protected Dictionary<Enum, int> attackDamage;

        /// <summary>
        /// Hitboxes currently active
        /// </summary>
        public List<Rectangle> Hitboxes
        {
            get { return hitboxes; }
        }

        public BossFight(string directory)
        {
            BossData bufferedData = Loader.LoadBoss(directory);
            bufferedData.Load(this);
            hitboxes = new List<Rectangle>();
            rng = new Random();

            //read properties from file
            maxHealth = int.Parse(properties["MaxHealth"]);
            health = int.Parse(properties["MaxHealth"]);
            this.center = new Vector2(
                int.Parse(properties["X"]),
                int.Parse(properties["Y"]));

            this.cameraTarget = new Vector2(
                int.Parse(properties["CameraX"]),
                int.Parse(properties["CameraY"]));
            attackDamage = new Dictionary<Enum, int>();

            //set camera target on a location
            //Camera.SetTarget(this.cameraTarget);

            if (hitboxSprite == null)
                hitboxSprite = Loader.LoadTexture("../../../Content/Entities/hitbox");

            //make this the current boss
            if (Boss == null)
            Boss = this;
        }

        public virtual void Update(GameTime gt)
        {
            if (activeAnimation != null)
                activeAnimation.UpdateAnimation(gt);

            //clear hitboxes, then add updated ones
            hitboxes.Clear();

            List<Rectangle> list = hitboxData[currentBehavior.ToString()];

            foreach (Rectangle rect in list)
            {
                hitboxes.Add(new Rectangle(
                    faceDirection == FaceDirection.Right ? rect.X + (int)center.X : -rect.X - rect.Width + (int)center.X,
                    rect.Y + (int)center.Y,
                    rect.Width,
                    rect.Height));
            }
        }

        /// <summary>
        /// Switches the animation currently playing to another
        /// </summary>
        /// <param name="animation">The animation to switch to</param>
        protected virtual void SwitchBehavior(Enum animationEnum, bool resetAnimation = true)
        {
            activeAnimation = animations[Math.Abs(Convert.ToInt32(animationEnum))];
            if (Convert.ToInt32(animationEnum) < 0)
                activeAnimation.AnimationEndSet = true;

            
            if (resetAnimation)
                activeAnimation.Reset();
        }

        protected virtual void DealDamage(List<IDamageable> damageables)
        {
            //damage all entities hit
            foreach (IDamageable damageable in damageables)
            damageable.TakeDamage(attackDamage[currentAttack]);
        }

        public virtual void Draw(SpriteBatch batch)
        {

            //apply offset
            Vector2 temp = Camera.RelativePosition(center);

            activeAnimation.Draw(batch, GameMain.ActiveScale, spritesheet, temp);

            if (DrawHitboxes)
            {
                DrawHitbox(batch);
            }
        }

        public void DrawHitbox(SpriteBatch batch)
        {
            //draw each hitbox
            foreach (Rectangle hurtbox in hitboxes)
            {
                Vector2 position = Camera.RelativePosition(
                new Vector2(
                    hurtbox.X,
                    hurtbox.Y
                    )
                );

                batch.Draw(
                    hitboxSprite,
                    new Rectangle(
                        (int)(position.X),
                        (int)(position.Y),
                        (int)(hurtbox.Width * GameMain.ActiveScale.X),
                        (int)(hurtbox.Height * GameMain.ActiveScale.Y)
                        ),
                    Color.Orange
                    );

                batch.Draw(
                    hitboxSprite,
                    new Rectangle(Camera.RelativePosition(center).ToPoint() - new Point(2, 2), new Point(5, 5)),
                    Color.White);
            }
        }

        public void CheckCollision()
        {
            //check if a player projectile hits this
            List<PlayerProjectile> hits;
            if ((hits = GameManager.entities
                    .GetAllOfType<PlayerProjectile>()
                        .FindAll(bullet =>
                            hitboxes.Exists(rect => rect.Intersects(bullet.Hitbox))
                            ))
                != null)
            {
                // if it does, take damage
                foreach (PlayerProjectile hit in hits)
                {
                    health -= hit.Damage;
                    hit.Collisions--;
                    
                }
            }

            //damage player if it collides with this
            Player player = GameManager.entities.GetAllOfType<Player>()[0];

            
            if (hitboxes.Exists(rect => rect.Intersects(player.Hitbox)))
            {
                player.TakeDamage(int.Parse(properties["Collide"]));
            }
            
        }

        internal class BossData
        {
            private Dictionary<string, string> properties;
            private List<Animation> animations;
            private Texture2D spritesheet;
            private Dictionary<string, List<Rectangle>> hitboxes;

            public BossData(Dictionary<string, string> properties,
                    List<Animation> animations, Texture2D spritesheet,
                    Dictionary<string, List<Rectangle>> hitboxes)
            {
                this.properties = properties;
                this.animations = animations;
                this.spritesheet = spritesheet;
                this.hitboxes = hitboxes;
            }

            public void Load(BossFight boss)
            {
                boss.properties = properties;
                boss.animations = animations;
                boss.spritesheet = spritesheet;
                boss.hitboxData = hitboxes;
            }


        }


    }
}
