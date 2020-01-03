using System.Collections.Generic;
using PAD.InputData.PDDL.Traits;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL durative-action effects.
    /// </summary>
    public class DurativeEffects : List<DurativeEffect>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return (Count == 0) ? "()" : this.ToBlockString(null, true, true);
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(effect => effect.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL durative effect (da-effect).
    /// </summary>
    public abstract class DurativeEffect : IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Input data structure for a PDDL durative effect expression of type "forall".
    /// </summary>
    public class ForallDurativeEffect : DurativeEffect
    {
        /// <summary>
        /// List of expression parameters.
        /// </summary>
        public Parameters Parameters { get; set; }

        /// <summary>
        /// List of effects for the expression to be applied.
        /// </summary>
        public DurativeEffects Effects { get; set; }

        /// <summary>
        /// Constructs the durative effect.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <param name="effects">List of effects.</param>
        public ForallDurativeEffect(Parameters parameters, DurativeEffects effects)
        {
            Parameters = parameters;
            Effects = effects;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(forall {Parameters} {Effects})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Parameters.Accept(visitor);
            Effects.Accept(visitor);
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL (conditional) action effect of type "when".
    /// </summary>
    public class WhenDurativeEffect : DurativeEffect
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public DurativeExpression Expression { get; set; }

        /// <summary>
        /// Argument timed effect.
        /// </summary>
        public TimedEffect Effect { get; set; }

        /// <summary>
        /// Constructs the durative effect.
        /// </summary>
        /// <param name="expression">Argument expression.</param>
        /// <param name="effect">Argument timed effect.</param>
        public WhenDurativeEffect(DurativeExpression expression, TimedEffect effect)
        {
            Expression = expression;
            Effect = effect;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(when {Expression} {Effect})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Expression.Accept(visitor);
            Effect.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a PDDL timed effect.
    /// </summary>
    public abstract class TimedEffect : DurativeEffect
    {
    }

    /// <summary>
    /// Input data structure for an "at" timed effect expression.
    /// </summary>
    public class AtTimedEffect : TimedEffect
    {
        /// <summary>
        /// Time specifier.
        /// </summary>
        public TimeSpecifier TimeSpecifier { get; set; }

        /// <summary>
        /// List of effects for the expression to be applied.
        /// </summary>
        public PrimitiveEffects Effects { get; set; }

        /// <summary>
        /// Constructs the durative effect.
        /// </summary>
        /// <param name="timeSpecifier">Time specifier.</param>
        /// <param name="effects">List of primitive effects.</param>
        public AtTimedEffect(TimeSpecifier timeSpecifier, PrimitiveEffects effects)
        {
            TimeSpecifier = timeSpecifier;
            Effects = effects;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(at {TimeSpecifier.EnumToString()} {Effects})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Effects.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for an "assign" timed effect expression.
    /// </summary>
    public class AssignTimedEffect : TimedEffect
    {
        /// <summary>
        /// Assign operator specifier.
        /// </summary>
        public TimedEffectAssignOperator AssignOperator { get; set; }

        /// <summary>
        /// Numeric function of which value will be assigned.
        /// </summary>
        public NumericFunction Function { get; set; }

        /// <summary>
        /// Timed numeric value to be assigned.
        /// </summary>
        public TimedNumericExpression Value { get; set; }

        /// <summary>
        /// Constructs the durative effect.
        /// </summary>
        /// <param name="assignOperator">Assign operator specifier.</param>
        /// <param name="function">Numeric function for assignment.</param>
        /// <param name="value">Timed numeric expression to be assigned.</param>
        public AssignTimedEffect(TimedEffectAssignOperator assignOperator, NumericFunction function, TimedNumericExpression value)
        {
            AssignOperator = assignOperator;
            Function = function;
            Value = value;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"({AssignOperator.EnumToString()} {Function} {Value})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Function.Accept(visitor);
            Value.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a PDDL timed numeric expression (f-exp-t).
    /// </summary>
    public abstract class TimedNumericExpression : IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Input data structure for a primitive timed numeric expression - in the form of "#t".
    /// </summary>
    public class PrimitiveTimedNumericExpression : TimedNumericExpression
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return "#t";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    /// <summary>
    /// Input data structure for a compound timed numeric expression - in the form of "(* #t numericExpression)" or "(* numericExpression #t)".
    /// </summary>
    public class CompoundTimedNumericExpression : TimedNumericExpression
    {
        /// <summary>
        /// Argument numeric expression.
        /// </summary>
        public NumericExpression NumericExpression { get; set; }

        /// <summary>
        /// Constructs the durative effect.
        /// </summary>
        /// <param name="numericExpression">Numeric expression.</param>
        public CompoundTimedNumericExpression(NumericExpression numericExpression)
        {
            NumericExpression = numericExpression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(* #t {NumericExpression})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            NumericExpression.Accept(visitor);
        }
    }
}
