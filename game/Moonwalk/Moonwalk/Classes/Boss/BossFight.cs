using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Boss
{
    internal abstract class BossFight
    {
        public static BossFight Boss = null;
        private static Texture2D hitboxSprite = null;

        protected int maxHealth;
        protected int health;
        protected Random rng;
        protected Vector2 center;
        protected Vector2 relativePosition;
        protected Animation activeAnimation;

        internal protected Dictionary<string, string> properties = null;
        internal protected List<Animation> animations = null;
        internal protected Texture2D spritesheet = null;
        internal protected Dictionary<string, List<Rectangle>> hitboxes = null;

        public BossFight(string directory, Vector2 center)
        {
            BossData bufferedData = Loader.LoadBoss(directory);
            bufferedData.Load(this);

            maxHealth = int.Parse(properties["MaxHealth"]);
            health = int.Parse(properties["MaxHealth"]);
            this.center = center;

            if (hitboxSprite == null)
                hitboxSprite = Loader.LoadTexture("../../../Content/Entities/hitbox");

            Boss = this;
        }

        public virtual void Update(GameTime gt)
        {
            if (activeAnimation != null)
                activeAnimation.UpdateAnimation(gt);
        }

        /// <summary>
        /// Switches the animation currently playing to another
        /// </summary>
        /// <param name="animation">The animation to switch to</param>
        protected virtual void SwitchAnimation(Enum animationEnum, bool resetAnimation = true)
        {
            activeAnimation = animations[Convert.ToInt32(animationEnum)];
            if (resetAnimation)
                activeAnimation.Reset();
        }

        public virtual void Draw(SpriteBatch batch)
        {

            //apply offset
            Vector2 temp = Camera.RelativePosition(center);

            activeAnimation.Draw(batch, GameMain.ActiveScale, spritesheet, temp);
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
                boss.hitboxes = hitboxes;
            }


        }


    }
}
