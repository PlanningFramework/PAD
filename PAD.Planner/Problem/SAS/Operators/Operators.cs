using System.Collections.Generic;
using System;
// ReSharper disable IdentifierTypo

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
            int operatorIndex = 0;
            inputData.ForEach(oper => Add(new Operator(oper, operatorIndex++, axiomRules, mutexGroups)));
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
