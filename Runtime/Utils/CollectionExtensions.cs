using System.Collections.Generic;
using UnityEngine;

namespace PolyternityStuff.Utils
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns a random element from the collection.
        /// </summary>
        public static T GetRandom<T>(this IList<T> items) => items[Random.Range(0, items.Count)];
    
        /// <summary>
        /// Returns a number of unique items from the collection.
        /// </summary>
        public static T[] GetUnique<T>(this IList<T> items, int count)
        {
            var result = new T[count];
            
            var numToChoose = count;
    
            for (int numLeft = items.Count; numLeft > 0; numLeft--)
            {
                var prob = (float) numToChoose / numLeft;
    
                if (Random.value <= prob) {
                    numToChoose--;
                    result[numToChoose] = items[numLeft - 1];
    
                    if (numToChoose == 0) {
                        break;
                    }
                }
            }
    
            result.Shuffle();
            
            return result;
        }
        
        /// <summary>
        /// Shuffles the collection.
        /// </summary>
        public static void Shuffle<T> (this IList<T> items)
        {
            var n = items.Count;
            while (n > 1) 
            {
                var k = Random.Range(0, n--);
                (items[n], items[k]) = (items[k], items[n]);
            }
        }
        
        /// <summary>
        /// Returns a string with all the elements separated by the given string.
        /// </summary>
        public static string ToPrettyString<T>(this IList<T> list, string separator)
        {
            if (list == null) return string.Empty;
            
            var builder = new System.Text.StringBuilder();
            
            for (var i = 0; i < list.Count - 1; i++)
            {
                builder.Append(list[i]);
                builder.Append(separator);
            }
    
            builder.Append(list[list.Count - 1]);
            return builder.ToString();
        }
    }
}
