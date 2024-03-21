using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Interfaces
{
    public interface IDamageable : ICollidable
    {
        /// <summary>
        /// Health of the entity
        /// </summary>
        int Health { get; set; }

        /// <summary>
        /// Reduces health by a certain amount
        /// </summary>
        /// <param name="damage"></param>
        void TakeDamage(int damage);
    }
}
