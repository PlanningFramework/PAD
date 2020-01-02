using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL constraints.
    /// </summary>
    public class Constraints : List<Constraint>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString(":constraints", true, true);
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(constraint => constraint.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL constraint.
    /// </summary>
    public abstract class Constraint : IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Input data structure for a single PDDL constraint of preference type.
    /// </summary>
    public class PreferenceConstraint : Constraint
    {
        /// <summary>
        /// Preference name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Argument constraint.
        /// </summary>
        public Constraints Constraints { get; set; } = null;

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="name">Preference name.</param>
        /// <param name="constraints">Argument constraints.</param>
        public PreferenceConstraint(string name, Constraints constraints)
        {
            Name = name;
            Constraints = constraints;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(preference {Name} {Constraints.ToBlockString(null, false, true)})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Constraints.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL constraint of type "forall".
    /// </summary>
    public class ForallConstraint : Constraint
    {
        /// <summary>
        /// List of expression parameters.
        /// </summary>
        public Parameters Parameters { get; set; } = new Parameters();

        /// <summary>
        /// List of constraints for the expression to be applied.
        /// </summary>
        public Constraints Constraints { get; set; } = new Constraints();

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <param name="constraints">List of constraints.</param>
        public ForallConstraint(Parameters parameters, Constraints constraints)
        {
            Parameters = parameters;
            Constraints = constraints;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(forall {Parameters} {Constraints.ToBlockString(null, false, true)})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Parameters.Accept(visitor);
            Constraints.Accept(visitor);
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL constraint of type "at-end".
    /// </summary>
    public class AtEndConstraint : Constraint
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; } = null;

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="expression">Argument expression.</param>
        public AtEndConstraint(Expression expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(at end {Expression})";
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
    /// Input data structure for a single PDDL constraint of type "always".
    /// </summary>
    public class AlwaysConstraint : Constraint
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; } = null;

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="expression">Argument expression.</param>
        public AlwaysConstraint(Expression expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(always {Expression})";
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
    /// Input data structure for a single PDDL constraint of type "sometime".
    /// </summary>
    public class SometimeConstraint : Constraint
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; } = null;

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="expression">Argument expression.</param>
        public SometimeConstraint(Expression expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(sometime {Expression})";
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
    /// Input data structure for a single PDDL constraint of type "within".
    /// </summary>
    public class WithinConstraint : Constraint
    {
        /// <summary>
        /// Argument number.
        /// </summary>
        public double Number { get; set; } = 0.0;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; } = null;

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="number">Argument number.</param>
        /// <param name="expression">Argument expression.</param>
        public WithinConstraint(double number, Expression expression)
        {
            Number = number;
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(within {Number} {Expression})";
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
    /// Input data structure for a single PDDL constraint of type "at-most-once".
    /// </summary>
    public class AtMostOnceConstraint : Constraint
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; } = null;

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="expression">Argument expression.</param>
        public AtMostOnceConstraint(Expression expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(at-most-once {Expression})";
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
    /// Input data structure for a single PDDL constraint of type "sometime-after".
    /// </summary>
    public class SometimeAfterConstraint : Constraint
    {
        /// <summary>
        /// First argument expression.
        /// </summary>
        public Expression Expression1 { get; set; } = null;

        /// <summary>
        /// Second argument expression.
        /// </summary>
        public Expression Expression2 { get; set; } = null;

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="expression1">First argument expression.</param>
        /// <param name="expression2">Second argument expression.</param>
        public SometimeAfterConstraint(Expression expression1, Expression expression2)
        {
            Expression1 = expression1;
            Expression2 = expression2;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(sometime-after {Expression1} {Expression2})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Expression1.Accept(visitor);
            Expression2.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL constraint of type "sometime-before".
    /// </summary>
    public class SometimeBeforeConstraint : Constraint
    {
        /// <summary>
        /// First argument expression.
        /// </summary>
        public Expression Expression1 { get; set; } = null;

        /// <summary>
        /// Second argument expression.
        /// </summary>
        public Expression Expression2 { get; set; } = null;

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="expression1">First argument expression.</param>
        /// <param name="expression2">Second argument expression.</param>
        public SometimeBeforeConstraint(Expression expression1, Expression expression2)
        {
            Expression1 = expression1;
            Expression2 = expression2;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(sometime-before {Expression1} {Expression2})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Expression1.Accept(visitor);
            Expression2.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL constraint of type "always-within".
    /// </summary>
    public class AlwaysWithinConstraint : Constraint
    {
        /// <summary>
        /// Argument number.
        /// </summary>
        public double Number { get; set; } = 0.0;

        /// <summary>
        /// First argument expression.
        /// </summary>
        public Expression Expression1 { get; set; } = null;

        /// <summary>
        /// Second argument expression.
        /// </summary>
        public Expression Expression2 { get; set; } = null;

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="number">Argument number.</param>
        /// <param name="expression1">First argument expression.</param>
        /// <param name="expression2">Second argument expression.</param>
        public AlwaysWithinConstraint(double number, Expression expression1, Expression expression2)
        {
            Number = number;
            Expression1 = expression1;
            Expression2 = expression2;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(always-within {Number} {Expression1} {Expression2})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Expression1.Accept(visitor);
            Expression2.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL constraint of type "hold-during".
    /// </summary>
    public class HoldDuringConstraint : Constraint
    {
        /// <summary>
        /// First argument number.
        /// </summary>
        public double Number1 { get; set; } = 0.0;

        /// <summary>
        /// Second argument number.
        /// </summary>
        public double Number2 { get; set; } = 0.0;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; } = null;

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="number1">First argument number.</param>
        /// <param name="number2">Second argument number.</param>
        /// <param name="expression">Agument expression.</param>
        public HoldDuringConstraint(double number1, double number2, Expression expression)
        {
            Number1 = number1;
            Number2 = number2;
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(hold-during {Number1} {Number2} {Expression})";
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
    /// Input data structure for a single PDDL constraint of type "hold-after".
    /// </summary>
    public class HoldAfterConstraint : Constraint
    {
        /// <summary>
        /// Argument number.
        /// </summary>
        public double Number { get; set; } = 0.0;

        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; } = null;

        /// <summary>
        /// Constructs the contraint.
        /// </summary>
        /// <param name="number">Argument number.</param>
        /// <param name="expression">Argument expression.</param>
        public HoldAfterConstraint(double number, Expression expression)
        {
            Number = number;
            Expression = expression;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(hold-after {Number} {Expression})";
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
