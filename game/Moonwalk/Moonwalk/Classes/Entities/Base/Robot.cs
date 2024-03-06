using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Moonwalk.Classes.Entities.Base
{
    internal class Robot : PlayerControlled
    {
        //Change this to private later
        public Robot(Vector2 position, string directory) : base(position, directory)
        {
            physicsState = PhysicsState.Linear;
        }

        public override void Input(StoredInput input)
        {
            velocity = input.CurrentMouse.Position.ToVector2() - position;
        }
    }
}
