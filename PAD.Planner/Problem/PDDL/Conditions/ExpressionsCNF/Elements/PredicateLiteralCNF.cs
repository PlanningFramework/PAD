using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Predicate literal for conjunctive-normal-form expressions.
    /// </summary>
    public class PredicateLiteralCNF : LiteralCNF
    {
        /// <summary>
        /// Predicate atom.
        /// </summary>
        public IAtom PredicateAtom { set; get; }

        /// <summary>
        /// ID manager of the corresponding planning problem.
        /// </summary>
        private IdManager IdManager { get; }

        /// <summary>
        /// Constructs the literal from the given expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <param name="isNegated">Is the literal negated?</param>
        public PredicateLiteralCNF(PredicateExpression expression, bool isNegated)
        {
            PredicateAtom = expression.PredicateAtom;
            IsNegated = isNegated;
            IdManager = expression.IdManager;
        }

        /// <summary>
        /// Constructs the literal from the given expression.
        /// </summary>
        /// <param name="predicateAtom">Source predicate expression atom.</param>
        /// <param name="isNegated">Is the literal negated?</param>
        /// <param name="idManager">ID manager.</param>
        public PredicateLiteralCNF(IAtom predicateAtom, bool isNegated, IdManager idManager)
        {
            PredicateAtom = predicateAtom;
            IsNegated = isNegated;
            IdManager = idManager;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            string predicateString = PredicateAtom.ToString(IdManager.Predicates);
            return (IsNegated) ? $"(not {predicateString})" : predicateString;
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(PredicateAtom, IsNegated, "PredicateLiteralCNF");
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

            PredicateLiteralCNF other = obj as PredicateLiteralCNF;
            if (other == null)
            {
                return false;
            }

            return (PredicateAtom.Equals(other.PredicateAtom) && IsNegated == other.IsNegated);
        }

        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        public override IConjunctCNF Clone()
        {
            return new PredicateLiteralCNF(PredicateAtom.Clone(), IsNegated, IdManager);
        }

        /// <summary>
        /// Gets the concrete literals of the conjunct.
        /// </summary>
        /// <returns>Literals.</returns>
        public override IEnumerable<LiteralCNF> GetLiterals()
        {
            yield return this;
        }

        /// <summary>
        /// Accepts a conjunctive-normal-form expression relevance evaluation visitor.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        public override Tuple<bool, bool> Accept(IElementCNFRelevanceEvaluationVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a conjunctive-normal-form expression backwards applier visitor.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        public override IElementCNF Accept(IElementCNFBackwardsApplierVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a conjunctive-normal-form expression evaluator.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        public override bool Accept(IConditionsCNFEvalVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a visitor counting some specific property of the conjunctive-normal-form expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        public override Tuple<int, int> Accept(IConditionsCNFPropCountVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a visitor counting some specific property of the conjunctive-normal-form expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        public override Tuple<double, double> Accept(IConditionsCNFPropEvalVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a generic visitor od the conjunctive-normal-form expressions.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IConditionsCNFVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
