using System.Collections.Generic;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Data structure for variables definition in the SAS+ planning problem.
    /// </summary>
    public class Variables : List<Variable>
    {
        /// <summary>
        /// Constructs variables definition from the input data.
        /// </summary>
        /// <param name="inputData">Input data.</param>
        public Variables(InputData.SAS.Variables inputData)
        {
            int variableID = 0;
            inputData.ForEach(variable => Add(new Variable(variable, variableID++)));
        }

        /// <summary>
        /// Sets the reference to the planning problem operators.
        /// </summary>
        /// <param name="operators">Operators of the planning problem.</param>
        public void SetOperators(Operators operators)
        {
            ForEach(variable => variable.SetOperators(operators));
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
