using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Moonwalk.Classes.Entities.Base
{
    /// <summary>
    /// Contains universal functionality for all entities
    /// </summary>
    internal abstract class Entity
    {
        // Contains the entity's sprite table and position
        protected Rectangle entity;

        public abstract void Update(GameTime gt);

        public abstract void Draw(SpriteBatch sb);
    }
}
