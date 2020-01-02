using System.Collections.Generic;
using System.Diagnostics;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Builder class for a construction of simplified CNF expressions. In CNF (conjuctive-normal-form), the expression needs to be strictly in a form of a conjuntion
    /// of clauses (which are a disjunction of literals, or literals itselves). The given expression is firstly converted into CNF via ExpressionToCNFTransformer but
    /// remains in the original data structure (IExpression). Then the expression is transformed into a separate, more compact, data structure (ConditionsCNF). CNF
    /// expression is a convenient form for further processing (e.g. evaluating conditions against operator effects to determine relevant operators).
    /// </summary>
    public class ConditionsCNFBuilder : BaseExpressionVisitor
    {
        /// <summary>
        /// Expression CNF transformer (while the result is still an IExpression, not ConditionsCNF).
        /// </summary>
        private ExpressionToCNFTransformer ExpressionToCNFTransformer { set; get; } = null;

        /// <summary>
        /// Evaluation manager.
        /// </summary>
        private EvaluationManager EvaluationManager { set; get; } = null;

        /// <summary>
        /// Stack of CNF expression parts.
        /// </summary>
        private Stack<IConjunctCNF> Stack { set; get; } = new Stack<IConjunctCNF>();

        /// <summary>
        /// Is the currently processed subexpression negated?
        /// </summary>
        private bool IsNegated { set; get; } = false;

        /// <summary>
        /// Creates the expression CNF builder.
        /// </summary>
        /// <param name="evaluationManager">Evaluation manager.</param>
        public ConditionsCNFBuilder(EvaluationManager evaluationManager)
        {
            ExpressionToCNFTransformer = new ExpressionToCNFTransformer(evaluationManager);
            EvaluationManager = evaluationManager;
        }

        /// <summary>
        /// Builds CNF conditions form from the given conditions.
        /// </summary>
        /// <param name="conditions">Source conditions.</param>
        /// <returns>Expression in CNF.</returns>
        public ConditionsCNF Build(Conditions conditions)
        {
            IExpression expression = conditions.GetWrappedConditions();

            if (expression == null)
            {
                return null;
            }

            IExpression transformedExpression = ExpressionToCNFTransformer.Transform(expression);

            Debug.Assert(Stack.Count == 0);
            Stack.Clear();

            transformedExpression.Accept(this);

            ConditionsCNF conditionsCNF = new ConditionsCNF(EvaluationManager, conditions.Parameters);
            while (Stack.Count != 0)
            {
                conditionsCNF.Add(Stack.Pop());
            }
            return conditionsCNF;
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public override void Visit(AndExpression expression)
        {
            expression.Children.ForEach(child => child.Accept(this));
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public override void Visit(OrExpression expression)
        {
            expression.Children.ForEach(child => child.Accept(this));

            ClauseCNF clause = new ClauseCNF();
            for (int i = 0; i < expression.Children.Count; ++i)
            {
                LiteralCNF literal = Stack.Pop() as LiteralCNF;
                if (literal != null)
                {
                    clause.Add(literal);
                }
                else
                {
                    Debug.Assert(false, "Source expression not in CNF!");
                }
            }
            Stack.Push(clause);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public override void Visit(NotExpression expression)
        {
            IsNegated = true;
            expression.Child.Accept(this);
            IsNegated = false;
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public override void Visit(PredicateExpression expression)
        {
            Stack.Push(new PredicateLiteralCNF(expression, IsNegated));
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public override void Visit(EqualsExpression expression)
        {
            Stack.Push(new EqualsLiteralCNF(expression, IsNegated));
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">Expression.</param>
        public override void Visit(NumericCompareExpression expression)
        {
            Stack.Push(new NumericCompareLiteralCNF(expression, IsNegated));
        }
    }
}
