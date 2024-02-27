using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers.Map
{
    // These enums' names should match their respecitive directory name
    enum MapGroups {
        Test
    }

    /// <summary>
    /// Handles loading levels from file and level geometry
    /// </summary>
    internal sealed class MapManager
    {
        private const string RootDirectory = "../../../Content/Maps/";
        private static MapManager instance = null;
        private List<Map> maps;


        private MapManager() { }


        /// <summary>
        /// Gets Level's singleton instance
        /// </summary>
        /// <returns>A Level object</returns>
        public static MapManager GetInstance()
        {
            if (instance == null)
                instance = new MapManager();

            return instance;
        }

        public void Load(MapGroups group) {
            maps = new List<Map>();

            switch (group) {
                case MapGroups.Test:
                    maps.Add(new Map(
                        $"{RootDirectory}{group.ToString()}",
                        new Vector2(0, 0)));
                    break;
            }
            
        }

        public void Draw(SpriteBatch sb, Vector2 globalScale) {
            foreach (Map map in maps)
                map.Draw(sb, globalScale);
        }
    }
}
