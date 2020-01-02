﻿using System.Collections.Generic;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of SAS+ state labels in the relaxed planning graph, i.e. label values of variables calculated in the forward
    /// cost heuristic evaluation of the relaxed planning graph.
    /// </summary>
    public class StateLabels : Dictionary<IAssignment, double>, IStateLabels
    {
        /// <summary>
        /// Constructs empty state lables.
        /// </summary>
        public StateLabels()
        {
        }

        /// <summary>
        /// Constructs the initial state lables.
        /// </summary>
        /// <param name="initialState">Initial state of the original problem.</param>
        public StateLabels(RelaxedState initialState)
        {
            for (int i = 0; i < initialState.GetLength(); ++i)
            {
                Add(new Assignment(i, initialState.GetValue(i)), 0);
            }
        }

        /// <summary>
        /// Constructs state labels from other state labels.
        /// </summary>
        public StateLabels(StateLabels other) : base(other)
        {
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            StateLabels other = obj as StateLabels;
            if (other == null)
            {
                return false;
            }

            return CollectionsEquality.Equals(this, other);
        }
    }
}
