
namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// An interface for AST node visitable by IAstVisitor.
    /// </summary>
    public interface IAstVisitableNode
    {
        /// <summary>
        /// Accept method for the AST visitor.
        /// </summary>
        /// <param name="visitor">AST visitor.</param>
        void Accept(IAstVisitor visitor);
    }

    /// <summary>
    /// An interface for AST visitor.
    /// </summary>
    public interface IAstVisitor
    {
        // root domain and problem nodes

        void Visit(DomainAstNode node);
        void Visit(ProblemAstNode node);

        // domain sections (derived from DomainSectionAstNode)

        void Visit(DomainRequirementsAstNode node);
        void Visit(DomainTypesAstNode node);
        void Visit(DomainConstantsAstNode node);
        void Visit(DomainPredicatesAstNode node);
        void Visit(DomainFunctionsAstNode node);
        void Visit(DomainConstraintsAstNode node);
        void Visit(DomainActionAstNode node);
        void Visit(DomainDurActionAstNode node);
        void Visit(DomainDerivedPredAstNode node);

        // problem sections (derived from ProblemSectionAstNode)

        void Visit(ProblemRequirementsAstNode node);
        void Visit(ProblemObjectsAstNode node);
        void Visit(ProblemInitAstNode node);
        void Visit(ProblemGoalAstNode node);
        void Visit(ProblemConstraintsAstNode node);
        void Visit(ProblemMetricAstNode node);
        void Visit(ProblemLengthAstNode node);

        // logical expressions for action (derived from GDAstNode)

        void Visit(AndGDAstNode node);
        void Visit(OrGDAstNode node);
        void Visit(NotGDAstNode node);
        void Visit(ImplyGDAstNode node);
        void Visit(ExistsGDAstNode node);
        void Visit(ForallGDAstNode node);
        void Visit(PredicateGDAstNode node);
        void Visit(EqualsOpGDAstNode node);
        void Visit(NumCompGDAstNode node);
        void Visit(PreferenceGDAstNode node);

        // logical expressions for timed constraints (derived from ConGDAstNode)

        void Visit(AndConGDAstNode node);
        void Visit(ForallConGDAstNode node);
        void Visit(AtEndConGDAstNode node);
        void Visit(AlwaysConGDAstNode node);
        void Visit(SometimeConGDAstNode node);
        void Visit(WithinConGDAstNode node);
        void Visit(AtMostOnceConGDAstNode node);
        void Visit(SometimeAfterConGDAstNode node);
        void Visit(SometimeBeforeConGDAstNode node);
        void Visit(AlwaysWithinConGDAstNode node);
        void Visit(HoldDuringConGDAstNode node);
        void Visit(HoldAfterConGDAstNode node);
        void Visit(PreferenceConGDAstNode node);

        // logical expressions for durative action (derived from DaGDAstNode)

        void Visit(AndDaGDAstNode node);
        void Visit(ForallDaGDAstNode node);
        void Visit(PreferenceDaGDAstNode node);
        void Visit(AtTimedDaGDAstNode node);
        void Visit(OverTimedDaGDAstNode node);

        // duration contraints for durative action (derived from DurationConstraintAstNode)

        void Visit(AndSimpleDurationConstraintsAstNode node);
        void Visit(AtSimpleDurationConstraintAstNode node);
        void Visit(CompOpSimpleDurationConstraintAstNode node);

        // effects for action (derived from EffectAstNode)

        void Visit(AndCEffectsAstNode node);
        void Visit(ForallCEffectAstNode node);
        void Visit(WhenCEffectAstNode node);
        void Visit(AndPEffectsAstNode node);
        void Visit(PredicatePEffectAstNode node);
        void Visit(EqualsOpPEffectAstNode node);
        void Visit(NotPEffectAstNode node);
        void Visit(AssignPEffectAstNode node);
        void Visit(UndefinedFuncValAstNode node);

        // effects for durative action (derived from DaEffectAstNode)

        void Visit(AndDaEffectsAstNode node);
        void Visit(ForallDaEffectAstNode node);
        void Visit(WhenDaEffectAstNode node);
        void Visit(AtTimedEffectAstNode node);
        void Visit(AssignTimedEffectAstNode node);
        void Visit(TimedNumericExpressionAstNode node);

        // init elements (derived from InitElemAstNode)

        void Visit(PredicateInitElemAstNode node);
        void Visit(EqualsOpInitElemAstNode node);
        void Visit(NotInitElemAstNode node);
        void Visit(AtInitElemAstNode node);

        // terms or numeric terms (derived from TermOrNumericAstNode)

        void Visit(IdentifierTermAstNode node);
        void Visit(FunctionTermAstNode node);
        void Visit(NumberTermAstNode node);
        void Visit(NumericOpAstNode node);
        void Visit(MetricPreferenceViolationAstNode node);
        void Visit(DurationVariableTermAstNode noda);

        // typed blocks and skeletons

        void Visit(TypedListAstNode node);
        void Visit(FunctionTypedListAstNode node);
        void Visit(PredicateSkeletonAstNode node);
    }

    /// <summary>
    /// Base AST visitor, defining blank actions for all the AST nodes. Derived visitors from this class specify custom
    /// handle actions for the subset of the AST node types.
    /// </summary>
    public abstract class BaseAstVisitor : IAstVisitor
    {
        /// <summary>
        /// Evaluates the given AST.
        /// </summary>
        /// <param name="rootNode">Root node of the AST.</param>
        public void Evaluate(IAstVisitableNode rootNode)
        {
            if (rootNode != null)
            {
                rootNode.Accept(this);
            }
        }

        // root domain and problem nodes

        public virtual void Visit(DomainAstNode node) { }
        public virtual void Visit(ProblemAstNode node) { }

        // domain sections (derived from DomainSectionAstNode)

        public virtual void Visit(DomainRequirementsAstNode node) { }
        public virtual void Visit(DomainTypesAstNode node) { }
        public virtual void Visit(DomainConstantsAstNode node) { }
        public virtual void Visit(DomainPredicatesAstNode node) { }
        public virtual void Visit(DomainFunctionsAstNode node) { }
        public virtual void Visit(DomainConstraintsAstNode node) { }
        public virtual void Visit(DomainActionAstNode node) { }
        public virtual void Visit(DomainDurActionAstNode node) { }
        public virtual void Visit(DomainDerivedPredAstNode node) { }

        // problem sections (derived from ProblemSectionAstNode)

        public virtual void Visit(ProblemRequirementsAstNode node) { }
        public virtual void Visit(ProblemObjectsAstNode node) { }
        public virtual void Visit(ProblemInitAstNode node) { }
        public virtual void Visit(ProblemGoalAstNode node) { }
        public virtual void Visit(ProblemConstraintsAstNode node) { }
        public virtual void Visit(ProblemMetricAstNode node) { }
        public virtual void Visit(ProblemLengthAstNode node) { }

        // logical expressions for action (derived from GDAstNode)

        public virtual void Visit(AndGDAstNode node) { }
        public virtual void Visit(OrGDAstNode node) { }
        public virtual void Visit(NotGDAstNode node) { }
        public virtual void Visit(ImplyGDAstNode node) { }
        public virtual void Visit(ExistsGDAstNode node) { }
        public virtual void Visit(ForallGDAstNode node) { }
        public virtual void Visit(PredicateGDAstNode node) { }
        public virtual void Visit(EqualsOpGDAstNode node) { }
        public virtual void Visit(NumCompGDAstNode node) { }
        public virtual void Visit(PreferenceGDAstNode node) { }

        // logical expressions for timed constraints (derived from ConGDAstNode)

        public virtual void Visit(AndConGDAstNode node) { }
        public virtual void Visit(ForallConGDAstNode node) { }
        public virtual void Visit(AtEndConGDAstNode node) { }
        public virtual void Visit(AlwaysConGDAstNode node) { }
        public virtual void Visit(SometimeConGDAstNode node) { }
        public virtual void Visit(WithinConGDAstNode node) { }
        public virtual void Visit(AtMostOnceConGDAstNode node) { }
        public virtual void Visit(SometimeAfterConGDAstNode node) { }
        public virtual void Visit(SometimeBeforeConGDAstNode node) { }
        public virtual void Visit(AlwaysWithinConGDAstNode node) { }
        public virtual void Visit(HoldDuringConGDAstNode node) { }
        public virtual void Visit(HoldAfterConGDAstNode node) { }
        public virtual void Visit(PreferenceConGDAstNode node) { }

        // logical expressions for durative action (derived from DaGDAstNode)

        public virtual void Visit(AndDaGDAstNode node) { }
        public virtual void Visit(ForallDaGDAstNode node) { }
        public virtual void Visit(PreferenceDaGDAstNode node) { }
        public virtual void Visit(AtTimedDaGDAstNode node) { }
        public virtual void Visit(OverTimedDaGDAstNode node) { }

        // duration contraints for durative action (derived from DurationConstraintAstNode)

        public virtual void Visit(AndSimpleDurationConstraintsAstNode node) { }
        public virtual void Visit(AtSimpleDurationConstraintAstNode node) { }
        public virtual void Visit(CompOpSimpleDurationConstraintAstNode node) { }

        // effects for action (derived from EffectAstNode)

        public virtual void Visit(AndCEffectsAstNode node) { }
        public virtual void Visit(ForallCEffectAstNode node) { }
        public virtual void Visit(WhenCEffectAstNode node) { }
        public virtual void Visit(AndPEffectsAstNode node) { }
        public virtual void Visit(PredicatePEffectAstNode node) { }
        public virtual void Visit(EqualsOpPEffectAstNode node) { }
        public virtual void Visit(NotPEffectAstNode node) { }
        public virtual void Visit(AssignPEffectAstNode node) { }
        public virtual void Visit(UndefinedFuncValAstNode node) { }

        // effects for durative action (derived from DaEffectAstNode)

        public virtual void Visit(AndDaEffectsAstNode node) { }
        public virtual void Visit(ForallDaEffectAstNode node) { }
        public virtual void Visit(WhenDaEffectAstNode node) { }
        public virtual void Visit(AtTimedEffectAstNode node) { }
        public virtual void Visit(AssignTimedEffectAstNode node) { }
        public virtual void Visit(TimedNumericExpressionAstNode node) { }

        // init elements (derived from InitElemAstNode)

        public virtual void Visit(PredicateInitElemAstNode node) { }
        public virtual void Visit(EqualsOpInitElemAstNode node) { }
        public virtual void Visit(NotInitElemAstNode node) { }
        public virtual void Visit(AtInitElemAstNode node) { }

        // terms or numeric terms (derived from TermOrNumericAstNode)

        public virtual void Visit(IdentifierTermAstNode node) { }
        public virtual void Visit(FunctionTermAstNode node) { }
        public virtual void Visit(NumberTermAstNode node) { }
        public virtual void Visit(NumericOpAstNode node) { }
        public virtual void Visit(MetricPreferenceViolationAstNode node) { }
        public virtual void Visit(DurationVariableTermAstNode noda) { }

        // typed blocks and skeletons

        public virtual void Visit(TypedListAstNode node) { }
        public virtual void Visit(FunctionTypedListAstNode node) { }
        public virtual void Visit(PredicateSkeletonAstNode node) { }
    }
}
