using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Collection of pattern values mapped to the corresponding computed distances in the abstracted planning problem.
    /// </summary>
    public class PatternValuesDistances : Dictionary<int[], double>
    {
        /// <summary>
        /// Maximal distance constant. Used when the database does not contain a computed distance for the specified pattern values
        /// (i.e. the evaluated state probably does not lead to goals, according to the chosen abstraction).
        /// </summary>
        public const int MAX_DISTANCE = int.MaxValue;

        /// <summary>
        /// Constructs an empty collection.
        /// </summary>
        public PatternValuesDistances() : base(new ArrayEqualityComparer())
        {
        }

        /// <summary>
        /// Gets the distance for the specified pattern values.
        /// </summary>
        /// <param name="values">Pattern values.</param>
        /// <returns>Distance to goals for the given pattern values.</returns>
        public double GetDistance(int[] values)
        {
            double distance = 0;

            if (!TryGetValue(values, out distance))
            {
                // database does not contain a computed distance for the given pattern values (i.e. probably not leading to goals)
                return MAX_DISTANCE;
            }

            return distance;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            List<string> itemList = new List<string>();
            foreach (var item in this)
            {
                itemList.Add($"{{[{string.Join(", ", item.Key)}] = {item.Value}}}");
            }
            return string.Join(", ", itemList);
        }
    }
}
