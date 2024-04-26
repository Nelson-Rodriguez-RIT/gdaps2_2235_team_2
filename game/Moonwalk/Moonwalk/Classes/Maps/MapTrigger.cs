using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Managers;

namespace Moonwalk.Classes.Maps
{
    /// <summary>
    /// Terrain that once entered, loads a new map
    /// </summary>
    internal class MapTrigger : Terrain
    {
        private string target;

        public MapTrigger(Rectangle hitbox, string target //the name of the map
            )
                : base(hitbox) {
            this.collidable = false;
            this.target = target;
            OnCollision += LoadMap;
        }

        private void LoadMap() {
            if (Map.LoadedMapName != target)
                Map.LoadMap(target, true);
        }
    }
}
