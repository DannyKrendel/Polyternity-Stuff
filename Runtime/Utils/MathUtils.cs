using UnityEngine;

namespace PolyternityStuff.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Maps a value from one range of numbers to another.
        /// </summary>
        public static float Map(this float value, float a, float b, float a1 = 0, float b2 = 1) =>
            (value - a) / (b - a) * (b2 - a1) + a1;

        /// <summary>
        /// Maps a value from one range of numbers to another.
        /// </summary>
        public static float Map(this float value, Vector2 from, Vector2 to) => 
            value.Map(from.x, from.y, to.x, to.y);
        
        /// <summary>
        /// Determines where a value lies between two points without clamping it.
        /// </summary>
        public static float InverseLerpUnclamped(float a, float b, float t) => (t - a) / (b - a);
    }
}