using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Managers;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Moonwalk.Classes.Entities
{
    internal class TestEnemy : Enemy
    {
        private enum Animations
        {
            StaticIdle,
            Wake,
            Move,
            Charge,
            Shoot,
            Dash,
            Damaged,
            Death
        }

        FaceDirection faceDirection;

        public TestEnemy(Vector2 position) : base(position, "../../../Content/Entities/TestEnemy")
        {
            health = int.Parse(properties["Health"]);
            damage = int.Parse(properties["Damage"]);
            SwitchAnimation(Animations.Move);
            gravity = 50f;
            acceleration = new Vector2(0, gravity);
            spriteScale = 1;
            maxXVelocity = 50;

        }

        public override void AI(Vector2 target)
        {
            float xDifference = VectorMath.VectorDifference(vectorPosition, target).X;

            if (xDifference > 0)
            {
                faceDirection = FaceDirection.Right;
            }
            else if (xDifference < 0)
            {
                faceDirection = FaceDirection.Left;
            }

            acceleration.X = 60 * (faceDirection == FaceDirection.Right ? 1 : -1);
            
        }

    }
}
