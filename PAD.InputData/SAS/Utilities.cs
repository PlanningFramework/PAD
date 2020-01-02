using System.Collections.Generic;
using System;

namespace PAD.InputData.SAS
{
    /// <summary>
    /// Utility functions for SAS+ input data structures.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Joins a list of lines into a single string. The lines are separated by a new-line character.
        /// </summary>
        /// <param name="lines">Lines to be joined.</param>
        /// <returns>Joined list of lines.</returns>
        public static string JoinLines(params object[] lines)
        {
            List<string> linesList = new List<string>();
            foreach (var line in lines)
            {
                linesList.Add(line.ToString());
            }

            return JoinLines(linesList);
        }

        /// <summary>
        /// Joins a list of lines into a single string. The lines are separated by a new-line character.
        /// </summary>
        /// <typeparam name="T">Type of items in the list.</typeparam>
        /// <param name="lines">Lines to be joined.</param>
        /// <returns>Joined list of lines.</returns>
        public static string JoinLines<T>(IEnumerable<T> lines)
        {
            List<string> linesList = new List<string>();
            foreach (var line in lines)
            {
                linesList.Add(line.ToString());
            }

            return JoinLines(linesList);
        }

        /// <summary>
        /// Joins a list of lines into a single string. The lines are separated by a new-line character.
        /// </summary>
        /// <param name="lines">Lines to be joined.</param>
        /// <returns>Joined list of lines.</returns>
        private static string JoinLines(List<string> lines)
        {
            lines.RemoveAll(x => string.IsNullOrEmpty(x));
            return string.Join(Environment.NewLine, lines);
        }
    }
}
