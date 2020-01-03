using System.Collections.Generic;
// ReSharper disable CommentTypo

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Transformer converting the given expression into the conjunctive-normal-form (CNF), i.e. strictly a form of a conjunction of clauses (which are a disjunction of
    /// literals, or literals itselves). The transformation into NNF is done at first, then a distribution of disjunctions (via deMorgan laws) is performed. The literals
    /// are either predicate expressions, equals expressions or numeric compare expressions, and negations of these. CNF expression is a convenient form for further
    /// processing (e.g. evaluating conditions against operator effects to determine relevant operators).
    /// </summary>
    public class ExpressionToCNFTransformer : BaseExpressionTransformVisitor
    {
        /// <summary>
        /// Expression NNF transformer.
        /// </summary>
        private ExpressionToNNFTransformer ExpressionToNNFTransformer { get; }

        /// <summary>
        /// Creates the expression CNF transformer.
        /// </summary>
        /// <param name="evaluationManager">Evaluation manager.</param>
        public ExpressionToCNFTransformer(EvaluationManager evaluationManager)
        {
            ExpressionToNNFTransformer = new ExpressionToNNFTransformer(evaluationManager);
        }

        /// <summary>
        /// Transforms the given expression into the CNF.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Expression in CNF.</returns>
        public IExpression Transform(IExpression expression)
        {
            IExpression expressionNNF = ExpressionToNNFTransformer.Transform(expression);
            return expressionNNF.Accept(this);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(AndExpression expression)
        {
            HashSet<IExpression> children = new HashSet<IExpression>();
            foreach (var child in expression.Children)
            {
                IExpression element = child.Accept(this);

                AndExpression andExpr = element as AndExpression;
                if (andExpr != null)
                {
                    // unfold inner AND expression and remove duplicates
                    foreach (var andExprElem in andExpr.Children)
                    {
                        children.Add(andExprElem);
                    }
                }
                else
                {
                    children.Add(element);
                }
            }
            return new AndExpression(new List<IExpression>(children)); // always in CNF
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(OrExpression expression)
        {
            List<IExpression> children = new List<IExpression>();
            foreach (var child in expression.Children)
            {
                children.Add(child.Accept(this));
            }

            HashSet<IExpression> primitives = new HashSet<IExpression>();
            HashSet<AndExpression> andExpressions = new HashSet<AndExpression>();

            foreach (var element in children)
            {
                AndExpression andExpression = element as AndExpression;
                if (andExpression != null)
                {
                    andExpressions.Add(andExpression);
                    continue;
                }

                OrExpression orExpression = element as OrExpression;
                if (orExpression != null)
                {
                    foreach (var orChild in orExpression.Children)
                    {
                        primitives.Add(orChild);
                    }
                }
                else
                {
                    // predicate, equals, numericCompare, forall, exists
                    primitives.Add(element);
                }
            }

            OrExpression primitivesClause = new OrExpression(new List<IExpression>(primitives));

            if (andExpressions.Count == 0)
            {
                return primitivesClause;
            }

            List<OrExpression> clauses = new List<OrExpression>();
            if (primitives.Count != 0)
            {
                clauses.Add(primitivesClause);
            }

            return new AndExpression(DistributeDisjunctions(new List<AndExpression>(andExpressions), clauses));
        }

        /// <summary>
        /// Distributes disjunctions deeper into the expression (by deMorgan laws) - i.e. we need to push ORs inside the expression and pull ANDs
        /// outside the expression to get the CNF representation. We also filter out duplicates in the clauses and conjunctions (indepotence property).
        /// </summary>
        /// <param name="andExpressions">Conjunctions on the given level.</param>
        /// <param name="clauses">Disjunctions on the given level.</param>
        /// <returns>List of clauses or literals.</returns>
        private static List<IExpression> DistributeDisjunctions(List<AndExpression> andExpressions, List<OrExpression> clauses)
        {
            foreach (var andExpr in andExpressions)
            {
                List<OrExpression> newClauses = new List<OrExpression>();
                foreach (var andElem in andExpr.Children)
                {
                    HashSet<IExpression> andElemItems = new HashSet<IExpression>();

                    OrExpression orExpr = andElem as OrExpression;
                    if (orExpr != null)
                    {
                        orExpr.Children.ForEach(child => andElemItems.Add(child));
                    }
                    else
                    {
                        andElemItems.Add(andElem);
                    }

                    if (clauses.Count == 0)
                    {
                        newClauses.Add(new OrExpression(new List<IExpression>(andElemItems)));
                    }
                    else
                    {
                        foreach (var clause in clauses)
                        {
                            HashSet<IExpression> newOrChildren = new HashSet<IExpression>();
                            clause.Children.ForEach(child => newOrChildren.Add(child));
                            newOrChildren.UnionWith(andElemItems);
                            newClauses.Add(new OrExpression(new List<IExpression>(newOrChildren)));
                        }
                    }
                }
                clauses = newClauses;
            }

            HashSet<IExpression> clausesList = new HashSet<IExpression>();
            clauses.ForEach(clause => clausesList.Add(clause));
            return new List<IExpression>(clausesList);
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(ExistsExpression expression)
        {
            return new ExistsExpression(expression.Parameters, expression.Child.Accept(this));
        }

        /// <summary>
        /// Visits and transforms the expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Transformed expression.</returns>
        public override IExpression Visit(ForallExpression expression)
        {
            return new ForallExpression(expression.Parameters, expression.Child.Accept(this));
        }
    }
}
