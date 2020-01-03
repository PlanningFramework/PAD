using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Collector of used parameters/variables inside the conditions.
    /// </summary>
    public class ConditionsParametersCollector : BaseNumericExpressionVisitor, IExpressionVisitor
    {
        /// <summary>
        /// Collection of used parameters/variables.
        /// </summary>
        private HashSet<int> CollectedParameters { get; } = new HashSet<int>();

        /// <summary>
        /// Collects the parameters used in the specified conditions.
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Collection of used parameters.</returns>
        public HashSet<int> Collect(Conditions conditions)
        {
            CollectedParameters.Clear();

            foreach (var expression in conditions)
            {
                expression.Accept(this);
            }

            return CollectedParameters;
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(PreferenceExpression expression)
        {
            expression.Child.Accept(this);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(PredicateExpression expression)
        {
            ExtractParameters(expression.PredicateAtom);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(EqualsExpression expression)
        {
            ExtractParameters(expression.LeftArgument);
            ExtractParameters(expression.RightArgument);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(AndExpression expression)
        {
            foreach (var subExpression in expression.Children)
            {
                subExpression.Accept(this);
            }
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(OrExpression expression)
        {
            foreach (var subExpression in expression.Children)
            {
                subExpression.Accept(this);
            }
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(NotExpression expression)
        {
            expression.Child.Accept(this);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(ImplyExpression expression)
        {
            expression.LeftChild.Accept(this);
            expression.RightChild.Accept(this);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(ExistsExpression expression)
        {
            expression.Child.Accept(this);

            foreach (var parameter in expression.Parameters)
            {
                if (CollectedParameters.Contains(parameter.ParameterNameId))
                {
                    CollectedParameters.Remove(parameter.ParameterNameId);
                }
            }
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(ForallExpression expression)
        {
            expression.Child.Accept(this);

            foreach (var parameter in expression.Parameters)
            {
                if (CollectedParameters.Contains(parameter.ParameterNameId))
                {
                    CollectedParameters.Remove(parameter.ParameterNameId);
                }
            }
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(NumericCompareExpression expression)
        {
            expression.LeftArgument.Accept(this);
            expression.RightArgument.Accept(this);
        }

        /// <summary>
        /// Evaluates the numeric expression.
        /// </summary>
        /// <param name="expression">Numeric expression.</param>
        public override void Visit(NumericFunction expression)
        {
            ExtractParameters(expression.FunctionAtom);
        }

        /// <summary>
        /// Extracts the parameters from the given atom.
        /// </summary>
        /// <param name="atom">Atom.</param>
        private void ExtractParameters(IAtom atom)
        {
            foreach (var term in atom.GetTerms())
            {
                ExtractParameters(term);
            }
        }

        /// <summary>
        /// Extracts the parameters from the given term.
        /// </summary>
        /// <param name="term">Term.</param>
        private void ExtractParameters(ITerm term)
        {
            VariableTerm variableTerm = term as VariableTerm;
            if (variableTerm != null)
            {
                CollectedParameters.Add(variableTerm.NameId);
                return;
            }

            ObjectFunctionTerm objectFunctionTerm = term as ObjectFunctionTerm;
            if (objectFunctionTerm != null)
            {
                ExtractParameters(objectFunctionTerm.FunctionAtom);
            }
        }
    }
}
