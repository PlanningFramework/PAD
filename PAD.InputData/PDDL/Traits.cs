using System.Collections.Generic;
using System.Diagnostics;

namespace PAD.InputData.PDDL.Traits
{
    /// <summary>
    /// Duration comparer specification.
    /// </summary>
    public enum DurationComparer
    {
        LTE, GTE, EQ
    }

    /// <summary>
    /// Assign operator specification.
    /// </summary>
    public enum AssignOperator
    {
        ASSIGN, SCALE_UP, SCALE_DOWN, INCREASE, DECREASE
    }

    /// <summary>
    /// Assign operator for timed effect specification.
    /// </summary>
    public enum TimedEffectAssignOperator
    {
        INCREASE, DECREASE
    }

    /// <summary>
    /// Time specifier.
    /// </summary>
    public enum TimeSpecifier
    {
        START, END
    }

    /// <summary>
    /// Interval specififer.
    /// </summary>
    public enum IntervalSpecifier
    {
        ALL
    }

    /// <summary>
    /// Optimization specifier.
    /// </summary>
    public enum OptimizationSpecifier
    {
        NONE, MINIMIZE, MAXIMIZE
    }

    /// <summary>
    /// Length specifier.
    /// </summary>
    public enum LengthSpecifier
    {
        SERIAL, PARALLEL
    }

    /// <summary>
    /// Numeric comparer specifier.
    /// </summary>
    public enum NumericComparer
    {
        LT, LTE, GT, GTE, EQ
    }

    /// <summary>
    /// Numeric operator specifier.
    /// </summary>
    public enum NumericOperator
    {
        PLUS, MINUS, MUL, DIV
    }

    /// <summary>
    /// PDDL requirement specifier.
    /// </summary>
    public enum Requirement
    {
        STRIPS,
        TYPING,
        NEGATIVE_PRECONDITIONS,
        DISJUNCTIVE_PRECONDITIONS,
        EQUALITY,
        EXISTENTIAL_PRECONDITIONS,
        UNIVERSAL_PRECONDITIONS,
        QUANTIFIED_PRECONDITIONS,
        CONDITIONAL_EFFECTS,
        FLUENTS,
        NUMERIC_FLUENTS,
        OBJECT_FLUENTS,
        ADL,
        DURATIVE_ACTIONS,
        DURATION_INEQUALITIES,
        CONTINUOUS_EFFECTS,
        DERIVED_PREDICATES,
        TIMED_INITIAL_LITERALS,
        PREFERENCES,
        CONSTRAINTS,
        ACTIONS_COSTS
    }
}

namespace PAD.InputData.PDDL
{
    using PAD.InputData.PDDL.Traits;

    /// <summary>
    /// Static class for string conversion extensions.
    /// </summary>
    public static class ToStringExtensions
    {
        /// <summary>
        /// Converts the enum to string representation.
        /// </summary>
        /// <param name="comparer">Value to be converted.</param>
        /// <returns>String representation.</returns>
        public static string EnumToString(this NumericComparer comparer)
        {
            switch (comparer)
            {
                case NumericComparer.LT:
                    return "<";
                case NumericComparer.LTE:
                    return "<=";
                case NumericComparer.GT:
                    return ">";
                case NumericComparer.GTE:
                    return ">=";
                case NumericComparer.EQ:
                    return "=";
                default:
                    Debug.Assert(false);
                    return comparer.ToString();
            }
        }

        /// <summary>
        /// Converts the enum to string representation.
        /// </summary>
        /// <param name="assignOperator">Value to be converted.</param>
        /// <returns>String representation.</returns>
        public static string EnumToString(this AssignOperator assignOperator)
        {
            switch (assignOperator)
            {
                case AssignOperator.ASSIGN:
                    return "assign";
                case AssignOperator.SCALE_UP:
                    return "scale-up";
                case AssignOperator.SCALE_DOWN:
                    return "scale-down";
                case AssignOperator.INCREASE:
                    return "increase";
                case AssignOperator.DECREASE:
                    return "decrease";
                default:
                    Debug.Assert(false);
                    return assignOperator.ToString();
            }
        }

        /// <summary>
        /// Converts the enum to string representation.
        /// </summary>
        /// <param name="durationComparer">Value to be converted.</param>
        /// <returns>String representation.</returns>
        public static string EnumToString(this DurationComparer durationComparer)
        {
            switch (durationComparer)
            {
                case DurationComparer.LTE:
                    return "<=";
                case DurationComparer.GTE:
                    return ">=";
                case DurationComparer.EQ:
                    return "=";
                default:
                    Debug.Assert(false);
                    return durationComparer.ToString();
            }
        }

        /// <summary>
        /// Converts the enum to string representation.
        /// </summary>
        /// <param name="timeSpecifier">Value to be converted.</param>
        /// <returns>String representation.</returns>
        public static string EnumToString(this TimeSpecifier timeSpecifier)
        {
            switch (timeSpecifier)
            {
                case TimeSpecifier.START:
                    return "start";
                case TimeSpecifier.END:
                    return "end";
                default:
                    Debug.Assert(false);
                    return timeSpecifier.ToString();
            }
        }

        /// <summary>
        /// Converts the enum to string representation.
        /// </summary>
        /// <param name="intervalSpecifier">Value to be converted.</param>
        /// <returns>String representation.</returns>
        public static string EnumToString(this IntervalSpecifier intervalSpecifier)
        {
            switch (intervalSpecifier)
            {
                case IntervalSpecifier.ALL:
                    return "all";
                default:
                    Debug.Assert(false);
                    return intervalSpecifier.ToString();
            }
        }

        /// <summary>
        /// Converts the enum to string representation.
        /// </summary>
        /// <param name="assignOperator">Value to be converted.</param>
        /// <returns>String representation.</returns>
        public static string EnumToString(this TimedEffectAssignOperator assignOperator)
        {
            switch (assignOperator)
            {
                case TimedEffectAssignOperator.INCREASE:
                    return "increase";
                case TimedEffectAssignOperator.DECREASE:
                    return "decrease";
                default:
                    Debug.Assert(false);
                    return assignOperator.ToString();
            }
        }

        /// <summary>
        /// Converts the enum to string representation.
        /// </summary>
        /// <param name="lengthSpecifier">Value to be converted.</param>
        /// <returns>String representation.</returns>
        public static string EnumToString(this LengthSpecifier lengthSpecifier)
        {
            switch (lengthSpecifier)
            {
                case LengthSpecifier.SERIAL:
                    return ":serial";
                case LengthSpecifier.PARALLEL:
                    return ":parallel";
                default:
                    Debug.Assert(false);
                    return lengthSpecifier.ToString();
            }
        }

        /// <summary>
        /// Converts the enum to string representation.
        /// </summary>
        /// <param name="optimizationSpecifier">Value to be converted.</param>
        /// <returns>String representation.</returns>
        public static string EnumToString(this OptimizationSpecifier optimizationSpecifier)
        {
            switch (optimizationSpecifier)
            {
                case OptimizationSpecifier.NONE:
                    return "none";
                case OptimizationSpecifier.MINIMIZE:
                    return "minimize";
                case OptimizationSpecifier.MAXIMIZE:
                    return "maximize";
                default:
                    Debug.Assert(false);
                    return optimizationSpecifier.ToString();
            }
        }

        /// <summary>
        /// Converts the enum to string representation.
        /// </summary>
        /// <param name="requirement">Value to be converted.</param>
        /// <returns>String representation.</returns>
        public static string EnumToString(this Requirement requirement)
        {
            switch (requirement)
            {
                case Requirement.STRIPS:
                    return ":strips";
                case Requirement.TYPING:
                    return ":typing";
                case Requirement.NEGATIVE_PRECONDITIONS:
                    return ":negative-preconditions";
                case Requirement.DISJUNCTIVE_PRECONDITIONS:
                    return ":disjunctive-preconditions";
                case Requirement.EQUALITY:
                    return ":equality";
                case Requirement.EXISTENTIAL_PRECONDITIONS:
                    return ":existential-preconditions";
                case Requirement.UNIVERSAL_PRECONDITIONS:
                    return ":universal-preconditions";
                case Requirement.QUANTIFIED_PRECONDITIONS:
                    return ":quantified-preconditions";
                case Requirement.CONDITIONAL_EFFECTS:
                    return ":conditional-effects";
                case Requirement.FLUENTS:
                    return ":fluents";
                case Requirement.NUMERIC_FLUENTS:
                    return ":numeric-fluents";
                case Requirement.OBJECT_FLUENTS:
                    return ":object-fluents";
                case Requirement.ADL:
                    return ":adl";
                case Requirement.DURATIVE_ACTIONS:
                    return ":durative-actions";
                case Requirement.DURATION_INEQUALITIES:
                    return ":duration-inequalities";
                case Requirement.CONTINUOUS_EFFECTS:
                    return ":continuous-effects";
                case Requirement.DERIVED_PREDICATES:
                    return ":derived-predicates";
                case Requirement.TIMED_INITIAL_LITERALS:
                    return ":timed-initial-literals";
                case Requirement.PREFERENCES:
                    return ":preferences";
                case Requirement.CONSTRAINTS:
                    return ":constraints";
                case Requirement.ACTIONS_COSTS:
                    return ":action-costs";
                default:
                    Debug.Assert(false);
                    return requirement.ToString();
            }
        }

        /// <summary>
        /// Converts the list to string representation in a form of PDDL block.
        /// </summary>
        /// <typeparam name="T">Type of items in the list.</typeparam>
        /// <param name="list">List to be converted.</param>
        /// <param name="blockName">Name of the block. If null, the enclosing block is not used.</param>
        /// <param name="returnEmptyIfListEmpty">If the parameter is true and the list is empty, an empty string is returned, ignoring the enclosing block.</param>
        /// <param name="listWithinAndExpression">If the parameter is true, the list is enclosed within an extra and-block.</param>
        /// <returns>String representation of the list.</returns>
        public static string ToBlockString<T>(this List<T> list, string blockName = null, bool returnEmptyIfListEmpty = true, bool listWithinAndExpression = false)
        {
            if (returnEmptyIfListEmpty && list.Count == 0)
            {
                return "";
            }

            string blockPrefix = "";
            string blockSuffix = "";

            if (blockName != null)
            {
                blockPrefix = "(" + blockName + (blockName.Length == 0 ? "" : " ");
                blockSuffix = ")";
            }

            string andPrefix = (listWithinAndExpression && (list.Count >= 2)) ? "(and " : "";
            string andSuffix = (listWithinAndExpression && (list.Count >= 2)) ? ")" : "";

            return $"{blockPrefix}{andPrefix}{string.Join(" ", list)}{andSuffix}{blockSuffix}";
        }

        /// <summary>
        /// Checks the given types list and if it's null or empty, then returns a new list with a default type.
        /// </summary>
        /// <param name="types">Type list.</param>
        /// <param name="defaultType">Default type.</param>
        /// <returns>List of types or a new list with the default type.</returns>
        public static string[] GetTypesOrDefault(this string[] types, string defaultType)
        {
            if (types == null || types.Length == 0 || (types.Length == 1 && string.IsNullOrEmpty(types[0])))
            {
                return new string[]{ defaultType };
            }

            return types;
        }

        /// <summary>
        /// Converts the type(s) name into a form of PDDL type specification, i.e. form of '- typeA' or '- (either typeA typeB)'.
        /// </summary>
        /// <param name="types">List of types to be converted.</param>
        /// <returns>String representation of type specification.</returns>
        public static string ToTypeSpecString(this List<string> types)
        {
            if (types.Count == 0)
            {
                return "";
            }

            if (types.Count == 1)
            {
                return $" - {types[0]}";
            }

            return $" - (either {string.Join(" ", types)})";
        }

        /// <summary>
        /// Checks whether two strings are equal in content, regardless of upper-/lower-case.
        /// </summary>
        /// <param name="thisString">First string.</param>
        /// <param name="other">String to compare.</param>
        /// <returns>True if the given strings are equal, ignoring case.</returns>
        public static bool EqualsNoCase(this string thisString, string other)
        {
            return thisString.Equals(other, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
