using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for a list of PDDL numeric expressions.
    /// </summary>
    public class NumericExpressions : List<NumericExpression>, IVisitable
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
            ForEach(numericExpression => numericExpression.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL numeric expression.
    /// </summary>
    public abstract class NumericExpression : IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Input data structure for a number.
    /// </summary>
    public class Number : NumericExpression
    {
        /// <summary>
        /// Number value.
        /// </summary>
        public double Value { get; set; } = 0.0;

        /// <summary>
        /// Constructs the numeric expression.
        /// </summary>
        /// <param name="value">Number value.</param>
        public Number(double value)
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
    /// Input data structure for a numeric function.
    /// </summary>
    public class NumericFunction : NumericExpression
    {
        /// <summary>
        /// Function name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Argument terms.
        /// </summary>
        public Terms Terms { get; set; } = new Terms();

        /// <summary>
        /// Constructs the numeric expression.
        /// </summary>
        /// <param name="name">Function name.</param>
        public NumericFunction(string name)
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
    /// Input data structure for a special "?duration" variable. This variable is valid for use only inside durative-action effects!
    /// </summary>
    public class DurationVariable : NumericExpression
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return "?duration";
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
    /// Input data structure for a numeric operation.
    /// </summary>
    public abstract class NumericOperation : NumericExpression
    {
    }

    /// <summary>
    /// Input data structure for a "plus" numeric operation.
    /// </summary>
    public class Plus : NumericOperation
    {
        /// <summary>
        /// Numeric arguments.
        /// </summary>
        public NumericExpressions Arguments { get; set; } = new NumericExpressions();

        /// <summary>
        /// Constructs the numeric operation.
        /// </summary>
        /// <param name="arguments">Numeric arguments.</param>
        public Plus(NumericExpressions arguments)
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
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a "multiply" numeric operation.
    /// </summary>
    public class Multiply : NumericOperation
    {
        /// <summary>
        /// Numeric arguments.
        /// </summary>
        public NumericExpressions Arguments { get; set; } = new NumericExpressions();

        /// <summary>
        /// Constructs the numeric operation.
        /// </summary>
        /// <param name="arguments">Numeric arguments.</param>
        public Multiply(NumericExpressions arguments)
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
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a "minus" numeric operation.
    /// </summary>
    public class Minus : NumericOperation
    {
        /// <summary>
        /// First numeric argument.
        /// </summary>
        public NumericExpression Argument1 { get; set; } = null;

        /// <summary>
        /// Second numeric argument.
        /// </summary>
        public NumericExpression Argument2 { get; set; } = null;

        /// <summary>
        /// Constructs the numeric operation.
        /// </summary>
        /// <param name="argument1">First numeric argument.</param>
        /// <param name="argument2">Second numeric argument.</param>
        public Minus(NumericExpression argument1, NumericExpression argument2)
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
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a "divide" numeric operation.
    /// </summary>
    public class Divide : NumericOperation
    {
        /// <summary>
        /// First numeric argument.
        /// </summary>
        public NumericExpression Argument1 { get; set; } = null;

        /// <summary>
        /// Second numeric argument.
        /// </summary>
        public NumericExpression Argument2 { get; set; } = null;

        /// <summary>
        /// Constructs the numeric operation.
        /// </summary>
        /// <param name="argument1">First numeric argument.</param>
        /// <param name="argument2">Second numeric argument.</param>
        public Divide(NumericExpression argument1, NumericExpression argument2)
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
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a "unary-minus" numeric operation.
    /// </summary>
    public class UnaryMinus : NumericOperation
    {
        /// <summary>
        /// Numeric argument.
        /// </summary>
        public NumericExpression Argument { get; set; } = null;

        /// <summary>
        /// Constructs the numeric operation.
        /// </summary>
        /// <param name="argument">Numeric argument.</param>
        public UnaryMinus(NumericExpression argument)
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
            visitor.PostVisit(this);
        }
    }
}
