using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonwalk.Classes.Entities.Base;
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

        protected List<Rectangle> hitboxes;
        protected int maxHealth;
        protected int health;
        protected Random rng;
        protected Vector2 center;
        protected Animation activeAnimation;

        internal protected Dictionary<string, string> properties = null;
        internal protected List<Animation> animations = null;
        internal protected Texture2D spritesheet = null;

        public BossFight(string directory)
        {
            BossData bufferedData = Loader.LoadBoss(directory);
            bufferedData.Load(this);

            maxHealth = int.Parse(properties["MaxHealth"]);
            health = int.Parse(properties["MaxHealth"]);

            Boss = this;
        }

        public virtual void Update(GameTime gt)
        {
            if (activeAnimation != null)
                activeAnimation.UpdateAnimation(gt);
        }

        internal class BossData
        {
            private Dictionary<string, string> properties;
            private List<Animation> animations;
            private Texture2D spritesheet;

            public BossData(Dictionary<string, string> properties,
                    List<Animation> animations, Texture2D spritesheet)
            {
                this.properties = properties;
                this.animations = animations;
                this.spritesheet = spritesheet;
            }

            public void Load(BossFight boss)
            {
                boss.properties = properties;
                boss.animations = animations;
                boss.spritesheet = spritesheet;
            }


        }
    }
}
