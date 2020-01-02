using System.Collections.Generic;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Rigid relations compliance evaluator. Detects violations of rigid relations in condition expressions, while ignoring other (non-rigid) relations.
    /// </summary>
    public class RigidRelationsComplianceEvaluator : IExpressionEvalVisitor
    {
        /// <summary>
        /// Grounding manager.
        /// </summary>
        private GroundingManager GroundingManager { set; get; } = null;

        /// <summary>
        /// Rigid relations of the planning problem.
        /// </summary>
        private RigidRelations RigidRelations { set; get; } = null;

        /// <summary>
        /// Currently used variables substitution.
        /// </summary>
        private ISubstitution Substitution { set; get; } = null;

        /// <summary>
        /// Constructs the evaluator.
        /// </summary>
        /// <param name="groundingManager">Grounding manager.</param>
        /// <param name="rigidRelations">Rigid relations.</param>
        public RigidRelationsComplianceEvaluator(GroundingManager groundingManager, RigidRelations rigidRelations = null)
        {
            GroundingManager = groundingManager;
            RigidRelations = (rigidRelations != null) ? rigidRelations : new RigidRelations();
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
        /// <returns>True if the expression is in compliance with rigid relations, false otherwise.</returns>
        public bool Evaluate(IExpression expression, ISubstitution substitution)
        {
            Substitution = substitution;
            return expression.Accept(this);
        }

        /// <summary>
        /// Visits and evaluates predicate expression.
        /// </summary>
        /// <param name="expression">Predicate expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        public bool Visit(PredicateExpression expression)
        {
            // if the predicate is a rigid relation, then check whether it has the correct value
            if (RigidRelations.IsPredicateRigidRelation(expression.PredicateAtom))
            {
                IAtom predicateAtom = GroundingManager.GroundAtom(expression.PredicateAtom, Substitution);
                return (RigidRelations.Contains(predicateAtom));
            }
            return true;
        }

        /// <summary>
        /// Visits and evaluates equals expression.
        /// </summary>
        /// <param name="expression">Equals expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        public bool Visit(EqualsExpression expression)
        {
            return true;
        }

        /// <summary>
        /// Visits and evaluates relational operator expression.
        /// </summary>
        /// <param name="expression">Relational operator expression.</param>
        /// <returns>True if the specified expression evaluates as true, false otherwise.</returns>
        public bool Visit(NumericCompareExpression expression)
        {
            return true;
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
    }
}
