using System.Collections.Generic;
using System;
// ReSharper disable CommentTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Condition evaluation visitor. Evaluates PDDL logical expressions (e.g. operator preconditions, or goal conditions).
    /// </summary>
    public class ExpressionEvaluator : IExpressionEvalVisitor
    {
        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { get; }

        /// <summary>
        /// Numeric evaluator.
        /// </summary>
        private Lazy<NumericExpressionEvaluator> NumericEvaluator { get; }

        /// <summary>
        /// Rigid relations of the planning problem.
        /// </summary>
        private RigidRelations RigidRelations { set; get; }

        /// <summary>
        /// Currently used variables substitution.
        /// </summary>
        private ISubstitution Substitution { set; get; }

        /// <summary>
        /// Reference state for the evaluation.
        /// </summary>
        private IState ReferenceState { set; get; }

        /// <summary>
        /// Constructs the logical evaluator.
        /// </summary>
        /// <param name="groundingManager">Grounding manager.</param>
        /// <param name="rigidRelations">Rigid relations.</param>
        public ExpressionEvaluator(GroundingManager groundingManager, RigidRelations rigidRelations = null)
        {
            GroundingManager = groundingManager;
            NumericEvaluator = new Lazy<NumericExpressionEvaluator>(() => new NumericExpressionEvaluator(groundingManager));
            RigidRelations = rigidRelations ?? new RigidRelations();
        }

        /// <summary>
        /// Set the rigid properties of the corresponding planning problem.
        /// </summary>
        /// <param name="rigidRelations">Rigid relations of the planning problem.</param>
        public void SetRigidRelations(RigidRelations rigidRelations)
        {
            RigidRelations = rigidRelations;
        }

        /// <summary>
        /// Evaluates PDDL logical expression.
        /// </summary>
        /// <param name="expression">Logical expression to be evaluated.</param>
        /// <param name="substitution">Used variable substitution.</param>
        /// <param name="referenceState">Reference state.</param>
        /// <returns>True if the expression evaluates as true, false otherwise.</returns>
        public bool Evaluate(IExpression expression, ISubstitution substitution, IState referenceState)
        {
            Substitution = substitution;
            ReferenceState = referenceState;
            return expression.Accept(this);
        }

        /// <summary>
        /// Visits and evaluates predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        public bool Visit(PredicateExpression expression)
        {
            IAtom groundedPredicateAtom = GroundingManager.GroundAtomDeep(expression.PredicateAtom, Substitution, ReferenceState);
            return (RigidRelations.Contains(groundedPredicateAtom) || ReferenceState.HasPredicate(groundedPredicateAtom));
        }

        /// <summary>
        /// Visits and evaluates equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        public bool Visit(EqualsExpression expression)
        {
            return GroundingManager.GroundTermDeep(expression.LeftArgument, Substitution, ReferenceState).Equals(
                   GroundingManager.GroundTermDeep(expression.RightArgument, Substitution, ReferenceState));
        }

        /// <summary>
        /// Visits and evaluates exists expression.
        /// </summary>
        /// <param name="expression">Exists expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        public bool Visit(ExistsExpression expression)
        {
            IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(expression.Parameters);
            foreach (var localSubstitution in localSubstitutions)
            {
                Substitution.AddLocalSubstitution(localSubstitution);
                bool subExpressionResult = expression.Child.Accept(this);
                Substitution.RemoveLocalSubstitution(localSubstitution);

                if (subExpressionResult)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Visits and evaluates forall expression.
        /// </summary>
        /// <param name="expression">Forall expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        public bool Visit(ForallExpression expression)
        {
            IEnumerable<ISubstitution> localSubstitutions = GroundingManager.GenerateAllLocalSubstitutions(expression.Parameters);
            foreach (var localSubstitution in localSubstitutions)
            {
                Substitution.AddLocalSubstitution(localSubstitution);
                bool subExpressionResult = expression.Child.Accept(this);
                Substitution.RemoveLocalSubstitution(localSubstitution);

                if (!subExpressionResult)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Visits and evaluates relational operator expression.
        /// </summary>
        /// <param name="expression">Relational operator expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        public bool Visit(NumericCompareExpression expression)
        {
            double leftValue = NumericEvaluator.Value.Evaluate(expression.LeftArgument, Substitution, ReferenceState);
            double rightValue = NumericEvaluator.Value.Evaluate(expression.RightArgument, Substitution, ReferenceState);
            return NumericCompareExpression.ApplyCompare(expression.Operator, leftValue, rightValue);
        }
    }
}
