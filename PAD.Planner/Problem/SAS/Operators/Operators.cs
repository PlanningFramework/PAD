using System.Collections.Generic;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Data structure for SAS+ grounded operators.
    /// </summary>
    public class Operators : List<IOperator>
    {
        /// <summary>
        /// Constructs the list of grounded SAS+ operators from the input data.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        /// <param name="axiomRules">Axiom rules of the SAS+ planning problem.</param>
        /// <param name="mutexGroups">Mutex groups of the SAS+ planning problem.</param>
        public Operators(InputData.SAS.Operators inputData, AxiomRules axiomRules, MutexGroups mutexGroups)
        {
            int operIndex = 0;
            inputData.ForEach(oper => Add(new Operator(oper, operIndex++, axiomRules, mutexGroups)));
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
