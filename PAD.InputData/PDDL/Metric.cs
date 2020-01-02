using System.Collections.Generic;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for a PDDL metric specification.
    /// </summary>
    public class Metric : IVisitable
    {
        /// <summary>
        /// Optimization specifier.
        /// </summary>
        public OptimizationSpecifier OptimizationSpecifier { get; set; } = OptimizationSpecifier.NONE;

        /// <summary>
        /// Argument metric expression.
        /// </summary>
        public MetricExpression Expression { get; set; } = null;

        /// <summary>
        /// Constructs a metric specification.
        /// </summary>
        public Metric()
        {
        }

        /// <summary>
        /// Constructs a metric specification.
        /// </summary>
        /// <param name="optimizationSpecifier">Optimization specifier.</param>
        /// <param name="expression">Argument metric expression.</param>
        public Metric(OptimizationSpecifier optimizationSpecifier, MetricExpression expression)
        {
            OptimizationSpecifier = optimizationSpecifier;
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            if (OptimizationSpecifier == OptimizationSpecifier.NONE)
            {
                return "";
            }
            return $"(:metric {OptimizationSpecifier.EnumToString()} {Expression})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            if (Expression != null)
            {
                Expression.Accept(visitor);
            }
        }
    }

    /// <summary>
    /// Input data structure for a list of PDDL metric expressions.
    /// </summary>
    public class MetricExpressions : List<MetricExpression>, IVisitable
    {
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
    /// Input data structure for a PDDL metric expression.
    /// </summary>
    public abstract class MetricExpression : IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Input data structure for a number metric expression.
    /// </summary>
    public class MetricNumber : MetricExpression
    {
        /// <summary>
        /// Number value.
        /// </summary>
        public double Value { get; set; } = 0.0;

        /// <summary>
        /// Constructs a metric expression.
        /// </summary>
        /// <param name="value">Number value.</param>
        public MetricNumber(double value)
        {
            Value = value;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Value.ToString();
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
    /// Input data structure for a numeric function metric expression.
    /// </summary>
    public class MetricNumericFunction : MetricExpression
    {
        /// <summary>
        /// Function name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Argument terms.
        /// </summary>
        public ConstantTerms Terms { get; set; } = new ConstantTerms();

        /// <summary>
        /// Constructs a metric expression.
        /// </summary>
        /// <param name="name">Function name.</param>
        public MetricNumericFunction(string name)
        {
            Name = name;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return (Terms.Count > 0) ? $"({Name} {Terms})" : $"({Name})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Terms.Accept(visitor);
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a "total-time" function metric expression.
    /// </summary>
    public class MetricTotalTime : MetricExpression
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return "total-time";
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
    /// Input data structure for a "is-violated" metric expression.
    /// </summary>
    public class MetricPreferenceViolation : MetricExpression
    {
        /// <summary>
        /// Preference name.
        /// </summary>
        public string PreferenceName { get; set; } = "";

        /// <summary>
        /// Constructs a metric expression.
        /// </summary>
        /// <param name="preferenceName">Preference name.</param>
        public MetricPreferenceViolation(string preferenceName)
        {
            PreferenceName = preferenceName;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(is-violated {PreferenceName})";
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
    /// Input data structure for a metric numeric operation.
    /// </summary>
    public abstract class MetricNumericOperation : MetricExpression
    {
    }

    /// <summary>
    /// Input data structure for a "plus" metric numeric operation.
    /// </summary>
    public class MetricPlus : MetricNumericOperation
    {
        /// <summary>
        /// Numeric arguments.
        /// </summary>
        public MetricExpressions Arguments { get; set; } = new MetricExpressions();

        /// <summary>
        /// Constructs a metric numeric operation.
        /// </summary>
        /// <param name="arguments">Numeric arguments.</param>
        public MetricPlus(MetricExpressions arguments)
        {
            Arguments = arguments;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Arguments.ToBlockString("+", false);
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
    /// Input data structure for a "multiply" metric numeric operation.
    /// </summary>
    public class MetricMultiply : MetricNumericOperation
    {
        /// <summary>
        /// Numeric arguments.
        /// </summary>
        public MetricExpressions Arguments { get; set; } = new MetricExpressions();

        /// <summary>
        /// Constructs a metric numeric operation.
        /// </summary>
        /// <param name="arguments">Numeric arguments.</param>
        public MetricMultiply(MetricExpressions arguments)
        {
            Arguments = arguments;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Arguments.ToBlockString("*", false);
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
    /// Input data structure for a "minus" metric numeric operation.
    /// </summary>
    public class MetricMinus : MetricNumericOperation
    {
        /// <summary>
        /// First numeric argument.
        /// </summary>
        public MetricExpression Argument1 { get; set; } = null;

        /// <summary>
        /// Second numeric argument.
        /// </summary>
        public MetricExpression Argument2 { get; set; } = null;

        /// <summary>
        /// Constructs a metric numeric operation.
        /// </summary>
        /// <param name="argument1">First numeric argument.</param>
        /// <param name="argument2">Second numeric argument.</param>
        public MetricMinus(MetricExpression argument1, MetricExpression argument2)
        {
            Argument1 = argument1;
            Argument2 = argument2;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(- {Argument1} {Argument2})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Argument1.Accept(visitor);
            Argument2.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a "divide" metric numeric operation.
    /// </summary>
    public class MetricDivide : MetricNumericOperation
    {
        /// <summary>
        /// First numeric argument.
        /// </summary>
        public MetricExpression Argument1 { get; set; } = null;

        /// <summary>
        /// Second numeric argument.
        /// </summary>
        public MetricExpression Argument2 { get; set; } = null;

        /// <summary>
        /// Constructs a metric numeric operation.
        /// </summary>
        /// <param name="argument1">First numeric argument.</param>
        /// <param name="argument2">Second numeric argument.</param>
        public MetricDivide(MetricExpression argument1, MetricExpression argument2)
        {
            Argument1 = argument1;
            Argument2 = argument2;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(/ {Argument1} {Argument2})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Argument1.Accept(visitor);
            Argument2.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a "unary-minus" metric numeric operation.
    /// </summary>
    public class MetricUnaryMinus : MetricNumericOperation
    {
        /// <summary>
        /// Numeric argument.
        /// </summary>
        public MetricExpression Argument { get; set; } = null;

        /// <summary>
        /// Constructs a metric numeric operation.
        /// </summary>
        /// <param name="argument">Numeric argument.</param>
        public MetricUnaryMinus(MetricExpression argument)
        {
            Argument = argument;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(- {Argument})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Argument.Accept(visitor);
        }
    }
}
