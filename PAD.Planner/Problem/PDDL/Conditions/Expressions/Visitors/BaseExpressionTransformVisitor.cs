
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Base visitor implementation transforming PDDL logical expressions.
    /// </summary>
    public class BaseExpressionTransformVisitor : IExpressionTransformVisitor
    {
        public virtual IExpression Visit(PreferenceExpression expression) { return expression; }
        public virtual IExpression Visit(PredicateExpression expression) { return expression; }
        public virtual IExpression Visit(EqualsExpression expression) { return expression; }
        public virtual IExpression Visit(AndExpression expression) { return expression; }
        public virtual IExpression Visit(OrExpression expression) { return expression; }
        public virtual IExpression Visit(NotExpression expression) { return expression; }
        public virtual IExpression Visit(ImplyExpression expression) { return expression; }
        public virtual IExpression Visit(ExistsExpression expression) { return expression; }
        public virtual IExpression Visit(ForallExpression expression) { return expression; }
        public virtual IExpression Visit(NumericCompareExpression expression) { return expression; }
    }
}
