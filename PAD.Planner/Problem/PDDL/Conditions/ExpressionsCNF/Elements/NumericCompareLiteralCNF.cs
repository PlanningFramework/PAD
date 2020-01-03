using System.Collections.Generic;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Numeric compare literal for conjunctive-normal-form expressions.
    /// </summary>
    public class NumericCompareLiteralCNF : LiteralCNF
    {
        /// <summary>
        /// Compare operator.
        /// </summary>
        public NumericCompareExpression.RelationalOperator Operator { set; get; }

        /// <summary>
        /// Left numeric expression.
        /// </summary>
        public INumericExpression LeftArgument { set; get; }

        /// <summary>
        /// Right numeric expression.
        /// </summary>
        public INumericExpression RightArgument { set; get; }

        /// <summary>
        /// Constructs the literal from the given expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <param name="isNegated">Is the literal negated?</param>
        public NumericCompareLiteralCNF(NumericCompareExpression expression, bool isNegated)
        {
            Operator = expression.Operator;
            LeftArgument = expression.LeftArgument;
            RightArgument = expression.RightArgument;
            IsNegated = isNegated;
        }

        /// <summary>
        /// Constructs the literal from the given expression.
        /// </summary>
        /// <param name="relOperator">Relational operator.</param>
        /// <param name="leftArgument">Left argument term.</param>
        /// <param name="rightArgument">Right argument term.</param>
        /// <param name="isNegated">Is the literal negated?</param>
        public NumericCompareLiteralCNF(NumericCompareExpression.RelationalOperator relOperator, INumericExpression leftArgument, INumericExpression rightArgument, bool isNegated)
        {
            Operator = relOperator;
            LeftArgument = leftArgument;
            RightArgument = rightArgument;
            IsNegated = isNegated;
        }

        /// <summary>
        /// Checks whether the literal is in the form of a simple numeric function assignment, e.g. (= (numFunc) 8.5),
        /// and in such case returns the pair of numeric function and its assigned value. Otherwise returns null.
        /// </summary>
        /// <returns>Numeric function assignment with its value, if the literal is in such form. Null otherwise.</returns>
        public Tuple<NumericFunction, Number> TryGetNumericFunctionAssignment()
        {
            if (Operator != NumericCompareExpression.RelationalOperator.EQ)
            {
                return null;
            }

            NumericFunction numFunc = LeftArgument as NumericFunction;
            Number number = RightArgument as Number;

            if (numFunc == null || number == null)
            {
                numFunc = RightArgument as NumericFunction;
                number = LeftArgument as Number;
            }

            return (numFunc == null || number == null) ? null : Tuple.Create(numFunc, number);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            string numCompString = $"({NumericCompareExpression.ToString(Operator)} {LeftArgument} {RightArgument})";
            return (IsNegated) ? $"(not {numCompString})" : numCompString;
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Operator, LeftArgument, RightArgument, IsNegated, "NumericCompareLiteralCNF");
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

            NumericCompareLiteralCNF other = obj as NumericCompareLiteralCNF;
            if (other == null)
            {
                return false;
            }

            return (Operator.Equals(other.Operator) &&
                    LeftArgument.Equals(other.LeftArgument) &&
                    RightArgument.Equals(other.RightArgument) &&
                    IsNegated == other.IsNegated);
        }

        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        public override IConjunctCNF Clone()
        {
            return new NumericCompareLiteralCNF(Operator, LeftArgument.Clone(), RightArgument.Clone(), IsNegated);
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
