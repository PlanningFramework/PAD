using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Collector of used predicates in the specified conditions.
    /// </summary>
    public class ConditionsUsedPredicatesCollector : IConditionsCNFVisitor
    {
        /// <summary>
        /// List of collected predicate atoms.
        /// </summary>
        public HashSet<IAtom> Atoms { set; get; }

        /// <summary>
        /// Gets a list of atoms used in the specified conditions.
        /// </summary>
        /// <param name="conditions">Conditions to be evaluated.</param>
        /// <returns>Collection of used atoms.</returns>
        public HashSet<IAtom> Collect(IConditions conditions)
        {
            ConditionsCNF conditionsCNF = (ConditionsCNF)conditions.GetCNF();

            Atoms = new HashSet<IAtom>();

            conditionsCNF.Accept(this);

            return Atoms;
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(PredicateLiteralCNF expression)
        {
            Atoms.Add(expression.PredicateAtom);
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(EqualsLiteralCNF expression)
        {
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public void Visit(NumericCompareLiteralCNF expression)
        {
        }
    }
}
