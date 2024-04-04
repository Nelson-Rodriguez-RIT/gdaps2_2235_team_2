using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Moonwalk.Classes.Entities.Base;

namespace Moonwalk.Interfaces
{
    public interface IInteractible
    {
        //needs a hitbox for collision
        Rectangle Hitbox { get; }

        //need a way to check if currently colliding (with player)
        //bool CheckCollision(Entity entity);

        //need a way to check if object has been interacted with
        //bool IsInteracted(Entity entity);

    }
}
