using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into a problem structure.
    /// </summary>
    public class ToProblemConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public Problem ProblemData { get; private set; } = new Problem();

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ProblemAstNode astNode)
        {
            ProblemData.Name = astNode.ProblemName;
            ProblemData.DomainName = astNode.CorrespondingDomainName;
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ProblemRequirementsAstNode astNode)
        {
            astNode.RequirementsList.ForEach(requirement => ProblemData.Requirements.Add(requirement));

            if (ProblemData.Requirements.Count == 0)
            {
                ProblemData.Requirements.Add(Traits.Requirement.STRIPS);
            }
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ProblemObjectsAstNode astNode)
        {
            astNode.ObjectsList.TypedIdentifiers.ForEach(objElem => ProblemData.Objects.Add(new Object(objElem.Item1, objElem.Item2.Split(';'))));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ProblemInitAstNode astNode)
        {
            astNode.InitElemList.ForEach(initElem => ProblemData.Init.Add(MasterExporter.ToInitElement(initElem)));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ProblemGoalAstNode astNode)
        {
            ProblemData.Goal = MasterExporter.ToGoal(astNode);
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ProblemConstraintsAstNode astNode)
        {
            ProblemData.Constraints = MasterExporter.ToConstraints(astNode.Expression);
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ProblemMetricAstNode astNode)
        {
            ProblemData.Metric = new Metric(astNode.OptimizationSpecifier, MasterExporter.ToMetricExpression(astNode.MetricExpression));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(ProblemLengthAstNode astNode)
        {
            astNode.LengthSpecifications.ForEach(lengthElem => ProblemData.Length.Add(new LengthSpecElement(lengthElem.Item1, lengthElem.Item2)));
        }
    }
}
