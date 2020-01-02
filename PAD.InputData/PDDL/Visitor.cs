
namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Interface for a common PDDL input data structure visitable by a generic visitor.
    /// </summary>
    public interface IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Interface for a common PDDL input data structure visitor.
    /// </summary>
    public interface IVisitor
    {
        // root structures

        void Visit(Domain data);
        void Visit(Problem data);

        // basic definition structures 

        void Visit(Requirements data);
        void Visit(Type data);
        void Visit(Constant data);
        void Visit(Predicate data);
        void Visit(Function data);
        void Visit(Action data);
        void Visit(DurativeAction data);
        void Visit(DerivedPredicate data);
        void Visit(Object data);
        void Visit(Metric data);
        void Visit(LengthSpecElement data);
        void PostVisit(Action data);
        void PostVisit(DurativeAction data);
        void PostVisit(DerivedPredicate data);

        // parameters and terms

        void Visit(Parameter data);
        void Visit(DefinitionTerm data);
        void Visit(ConstantTerm data);
        void Visit(VariableTerm data);
        void Visit(ObjectFunctionTerm data);
        void PostVisit(ObjectFunctionTerm data);

        // derived from Constraint

        void Visit(PreferenceConstraint data);
        void Visit(ForallConstraint data);
        void Visit(AtEndConstraint data);
        void Visit(AlwaysConstraint data);
        void Visit(SometimeConstraint data);
        void Visit(WithinConstraint data);
        void Visit(AtMostOnceConstraint data);
        void Visit(SometimeAfterConstraint data);
        void Visit(SometimeBeforeConstraint data);
        void Visit(AlwaysWithinConstraint data);
        void Visit(HoldDuringConstraint data);
        void Visit(HoldAfterConstraint data);
        void PostVisit(ForallConstraint data);

        // derived from Expression

        void Visit(PreferenceExpression data);
        void Visit(PredicateExpression data);
        void Visit(EqualsExpression data);
        void Visit(AndExpression data);
        void Visit(OrExpression data);
        void Visit(NotExpression data);
        void Visit(ImplyExpression data);
        void Visit(ExistsExpression data);
        void Visit(ForallExpression data);
        void Visit(NumericCompareExpression data);
        void PostVisit(PreferenceExpression data);
        void PostVisit(PredicateExpression data);
        void PostVisit(EqualsExpression data);
        void PostVisit(AndExpression data);
        void PostVisit(OrExpression data);
        void PostVisit(NotExpression data);
        void PostVisit(ImplyExpression data);
        void PostVisit(ExistsExpression data);
        void PostVisit(ForallExpression data);
        void PostVisit(NumericCompareExpression data);

        // derived from NumericExpression

        void Visit(Number data);
        void Visit(NumericFunction data);
        void Visit(DurationVariable data);
        void Visit(Plus data);
        void Visit(Multiply data);
        void Visit(Minus data);
        void Visit(Divide data);
        void Visit(UnaryMinus data);
        void PostVisit(NumericFunction data);
        void PostVisit(Plus data);
        void PostVisit(Multiply data);
        void PostVisit(Minus data);
        void PostVisit(Divide data);
        void PostVisit(UnaryMinus data);

        // derived from Effect

        void Visit(ForallEffect data);
        void Visit(WhenEffect data);
        void Visit(PredicateEffect data);
        void Visit(EqualsEffect data);
        void Visit(NotEffect data);
        void Visit(NumericAssignEffect data);
        void Visit(ObjectAssignEffect data);
        void PostVisit(ForallEffect data);
        void PostVisit(WhenEffect data);
        void PostVisit(PredicateEffect data);
        void PostVisit(EqualsEffect data);
        void PostVisit(NotEffect data);
        void PostVisit(NumericAssignEffect data);
        void PostVisit(ObjectAssignEffect data);

        // dereived from DurativeConstraint

        void Visit(CompareDurativeConstraint data);
        void Visit(AtDurativeConstraint data);

        // derived from DurativeExpression

        void Visit(AndDurativeExpression data);
        void Visit(ForallDurativeExpression data);
        void Visit(PreferencedTimedExpression data);
        void Visit(AtTimedExpression data);
        void Visit(OverTimedExpression data);
        void PostVisit(ForallDurativeExpression data);

        // derived from DurativeEffect

        void Visit(ForallDurativeEffect data);
        void Visit(WhenDurativeEffect data);
        void Visit(AtTimedEffect data);
        void Visit(AssignTimedEffect data);
        void Visit(PrimitiveTimedNumericExpression data);
        void Visit(CompoundTimedNumericExpression data);
        void PostVisit(ForallDurativeEffect data);

        // derived from InitElement

        void Visit(PredicateInitElement data);
        void Visit(EqualsInitElement data);
        void Visit(NotInitElement data);
        void Visit(AtInitElement data);
        void Visit(EqualsNumericFunctionInitElement data);
        void Visit(EqualsObjectFunctionInitElement data);
        void Visit(BasicNumericFunctionTerm data);
        void Visit(BasicObjectFunctionTerm data);
        void PostVisit(PredicateInitElement data);
        void PostVisit(EqualsObjectFunctionInitElement data);
        void PostVisit(BasicNumericFunctionTerm data);
        void PostVisit(BasicObjectFunctionTerm data);

        // derived from MetricExpression

        void Visit(MetricNumber data);
        void Visit(MetricNumericFunction data);
        void Visit(MetricTotalTime data);
        void Visit(MetricPreferenceViolation data);
        void Visit(MetricPlus data);
        void Visit(MetricMultiply data);
        void Visit(MetricMinus data);
        void Visit(MetricDivide data);
        void Visit(MetricUnaryMinus data);
        void PostVisit(MetricNumericFunction data);
    }

    /// <summary>
    /// A base visitor for input PDDL data with null behaviour, implementing interface IVisitor. Derived visitors from this class can
    /// implement handles for only a subset of input structures.
    /// </summary>
    public class BaseVisitor : IVisitor
    {
        // root structures

        public virtual void Visit(Domain data) { }
        public virtual void Visit(Problem data) { }

        // basic definition structures 

        public virtual void Visit(Requirements data) { }
        public virtual void Visit(Type data) { }
        public virtual void Visit(Constant data) { }
        public virtual void Visit(Predicate data) { }
        public virtual void Visit(Function data) { }
        public virtual void Visit(Action data) { }
        public virtual void Visit(DurativeAction data) { }
        public virtual void Visit(DerivedPredicate data) { }
        public virtual void Visit(Object data) { }
        public virtual void Visit(Metric data) { }
        public virtual void Visit(LengthSpecElement data) { }
        public virtual void PostVisit(Action data) { }
        public virtual void PostVisit(DurativeAction data) { }
        public virtual void PostVisit(DerivedPredicate data) { }

        // parameters and terms

        public virtual void Visit(Parameter data) { }
        public virtual void Visit(DefinitionTerm data) { }
        public virtual void Visit(ConstantTerm data) { }
        public virtual void Visit(VariableTerm data) { }
        public virtual void Visit(ObjectFunctionTerm data) { }
        public virtual void PostVisit(ObjectFunctionTerm data) { }

        // derived from Constraint

        public virtual void Visit(PreferenceConstraint data) { }
        public virtual void Visit(ForallConstraint data) { }
        public virtual void Visit(AtEndConstraint data) { }
        public virtual void Visit(AlwaysConstraint data) { }
        public virtual void Visit(SometimeConstraint data) { }
        public virtual void Visit(WithinConstraint data) { }
        public virtual void Visit(AtMostOnceConstraint data) { }
        public virtual void Visit(SometimeAfterConstraint data) { }
        public virtual void Visit(SometimeBeforeConstraint data) { }
        public virtual void Visit(AlwaysWithinConstraint data) { }
        public virtual void Visit(HoldDuringConstraint data) { }
        public virtual void Visit(HoldAfterConstraint data) { }
        public virtual void PostVisit(ForallConstraint data) { }

        // derived from Expression

        public virtual void Visit(PreferenceExpression data) { }
        public virtual void Visit(PredicateExpression data) { }
        public virtual void Visit(EqualsExpression data) { }
        public virtual void Visit(AndExpression data) { }
        public virtual void Visit(OrExpression data) { }
        public virtual void Visit(NotExpression data) { }
        public virtual void Visit(ImplyExpression data) { }
        public virtual void Visit(ExistsExpression data) { }
        public virtual void Visit(ForallExpression data) { }
        public virtual void Visit(NumericCompareExpression data) { }
        public virtual void PostVisit(PreferenceExpression data) { }
        public virtual void PostVisit(PredicateExpression data) { }
        public virtual void PostVisit(EqualsExpression data) { }
        public virtual void PostVisit(AndExpression data) { }
        public virtual void PostVisit(OrExpression data) { }
        public virtual void PostVisit(NotExpression data) { }
        public virtual void PostVisit(ImplyExpression data) { }
        public virtual void PostVisit(ExistsExpression data) { }
        public virtual void PostVisit(ForallExpression data) { }
        public virtual void PostVisit(NumericCompareExpression data) { }

        // derived from NumericExpression

        public virtual void Visit(Number data) { }
        public virtual void Visit(NumericFunction data) { }
        public virtual void Visit(DurationVariable data) { }
        public virtual void Visit(Plus data) { }
        public virtual void Visit(Multiply data) { }
        public virtual void Visit(Minus data) { }
        public virtual void Visit(Divide data) { }
        public virtual void Visit(UnaryMinus data) { }
        public virtual void PostVisit(NumericFunction data) { }
        public virtual void PostVisit(Plus data) { }
        public virtual void PostVisit(Multiply data) { }
        public virtual void PostVisit(Minus data) { }
        public virtual void PostVisit(Divide data) { }
        public virtual void PostVisit(UnaryMinus data) { }

        // derived from Effect

        public virtual void Visit(ForallEffect data) { }
        public virtual void Visit(WhenEffect data) { }
        public virtual void Visit(PredicateEffect data) { }
        public virtual void Visit(EqualsEffect data) { }
        public virtual void Visit(NotEffect data) { }
        public virtual void Visit(NumericAssignEffect data) { }
        public virtual void Visit(ObjectAssignEffect data) { }
        public virtual void PostVisit(ForallEffect data) { }
        public virtual void PostVisit(WhenEffect data) { }
        public virtual void PostVisit(PredicateEffect data) { }
        public virtual void PostVisit(EqualsEffect data) { }
        public virtual void PostVisit(NotEffect data) { }
        public virtual void PostVisit(NumericAssignEffect data) { }
        public virtual void PostVisit(ObjectAssignEffect data) { }

        // dereived from DurativeConstraint

        public virtual void Visit(CompareDurativeConstraint data) { }
        public virtual void Visit(AtDurativeConstraint data) { }

        // derived from DurativeExpression

        public virtual void Visit(AndDurativeExpression data) { }
        public virtual void Visit(ForallDurativeExpression data) { }
        public virtual void Visit(PreferencedTimedExpression data) { }
        public virtual void Visit(AtTimedExpression data) { }
        public virtual void Visit(OverTimedExpression data) { }
        public virtual void PostVisit(ForallDurativeExpression data) { }

        // derived from DurativeEffect

        public virtual void Visit(ForallDurativeEffect data) { }
        public virtual void Visit(WhenDurativeEffect data) { }
        public virtual void Visit(AtTimedEffect data) { }
        public virtual void Visit(AssignTimedEffect data) { }
        public virtual void Visit(PrimitiveTimedNumericExpression data) { }
        public virtual void Visit(CompoundTimedNumericExpression data) { }
        public virtual void PostVisit(ForallDurativeEffect data) { }

        // derived from InitElement

        public virtual void Visit(PredicateInitElement data) { }
        public virtual void Visit(EqualsInitElement data) { }
        public virtual void Visit(NotInitElement data) { }
        public virtual void Visit(AtInitElement data) { }
        public virtual void Visit(EqualsNumericFunctionInitElement data) { }
        public virtual void Visit(EqualsObjectFunctionInitElement data) { }
        public virtual void Visit(BasicNumericFunctionTerm data) { }
        public virtual void Visit(BasicObjectFunctionTerm data) { }
        public virtual void PostVisit(PredicateInitElement data) { }
        public virtual void PostVisit(EqualsObjectFunctionInitElement data) { }
        public virtual void PostVisit(BasicNumericFunctionTerm data) { }
        public virtual void PostVisit(BasicObjectFunctionTerm data) { }

        // derived from MetricExpression

        public virtual void Visit(MetricNumber data) { }
        public virtual void Visit(MetricNumericFunction data) { }
        public virtual void Visit(MetricTotalTime data) { }
        public virtual void Visit(MetricPreferenceViolation data) { }
        public virtual void Visit(MetricPlus data) { }
        public virtual void Visit(MetricMultiply data) { }
        public virtual void Visit(MetricMinus data) { }
        public virtual void Visit(MetricDivide data) { }
        public virtual void Visit(MetricUnaryMinus data) { }
        public virtual void PostVisit(MetricNumericFunction data) { }
    }
}

