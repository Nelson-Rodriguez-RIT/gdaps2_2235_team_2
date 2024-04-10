﻿using Microsoft.Xna.Framework;
using Moonwalk.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Interfaces
{
    internal interface IControllable : IMovable
    {
        void Input(StoredInput input, GameTime gameTime);
    }
}
