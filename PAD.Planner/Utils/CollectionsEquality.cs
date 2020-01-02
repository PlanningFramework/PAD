using System.Collections.Generic;
using System.Linq;

namespace PAD.Planner
{
    /// <summary>
    /// Auxiliary class for evaluating of collections equality.
    /// </summary>
    public static class CollectionsEquality
    {
        /// <summary>
        /// Checks whether two given sets are equal.
        /// </summary>
        /// <typeparam name="TKey">Set key type.</typeparam>
        /// <param name="set1">First set.</param>
        /// <param name="set2">Second set.</param>
        /// <returns>True, if the sets are equal, false otherwise.</returns>
        public static bool Equals<TKey>(ISet<TKey> set1, ISet<TKey> set2)
        {
            if (set1 == set2)
            {
                return true;
            }
            else if (set1 == null || set2 == null)
            {
                return false;
            }

            return set1.SetEquals(set2);
        }

        /// <summary>
        /// Checks whether two given dictionaries are equal.
        /// </summary>
        /// <typeparam name="TKey">Dictionary key type.</typeparam>
        /// <typeparam name="TValue">Dictionary value type.</typeparam>
        /// <param name="dictionary1">First dictionary.</param>
        /// <param name="dictionary2">Second dictionary.</param>
        /// <returns>True, if the dictionaries are equal, false otherwise.</returns>
        public static bool Equals<TKey, TValue>(IDictionary<TKey, TValue> dictionary1, IDictionary<TKey, TValue> dictionary2)
        {
            if (dictionary1 == dictionary2)
            {
                return true;
            }
            else if (dictionary1 == null || dictionary2 == null)
            {
                return false;
            }

            return (dictionary1.Keys.Count == dictionary2.Keys.Count && dictionary1.Keys.All(k => dictionary2.ContainsKey(k) && object.Equals(dictionary2[k], dictionary1[k])));
        }

        /// <summary>
        /// Checks whether two given sequences are equal.
        /// </summary>
        /// <typeparam name="TKey">Enumerable key type.</typeparam>
        /// <param name="sequence1">First sequence.</param>
        /// <param name="sequence2">Second sequence.</param>
        /// <returns>True, if the sequences are equal, false otherwise.</returns>
        public static bool Equals<TKey>(IEnumerable<TKey> sequence1, IEnumerable<TKey> sequence2)
        {
            if (sequence1 == sequence2)
            {
                return true;
            }
            else if (sequence1 == null || sequence2 == null)
            {
                return false;
            }

            return sequence1.SequenceEqual(sequence2);
        }

        /// <summary>
        /// Checks whether two given integer arrays are equal.
        /// </summary>
        /// <param name="array1">First array.</param>
        /// <param name="array2">Second array.</param>
        /// <returns>True, if the arrays are equal, false otherwise.</returns>
        public static bool Equals(int[] array1, int[] array2)
        {
            if (array1 == array2)
            {
                return true;
            }
            else if (array1 == null || array2 == null)
            {
                return false;
            }
            else if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether two given arrays of integer lists are equal.
        /// </summary>
        /// <param name="array1">First array.</param>
        /// <param name="array2">Second array.</param>
        /// <returns>True, if the arrays are equal, false otherwise.</returns>
        public static bool Equals(List<int>[] array1, List<int>[] array2)
        {
            if (array1 == array2)
            {
                return true;
            }
            else if (array1 == null || array2 == null)
            {
                return false;
            }
            else if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; ++i)
            {
                if (array1[i].Count != array2[i].Count)
                {
                    return false;
                }

                for (int j = 0; j < array1[i].Count; ++j)
                {
                    if (array1[i][j] != array2[i][j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Checks whether two given sequences of lists are equal.
        /// </summary>
        /// <typeparam name="TKey">List key type.</typeparam>
        /// <param name="sequence1">First sequence.</param>
        /// <param name="sequence2">Second sequence.</param>
        /// <returns>True, if the sequences are equal, false otherwise.</returns>
        public static bool Equals<TKey>(List<List<TKey>> sequence1, List<List<TKey>> sequence2)
        {
            if (sequence1 == sequence2)
            {
                return true;
            }
            else if (sequence1 == null || sequence2 == null)
            {
                return false;
            }
            else if (sequence1.Count != sequence2.Count)
            {
                return false;
            }

            for (int i = 0; i < sequence1.Count; ++i)
            {
                if (!Equals(sequence1[i], sequence2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Auxiliary comparer (base) class defining a comparison of two integer arrays.
    /// </summary>
    public abstract class ArrayEqualityComp : IEqualityComparer<int[]>
    {
        /// <summary>
        /// Checks whether two given arrays are equal.
        /// </summary>
        /// <param name="first">First array.</param>
        /// <param name="second">Second array.</param>
        /// <returns>True, if the arrays are equal, false otherwise.</returns>
        public bool Equals(int[] first, int[] second)
        {
            if (first == second)
            {
                return true;
            }
            else if (first == null || second == null)
            {
                return false;
            }
            else if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the hash code for an array.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <returns>Hash code.</returns>
        public abstract int GetHashCode(int[] array);
    }

    /// <summary>
    /// Auxiliary comparer class defining a comparison of two integer arrays.
    /// </summary>
    public sealed class ArrayEqualityComparer : ArrayEqualityComp
    {
        /// <summary>
        /// Gets the hash code for an array.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <returns>Hash code.</returns>
        public override int GetHashCode(int[] array)
        {
            unchecked
            {
                if (array == null)
                {
                    return 0;
                }

                int hash = 17;
                foreach (int element in array)
                {
                    hash = hash * 31 + element;
                }
                return hash;
            }
        }
    }

    /// <summary>
    /// Auxiliary comparer class defining a comparison of two integer arrays.
    /// </summary>
    public sealed class ArrayEqualityComparer1 : ArrayEqualityComp
    {
        /// <summary>
        /// Gets the hash code for an array.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <returns>Hash code.</returns>
        public override int GetHashCode(int[] array)
        {
            unchecked
            {
                if (array == null)
                {
                    return 0;
                }

                int result = 0;
                int shift = 0;

                for (int i = 0; i < array.Length; ++i)
                {
                    shift = (shift + 11) % 21;
                    result ^= (array[i] + 1024) << shift;
                }
                return result;
            }
        }
    }

    /// <summary>
    /// Auxiliary comparer class defining a comparison of two integer arrays.
    /// </summary>
    public sealed class ArrayEqualityComparer2 : ArrayEqualityComp
    {
        /// <summary>
        /// Gets the hash code for an array.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <returns>Hash code.</returns>
        public override int GetHashCode(int[] array)
        {
            unchecked
            {
                if (array == null)
                {
                    return 0;
                }

                int result = array.Length;
                for (int i = 0; i < array.Length; ++i)
                {
                    result += array[i];
                }
                return result;
            }
        }
    }
}
