using System.Collections.Generic;
using PAD.InputData.PDDL.Traits;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL.Validator
{
    /// <summary>
    /// Visitor for the PDDL input data, performing validation to the PDDL specification. That includes: correct specification of requirements, correct typing,
    /// correct use of action-costs, fluents, use of previously defined predicates, functions, constants etc. Throws ValidationException in case of a validation failure.
    /// </summary>
    public class ValidateVisitor : BaseVisitor
    {
        /// <summary>
        /// Domain context.
        /// </summary>
        private Domain DomainContext { set; get; }

        /// <summary>
        /// Problem context.
        /// </summary>
        private Problem ProblemContext { set; get; }

        /// <summary>
        /// Location specification in the input data (in case of a validation failure).
        /// </summary>
        private string Location { set; get; } = "";

        /// <summary>
        /// Currently valid parameters.
        /// </summary>
        private Parameters ActiveParameters { get; } = new Parameters();

        /// <summary>
        /// Checks the given domain data.
        /// </summary>
        /// <param name="domain">Domain input data.</param>
        public void CheckDomain(Domain domain)
        {
            DomainContext = domain;
            ProblemContext = new Problem();
            domain.Accept(this);
        }

        /// <summary>
        /// Checks the given problem data for the corresponding domain data.
        /// </summary>
        /// <param name="domain">Domain input data.</param>
        /// <param name="problem">Problem input data.</param>
        public void CheckProblem(Domain domain, Problem problem)
        {
            DomainContext = domain;
            ProblemContext = problem;
            problem.Accept(this);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Problem data)
        {
            if (!data.DomainName.EqualsNoCase(DomainContext.Name))
            {
                throw GetException($"The corresponding domain name of problem {data.Name} does not match with the given domain {DomainContext.Name}.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Type data)
        {
            if (!IsTypingEnabled())
            {
                throw GetException($"Definition of type {data.TypeName} present, but requirement :typing not specified.");
            }

            foreach (var baseType in data.BaseTypeNames)
            {
                if (baseType.EqualsNoCase("number"))
                {
                    throw GetException($"Type {data.TypeName} cannot be derived from the built-in type 'number'.");
                }

                if (IsCustomType(baseType) && !DomainContext.Types.Exists(type => type.TypeName.EqualsNoCase(baseType)))
                {
                    throw GetException($"Type {data.TypeName} is derived from an undefined type {baseType}.");
                }
            }

            if (data.TypeName.EqualsNoCase("number"))
            {
                throw GetException("Type 'number' is built-in type and cannot be redefined.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Constant data)
        {
            if (!IsTypingEnabled() && AreCustomTypes(data.TypeNames))
            {
                throw GetException($"Definition of constant {data.ConstantName} uses types, but requirement :typing not specified.");
            }

            if (IsTypingEnabled())
            {
                string invalidType;
                if (!AreTypesDefined(data.TypeNames, out invalidType))
                {
                    throw GetException($"Use of undefined type {invalidType} in {data.ConstantName} constant definition.");
                }
            }

            if (DomainContext.Functions.Exists(function => function.Name.EqualsNoCase(data.ConstantName) && function.Terms.Count == 0))
            {
                throw GetException($"Definition of constant {data.ConstantName} uses a name already used for a nullary function.");
            }

            if (ProblemContext.Objects.Exists(obj => obj.ObjectName.EqualsNoCase(data.ConstantName)))
            {
                throw GetException($"Definition of constant {data.ConstantName} uses a name already used for an object.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Predicate data)
        {
            if (DomainContext.Predicates.FindAll(predicate => predicate.Name.EqualsNoCase(data.Name) && predicate.Terms.Count == data.Terms.Count).Count > 1)
            {
                throw GetException($"Duplicate definition of predicate {data.Name}. The definitions have to differ in name or number of parameters.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Function data)
        {
            if (!IsTypingEnabled() && IsCustomReturnType(data.ReturnValueTypes))
            {
                throw GetException($"Definition of function {data.Name} uses a custom return type, but requirement :typing not specified.");
            }

            if (data.IsNumericFunction())
            {
                if (!IsNumericFluentsEnabled() && !IsActionCostsEnabled())
                {
                    throw GetException($"Numeric function {data.Name} defined, but neither :numeric-fluents or :action-costs requirements specified.");
                }
            }
            else if (!IsObjectFluentsEnabled())
            {
                throw GetException($"Object function {data.Name} defined, but requirement :object-fluents not specified.");
            }

            if (IsTypingEnabled())
            {
                string invalidType;
                if (!AreTypesDefined(data.ReturnValueTypes, out invalidType, true))
                {
                    throw GetException($"Use of undefined type {invalidType} in function definition.");
                }
            }

            if (DomainContext.Functions.FindAll(function => function.Name.EqualsNoCase(data.Name) && function.Terms.Count == data.Terms.Count).Count > 1)
            {
                throw GetException($"Duplicate definition of function {data.Name}. The definitions have to differ in name or number of parameters.");
            }

            if (data.Terms.Count == 0 && DomainContext.Constants.Exists(constant => constant.ConstantName.EqualsNoCase(data.Name)))
            {
                throw GetException($"Definition of nullary function {data.Name} uses a name already used for a constant.");
            }

            if (data.Terms.Count == 0 && ProblemContext.Objects.Exists(obj => obj.ObjectName.EqualsNoCase(data.Name)))
            {
                throw GetException($"Definition of nullary function {data.Name} uses a name already used for an object.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(DefinitionTerm data)
        {
            if (!IsTypingEnabled() && AreCustomTypes(data.TypeNames))
            {
                throw GetException($"Term {data.TermName} in a predicate or function definition uses types, but requirement :typing not specified.");
            }

            if (IsTypingEnabled())
            {
                string invalidType;
                if (!AreTypesDefined(data.TypeNames, out invalidType))
                {
                    throw GetException($"Use of undefined type {invalidType} in predicate or function definition.");
                }
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Action data)
        {
            if (DomainContext.Actions.FindAll(action => action.Name.EqualsNoCase(data.Name) && action.Parameters.Count == data.Parameters.Count).Count > 1)
            {
                throw GetException($"Duplicate definition of {data.Name} action. Action names or number of parameters have to differ.");
            }

            if (!AreParametersUnique(data.Parameters))
            {
                throw GetException($"Parameters specification of action {data.Name} contains duplicate variable names.");
            }

            AddActiveParameters(data.Parameters);
            Location = $"Action {data.Name}";
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(Action data)
        {
            RemoveFromActiveParameters(data.Parameters);
            Location = "";
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(DurativeAction data)
        {
            if (!IsDurativeActionsEnabled())
            {
                throw GetException($"Durative action {data.Name} defined, but requirement :durative-actions not specified.");
            }

            if (DomainContext.DurativeActions.FindAll(action => action.Name.EqualsNoCase(data.Name) && action.Parameters.Count == data.Parameters.Count).Count > 1)
            {
                throw GetException($"Duplicate definition of {data.Name} durative action. Action names or number of parameters have to differ.");
            }

            if (!AreParametersUnique(data.Parameters))
            {
                throw GetException($"Parameters specification of durative action {data.Name} contains duplicate variable names.");
            }

            AddActiveParameters(data.Parameters);
            Location = $"Durative-action {data.Name}";
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(DurativeAction data)
        {
            RemoveFromActiveParameters(data.Parameters);
            Location = "";
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(DerivedPredicate data)
        {
            if (!IsDerivedPredicateEnabled())
            {
                throw GetException($"Derived predicate {data.Predicate.Name} defined, but requirement :derived-predicates not specified.");
            }

            if (!DomainContext.Predicates.Exists(predicate => predicate.Name.EqualsNoCase(data.Predicate.Name) && predicate.Terms.Count == data.Predicate.Terms.Count))
            {
                throw GetException($"Derived predicate {data.Predicate.Name} specified, but the corresponding predicate not defined.");
            }

            AddActiveParameters(TermsToParameters(data.Predicate.Terms));
            Location = $"Derived-predicate {data.Predicate.Name}";
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(DerivedPredicate data)
        {
            RemoveFromActiveParameters(TermsToParameters(data.Predicate.Terms));
            Location = "";
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Object data)
        {
            if (!IsTypingEnabled() && AreCustomTypes(data.TypeNames))
            {
                throw GetException($"Definition of object {data.ObjectName} uses types, but requirement :typing not specified.");
            }

            if (IsTypingEnabled())
            {
                string invalidType;
                if (!AreTypesDefined(data.TypeNames, out invalidType))
                {
                    throw GetException($"Use of undefined type {invalidType} in {data.ObjectName} object definition.");
                }
            }

            if (DomainContext.Constants.Exists(constant => constant.ConstantName.EqualsNoCase(data.ObjectName)))
            {
                throw GetException($"Definition of object {data.ObjectName} uses a name already used for a constant.");
            }

            if (DomainContext.Functions.Exists(function => function.Name.EqualsNoCase(data.ObjectName) && function.Terms.Count == 0))
            {
                throw GetException($"Definition of object {data.ObjectName} uses a name already used for a nullary function.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Metric data)
        {
            if (data.OptimizationSpecifier == OptimizationSpecifier.NONE)
            {
                return;
            }

            if (!IsNumericFluentsEnabled())
            {
                if (IsActionCostsEnabled())
                {
                    if (IsDurativeActionsEnabled())
                    {
                        if (data.OptimizationSpecifier != OptimizationSpecifier.MINIMIZE || !IsLinearCombiOfTotalCostAndTotalTime(data.Expression))
                        {
                            throw GetException("The only acceptable metric for action-costs is to minimize linear combination of total-cost and total-time fluents.");
                        }
                    }
                    else
                    {
                        if (data.OptimizationSpecifier != OptimizationSpecifier.MINIMIZE || !IsTotalCostFunction(data.Expression))
                        {
                            throw GetException("The only acceptable metric for action-costs is to minimize special total-cost fluent.");
                        }
                    }
                }
                else
                {
                    throw GetException("Problem metric defined, but requirement :numeric-fluents not specified.");
                }
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(Parameter data)
        {
            if (!IsTypingEnabled() && AreCustomTypes(data.TypeNames))
            {
                throw GetException($"Parameter {data.ParameterName} uses types, but requirement :typing not specified.");
            }

            if (IsTypingEnabled())
            {
                string invalidType;
                if (!AreTypesDefined(data.TypeNames, out invalidType))
                {
                    throw GetException($"Use of undefined type {invalidType} in parameters list.");
                }
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(ConstantTerm data)
        {
            if (!IsConstantDefined(data.Name))
            {
                throw GetException($"Use of undefined constant/object {data.Name} in an expression.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(VariableTerm data)
        {
            if (!IsVariableDefined(data.Name))
            {
                throw GetException($"Use of undefined variable {data.Name} in an expression.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(ObjectFunctionTerm data)
        {
            if (!IsObjectFluentsEnabled())
            {
                throw GetException("Object function term used, but requirement :object-fluents not specified.");
            }

            if (!IsObjectFunctionDefined(data.Name, data.Terms.Count))
            {
                throw GetException($"Use of undefined object function {data.Name} in an expression.");
            }
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(ObjectFunctionTerm data)
        {
            if (!DoFunctionArgumentsMatchRequiredType(data.Name, GetTermTypes(data.Terms)))
            {
                throw GetException($"One or more arguments of the object function term {data.Name} not matching required function parameter type(s).");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(PreferenceConstraint data)
        {
            if (!IsPreferencesEnabled())
            {
                throw GetException("Preference constraint expression used, but requirement :preferences not specified.");
            }

            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(ForallConstraint data)
        {
            if (!IsUniversalPreconditionsEnabled())
            {
                throw GetException("Forall constraint expression used, but requirement :universal-preconditions not specified.");
            }

            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }

            if (!AreParametersUnique(data.Parameters))
            {
                throw GetException("Parameters specification of forall constraint contains duplicate variable names.");
            }

            AddActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(ForallConstraint data)
        {
            RemoveFromActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(AtEndConstraint data)
        {
            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(AlwaysConstraint data)
        {
            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(SometimeConstraint data)
        {
            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(WithinConstraint data)
        {
            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(AtMostOnceConstraint data)
        {
            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(SometimeAfterConstraint data)
        {
            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(SometimeBeforeConstraint data)
        {
            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(AlwaysWithinConstraint data)
        {
            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(HoldDuringConstraint data)
        {
            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(HoldAfterConstraint data)
        {
            if (!IsConstraintsEnabled())
            {
                throw GetException("Constraints defined, but requirement :constraints not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(PreferenceExpression data)
        {
            if (!IsPreferencesEnabled())
            {
                throw GetException("Preference expression used, but requirement :preferences not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(PredicateExpression data)
        {
            if (!IsPredicateDefined(data.Name, data.Terms.Count))
            {
                throw GetException($"Use of undefined predicate {data.Name} in an expression.");
            }
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(PredicateExpression data)
        {
            if (!DoPredicateArgumentsMatchRequiredType(data.Name, GetTermTypes(data.Terms)))
            {
                throw GetException($"One or more arguments of the predicate expression {data.Name} not matching required predicate parameter type(s).");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(EqualsExpression data)
        {
            if (!IsEqualityEnabled())
            {
                throw GetException("Equals expression used, but requirement :equality not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(OrExpression data)
        {
            if (!IsDisjunctivePreconditionsEnabled())
            {
                throw GetException("Or expression used, but requirement :disjunctive-preconditions not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(NotExpression data)
        {
            if (data.Argument is AtomicFormulaExpression)
            {
                if (!IsNegativePreconditionsEnabled())
                {
                    throw GetException("Not literal used, but requirement :negative-preconditions not specified.");
                }
            }
            else if (!IsDisjunctivePreconditionsEnabled())
            {
                throw GetException("Not expression used, but requirement :disjunctive-preconditions not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(ImplyExpression data)
        {
            if (!IsDisjunctivePreconditionsEnabled())
            {
                throw GetException("Imply expression used, but requirement :disjunctive-preconditions not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(ExistsExpression data)
        {
            if (!IsExistentialPreconditionsEnabled())
            {
                throw GetException("Exists expression used, but requirement :existential-preconditions not specified.");
            }

            if (!AreParametersUnique(data.Parameters))
            {
                throw GetException("Parameters specification of exists expression contains duplicate variable names.");
            }

            AddActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(ExistsExpression data)
        {
            RemoveFromActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(ForallExpression data)
        {
            if (!IsUniversalPreconditionsEnabled())
            {
                throw GetException("Forall expression used, but requirement :universal-preconditions not specified.");
            }

            if (!AreParametersUnique(data.Parameters))
            {
                throw GetException("Parameters specification of forall expression contains duplicate variable names.");
            }

            AddActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(ForallExpression data)
        {
            RemoveFromActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(NumericCompareExpression data)
        {
            if (!IsNumericFluentsEnabled())
            {
                throw GetException("Numeric compare expression used, but requirement :numeric-fluents not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(NumericFunction data)
        {
            if (!IsNumericFunctionDefined(data.Name, data.Terms.Count))
            {
                throw GetException($"Use of undefined numeric function {data.Name}.");
            }
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(NumericFunction data)
        {
            if (!DoFunctionArgumentsMatchRequiredType(data.Name, GetTermTypes(data.Terms)))
            {
                throw GetException($"One or more arguments of the numeric function {data.Name} not matching required function parameter type(s).");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(DurationVariable data)
        {
            if (!IsDurationInequalitiesEnabled())
            {
                throw GetException("Duration variable used, but requirement :duration-inequalities not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(ForallEffect data)
        {
            if (!IsConditionalEffectsEnabled())
            {
                throw GetException("Forall effect used, but requirement :conditional-effects not specified.");
            }

            if (!AreParametersUnique(data.Parameters))
            {
                throw GetException("Parameters specification of forall effect contains duplicate variable names.");
            }

            AddActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(ForallEffect data)
        {
            RemoveFromActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(WhenEffect data)
        {
            if (!IsConditionalEffectsEnabled())
            {
                throw GetException("When effect used, but requirement :conditional-effects not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(PredicateEffect data)
        {
            if (!IsPredicateDefined(data.Name, data.Terms.Count))
            {
                throw GetException($"Use of undefined predicate {data.Name} in an effect.");
            }
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(PredicateEffect data)
        {
            if (!DoPredicateArgumentsMatchRequiredType(data.Name, GetTermTypes(data.Terms)))
            {
                throw GetException($"One or more arguments of the predicate effect {data.Name} not matching required predicate parameter type(s).");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(EqualsEffect data)
        {
            if (!IsEqualityEnabled())
            {
                throw GetException("Equals effect used, but requirement :equality not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(NumericAssignEffect data)
        {
            if (!IsNumericFluentsEnabled())
            {
                if (data.AssignOperator == AssignOperator.INCREASE && IsFunctionSpecialTotalCost(data.Function) && IsActionCostsEnabled())
                {
                    if (!IsCorrectActionCostsNumericExpressionToAssign(data.Value))
                    {
                        throw GetException($"Action-cost numeric function {data.Function.Name} can only have assigned non-negative number or another action-cost function.");
                    }
                }
                else
                {
                    throw GetException($"Assignment effect to numeric function {data.Function.Name} used, but requirement :numeric-fluents not specified.");
                }
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(ObjectAssignEffect data)
        {
            if (!IsObjectFluentsEnabled())
            {
                throw GetException($"Assignment effect to object function {data.Function.Name} used, but requirement :object-fluents not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(ObjectAssignEffect data)
        {
            if (!DoObjectFunctionTermMatchAssignedTermType(data.Function.Name, data.Function.Terms.Count, data.Value))
            {
                throw GetException($"Type of the assigned term {data.Value} does not match the return type of function {data.Function.Name} in the assignment effect.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(CompareDurativeConstraint data)
        {
            if (DomainContext.Constraints.Count > 1 && !IsDurationInequalitiesEnabled())
            {
                throw GetException("Multiple duration constraints used, but requirement :duration-inequalities not specified.");
            }

            if (data.DurationComparer == DurationComparer.LTE || data.DurationComparer == DurationComparer.GTE)
            {
                if (!IsDurationInequalitiesEnabled())
                {
                    throw GetException("Inequalities used in duration constraints, but requirement :duration-inequalities not specified.");
                }
            }

            if (!(data.Value is Number) && !IsNumericFluentsEnabled())
            {
                throw GetException("Duration constraint is compared to a general numeric expression, but requirement :numeric-fluents not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(AtDurativeConstraint data)
        {
            if (DomainContext.Constraints.Count > 1 && !IsDurationInequalitiesEnabled())
            {
                throw GetException("Multiple duration constraints used, but requirement :duration-inequalities not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(ForallDurativeExpression data)
        {
            if (!IsUniversalPreconditionsEnabled())
            {
                throw GetException("Forall durative expression used, but requirement :universal-preconditions not specified.");
            }

            if (!AreParametersUnique(data.Parameters))
            {
                throw GetException("Parameters specification of forall durative expression contains duplicate variable names.");
            }

            AddActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(ForallDurativeExpression data)
        {
            RemoveFromActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(PreferencedTimedExpression data)
        {
            if (!IsPreferencesEnabled())
            {
                throw GetException("Preference timed expression used, but requirement :preferences not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(ForallDurativeEffect data)
        {
            if (!IsConditionalEffectsEnabled())
            {
                throw GetException("Forall durative effect used, but requirement :conditional-effects not specified.");
            }

            if (!AreParametersUnique(data.Parameters))
            {
                throw GetException("Parameters specification of forall durative effect contains duplicate variable names.");
            }

            AddActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(ForallDurativeEffect data)
        {
            RemoveFromActiveParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(WhenDurativeEffect data)
        {
            if (!IsConditionalEffectsEnabled())
            {
                throw GetException("Forall durative effect used, but requirement :conditional-effects not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(AssignTimedEffect data)
        {
            if (!IsNumericFluentsEnabled() || !IsContinuousEffectsEnabled())
            {
                throw GetException("Timed effect assignment used, but requirement :numeric-fluents or :continuous-effects not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(CompoundTimedNumericExpression data)
        {
            if (!IsNumericFluentsEnabled())
            {
                throw GetException("Numeric expression used in timed effects, but requirement :numeric-fluents not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(PredicateInitElement data)
        {
            if (!IsPredicateDefined(data.Name, data.Terms.Count))
            {
                throw GetException($"Use of undefined predicate {data.Name} as init element.");
            }
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(PredicateInitElement data)
        {
            if (!DoPredicateArgumentsMatchRequiredType(data.Name, GetTermTypes(data.Terms)))
            {
                throw GetException($"One or more arguments of the predicate init element {data.Name} not matching required predicate parameter type(s).");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(EqualsInitElement data)
        {
            if (!IsEqualityEnabled())
            {
                throw GetException("Equals literal used as init element, but requirement :equality not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(AtInitElement data)
        {
            if (!IsTimedInitialLiteralsEnabled())
            {
                throw GetException("Timed literal used as init element, but requirement :timed-initial-literals not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(EqualsNumericFunctionInitElement data)
        {
            if (!IsNumericFluentsEnabled())
            {
                if (IsActionCostsEnabled())
                {
                    if (IsFunctionSpecialTotalCost(data.Function) && !data.Number.Equals(0.0))
                    {
                        throw GetException("If the special numeric fluent total-cost is used as an action-cost, it has to be inited to zero value.");
                    }

                    if (data.Number < 0.0)
                    {
                        throw GetException($"Action cost numeric fluent {data.Function.Name} has to inited to non-negative value.");
                    }
                }
                else
                {
                    throw GetException("Equals numeric assignment used as init element, but requirement :numeric-fluents not specified.");
                }
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(EqualsObjectFunctionInitElement data)
        {
            if (!IsObjectFluentsEnabled())
            {
                throw GetException("Equals object assignment used as init element, but requirement :object-fluents not specified.");
            }
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(EqualsObjectFunctionInitElement data)
        {
            if (!DoObjectFunctionTermMatchAssignedTermType(data.Function.Name, data.Function.Terms.Count, data.Term))
            {
                throw GetException($"Type of the initializing term {data.Term} does not match the return type of function {data.Function.Name}.");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(BasicNumericFunctionTerm data)
        {
            if (!IsNumericFunctionDefined(data.Name, data.Terms.Count))
            {
                throw GetException($"Use of undefined numeric function {data.Name}.");
            }
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(BasicNumericFunctionTerm data)
        {
            if (!DoFunctionArgumentsMatchRequiredType(data.Name, GetTermTypes(data.Terms)))
            {
                throw GetException($"One or more arguments of the numeric function {data.Name} not matching required function parameter type(s).");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(BasicObjectFunctionTerm data)
        {
            if (!IsObjectFunctionDefined(data.Name, data.Terms.Count))
            {
                throw GetException($"Use of undefined object function {data.Name}.");
            }
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(BasicObjectFunctionTerm data)
        {
            if (!DoFunctionArgumentsMatchRequiredType(data.Name, GetTermTypes(data.Terms)))
            {
                throw GetException($"One or more arguments of the object function {data.Name} not matching required function parameter type(s).");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(MetricNumericFunction data)
        {
            if (!IsNumericFunctionDefined(data.Name, data.Terms.Count))
            {
                throw GetException($"Use of undefined numeric function {data.Name}.");
            }
        }

        /// <summary>
        /// Visits the given input data node (when leaving the node).
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(MetricNumericFunction data)
        {
            if (!DoFunctionArgumentsMatchRequiredType(data.Name, GetTermTypes(data.Terms)))
            {
                throw GetException($"One or more arguments of the numeric function {data.Name} not matching required function parameter type(s).");
            }
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(MetricPreferenceViolation data)
        {
            if (!IsPreferencesEnabled())
            {
                throw GetException("Preference metric expression used, but requirement :preferences not specified.");
            }
        }

        /// <summary>
        /// Checks whether typing is enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsTypingEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.TYPING);
        }

        /// <summary>
        /// Checks whether numeric fluents are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsNumericFluentsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.NUMERIC_FLUENTS);
        }

        /// <summary>
        /// Checks whether object fluents are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsObjectFluentsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.OBJECT_FLUENTS);
        }

        /// <summary>
        /// Checks whether constraints are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsConstraintsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.CONSTRAINTS);
        }

        /// <summary>
        /// Checks whether durative actions are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsDurativeActionsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.DURATIVE_ACTIONS);
        }

        /// <summary>
        /// Checks whether derived predicates are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsDerivedPredicateEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.DERIVED_PREDICATES);
        }

        /// <summary>
        /// Checks whether action costs are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsActionCostsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.ACTIONS_COSTS);
        }

        /// <summary>
        /// Checks whether universal preconditions are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsUniversalPreconditionsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.UNIVERSAL_PRECONDITIONS);
        }

        /// <summary>
        /// Checks whether existential preconditions are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsExistentialPreconditionsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.EXISTENTIAL_PRECONDITIONS);
        }

        /// <summary>
        /// Checks whether negative preconditions are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsNegativePreconditionsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.NEGATIVE_PRECONDITIONS);
        }

        /// <summary>
        /// Checks whether disjunctive preconditions are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsDisjunctivePreconditionsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.DISJUNCTIVE_PRECONDITIONS);
        }

        /// <summary>
        /// Checks whether preferences are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsPreferencesEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.PREFERENCES);
        }

        /// <summary>
        /// Checks whether equality is enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsEqualityEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.EQUALITY);
        }

        /// <summary>
        /// Checks whether conditional effects are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsConditionalEffectsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.CONDITIONAL_EFFECTS);
        }

        /// <summary>
        /// Checks whether duration inequalities are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsDurationInequalitiesEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.DURATION_INEQUALITIES);
        }

        /// <summary>
        /// Checks whether continuous effects are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsContinuousEffectsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.CONTINUOUS_EFFECTS);
        }

        /// <summary>
        /// Checks whether timed initial literals are enabled.
        /// </summary>
        /// <returns>True if the requirements is specified in the domain.</returns>
        private bool IsTimedInitialLiteralsEnabled()
        {
            return DomainContext.Requirements.Contains(Requirement.TIMED_INITIAL_LITERALS);
        }

        /// <summary>
        /// Checks whether the given type(s) are specified and differs from the built-in default type ('object').
        /// </summary>
        /// <param name="types">Type(s) to be checked.</param>
        /// <returns>True if all the types is user-specified.</returns>
        private static bool AreCustomTypes(List<string> types)
        {
            return types.TrueForAll(IsCustomType);
        }

        /// <summary>
        /// Checks whether the given type is specified and differs from the built-in default type ('object').
        /// </summary>
        /// <param name="type">Type to be checked.</param>
        /// <returns>True if the type is user-specified.</returns>
        private static bool IsCustomType(string type)
        {
            return (type.Length != 0 && !type.EqualsNoCase("object"));
        }

        /// <summary>
        /// Checks whether the given return type(s) are specified and differs from the built-in default type ('number').
        /// </summary>
        /// <param name="types">Type(s) to be checked.</param>
        /// <returns>True if the type is user-specified.</returns>
        private static bool IsCustomReturnType(List<string> types)
        {
            return types.TrueForAll(type => (type.Length != 0 && !type.EqualsNoCase("number")));
        }

        /// <summary>
        /// Checks whether the given numeric function is a total-cost function.
        /// </summary>
        /// <param name="function">Numeric function to be checked.</param>
        /// <returns>True if the function is 'total-cost'.</returns>
        private static bool IsFunctionSpecialTotalCost(NumericFunction function)
        {
            return IsFunctionTotalCostFluent(function.Name, function.Terms.Count);
        }

        /// <summary>
        /// Checks whether the given numeric function is a total-cost function.
        /// </summary>
        /// <param name="function">Numeric function to be checked.</param>
        /// <returns>True if the function is 'total-cost'.</returns>
        private static bool IsFunctionSpecialTotalCost(BasicNumericFunctionTerm function)
        {
            return IsFunctionTotalCostFluent(function.Name, function.Terms.Count);
        }

        /// <summary>
        /// Checks whether the given metric expression is a total-cost function.
        /// </summary>
        /// <param name="expression">Metric expression to be checked.</param>
        /// <returns>True if the expression is 'total-cost' function.</returns>
        private static bool IsTotalCostFunction(MetricExpression expression)
        {
            MetricNumericFunction function = expression as MetricNumericFunction;
            if (function == null)
            {
                return false;
            }

            return IsFunctionTotalCostFluent(function.Name, function.Terms.Count);
        }

        /// <summary>
        /// Checks whether the given numeric function is a total-cost function.
        /// </summary>
        /// <param name="functionName">Name of the numeric function to be checked.</param>
        /// <param name="termsCount">Terms count of the numeric function to be checked.</param>
        /// <returns>True if the function is 'total-cost'.</returns>
        private static bool IsFunctionTotalCostFluent(string functionName, int termsCount)
        {
            return functionName.EqualsNoCase("total-cost") && termsCount == 0;
        }

        /// <summary>
        /// Checks whether the given metric expression is a linear combination of total-cost and total-time functions.
        /// </summary>
        /// <param name="expression">Metric expression to be checked.</param>
        /// <returns>True if the expression is a linear combination of total-cost and total-time.</returns>
        private static bool IsLinearCombiOfTotalCostAndTotalTime(MetricExpression expression)
        {
            ActionCostMetricExpressionChecker visitor = new ActionCostMetricExpressionChecker();
            visitor.Evaluate(expression);
            return visitor.IsCorrect;
        }

        /// <summary>
        /// Checks whether the given numeric expression is a correct value to be assigned to an action-cost fluent.
        /// </summary>
        /// <param name="expression">Expression to be checked.</param>
        /// <returns>True if the expression is a valid value for an action-cost assignment.</returns>
        private static bool IsCorrectActionCostsNumericExpressionToAssign(NumericExpression expression)
        {
            Number number = expression as Number;
            if (number != null)
            {
                return (number.Value >= 0.0);
            }

            NumericFunction function = expression as NumericFunction;
            if (function != null)
            {
                return function.Terms.TrueForAll(term => (term is ConstantTerm || term is VariableTerm));
            }

            return false;
        }

        /// <summary>
        /// Checks whether the types from the given list have been previously defined.
        /// </summary>
        /// <param name="typeNames">Types to be checked.</param>
        /// <param name="invalidType">Output parameter for an invalid type name (if found).</param>
        /// <param name="isReturnType">Are the checked types function return types?</param>
        /// <returns>True if all the types have been previously defined.</returns>
        private bool AreTypesDefined(List<string> typeNames, out string invalidType, bool isReturnType = false)
        {
            invalidType = "";

            foreach (var singleTypeName in typeNames)
            {
                if (isReturnType && singleTypeName.EqualsNoCase("number"))
                {
                    invalidType = singleTypeName;
                    return (typeNames.Count == 1);
                }

                if (singleTypeName.EqualsNoCase("object"))
                {
                    continue;
                }

                if (!DomainContext.Types.Exists(type => type.TypeName.EqualsNoCase(singleTypeName)))
                {
                    invalidType = singleTypeName;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether the given predicate has been previously defined.
        /// </summary>
        /// <param name="predicateName">Name of the predicate to be checked.</param>
        /// <param name="termsCount">Terms count of the predicate to be checked.</param>
        /// <returns>True if the predicate has been previously defined.</returns>
        private bool IsPredicateDefined(string predicateName, int termsCount)
        {
            return DomainContext.Predicates.Exists(predicate => predicate.Name.EqualsNoCase(predicateName) && predicate.Terms.Count == termsCount);
        }

        /// <summary>
        /// Checks whether the given object function has been previously defined.
        /// </summary>
        /// <param name="functionName">Name of the predicate to be checked.</param>
        /// <param name="termsCount">Terms count of the predicate to be checked.</param>
        /// <returns>True if the object function has been previously defined.</returns>
        private bool IsObjectFunctionDefined(string functionName, int termsCount)
        {
            return DomainContext.Functions.Exists(function => function.Name.EqualsNoCase(functionName)
                && function.Terms.Count == termsCount && !function.IsNumericFunction());
        }

        /// <summary>
        /// Checks whether the given numeric function has been previously defined.
        /// </summary>
        /// <param name="functionName">Name of the predicate to be checked.</param>
        /// <param name="termsCount">Terms count of the predicate to be checked.</param>
        /// <returns>True if the numeric function has been previously defined.</returns>
        private bool IsNumericFunctionDefined(string functionName, int termsCount)
        {
            return DomainContext.Functions.Exists(function => function.Name.EqualsNoCase(functionName)
                && function.Terms.Count == termsCount && function.IsNumericFunction());
        }

        /// <summary>
        /// Checks whether the given constant has been previously defined.
        /// </summary>
        /// <param name="constantName">Name of the constant to be checked.</param>
        /// <returns>True if the constant has been previously defined.</returns>
        private bool IsConstantDefined(string constantName)
        {
            return DomainContext.Constants.Exists(constant => constant.ConstantName.EqualsNoCase(constantName))
                || ProblemContext.Objects.Exists(obj => obj.ObjectName.EqualsNoCase(constantName));
        }

        /// <summary>
        /// Checks whether the given variable has been previously defined, i.e. is valid in the current context.
        /// </summary>
        /// <param name="variableName">Name of the variable to be checked.</param>
        /// <returns>True if the variable has been previously defined.</returns>
        private bool IsVariableDefined(string variableName)
        {
            return ActiveParameters.Exists(parameter => parameter.ParameterName.EqualsNoCase(variableName));
        }

        /// <summary>
        /// Checks whether each parameter of the list has unique name.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <returns>True if all parameters of the list are unique.</returns>
        private static bool AreParametersUnique(Parameters parameters)
        {
            return parameters.TrueForAll(param => parameters.FindAll(referenceParam => referenceParam.ParameterName.EqualsNoCase(param.ParameterName)).Count == 1);
        }

        /// <summary>
        /// Adds the parameters to the currently active parameters (e.g. action parameters or forall-expression parameters)
        /// </summary>
        /// <param name="parameters">Parameters to be added.</param>
        private void AddActiveParameters(Parameters parameters)
        {
            ActiveParameters.AddRange(parameters);
        }

        /// <summary>
        /// Removes the parameters from the currently active parameters (e.g. end of action or forall-expression).
        /// </summary>
        /// <param name="parameters">Parameters to be removed.</param>
        private void RemoveFromActiveParameters(Parameters parameters)
        {
            ActiveParameters.RemoveAll(parameters.Contains);
        }

        /// <summary>
        /// Converts terms structure into parameters structure.
        /// </summary>
        /// <param name="terms">Terms to be converted.</param>
        /// <returns>Parameters corresponding to the given terms.</returns>
        private static Parameters TermsToParameters(DefinitionTerms terms)
        {
            Parameters parameters = new Parameters();
            foreach (var term in terms)
            {
                parameters.Add(new Parameter(term.TermName, term.TypeNames.ToArray()));
            }
            return parameters;
        }

        /// <summary>
        /// Checks whether the function return type matches type of the assigned term.
        /// </summary>
        /// <param name="functionName">Name of the object function.</param>
        /// <param name="termsCount">Number of function terms.</param>
        /// <param name="term">Assigned term.</param>
        /// <returns>True if the function return type matches the type of assigned term.</returns>
        private bool DoObjectFunctionTermMatchAssignedTermType(string functionName, int termsCount, Term term)
        {
            var functionDef = DomainContext.Functions.Find(funcDef => funcDef.Name.EqualsNoCase(functionName) && funcDef.Terms.Count == termsCount);
            return DoTermTypesMatch(GetTermTypes(new Terms{term})[0], functionDef.ReturnValueTypes.ToArray());
        }

        /// <summary>
        /// Checks whether the predicate is called with arguments of correct types.
        /// </summary>
        /// <param name="predicateName">Predicate name.</param>
        /// <param name="termTypes">Types of the predicate terms.</param>
        /// <returns>True if the predicate arguments match required types.</returns>
        private bool DoPredicateArgumentsMatchRequiredType(string predicateName, List<string[]> termTypes)
        {
            var predicateDef = DomainContext.Predicates.Find(predicate => predicate.Name.EqualsNoCase(predicateName) && predicate.Terms.Count == termTypes.Count);
            return DoFunctionOrPredicateArgumentsMatchRequiredType(predicateDef.Terms, termTypes);
        }

        /// <summary>
        /// Checks whether the function is called with arguments of correct types.
        /// </summary>
        /// <param name="functionName">Function name.</param>
        /// <param name="termTypes">Types of the function terms.</param>
        /// <returns>True if the function arguments match required types.</returns>
        private bool DoFunctionArgumentsMatchRequiredType(string functionName, List<string[]> termTypes)
        {
            var functionDef = DomainContext.Functions.Find(function => function.Name.EqualsNoCase(functionName) && function.Terms.Count == termTypes.Count);
            return DoFunctionOrPredicateArgumentsMatchRequiredType(functionDef.Terms, termTypes);
        }

        /// <summary>
        /// Checks whether predicate/function terms match required types.
        /// </summary>
        /// <param name="terms">Terms of a predicate or function.</param>
        /// <param name="termTypes">Term types of a predicate or function.</param>
        /// <returns>True if the given terms match required types.</returns>
        private bool DoFunctionOrPredicateArgumentsMatchRequiredType(DefinitionTerms terms, List<string[]> termTypes)
        {
            var allowedTermTypes = new List<string[]>();
            terms.ForEach(term => allowedTermTypes.Add(term.TypeNames.ToArray()));

            for (int i = 0; i < termTypes.Count; ++i)
            {
                if (!DoTermTypesMatch(termTypes[i], allowedTermTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether used types of the used term match allowed types of a predicate/function argument.
        /// </summary>
        /// <param name="termTypes">Types of terms.</param>
        /// <param name="allowedTypes">Allowed types for the term.</param>
        /// <returns>True if used term types match allowed types.</returns>
        private bool DoTermTypesMatch(IEnumerable<string> termTypes, string[] allowedTypes)
        {
            foreach (var termType in termTypes)
            {
                foreach (var allowedType in allowedTypes)
                {
                    if (IsTypeChildOfType(termType, allowedType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether typeA is a sub-type of typeB (i.e. typeB is within base types of typeA).
        /// </summary>
        /// <param name="typeA">First type.</param>
        /// <param name="typeB">Second type.</param>
        /// <returns>True if typeA is a sub-type of typeB.</returns>
        private bool IsTypeChildOfType(string typeA, string typeB)
        {
            // trivial case
            if (typeA.EqualsNoCase(typeB))
            {
                return true;
            }

            // object type can ob only a subtype of itself
            if (typeA.EqualsNoCase("object") || typeA.Length == 0)
            {
                return false;
            }

            // check whether any base type is a sub-type of typeB
            var typeObj = DomainContext.Types.Find(type => type.TypeName.EqualsNoCase(typeA));
            foreach (var baseType in typeObj.BaseTypeNames)
            {
                if (IsTypeChildOfType(baseType, typeB))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets types of terms from the list of constant terms.
        /// </summary>
        /// <param name="constantTerms">List of terms.</param>
        /// <returns>List of types corresponding to the list of terms.</returns>
        private List<string[]> GetTermTypes(ConstantTerms constantTerms)
        {
            var terms = new Terms();
            constantTerms.ForEach(term => terms.Add(term));
            return GetTermTypes(terms);
        }

        /// <summary>
        /// Gets types of terms from the list of terms.
        /// </summary>
        /// <param name="terms">List of terms.</param>
        /// <returns>List of types corresponding to the list of terms.</returns>
        private List<string[]> GetTermTypes(Terms terms)
        {
            List<string[]> typesList = new List<string[]>();

            foreach (var term in terms)
            {
                var constantTerm = term as ConstantTerm;
                if (constantTerm != null)
                {
                    var constantDef = DomainContext.Constants.Find(constant => constant.ConstantName.EqualsNoCase(constantTerm.Name));
                    if (constantDef != null)
                    {
                        typesList.Add(constantDef.TypeNames.ToArray());
                        continue;
                    }

                    var objectDef = ProblemContext.Objects.Find(obj => obj.ObjectName.EqualsNoCase(constantTerm.Name));
                    typesList.Add(objectDef.TypeNames.ToArray());
                    continue;
                }

                var variableTerm = term as VariableTerm;
                if (variableTerm != null)
                {
                    var variableDef = ActiveParameters.Find(parameter => parameter.ParameterName.EqualsNoCase(variableTerm.Name));
                    typesList.Add(variableDef.TypeNames.ToArray());
                    continue;
                }

                var functionTerm = term as ObjectFunctionTerm;
                if (functionTerm != null)
                {
                    var functionDef = DomainContext.Functions.Find(function => function.Name.EqualsNoCase(functionTerm.Name) && function.Terms.Count == functionTerm.Terms.Count);
                    typesList.Add(functionDef.ReturnValueTypes.ToArray());
                }
            }

            return typesList;
        }

        /// <summary>
        /// Gets the validation exception to be thrown, with the location in the input data (if specified).
        /// </summary>
        /// <param name="reason">Reason of a validation failure.</param>
        /// <returns>Validation exception to be thrown.</returns>
        private ValidationException GetException(string reason)
        {
            string location = (Location.Length != 0) ? $"{Location}: " : "";
            return new ValidationException(location + reason);
        }
    }

    /// <summary>
    /// Visitor for the metric expression, performing simple validation whether the given expression is a linear combination of total-cost and total-time fluents.
    /// </summary>
    public class ActionCostMetricExpressionChecker : BaseVisitor
    {
        /// <summary>
        /// Has the validation succeeded?
        /// </summary>
        public bool IsCorrect { set; get; } = true;

        /// <summary>
        /// Evaluates the given metric expression.
        /// </summary>
        /// <param name="expression">Metric expression to be evaluated.</param>
        public void Evaluate(MetricExpression expression)
        {
            expression.Accept(this);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(MetricNumericFunction data)
        {
            if (data.Terms.Count != 0 || (!data.Name.EqualsNoCase("total-cost") && !data.Name.EqualsNoCase("total-time")))
            {
                IsCorrect = false;
            }
        }
    }
}
