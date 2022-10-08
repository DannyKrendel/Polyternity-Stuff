using System;

namespace PolyternityStuff.Utils
{
    public static class EnumUtils<T> where T : Enum
    {
        /// <summary>
        /// Returns a random element from the enum.
        /// </summary>
        public static T GetRandom()
        {
            var values = Enum.GetValues(typeof(T));
            var index = UnityEngine.Random.Range(0, values.Length);
            return (T) values.GetValue(index);
        }

        /// <summary>
        /// Returns a number of names in the enum.
        /// </summary>
        public static int Count() => Enum.GetNames(typeof(T)).Length;
    }
}