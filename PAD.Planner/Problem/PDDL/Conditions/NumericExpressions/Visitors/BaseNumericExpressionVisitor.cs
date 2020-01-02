
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Base implementation of generic PDDL numeric expressions visitor.
    /// </summary>
    public class BaseNumericExpressionVisitor : INumericExpressionVisitor
    {
        public virtual void PostVisit(Plus expression) { }
        public virtual void PostVisit(Minus expression) { }
        public virtual void PostVisit(UnaryMinus expression) { }
        public virtual void PostVisit(Multiply expression) { }
        public virtual void PostVisit(Divide expression) { }
        public virtual void Visit(Number expression) { }
        public virtual void Visit(DurationVariable expression) { }
        public virtual void Visit(NumericFunction expression) { }
    }
}
