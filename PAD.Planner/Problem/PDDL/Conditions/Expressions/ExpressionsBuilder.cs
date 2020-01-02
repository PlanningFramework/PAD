using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Expressions builder converting input data expressions into PDDL logical expressions.
    /// </summary>
    public class ExpressionsBuilder : InputData.PDDL.BaseVisitor
    {
        /// <summary>
        /// Stack of expression parts.
        /// </summary>
        private Stack<IExpression> ExpressionStack { set; get; } = new Stack<IExpression>();

        /// <summary>
        /// Terms builder.
        /// </summary>
        private Lazy<TermsBuilder> TermsBuilder { set; get; } = null;

        /// <summary>
        /// ID manager converting predicate, function, constant and type names to their corresponding IDs.
        /// </summary>
        private IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Constructs the expressions builder.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        public ExpressionsBuilder(IDManager idManager)
        {
            IDManager = idManager;
            TermsBuilder = new Lazy<TermsBuilder>(() => new TermsBuilder(IDManager));
        }

        /// <summary>
        /// Builds PDDL logical expression from the input data.
        /// </summary>
        /// <param name="expression">Input data expression.</param>
        /// <returns>Built logical expression.</returns>
        public IExpression Build(InputData.PDDL.Expression expression)
        {
            Debug.Assert(ExpressionStack.Count == 0);
            ExpressionStack.Clear();

            expression.Accept(this);

            Debug.Assert(ExpressionStack.Count == 1);
            return ExpressionStack.Pop();
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.PreferenceExpression data)
        {
            var idManager = IDManager.Preferences;
            int preferenceNameID = idManager.IsRegistered(data.Name) ? idManager.GetID(data.Name) : idManager.Register(data.Name);
            ExpressionStack.Push(new PreferenceExpression(preferenceNameID, ExpressionStack.Pop(), IDManager));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.PredicateExpression data)
        {
            int predicateNameID = IDManager.Predicates.GetID(data.Name, data.Terms.Count);
            List<ITerm> terms = new List<ITerm>();

            data.Terms.ForEach(term => terms.Add(TermsBuilder.Value.Build(term)));

            ExpressionStack.Push(new PredicateExpression(new Atom(predicateNameID, terms), IDManager));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.EqualsExpression data)
        {
            ITerm firstArgument = TermsBuilder.Value.Build(data.Term1);
            ITerm secondArgument = TermsBuilder.Value.Build(data.Term2);

            ExpressionStack.Push(new EqualsExpression(firstArgument, secondArgument));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.AndExpression data)
        {
            List<IExpression> arguments = new List<IExpression>();
            for (int i = 0; i < data.Arguments.Count; ++i)
            {
                arguments.Add(ExpressionStack.Pop());
            }
            arguments.Reverse();
            ExpressionStack.Push(new AndExpression(arguments));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.OrExpression data)
        {
            List<IExpression> arguments = new List<IExpression>();
            for (int i = 0; i < data.Arguments.Count; ++i)
            {
                arguments.Add(ExpressionStack.Pop());
            }
            arguments.Reverse();
            ExpressionStack.Push(new OrExpression(arguments));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.NotExpression data)
        {
            ExpressionStack.Push(new NotExpression(ExpressionStack.Pop()));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.ImplyExpression data)
        {
            IExpression secondChild = ExpressionStack.Pop();
            IExpression firstChild = ExpressionStack.Pop();
            ExpressionStack.Push(new ImplyExpression(firstChild, secondChild));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InputData.PDDL.ForallExpression data)
        {
            IDManager.Variables.RegisterLocalParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.ForallExpression data)
        {
            ExpressionStack.Push(new ForallExpression(new Parameters(data.Parameters, IDManager), ExpressionStack.Pop()));
            IDManager.Variables.UnregisterLocalParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InputData.PDDL.ExistsExpression data)
        {
            IDManager.Variables.RegisterLocalParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.ExistsExpression data)
        {
            ExpressionStack.Push(new ExistsExpression(new Parameters(data.Parameters, IDManager), ExpressionStack.Pop()));
            IDManager.Variables.UnregisterLocalParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.NumericCompareExpression data)
        {
            NumericExpressionsBuilder numericExpressionsBuilder = new NumericExpressionsBuilder(IDManager);
            INumericExpression firstArgument = numericExpressionsBuilder.Build(data.NumericExpression1);
            INumericExpression secondArgument = numericExpressionsBuilder.Build(data.NumericExpression2);

            NumericCompareExpression.RelationalOperator relOperator = NumericCompareExpression.RelationalOperator.EQ;
            switch (data.NumericComparer)
            {
                case InputData.PDDL.Traits.NumericComparer.EQ:
                {
                    relOperator = NumericCompareExpression.RelationalOperator.EQ;
                    break;
                }
                case InputData.PDDL.Traits.NumericComparer.LT:
                {
                    relOperator = NumericCompareExpression.RelationalOperator.LT;
                    break;
                }
                case InputData.PDDL.Traits.NumericComparer.LTE:
                {
                    relOperator = NumericCompareExpression.RelationalOperator.LTE;
                    break;
                }
                case InputData.PDDL.Traits.NumericComparer.GT:
                {
                    relOperator = NumericCompareExpression.RelationalOperator.GT;
                    break;
                }
                case InputData.PDDL.Traits.NumericComparer.GTE:
                {
                    relOperator = NumericCompareExpression.RelationalOperator.GTE;
                    break;
                }
                default:
                {
                    Debug.Assert(false, "Unhandled operator!");
                    break;
                }
            }

            ExpressionStack.Push(new NumericCompareExpression(relOperator, firstArgument, secondArgument));
        }
    }
}
