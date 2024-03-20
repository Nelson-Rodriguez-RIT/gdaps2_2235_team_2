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

namespace Moonwalk.Classes.Helpful_Stuff
{
    /// <summary>
    /// Helps with math (use for refactoring later)
    /// </summary>
    internal static class VectorMath
    {
        /// <summary>
        /// Get the vector2 from a to b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector2 VectorDifference(Vector2 a, Vector2 b)
        {
            return new Vector2(b.X - a.X, b.Y - a.Y);
        }

        /// <summary>
        /// Get the magnitude of a vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static double VectorMagnitude(Vector2 vector)
        {
            return Math.Sqrt(
                Math.Pow(vector.X, 2) +
                Math.Pow(vector.Y, 2));
        }
    }
}
