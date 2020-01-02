using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Numeric expressions builder converting input data numeric expressions into PDDL numeric expressions.
    /// </summary>
    public class NumericExpressionsBuilder : InputData.PDDL.BaseVisitor
    {
        /// <summary>
        /// Stack of expression parts.
        /// </summary>
        private Stack<INumericExpression> ExpressionStack { set; get; } = new Stack<INumericExpression>();

        /// <summary>
        /// Terms builder.
        /// </summary>
        private Lazy<TermsBuilder> TermsBuilder { set; get; } = null;

        /// <summary>
        /// ID manager converting predicate, function, constant and type names to their corresponding IDs.
        /// </summary>
        private IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Constructs the numeric expressions builder.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        public NumericExpressionsBuilder(IDManager idManager)
        {
            IDManager = idManager;
            TermsBuilder = new Lazy<TermsBuilder>(() => new TermsBuilder(IDManager));
        }

        /// <summary>
        /// Builds PDDL numeric expression from the input data.
        /// </summary>
        /// <param name="expression">Input data expression.</param>
        /// <returns>Built numeric expression.</returns>
        public INumericExpression Build(InputData.PDDL.NumericExpression expression)
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
        public override void Visit(InputData.PDDL.Number data)
        {
            ExpressionStack.Push(new Number(data.Value));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InputData.PDDL.DurationVariable data)
        {
            ExpressionStack.Push(new DurationVariable());
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.NumericFunction data)
        {
            int functionNameID = IDManager.Functions.GetID(data.Name, data.Terms.Count);
            List<ITerm> argumentTerms = new List<ITerm>();

            foreach (var term in data.Terms)
            {
                argumentTerms.Add(TermsBuilder.Value.Build(term));
            }

            ExpressionStack.Push(new NumericFunction(new Atom(functionNameID, argumentTerms), IDManager));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.Plus data)
        {
            List<INumericExpression> arguments = new List<INumericExpression>();
            for (int i = 0; i < data.Arguments.Count; ++i)
            {
                arguments.Add(ExpressionStack.Pop());
            }
            arguments.Reverse();
            ExpressionStack.Push(new Plus(arguments));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.Multiply data)
        {
            List<INumericExpression> arguments = new List<INumericExpression>();
            for (int i = 0; i < data.Arguments.Count; ++i)
            {
                arguments.Add(ExpressionStack.Pop());
            }
            arguments.Reverse();
            ExpressionStack.Push(new Multiply(arguments));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.Minus data)
        {
            INumericExpression secondArgument = ExpressionStack.Pop();
            INumericExpression firstArgument = ExpressionStack.Pop();
            ExpressionStack.Push(new Minus(firstArgument, secondArgument));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.Divide data)
        {
            INumericExpression secondArgument = ExpressionStack.Pop();
            INumericExpression firstArgument = ExpressionStack.Pop();
            ExpressionStack.Push(new Divide(firstArgument, secondArgument));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.UnaryMinus data)
        {
            ExpressionStack.Push(new UnaryMinus(ExpressionStack.Pop()));
        }
    }
}
