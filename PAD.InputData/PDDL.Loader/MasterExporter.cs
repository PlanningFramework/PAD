using System.Diagnostics;
using PAD.InputData.PDDL.Loader.Ast;
using PAD.InputData.PDDL.Loader.DataExport;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader
{
    /// <summary>
    /// Master class for exporting AST into PDDL input data structures, i.e. the final product of the loader.
    /// </summary>
    public static class MasterExporter
    {
        /// <summary>
        /// Context of the current domain. Helps to determine semantics of some AST parts (e.g. function type).
        /// </summary>
        private static Domain DomainContext { set; get; }

        /// <summary>
        /// Sets the current domain context.
        /// </summary>
        /// <param name="domainContext">Domain context.</param>
        public static void SetDomainEvaluationContext(Domain domainContext)
        {
            DomainContext = domainContext;
        }

        /// <summary>
        /// Clears the current domain context.
        /// </summary>
        public static void ClearDomainEvaluationContext()
        {
            DomainContext = null;
        }

        /// <summary>
        /// Converts the given typed list into parameters.
        /// </summary>
        /// <param name="typedListAstNode">AST node.</param>
        /// <returns>Converted parameters.</returns>
        public static Parameters ToParameters(TypedListAstNode typedListAstNode)
        {
            Parameters newParameters = new Parameters();
            typedListAstNode.TypedIdentifiers.ForEach(paramElem => newParameters.Add(new Parameter(paramElem.Item1, paramElem.Item2.Split(';'))));
            return newParameters;
        }

        /// <summary>
        /// Converts the given GD into preconditions. A root AND-expression is split into a list of precondition expressions.
        /// </summary>
        /// <param name="gdAstNode">AST node.</param>
        /// <returns>Converted preconditions.</returns>
        public static Preconditions ToPreconditions(GdAstNode gdAstNode)
        {
            Preconditions preconditions = new Preconditions();

            Expression expression = ToExpression(gdAstNode);

            var andExpression = expression as AndExpression;
            if (andExpression != null)
            {
                preconditions.AddRange(andExpression.Arguments);
            }
            else if (expression != null)
            {
                preconditions.Add(expression);
            }

            return preconditions;
        }

        /// <summary>
        /// Converts the given GD into a expression.
        /// </summary>
        /// <param name="gdAstNode">AST node.</param>
        /// <returns>Converted expression.</returns>
        public static Expression ToExpression(GdAstNode gdAstNode)
        {
            ToExpressionConverter converter = new ToExpressionConverter();
            converter.Evaluate(gdAstNode);
            return converter.ExpressionData;
        }

        /// <summary>
        /// Checks whether the given general term/numeric AST is a term.
        /// </summary>
        /// <param name="termAstNode">AST node.</param>
        /// <returns>True if the general term/numeric AST is a term. False otherwise.</returns>
        public static bool IsTerm(TermOrNumericAstNode termAstNode)
        {
            return termAstNode is TermAstNode;
        }

        /// <summary>
        /// Converts the given general term/numeric AST into a term.
        /// </summary>
        /// <param name="termAstNode">AST node.</param>
        /// <returns>Converted term.</returns>
        public static Term ToTerm(TermOrNumericAstNode termAstNode)
        {
            TermAstNode term = termAstNode as TermAstNode;
            if (term != null)
            {
                return ToTerm(term);
            }
            Debug.Assert(false);
            return null;
        }

        /// <summary>
        /// Converts the given term AST into a term.
        /// </summary>
        /// <param name="termAstNode">AST node.</param>
        /// <returns>Converted term.</returns>
        public static Term ToTerm(TermAstNode termAstNode)
        {
            ToTermConverter converter = new ToTermConverter();
            converter.Evaluate(termAstNode);
            return converter.TermData;
        }

        /// <summary>
        /// Checks whether the given identifier term is an object function.
        /// </summary>
        /// <param name="termAstNode">AST node.</param>
        /// <returns>True if the term is an object function.</returns>
        public static bool IsIdentifierTermObjectFunction(IdentifierTermAstNode termAstNode)
        {
            return DomainContext.Functions.ContainsFunction(termAstNode.Name, 0, false);
        }

        /// <summary>
        /// Checks whether the given identifier term is a numeric function.
        /// </summary>
        /// <param name="termAstNode">AST node.</param>
        /// <returns>True if the term is a numeric function. False otherwise.</returns>
        public static bool IsIdentifierTermNumericFunction(IdentifierTermAstNode termAstNode)
        {
            return DomainContext.Functions.ContainsFunction(termAstNode.Name, 0, true);
        }

        /// <summary>
        /// Checks whether the given identifier term is a variable.
        /// </summary>
        /// <param name="termAstNode">AST node.</param>
        /// <returns>True if the term is a variable. False otherwise.</returns>
        public static bool IsIdentifierTermVariable(IdentifierTermAstNode termAstNode)
        {
            return termAstNode.Name.StartsWith("?");
        }

        /// <summary>
        /// Converts the given general term into a numeric expression.
        /// </summary>
        /// <param name="termAstNode">AST node.</param>
        /// <returns>Converted numeric expression.</returns>
        public static NumericExpression ToNumericExpression(TermOrNumericAstNode termAstNode)
        {
            ToNumericExpressionConverter converter = new ToNumericExpressionConverter();
            converter.Evaluate(termAstNode);
            return converter.ExpressionData;
        }

        /// <summary>
        /// Checks whether the given general term is a numeric expression.
        /// </summary>
        /// <param name="termAstNode">AST node.</param>
        /// <returns>True if the general term is a numeric expression.</returns>
        public static bool IsNumericExpression(TermOrNumericAstNode termAstNode)
        {
            if (termAstNode is NumericOpAstNode || termAstNode is NumberTermAstNode)
            {
                return true;
            }

            var functionTermAstNode = termAstNode as FunctionTermAstNode;
            if (functionTermAstNode != null)
            {
                return IsNumericFunction(functionTermAstNode);
            }

            var identifierTermAstNode = termAstNode as IdentifierTermAstNode;
            return identifierTermAstNode != null && IsIdentifierTermNumericFunction(identifierTermAstNode);
        }

        /// <summary>
        /// Converts the given general term into a constant term.
        /// </summary>
        /// <param name="termAstNode">AST node.</param>
        /// <returns>Converted constant term.</returns>
        public static ConstantTerm ToConstantTerm(TermOrNumericAstNode termAstNode)
        {
            var identifier = termAstNode as IdentifierTermAstNode;
            return identifier != null ? new ConstantTerm(identifier.Name) : null;
        }

        /// <summary>
        /// Converts the given general term into a basic numeric function term.
        /// </summary>
        /// <param name="termAstNode">AST node.</param>
        /// <returns>Converted basic numeric function term. Null if cannot be converted.</returns>
        public static BasicNumericFunctionTerm ToBasicNumericFunctionTerm(TermOrNumericAstNode termAstNode)
        {
            var function = termAstNode as FunctionTermAstNode;
            if (function != null && IsNumericFunction(function))
            {
                var numericFunction = new BasicNumericFunctionTerm(function.Name);
                function.Terms.ForEach(term => numericFunction.Terms.Add(ToConstantTerm(term)));
                return numericFunction;
            }

            var identifier = termAstNode as IdentifierTermAstNode;
            if (identifier != null && IsIdentifierTermNumericFunction(identifier))
            {
                return new BasicNumericFunctionTerm(identifier.Name);
            }

            return null;
        }

        /// <summary>
        /// Converts the given general term into a basic object function term.
        /// </summary>
        /// <param name="termAstNode">AST node.</param>
        /// <returns>Converted basic object function term. Null if cannot be converted.</returns>
        public static BasicObjectFunctionTerm ToBasicObjectFunctionTerm(TermOrNumericAstNode termAstNode)
        {
            var function = termAstNode as FunctionTermAstNode;
            if (function != null && IsObjectFunction(function))
            {
                var objectFunction = new BasicObjectFunctionTerm(function.Name);
                function.Terms.ForEach(term => objectFunction.Terms.Add(ToConstantTerm(term)));
                return objectFunction;
            }

            var identifier = termAstNode as IdentifierTermAstNode;
            if (identifier != null && IsIdentifierTermObjectFunction(identifier))
            {
                return new BasicObjectFunctionTerm(identifier.Name);
            }

            return null;
        }

        /// <summary>
        /// Checks whether the given function term is a numeric function.
        /// </summary>
        /// <param name="functionTermAstNode">AST node.</param>
        /// <returns>True if the function term is a numeric function. False otherwise.</returns>
        public static bool IsNumericFunction(FunctionTermAstNode functionTermAstNode)
        {
            return DomainContext.Functions.ContainsFunction(functionTermAstNode.Name, functionTermAstNode.Terms.Count, true);
        }

        /// <summary>
        /// Checks whether the given function term is a object function.
        /// </summary>
        /// <param name="functionTermAstNode">AST node.</param>
        /// <returns>True if the function term is a object function. False otherwise.</returns>
        public static bool IsObjectFunction(FunctionTermAstNode functionTermAstNode)
        {
            return DomainContext.Functions.ContainsFunction(functionTermAstNode.Name, functionTermAstNode.Terms.Count, false);
        }

        /// <summary>
        /// Converts the given con-GD into constraints.
        /// </summary>
        /// <param name="conGdAstNode">AST node.</param>
        /// <returns>Converted constraints.</returns>
        public static Constraints ToConstraints(ConGdAstNode conGdAstNode)
        {
            ToConstraintsConverter converter = new ToConstraintsConverter();
            converter.Evaluate(conGdAstNode);
            return converter.ConstraintsData;
        }

        /// <summary>
        /// Convert the given effects AST into an effects structure. A root AND-expression is split into a list of effects.
        /// </summary>
        /// <param name="effectAstNode">AST node.</param>
        /// <returns>Converted effects.</returns>
        public static Effects ToEffects(EffectAstNode effectAstNode)
        {
            ToEffectsConverter converter = new ToEffectsConverter();
            converter.Evaluate(effectAstNode);
            return converter.EffectsData;
        }

        /// <summary>
        /// Converts the given duration constraint AST into a durative constraints structure.
        /// </summary>
        /// <param name="durConstrAstNode">AST node.</param>
        /// <returns>Converted durative constraints.</returns>
        public static DurativeConstraints ToDurativeConstraints(DurationConstraintAstNode durConstrAstNode)
        {
            ToDurativeConstraintsConverter converter = new ToDurativeConstraintsConverter();
            converter.Evaluate(durConstrAstNode);
            return converter.ConstraintsData;
        }

        /// <summary>
        /// Convert the given da-GD into durative conditions. A root AND-expression is split into a list of condition expressions.
        /// </summary>
        /// <param name="daGdAstNode">AST node.</param>
        /// <returns>Converted durative conditions.</returns>
        public static DurativeConditions ToDurativeConditions(DaGdAstNode daGdAstNode)
        {
            DurativeConditions conditions = new DurativeConditions();

            DurativeExpression expression = ToDurativeExpression(daGdAstNode);

            var andExpression = expression as AndDurativeExpression;
            if (andExpression != null)
            {
                conditions.AddRange(andExpression.Arguments);
            }
            else if (expression != null)
            {
                conditions.Add(expression);
            }

            return conditions;
        }

        /// <summary>
        /// Converts the given da-GD into a durative expression.
        /// </summary>
        /// <param name="daGdAstNode">AST node.</param>
        /// <returns>Converted durative expression.</returns>
        public static DurativeExpression ToDurativeExpression(DaGdAstNode daGdAstNode)
        {
            ToDurativeExpressionConverter converter = new ToDurativeExpressionConverter();
            converter.Evaluate(daGdAstNode);
            return converter.ExpressionData;
        }

        /// <summary>
        /// Converts the given da-effect into a durative effects. A root AND-expression is split into a list of effects.
        /// </summary>
        /// <param name="daEffectAstNode">AST node.</param>
        /// <returns>Converted durative effects.</returns>
        public static DurativeEffects ToDurativeEffects(DaEffectAstNode daEffectAstNode)
        {
            ToDurativeEffectsConverter converter = new ToDurativeEffectsConverter();
            converter.Evaluate(daEffectAstNode);
            return converter.EffectsData;
        }

        /// <summary>
        /// Converts the given timed numeric expression AST into a timed numeric expression structure.
        /// </summary>
        /// <param name="timedNumericExpressionAstNode">AST node.</param>
        /// <returns>Converted timed numeric expression.</returns>
        public static TimedNumericExpression ToTimedNumericExpression(TimedNumericExpressionAstNode timedNumericExpressionAstNode)
        {
            if (!timedNumericExpressionAstNode.IsProductExpression)
            {
                return new PrimitiveTimedNumericExpression();
            }

            return new CompoundTimedNumericExpression(ToNumericExpression(timedNumericExpressionAstNode.ProductExprNumericFactor));
        }

        /// <summary>
        /// Convert the given init element AST into a init element structure.
        /// </summary>
        /// <param name="initElementAstNode">AST node.</param>
        /// <returns>Converted init element.</returns>
        public static InitElement ToInitElement(InitElemAstNode initElementAstNode)
        {
            ToInitElementConverter visitor = new ToInitElementConverter();
            visitor.Evaluate(initElementAstNode);
            return visitor.InitElementData;
        }

        /// <summary>
        /// Converts the given problem goal AST into a goal structure. A root AND-expression is split into a list of goal expressions.
        /// </summary>
        /// <param name="goalAstNode">AST node.</param>
        /// <returns>Converted goal.</returns>
        public static Goal ToGoal(ProblemGoalAstNode goalAstNode)
        {
            Goal goal = new Goal();

            Expression expression = ToExpression(goalAstNode.Condition);

            var andExpression = expression as AndExpression;
            if (andExpression != null)
            {
                goal.AddRange(andExpression.Arguments);
            }
            else if (expression != null)
            {
                goal.Add(expression);
            }

            return goal;
        }

        /// <summary>
        /// Converts the given general term into a metric expression.
        /// </summary>
        /// <param name="numericAstNode">AST node.</param>
        /// <returns>Converted metric expression.</returns>
        public static MetricExpression ToMetricExpression(TermOrNumericAstNode numericAstNode)
        {
            ToMetricExpressionConverter converter = new ToMetricExpressionConverter();
            converter.Evaluate(numericAstNode);
            return converter.ExpressionData;
        }

        /// <summary>
        /// Converts the given domain AST into a root domain structure.
        /// </summary>
        /// <param name="domainAstNode">AST node.</param>
        /// <returns>Converted domain.</returns>
        public static Domain ToDomain(DomainAstNode domainAstNode)
        {
            ToDomainConverter visitor = new ToDomainConverter();
            SetDomainEvaluationContext(visitor.DomainData);
            visitor.Evaluate(domainAstNode);
            ClearDomainEvaluationContext();
            return visitor.DomainData;
        }

        /// <summary>
        /// Converts the given problem AST into a root problem structure.
        /// </summary>
        /// <param name="domainContext">Corresponding domain context.</param>
        /// <param name="problemAstNode">AST node.</param>
        /// <returns>Converted problem.</returns>
        public static Problem ToProblem(Domain domainContext, ProblemAstNode problemAstNode)
        {
            ToProblemConverter visitor = new ToProblemConverter();
            SetDomainEvaluationContext(domainContext);
            visitor.Evaluate(problemAstNode);
            ClearDomainEvaluationContext();
            return visitor.ProblemData;
        }
    }
}
