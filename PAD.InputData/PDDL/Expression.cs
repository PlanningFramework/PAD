using System.Collections.Generic;
using PAD.InputData.PDDL.Traits;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL logical expressions (= goal descriptions, GD).
    /// </summary>
    public class Expressions : List<Expression>, IVisitable
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
    /// Input data structure for a PDDL logical expression (= goal description, GD).
    /// </summary>
    public abstract class Expression : IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Input data structure for a single PDDL logical expression of preference type.
    /// </summary>
    public class PreferenceExpression : Expression
    {
        /// <summary>
        /// Preference name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Argument { get; set; }

        /// <summary>
        /// Constructs the expression.
        /// </summary>
        /// <param name="name">Preference name.</param>
        /// <param name="argument">Argument expression.</param>
        public PreferenceExpression(string name, Expression argument)
        {
            Name = name;
            Argument = argument;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(preference {Name} {Argument})";
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

    /// <summary>
    /// Input data structure for an atomic formula expression (i.e. predicate or equals expression).
    /// </summary>
    public abstract class AtomicFormulaExpression : Expression
    {
    }

    /// <summary>
    /// Input data structure for a single PDDL logical expression of predicate type.
    /// </summary>
    public class PredicateExpression : AtomicFormulaExpression
    {
        /// <summary>
        /// Predicate name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Argument terms.
        /// </summary>
        public Terms Terms { get; set; } = new Terms();

        /// <summary>
        /// Constructs the expression.
        /// </summary>
        /// <param name="name">Predicate name.</param>
        public PredicateExpression(string name)
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
    /// Input data structure for a single PDDL logical expression of equals (=) type.
    /// </summary>
    public class EqualsExpression : AtomicFormulaExpression
    {
        /// <summary>
        /// First argument term.
        /// </summary>
        public Term Term1 { get; set; }

        /// <summary>
        /// Second argument term.
        /// </summary>
        public Term Term2 { get; set; }

        /// <summary>
        /// Constructs the expression.
        /// </summary>
        /// <param name="term1">First argument term.</param>
        /// <param name="term2">Second argument term.</param>
        public EqualsExpression(Term term1, Term term2)
        {
            Term1 = term1;
            Term2 = term2;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(= {Term1} {Term2})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Term1.Accept(visitor);
            Term2.Accept(visitor);
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL logical expression of type "and".
    /// </summary>
    public class AndExpression : Expression
    {
        /// <summary>
        /// List of argument expressions.
        /// </summary>
        public Expressions Arguments { get; set; } = new Expressions();

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
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL logical expression of type "or".
    /// </summary>
    public class OrExpression : Expression
    {
        /// <summary>
        /// List of argument expressions.
        /// </summary>
        public Expressions Arguments { get; set; } = new Expressions();

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Arguments.ToBlockString("or", false);
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
    /// Input data structure for a single PDDL logical expression of type "not".
    /// </summary>
    public class NotExpression : Expression
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Argument { get; set; }

        /// <summary>
        /// Constructs the expression.
        /// </summary>
        /// <param name="argument">Argument expression.</param>
        public NotExpression(Expression argument)
        {
            Argument = argument;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(not {Argument})";
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

    /// <summary>
    /// Input data structure for a single PDDL logical expression of type "imply".
    /// </summary>
    public class ImplyExpression : Expression
    {
        /// <summary>
        /// First argument expression.
        /// </summary>
        public Expression Argument1 { get; set; }

        /// <summary>
        /// Second argument expression.
        /// </summary>
        public Expression Argument2 { get; set; }

        /// <summary>
        /// Constructs the expression.
        /// </summary>
        /// <param name="argument1">First argument expression.</param>
        /// <param name="argument2">Second argument expression.</param>
        public ImplyExpression(Expression argument1, Expression argument2)
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
            return $"(imply {Argument1} {Argument2})";
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
    /// Input data structure for a single PDDL logical expression of type "exists".
    /// </summary>
    public class ExistsExpression : Expression
    {
        /// <summary>
        /// List of expression parameters.
        /// </summary>
        public Parameters Parameters { get; set; }

        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; }

        /// <summary>
        /// Constructs the expression.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <param name="expression">Argument expression.</param>
        public ExistsExpression(Parameters parameters, Expression expression)
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
            return $"(exists {Parameters} {Expression})";
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
    /// Input data structure for a single PDDL logical expression of type "forall".
    /// </summary>
    public class ForallExpression : Expression
    {
        /// <summary>
        /// List of expression parameters.
        /// </summary>
        public Parameters Parameters { get; set; }

        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; }

        /// <summary>
        /// Constructs the expression.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <param name="expression">Argument expression.</param>
        public ForallExpression(Parameters parameters, Expression expression)
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
    /// Input data structure for a single PDDL logical expression of numeric comparison type.
    /// </summary>
    public class NumericCompareExpression : Expression
    {
        /// <summary>
        /// Numeric comparer.
        /// </summary>
        public NumericComparer NumericComparer { get; set; }

        /// <summary>
        /// First argument expression.
        /// </summary>
        public NumericExpression NumericExpression1 { get; set; }

        /// <summary>
        /// Second argument expression.
        /// </summary>
        public NumericExpression NumericExpression2 { get; set; }

        /// <summary>
        /// Constructs the expression.
        /// </summary>
        /// <param name="numComparer">Numeric comparer.</param>
        /// <param name="numExpression1">First numeric argument.</param>
        /// <param name="numExpression2">Second numeric argument.</param>
        public NumericCompareExpression(NumericComparer numComparer, NumericExpression numExpression1, NumericExpression numExpression2)
        {
            NumericComparer = numComparer;
            NumericExpression1 = numExpression1;
            NumericExpression2 = numExpression2;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"({NumericComparer.EnumToString()} {NumericExpression1} {NumericExpression2})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            NumericExpression1.Accept(visitor);
            NumericExpression2.Accept(visitor);
            visitor.PostVisit(this);
        }
    }
}
