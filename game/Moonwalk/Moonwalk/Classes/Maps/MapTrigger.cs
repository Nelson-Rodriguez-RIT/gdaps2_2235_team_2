using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;

namespace Moonwalk.Classes.Maps
{
    internal class MapTrigger : Terrain
    {
        private string target;

        public MapTrigger(Rectangle hitbox, string target)
                : base(hitbox) {
            this.target = target;
            OnCollision += LoadMap;
        }

        private void LoadMap() {
            Map.LoadMap(target);
        }
    }
}
