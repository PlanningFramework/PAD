using System.Collections.Generic;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Data structure for axiom rules in the SAS+ planning problem, in the form of an ordered list of axiom layers (with the actual axiom rules).
    /// </summary>
    public class AxiomRules : List<AxiomLayer>
    {
        /// <summary>
        /// List of initial values for axiomatic variables.
        /// </summary>
        private List<IAssignment> InitialValues { get; } = new List<IAssignment>();

        /// <summary>
        /// Constructs axiom rules from the input data.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="variables">Variables data of the planning problem.</param>
        /// <param name="initialState">Initial state of the planning problem.</param>
        public AxiomRules(InputData.SAS.AxiomRules inputData, Variables variables, IState initialState)
        {
            // rearrange axiom rules by their actual axiom layers
            foreach (var inputAxiomRule in inputData)
            {
                AxiomRule axiomRule = new AxiomRule(inputAxiomRule);

                int affectedVariable = axiomRule.Assignment.GetVariable();
                int axiomLayerNo = variables[affectedVariable].AxiomLayer;

                while (axiomLayerNo >= Count)
                {
                    Add(new AxiomLayer());
                }

                this[axiomLayerNo].Add(axiomRule);
            }

            SetInitialValues(variables, initialState);
        }

        /// <summary>
        /// Stores the initial values of axiomatic variables from the initial state.
        /// </summary>
        /// <param name="variables">Variables data of the planning problem.</param>
        /// <param name="initialState">Initial state of the planning problem.</param>
        public void SetInitialValues(Variables variables, IState initialState)
        {
            if (Count == 0)
            {
                return;
            }

            InitialValues.Clear();

            foreach (var variable in variables)
            {
                if (variable.IsAxiomatic())
                {
                    InitialValues.Add(new Assignment(variable.Id, initialState.GetValue(variable.Id)));
                }
            }
        }

        /// <summary>
        /// Applies all the axiom rules of the SAS+ planning problem to the given state, by directly modifying it.
        /// </summary>
        /// <param name="state">State to be modified.</param>
        public void Apply(IState state)
        {
            // if no axiom rules exist, do nothing
            if (Count == 0)
            {
                return;
            }

            // firstly, set initial values for axiomatic variables
            foreach (var initialValue in InitialValues)
            {
                state.SetValue(initialValue);
            }

            // then we evaluate rules on each axiom layer
            foreach (var axiomLayer in this)
            {
                axiomLayer.Apply(state);
            }
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
