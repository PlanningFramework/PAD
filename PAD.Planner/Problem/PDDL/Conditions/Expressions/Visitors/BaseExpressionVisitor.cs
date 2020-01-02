
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Base implemention of generic PDDL logical expression visitor.
    /// </summary>
    public class BaseExpressionVisitor : IExpressionVisitor
    {
        public virtual void Visit(PreferenceExpression expression) { }
        public virtual void Visit(PredicateExpression expression) { }
        public virtual void Visit(EqualsExpression expression) { }
        public virtual void Visit(AndExpression expression) { }
        public virtual void Visit(OrExpression expression) { }
        public virtual void Visit(NotExpression expression) { }
        public virtual void Visit(ImplyExpression expression) { }
        public virtual void Visit(ExistsExpression expression) { }
        public virtual void Visit(ForallExpression expression) { }
        public virtual void Visit(NumericCompareExpression expression) { }
    }
}
