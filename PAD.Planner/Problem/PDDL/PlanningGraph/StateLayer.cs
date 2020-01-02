using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of a PDDL state layer in the relaxed planning graph, i.e. a collection of propositions (in form of a relaxed state)
    /// and corresponding labels for these propositions, calculated during forward cost evaluation of the relaxed planning problem.
    /// </summary>
    public class StateLayer : IStateLayer
    {
        /// <summary>
        /// Relaxed PDDL state (collection of propositions).
        /// </summary>
        public IState State { set; get; } = null;

        /// <summary>
        /// Corresponding labels for the relaxed state propositions.
        /// </summary>
        public StateLabels Labels { set; get; } = null;

        /// <summary>
        /// Constructs the state layer from the initial state of the problem.
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="initLabels">Should the labels be inited?</param>
        public StateLayer(IState state, bool initLabels = true)
        {
            State = state;
            if (initLabels)
            {
                Labels = new StateLabels(State);
            }
        }

        /// <summary>
        /// Constructs the state layer from other state layer.
        /// </summary>
        /// <param name="other">Other state layer.</param>
        public StateLayer(StateLayer other)
        {
            State = (IState)other.State.Clone();
            Labels = new StateLabels(other.Labels);
        }

        /// <summary>
        /// Constructs the state layer from the predecessing state layer and action layer.
        /// </summary>
        /// <param name="sLayer">State layer.</param>
        /// <param name="aLayer">Action layer.</param>
        public StateLayer(StateLayer sLayer, ActionLayer aLayer) : this(sLayer)
        {
            foreach (var actionNode in aLayer)
            {
                var appliedOperator = actionNode.Operator;
                var operatorLabel = actionNode.Label;

                var newState = (IState)appliedOperator.Apply(State);

                foreach (var atom in newState.GetPredicates())
                {
                    if (sLayer.State.HasPredicate(atom))
                    {
                        // this atom is not a result of the operator application -> ignore
                        continue;
                    }

                    StoreLabel(atom, operatorLabel);
                    State.AddPredicate(atom);
                }
            }
        }

        /// <summary>
        /// Stores the atom and its label. If the atom is already present, sets the lesser label of the two.
        /// </summary>
        /// <param name="atom">Predicate atom.</param>
        /// <param name="label">Predicate label.</param>
        private void StoreLabel(IAtom atom, double label)
        {
            double value = -1;
            if (Labels.TryGetValue(atom, out value))
            {
                Labels[atom] = Math.Min(value, label);
            }
            else
            {
                Labels.Add(atom, label);
            }
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(State, Labels);
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

            StateLayer other = obj as StateLayer;
            if (other == null)
            {
                return false;
            }

            return State.Equals(other.State) && Labels.Equals(other.Labels);
        }

        /// <summary>
        /// Gets the corresponding state.
        /// </summary>
        /// <returns>Corresponding state.</returns>
        public Planner.IState GetState()
        {
            return State;
        }

        /// <summary>
        /// Gets the corresponding state labels.
        /// </summary>
        /// <returns>Corresponding state labels.</returns>
        public IStateLabels GetStateLabels()
        {
            return Labels;
        }

        /// <summary>
        /// Checks whether the state layer has the specified proposition.
        /// </summary>
        /// <param name="proposition">Proposition.</param>
        /// <returns>True if the state layer has the given proposition, false otherwise.</returns>
        public bool HasProposition(IProposition proposition)
        {
            return State.HasPredicate(((Proposition)proposition).Atom);
        }
    }
}
