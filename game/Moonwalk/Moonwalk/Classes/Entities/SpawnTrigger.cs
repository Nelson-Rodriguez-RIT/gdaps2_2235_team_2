using Moonwalk.Classes.Entities.Base;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Entities {
    internal class SpawnTrigger<T> : Entity, ICollidable {

        public SpawnTrigger(Vector2 initalPosition, Vector2 size) : base(initalPosition, "", false, false) {

        }
    }
}
