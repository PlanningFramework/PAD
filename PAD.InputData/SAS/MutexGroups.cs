using System.Collections.Generic;

namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for SAS+ mutex groups.
    /// </summary>
    public class MutexGroups : List<MutexGroup>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines(Count, Utilities.JoinLines(this));
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(mutexGroup => mutexGroup.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single SAS+ mutex group (in the form of variable-value pairs).
    /// </summary>
    public class MutexGroup : List<Assignment>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines("begin_mutex_group", Count, Utilities.JoinLines(this), "end_mutex_group");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(assignment => assignment.Accept(visitor));
            visitor.Visit(this);
        }
    }
}
