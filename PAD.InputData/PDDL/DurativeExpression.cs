using System.Collections.Generic;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL durative-action logical expressions (da-GD).
    /// </summary>
    public class DurativeExpressions : List<DurativeExpression>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString();
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(expression => expression.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a PDDL durative-action logical expression (da-GD).
    /// </summary>
    public abstract class DurativeExpression : IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Input data structure for a PDDL durative-action logical expression of type "and".
    /// </summary>
    public class AndDurativeExpression : DurativeExpression
    {
        /// <summary>
        /// List of argument expressions.
        /// </summary>
        public DurativeExpressions Arguments { get; set; } = new DurativeExpressions();

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Arguments.ToBlockString("and", false);
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Arguments.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a PDDL durative-action logical expression of type "forall".
    /// </summary>
    public class ForallDurativeExpression : DurativeExpression
    {
        /// <summary>
        /// List of expression parameters.
        /// </summary>
        public Parameters Parameters { get; set; } = new Parameters();

        /// <summary>
        /// Argument expression.
        /// </summary>
        public DurativeExpression Expression { get; set; } = null;

        /// <summary>
        /// Constructs the durative expression.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <param name="expression">Argument expression.</param>
        public ForallDurativeExpression(Parameters parameters, DurativeExpression expression)
        {
            Parameters = parameters;
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(forall {Parameters} {Expression})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Parameters.Accept(visitor);
            Expression.Accept(visitor);
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a PDDL durative-action timed logical expression with possible preferences (pref-timed-GD).
    /// </summary>
    public abstract class PreferencableTimedExpression : DurativeExpression
    {
    }

    /// <summary>
    /// Input data structure for a PDDL durative-action preference timed logical expression.
    /// </summary>
    public class PreferencedTimedExpression : PreferencableTimedExpression
    {
        /// <summary>
        /// Preference name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Argument expression.
        /// </summary>
        public TimedExpression Expression { get; set; } = null;

        /// <summary>
        /// Constructs the durative expression.
        /// </summary>
        /// <param name="name">Preference name.</param>
        /// <param name="expression">Argument expression.</param>
        public PreferencedTimedExpression(string name, TimedExpression expression)
        {
            Name = name;
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(preference {Name} {Expression})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Expression.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a PDDL durative-action timed logical expression (timed-GD).
    /// </summary>
    public abstract class TimedExpression : PreferencableTimedExpression
    {
    }

    /// <summary>
    /// Input data structure for an "at" timed logical expression.
    /// </summary>
    public class AtTimedExpression : TimedExpression
    {
        /// <summary>
        /// Time specifier.
        /// </summary>
        public TimeSpecifier TimeSpecifier { get; set; } = TimeSpecifier.START;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; } = null;

        /// <summary>
        /// Constructs the durative expression.
        /// </summary>
        /// <param name="timeSpecifier">Time specifier.</param>
        /// <param name="expression">Argument expression.</param>
        public AtTimedExpression(TimeSpecifier timeSpecifier, Expression expression)
        {
            TimeSpecifier = timeSpecifier;
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(at {TimeSpecifier.EnumToString()} {Expression})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Expression.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for an "over" timed logical expression.
    /// </summary>
    public class OverTimedExpression : TimedExpression
    {
        /// <summary>
        /// Interval specifier.
        /// </summary>
        public IntervalSpecifier IntervalSpecifier { get; set; } = IntervalSpecifier.ALL;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; } = null;

        /// <summary>
        /// Constructs the durative expression.
        /// </summary>
        /// <param name="intervalSpecifier">Interval specifier.</param>
        /// <param name="expression">Argument expression.</param>
        public OverTimedExpression(IntervalSpecifier intervalSpecifier, Expression expression)
        {
            IntervalSpecifier = intervalSpecifier;
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(over {IntervalSpecifier.EnumToString()} {Expression})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Expression.Accept(visitor);
        }
    }
}
