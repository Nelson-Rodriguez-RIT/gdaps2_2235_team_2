using Moonwalk.Classes.Entities.Base;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Entities {
    internal class MapLoadingTrigger : Entity {

        public MapLoadingTrigger(Vector2 position) 
                : base(position, "../../../Content/Entities/MapLoadingTrigger") {

        }
    }
}
