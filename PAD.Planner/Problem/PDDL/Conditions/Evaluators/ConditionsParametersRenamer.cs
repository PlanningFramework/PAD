using System.Collections.Generic;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Renamer of the conditions parameters (renames occupied parameters with new parameter IDs).
    /// </summary>
    public class ConditionsParametersRenamer : BaseNumericExpressionVisitor, IConditionsCNFVisitor
    {
        /// <summary>
        /// Remapping of parameter IDs to the new IDs.
        /// </summary>
        private Dictionary<int, int> ParametersRemapping { get; } = new Dictionary<int, int>();

        /// <summary>
        /// Renames the parameters (and the corresponding occurences in the conditions), starting from the given free parameter ID.
        /// </summary>
        /// <param name="conditions">Conditions to be edited.</param>
        /// <param name="firstFreeParameterId">First free parameter ID.</param>
        public void Rename(ConditionsCNF conditions, int firstFreeParameterId)
        {
            // firstly, build a renaming map and rename the parameters

            ParametersRemapping.Clear();

            int currentParameterId = firstFreeParameterId;
            foreach (var parameter in conditions.Parameters)
            {
                ParametersRemapping.Add(parameter.ParameterNameId, currentParameterId);
                parameter.ParameterNameId = currentParameterId;
                ++currentParameterId;
            }

            // rename the conditions

            conditions.Accept(this);
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(PredicateLiteralCNF expression)
        {
            CheckAndRenameAtom(expression.PredicateAtom);
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(EqualsLiteralCNF expression)
        {
            CheckAndRenameTerm(expression.LeftArgument);
            CheckAndRenameTerm(expression.RightArgument);
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(NumericCompareLiteralCNF expression)
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
            CheckAndRenameAtom(expression.FunctionAtom);
        }

        /// <summary>
        /// Checks whether the atom terms need a rename of any parameter and performs the rename.
        /// </summary>
        /// <param name="atom">Predicate or function atom.</param>
        private void CheckAndRenameAtom(IAtom atom)
        {
            foreach (var term in atom.GetTerms())
            {
                CheckAndRenameTerm(term);
            }
        }

        /// <summary>
        /// Checks whether the term needs a rename and performs the rename.
        /// </summary>
        /// <param name="term">Predicate or function term.</param>
        private void CheckAndRenameTerm(ITerm term)
        {
            VariableTerm variableTerm = term as VariableTerm;
            if (variableTerm != null)
            {
                int newParameterId;
                if (ParametersRemapping.TryGetValue(variableTerm.NameId, out newParameterId))
                {
                    variableTerm.NameId = newParameterId;
                }
                return;
            }

            ObjectFunctionTerm objectFunctionTerm = term as ObjectFunctionTerm;
            if (objectFunctionTerm != null)
            {
                CheckAndRenameAtom(objectFunctionTerm.FunctionAtom);
            }
        }
    }
}
