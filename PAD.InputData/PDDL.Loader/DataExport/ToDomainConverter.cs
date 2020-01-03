using System.Collections.Generic;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Loader.DataExport
{
    /// <summary>
    /// Specific converter of an AST into a domain structure.
    /// </summary>
    public class ToDomainConverter : BaseAstVisitor
    {
        /// <summary>
        /// Loaded data to be returned.
        /// </summary>
        public Domain DomainData { get; } = new Domain();

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(DomainAstNode astNode)
        {
            DomainData.Name = astNode.DomainName;
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(DomainRequirementsAstNode astNode)
        {
            astNode.RequirementsList.ForEach(requirement => DomainData.Requirements.Add(requirement));

            if (DomainData.Requirements.Count == 0)
            {
                // :strips is the default requirement, if no other specified
                DomainData.Requirements.Add(Traits.Requirement.STRIPS);
            }
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(DomainTypesAstNode astNode)
        {
            astNode.TypesList.TypedIdentifiers.ForEach(type => DomainData.Types.Add(new Type(type.Item1, type.Item2.Split(';'))));

            // add implied types (base types that are not specifically defined)
            HashSet<string> impliedTypes = new HashSet<string>();
            foreach (var type in DomainData.Types)
            {
                foreach (var baseType in type.BaseTypeNames)
                {
                    if (baseType.Equals("") || baseType.Equals("object"))
                    {
                        continue;
                    }

                    if (!DomainData.Types.Exists(existingTypes => existingTypes.TypeName.Equals(baseType)))
                    {
                        impliedTypes.Add(baseType);
                    }
                }
            }

            foreach (var type in impliedTypes)
            {
                DomainData.Types.Add(new Type(type));
            }
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(DomainConstantsAstNode astNode)
        {
            astNode.ConstantsList.TypedIdentifiers.ForEach(constant => DomainData.Constants.Add(new Constant(constant.Item1, constant.Item2.Split(';'))));
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(DomainPredicatesAstNode astNode)
        {
            foreach (var predicateElem in astNode.PredicatesList)
            {
                Predicate newPredicate = new Predicate(predicateElem.Name);
                predicateElem.Arguments.TypedIdentifiers.ForEach(termElem => newPredicate.Terms.Add(new DefinitionTerm(termElem.Item1, termElem.Item2.Split(';'))));
                DomainData.Predicates.Add(newPredicate);
            }
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(DomainFunctionsAstNode astNode)
        {
            foreach (var functionElem in astNode.FunctionTypedList.FunctionsList)
            {
                Function newFunction = new Function(functionElem.Item1, functionElem.Item3.Split(';'));
                functionElem.Item2.TypedIdentifiers.ForEach(termElem => newFunction.Terms.Add(new DefinitionTerm(termElem.Item1, termElem.Item2.Split(';'))));
                DomainData.Functions.Add(newFunction);
            }
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(DomainConstraintsAstNode astNode)
        {
            DomainData.Constraints = MasterExporter.ToConstraints(astNode.Expression);
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(DomainActionAstNode astNode)
        {
            Action newAction = new Action
            {
                Name = astNode.Name,
                Parameters = MasterExporter.ToParameters(astNode.Parameters),
                Preconditions = MasterExporter.ToPreconditions(astNode.Preconditions),
                Effects = MasterExporter.ToEffects(astNode.Effects)
            };
            DomainData.Actions.Add(newAction);
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(DomainDurActionAstNode astNode)
        {
            DurativeAction newDurativeAction = new DurativeAction
            {
                Name = astNode.Name,
                Parameters = MasterExporter.ToParameters(astNode.Parameters),
                Durations = MasterExporter.ToDurativeConstraints(astNode.DurationConstraint),
                Conditions = MasterExporter.ToDurativeConditions(astNode.Condition),
                Effects = MasterExporter.ToDurativeEffects(astNode.Effect)
            };
            DomainData.DurativeActions.Add(newDurativeAction);
        }

        /// <summary>
        /// Handles the AST node visit.
        /// </summary>
        /// <param name="astNode">AST node.</param>
        public override void Visit(DomainDerivedPredAstNode astNode)
        {
            DerivedPredicate newDerivedPredicate = new DerivedPredicate
            {
                Expression = MasterExporter.ToExpression(astNode.Expression),
                Predicate = new Predicate(astNode.Predicate.Name)
            };
            astNode.Predicate.Arguments.TypedIdentifiers.ForEach(termElem => newDerivedPredicate.Predicate.Terms.Add(new DefinitionTerm(termElem.Item1, termElem.Item2.Split(';'))));
            DomainData.DerivedPredicates.Add(newDerivedPredicate);
        }
    }
}
