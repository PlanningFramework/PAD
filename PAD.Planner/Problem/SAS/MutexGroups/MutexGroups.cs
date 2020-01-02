using System.Collections.Generic;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Data structure for mutex (mutual-exclusion) groups in the SAS+ planning problem.
    /// </summary>
    public class MutexGroups : List<MutexGroup>
    {
        /// <summary>
        /// Constructs mutex groups definition from the input data.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        public MutexGroups(InputData.SAS.MutexGroups inputData)
        {
            inputData.ForEach(mutexGroup => Add(new MutexGroup(mutexGroup)));
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, this);
        }
    }
}
