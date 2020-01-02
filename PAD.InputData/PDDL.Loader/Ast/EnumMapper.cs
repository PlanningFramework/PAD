using System.Diagnostics;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL.Loader.Ast
{
    /// <summary>
    /// Auxilliary class for mapping string tokens to enum values used within AST.
    /// </summary>
    public static class EnumMapper
    {
        /// <summary>
        /// Converts string to TimeSpecifier enum value.
        /// </summary>
        /// <param name="str">String token.</param>
        /// <returns>Enum token.</returns>
        public static TimeSpecifier ToTimeSpecifier(string str)
        {
            if (str != null)
            {
                switch (str.ToUpper())
                {
                    case "START":
                        return TimeSpecifier.START;
                    case "END":
                        return TimeSpecifier.END;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return TimeSpecifier.START;
        }

        /// <summary>
        /// Converts string to AssignOperator enum value.
        /// </summary>
        /// <param name="str">String token.</param>
        /// <returns>Enum token.</returns>
        public static AssignOperator ToAssignOperator(string str)
        {
            if (str != null)
            {
                switch (str.ToUpper())
                {
                    case "ASSIGN":
                        return AssignOperator.ASSIGN;
                    case "SCALE-UP":
                        return AssignOperator.SCALE_UP;
                    case "SCALE-DOWN":
                        return AssignOperator.SCALE_DOWN;
                    case "INCREASE":
                        return AssignOperator.INCREASE;
                    case "DECREASE":
                        return AssignOperator.DECREASE;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return AssignOperator.ASSIGN;
        }

        /// <summary>
        /// Converts string to TimedEffectAssignOperator enum value.
        /// </summary>
        /// <param name="str">String token.</param>
        /// <returns>Enum token.</returns>
        public static TimedEffectAssignOperator ToTimedEffectAssignOperator(string str)
        {
            if (str != null)
            {
                switch (str.ToUpper())
                {
                    case "INCREASE":
                        return TimedEffectAssignOperator.INCREASE;
                    case "DECREASE":
                        return TimedEffectAssignOperator.DECREASE;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return TimedEffectAssignOperator.INCREASE;
        }

        /// <summary>
        /// Converts string to IntervalSpecifier enum value.
        /// </summary>
        /// <param name="str">String token.</param>
        /// <returns>Enum token.</returns>
        public static IntervalSpecifier ToIntervalSpecifier(string str)
        {
            if (str != null)
            {
                switch (str.ToUpper())
                {
                    case "ALL":
                        return IntervalSpecifier.ALL;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return IntervalSpecifier.ALL;
        }

        /// <summary>
        /// Converts string to OptimizationSpecifier enum value.
        /// </summary>
        /// <param name="str">String token.</param>
        /// <returns>Enum token.</returns>
        public static OptimizationSpecifier ToOptimizationSpecifier(string str)
        {
            if (str != null)
            {
                switch (str.ToUpper())
                {
                    case "MINIMIZE":
                        return OptimizationSpecifier.MINIMIZE;
                    case "MAXIMIZE":
                        return OptimizationSpecifier.MAXIMIZE;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return OptimizationSpecifier.MINIMIZE;
        }

        /// <summary>
        /// Converts string to LengthSpecifier enum value.
        /// </summary>
        /// <param name="str">String token.</param>
        /// <returns>Enum token.</returns>
        public static LengthSpecifier ToLengthSpecifier(string str)
        {
            if (str != null)
            {
                switch (str.ToUpper())
                {
                    case ":SERIAL":
                        return LengthSpecifier.SERIAL;
                    case ":PARALLEL":
                        return LengthSpecifier.PARALLEL;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return LengthSpecifier.SERIAL;
        }

        /// <summary>
        /// Converts string to Duration Comparer enum value.
        /// </summary>
        /// <param name="str">String token.</param>
        /// <returns>Enum token.</returns>
        public static DurationComparer ToDurationComparer(string str)
        {
            if (str != null)
            {
                switch (str.ToUpper())
                {
                    case "<=":
                        return DurationComparer.LTE;
                    case ">=":
                        return DurationComparer.GTE;
                    case "=":
                        return DurationComparer.EQ;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return DurationComparer.EQ;
        }

        /// <summary>
        /// Converts string to NumericComparer enum value.
        /// </summary>
        /// <param name="str">String token.</param>
        /// <returns>Enum token.</returns>
        public static NumericComparer ToNumericComparer(string str)
        {
            if (str != null)
            {
                switch (str.ToUpper())
                {
                    case "<":
                        return NumericComparer.LT;
                    case "<=":
                        return NumericComparer.LTE;
                    case ">":
                        return NumericComparer.GT;
                    case ">=":
                        return NumericComparer.GTE;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return NumericComparer.LT;
        }

        /// <summary>
        /// Converts string to NumericOperator enum value.
        /// </summary>
        /// <param name="str">String token.</param>
        /// <returns>Enum token.</returns>
        public static NumericOperator ToNumericOperator(string str)
        {
            if (str != null)
            {
                switch (str.ToUpper())
                {
                    case "+":
                        return NumericOperator.PLUS;
                    case "-":
                        return NumericOperator.MINUS;
                    case "*":
                        return NumericOperator.MUL;
                    case "/":
                        return NumericOperator.DIV;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return NumericOperator.PLUS;
        }

        /// <summary>
        /// Converts string to Requirement enum value.
        /// </summary>
        /// <param name="str">String token.</param>
        /// <returns>Enum token.</returns>
        public static Requirement ToRequirement(string str)
        {
            if (str != null)
            {
                switch (str.ToUpper())
                {
                    case ":STRIPS":
                        return Requirement.STRIPS;
                    case ":TYPING":
                        return Requirement.TYPING;
                    case ":NEGATIVE-PRECONDITIONS":
                        return Requirement.NEGATIVE_PRECONDITIONS;
                    case ":DISJUNCTIVE-PRECONDITIONS":
                        return Requirement.DISJUNCTIVE_PRECONDITIONS;
                    case ":EQUALITY":
                        return Requirement.EQUALITY;
                    case ":EXISTENTIAL-PRECONDITIONS":
                        return Requirement.EXISTENTIAL_PRECONDITIONS;
                    case ":UNIVERSAL-PRECONDITIONS":
                        return Requirement.UNIVERSAL_PRECONDITIONS;
                    case ":QUANTIFIED-PRECONDITIONS":
                        return Requirement.QUANTIFIED_PRECONDITIONS;
                    case ":CONDITIONAL-EFFECTS":
                        return Requirement.CONDITIONAL_EFFECTS;
                    case ":FLUENTS":
                        return Requirement.FLUENTS;
                    case ":NUMERIC-FLUENTS":
                        return Requirement.NUMERIC_FLUENTS;
                    case ":OBJECT-FLUENTS":
                        return Requirement.OBJECT_FLUENTS;
                    case ":ADL":
                        return Requirement.ADL;
                    case ":DURATIVE-ACTIONS":
                        return Requirement.DURATIVE_ACTIONS;
                    case ":DURATION-INEQUALITIES":
                        return Requirement.DURATION_INEQUALITIES;
                    case ":CONTINUOUS-EFFECTS":
                        return Requirement.CONTINUOUS_EFFECTS;
                    case ":DERIVED-PREDICATES":
                        return Requirement.DERIVED_PREDICATES;
                    case ":TIMED-INITIAL-LITERALS":
                        return Requirement.TIMED_INITIAL_LITERALS;
                    case ":PREFERENCES":
                        return Requirement.PREFERENCES;
                    case ":CONSTRAINTS":
                        return Requirement.CONSTRAINTS;
                    case ":ACTION-COSTS":
                        return Requirement.ACTIONS_COSTS;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return Requirement.STRIPS;
        }
    }
}
