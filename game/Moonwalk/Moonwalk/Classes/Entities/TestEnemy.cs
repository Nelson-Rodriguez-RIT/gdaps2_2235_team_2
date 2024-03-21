using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Moonwalk.Classes.Entities.Base;

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

        public TestEnemy(Vector2 position, Object[] args) :base("../../../Content/Entities/TestEnemy", position)
        {
            health = int.Parse(properties["Health"]);
            damage = int.Parse(properties["Damage"]);
            SwitchAnimation(Animations.Move);
            gravity = 50f;
        }

        protected override void AI()
        {
            throw new NotImplementedException();
        }
    }
}
