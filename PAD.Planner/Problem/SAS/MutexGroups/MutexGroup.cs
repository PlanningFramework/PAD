using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Data structure for a single SAS+ mutex group, in the form of variable-value assignments.
    /// </summary>
    public class MutexGroup : List<IAssignment>
    {
        /// <summary>
        /// Constructs a SAS+ mutex group.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        public MutexGroup(InputData.SAS.MutexGroup inputData)
        {
            inputData.ForEach(assignment => Add(new Assignment(assignment)));
        }

        /// <summary>
        /// Tries to find affected mutex item in the mutex group and returns its index, if it exists.
        /// </summary>
        /// <param name="assignment">Value assignment.</param>
        /// <param name="affectedItemIndex">Index of the affected item in the mutex group.</param>
        /// <returns>True if any mutex item affected by the assignment. False otherwise.</returns>
        public bool TryFindAffectedMutexItem(IAssignment assignment, out int affectedItemIndex)
        {
            for (int itemIndex = 0; itemIndex < Count; ++itemIndex)
            {
                if (this[itemIndex].Equals(assignment))
                {
                    affectedItemIndex = itemIndex;
                    return true;
                }
            }
            affectedItemIndex = -1;
            return false;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"({string.Join(", ", this)})";
        }
    }
}
