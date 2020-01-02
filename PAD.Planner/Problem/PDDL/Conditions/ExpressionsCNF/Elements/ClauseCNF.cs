using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Clause (disjunction of literals) for conjunctive-normal-form expressions.
    /// </summary>
    public class ClauseCNF : HashSet<LiteralCNF>, IConjunctCNF
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(or {string.Join(" ", this)})";
        }

        /// <summary>
        /// Constructs an empty clause.
        /// </summary>
        public ClauseCNF()
        {
        }

        /// <summary>
        /// Constructs the clause from given literals.
        /// </summary>
        /// <param name="literals">Literals.</param>
        public ClauseCNF(HashSet<LiteralCNF> literals) : base(literals)
        {
        }

        /// <summary>
        /// Constructs the clause from given literals.
        /// </summary>
        /// <param name="literals">Literals.</param>
        public ClauseCNF(params LiteralCNF[] literals) : base(new HashSet<LiteralCNF>(literals))
        {
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCodeForOrderNoMatterCollection(this).CombineHashCode("ClauseCNF");
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

            ClauseCNF other = obj as ClauseCNF;
            if (other == null)
            {
                return false;
            }

            return (CollectionsEquality.Equals(this, other));
        }

        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        public IConjunctCNF Clone()
        {
            var newLiterals = new HashSet<LiteralCNF>();
            foreach (var literal in this)
            {
                newLiterals.Add((LiteralCNF)literal.Clone());
            }
            return new ClauseCNF(newLiterals);
        }

        /// <summary>
        /// Gets the concrete literals of the conjunct.
        /// </summary>
        /// <returns>Literals.</returns>
        public IEnumerable<LiteralCNF> GetLiterals()
        {
            return this;
        }

        /// <summary>
        /// Accepts a conjunctive-normal-form expression relevance evaluation visitor.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        public Tuple<bool, bool> Accept(IElementCNFRelevanceEvaluationVisitor visitor)
        {
            bool positiveCondition = false;
            bool negativeCondition = false;

            foreach (var literal in this)
            {
                var result = literal.Accept(visitor);
                positiveCondition |= result.Item1;
                negativeCondition |= result.Item2;

                if (positiveCondition && negativeCondition)
                {
                    break;
                }

            }
            return Tuple.Create(positiveCondition, negativeCondition);
        }

        /// <summary>
        /// Accepts a conjunctive-normal-form expression backwards applier visitor.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        public IElementCNF Accept(IElementCNFBackwardsApplierVisitor visitor)
        {
            ClauseCNF newExpression = new ClauseCNF();
            foreach (var literal in this)
            {
                var resultExpression = literal.Accept(visitor);
                if (resultExpression == null)
                {
                    // any of element from the clause holds -> the whole clause holds
                    return null;
                }
                else
                {
                    newExpression.Add((LiteralCNF)resultExpression);
                }
            }
            return newExpression;
        }

        /// <summary>
        /// Accepts a conjunctive-normal-form expression evaluator.
        /// </summary>
        /// <param name="visitor">CNF expression visitor.</param>
        /// <returns>True if the expression is logically true, false otherwise.</returns>
        public bool Accept(IConditionsCNFEvalVisitor visitor)
        {
            foreach (var literal in this)
            {
                if (literal.Accept(visitor))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Accepts a visitor counting some specific property of the conjunctive-normal-form expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        public Tuple<int, int> Accept(IConditionsCNFPropCountVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a visitor counting some specific property of the conjunctive-normal-form expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        public Tuple<double, double> Accept(IConditionsCNFPropEvalVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a generic visitor od the conjunctive-normal-form expressions.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IConditionsCNFVisitor visitor)
        {
            foreach (var literal in this)
            {
                literal.Accept(visitor);
            }
        }
    }
}
