using System.Collections.Generic;

namespace PAD.Planner
{
    /// <summary>
    /// Auxiliary class for the generation of quality hash codes, with convenient methods that allow combining of multiple hash codes.
    /// Source: http://stackoverflow.com/.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Creates a hash code for the specified parameters.
        /// </summary>
        /// <typeparam name="T1">First type.</typeparam>
        /// <typeparam name="T2">Second type.</typeparam>
        /// <param name="arg1">First argument.</param>
        /// <param name="arg2">Second argument.</param>
        /// <returns>Hash code.</returns>
        public static int GetHashCode<T1, T2>(T1 arg1, T2 arg2)
        {
            unchecked
            {
                return 31 * arg1.GetHashCode() + arg2.GetHashCode();
            }
        }

        /// <summary>
        /// Creates a hash code for the specified parameters.
        /// </summary>
        /// <typeparam name="T1">First type.</typeparam>
        /// <typeparam name="T2">Second type.</typeparam>
        /// <typeparam name="T3">Third type.</typeparam>
        /// <param name="arg1">First argument.</param>
        /// <param name="arg2">Second argument.</param>
        /// <param name="arg3">Third argument.</param>
        /// <returns>Hash code.</returns>
        public static int GetHashCode<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            unchecked
            {
                int hash = arg1.GetHashCode();
                hash = 31 * hash + arg2.GetHashCode();
                return 31 * hash + arg3.GetHashCode();
            }
        }

        /// <summary>
        /// Creates a hash code for the specified parameters.
        /// </summary>
        /// <typeparam name="T1">First type.</typeparam>
        /// <typeparam name="T2">Second type.</typeparam>
        /// <typeparam name="T3">Third type.</typeparam>
        /// <typeparam name="T4">Fourth type.</typeparam>
        /// <param name="arg1">First argument.</param>
        /// <param name="arg2">Second argument.</param>
        /// <param name="arg3">Third argument.</param>
        /// <param name="arg4">Fourth argument.</param>
        /// <returns>Hash code.</returns>
        public static int GetHashCode<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            unchecked
            {
                int hash = arg1.GetHashCode();
                hash = 31 * hash + arg2.GetHashCode();
                hash = 31 * hash + arg3.GetHashCode();
                return 31 * hash + arg4.GetHashCode();
            }
        }

        /// <summary>
        /// Creates a hash code for the specified parameters.
        /// </summary>
        /// <typeparam name="T1">First type.</typeparam>
        /// <typeparam name="T2">Second type.</typeparam>
        /// <typeparam name="T3">Third type.</typeparam>
        /// <typeparam name="T4">Fourth type.</typeparam>
        /// <typeparam name="T5">Fifth type.</typeparam>
        /// <param name="arg1">First argument.</param>
        /// <param name="arg2">Second argument.</param>
        /// <param name="arg3">Third argument.</param>
        /// <param name="arg4">Fourth argument.</param>
        /// <param name="arg5">Fifth argument.</param>
        /// <returns>Hash code.</returns>
        public static int GetHashCode<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            unchecked
            {
                int hash = arg1.GetHashCode();
                hash = 31 * hash + arg2.GetHashCode();
                hash = 31 * hash + arg3.GetHashCode();
                hash = 31 * hash + arg4.GetHashCode();
                return 31 * hash + arg5.GetHashCode();
            }
        }

        /// <summary>
        /// Creates a hash code for the specified parameters.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="array">Array of items.</param>
        /// <returns>Hash code.</returns>
        public static int GetHashCode<T>(T[] array)
        {
            unchecked
            {
                if (array == null)
                {
                    return 0;
                }

                int hash = 17;
                foreach (var item in array)
                {
                    hash = 31 * hash + item.GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        /// Creates a hash code for the specified parameters.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="array">Array of lists.</param>
        /// <returns>Hash code.</returns>
        public static int GetHashCode<T>(List<T>[] array)
        {
            unchecked
            {
                if (array == null)
                {
                    return 0;
                }

                int hash = 17;
                foreach (List<T> element in array)
                {
                    int hash2 = 51;
                    foreach (var item in element)
                    {
                        hash2 = 41 * hash2 + item.GetHashCode();
                    }
                    hash = 31* hash + hash2;
                }
                return hash;
            }
        }

        /// <summary>
        /// Creates a hash code for the specified parameters.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="list">List of items.</param>
        /// <returns>Hash code.</returns>
        public static int GetHashCode<T>(IEnumerable<T> list)
        {
            unchecked
            {
                if (list == null)
                {
                    return 0;
                }

                int hash = 0;
                foreach (var item in list)
                {
                    hash = 31 * hash + item.GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        /// Creates a hash code for the specified parameters. Order of items in the collection does not matter (e.g. {1, 2, 3} is the same as {3, 2, 1}).
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="list">List of items.</param>
        /// <returns>Hash code.</returns>
        public static int GetHashCodeForOrderNoMatterCollection<T>(IEnumerable<T> list)
        {
            unchecked
            {
                if (list == null)
                {
                    return 0;
                }

                int hash = 0;
                int count = 0;
                foreach (var item in list)
                {
                    hash += item.GetHashCode();
                    count++;
                }
                return 31 * hash + count.GetHashCode();
            }
        }

        /// <summary>
        /// Creates a hash code for the specified parameters. Offers a different way to combine hash codes, e.g. return 0.CombineHashCode(field1).CombineHashCode(field2);
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="hashCode">Input hash code.</param>
        /// <param name="arg">Argument item.</param>
        /// <returns>Hash code.</returns>
        public static int CombineHashCode<T>(this int hashCode, T arg)
        {
            unchecked
            {
                return 31 * hashCode + arg.GetHashCode();
            }
        }
    }
}
