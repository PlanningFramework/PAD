using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of a SAS+ state layer in the relaxed planning graph, i.e. a collection of variable values (in form of a relaxed state)
    /// and corresponding labels for these values, calculated during forward cost evaluation of the relaxed planning problem.
    /// </summary>
    public class StateLayer : IStateLayer
    {
        /// <summary>
        /// Relaxed SAS+ state.
        /// </summary>
        public IState State { set; get; } = null;

        /// <summary>
        /// Corresponding labels for the relaxed state variables.
        /// </summary>
        public StateLabels Labels { set; get; } = null;

        /// <summary>
        /// Constructs the state layer from the initial state of the problem.
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="initLabels">Should the labels be inited?</param>
        public StateLayer(RelaxedState state, bool initLabels = true)
        {
            State = state;
            if (initLabels)
            {
                Labels = new StateLabels(state);
            }
        }

        /// <summary>
        /// Constructs the state layer from other state layer.
        /// </summary>
        /// <param name="other">Other state layer.</param>
        public StateLayer(StateLayer other)
        {
            State = (RelaxedState)other.State.Clone();
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
                var appliedOperator = (Operator)actionNode.Operator;
                var operatorLabel = actionNode.Label;

                foreach (var effect in appliedOperator.Effects)
                {
                    if (effect.IsApplicable(sLayer.State))
                    {
                        var assignment = effect.GetAssignment();
                        StoreLabel(assignment, operatorLabel);
                    }
                }

                State = (RelaxedState)appliedOperator.Apply(State);
            }
        }

        /// <summary>
        /// Stores the effect assignment and its label. If the assignment is already present, sets the lesser label of the two.
        /// </summary>
        /// <param name="assignment">Effect assignment.</param>
        /// <param name="label">Predicate label.</param>
        private void StoreLabel(IAssignment assignment, double label)
        {
            double value = -1;
            if (Labels.TryGetValue(assignment, out value))
            {
                Labels[assignment] = Math.Min(value, label);
            }
            else
            {
                Labels.Add(assignment, label);
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
            return State.HasValue(((Proposition)proposition).Assignment);
        }
    }
}
