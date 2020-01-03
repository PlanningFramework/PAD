using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for a PDDL init.
    /// </summary>
    public class Init : List<InitElement>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString(":init");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            ForEach(initElement => initElement.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL initial element.
    /// </summary>
    public abstract class InitElement : IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Input data structure for a PDDL literal initial element.
    /// </summary>
    public abstract class LiteralInitElement : InitElement
    {
    }

    /// <summary>
    /// Input data structure for a PDDL atomic formula initial element.
    /// </summary>
    public abstract class AtomicFormulaInitElement : LiteralInitElement
    {
    }

    /// <summary>
    /// Input data structure for a PDDL predicate initial element.
    /// </summary>
    public class PredicateInitElement : AtomicFormulaInitElement
    {
        /// <summary>
        /// Function name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Argument terms.
        /// </summary>
        public ConstantTerms Terms { get; set; } = new ConstantTerms();

        /// <summary>
        /// Constructs an init element.
        /// </summary>
        /// <param name="name">Predicate name.</param>
        public PredicateInitElement(string name)
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
    /// Input data structure for a PDDL "equals" initial element.
    /// </summary>
    public class EqualsInitElement : AtomicFormulaInitElement
    {
        /// <summary>
        /// First argument term.
        /// </summary>
        public ConstantTerm Term1 { get; set; }

        /// <summary>
        /// Second argument term.
        /// </summary>
        public ConstantTerm Term2 { get; set; }

        /// <summary>
        /// Constructs an init element.
        /// </summary>
        /// <param name="term1">First argument term.</param>
        /// <param name="term2">Second argument term.</param>
        public EqualsInitElement(ConstantTerm term1, ConstantTerm term2)
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
        }
    }

    /// <summary>
    /// Input data structure for a PDDL "not" initial element.
    /// </summary>
    public class NotInitElement : LiteralInitElement
    {
        /// <summary>
        /// Argument element.
        /// </summary>
        public AtomicFormulaInitElement Element { get; set; }

        /// <summary>
        /// Constructs an init element.
        /// </summary>
        /// <param name="element">Argument element.</param>
        public NotInitElement(AtomicFormulaInitElement element)
        {
            Element = element;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(not {Element})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Element.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a PDDL "at" initial element.
    /// </summary>
    public class AtInitElement : InitElement
    {
        /// <summary>
        /// Argument number.
        /// </summary>
        public double Number { get; set; }

        /// <summary>
        /// Argument element.
        /// </summary>
        public LiteralInitElement Element { get; set; }

        /// <summary>
        /// Constructs an init element.
        /// </summary>
        /// <param name="number">Argument number.</param>
        /// <param name="element">Argument element.</param>
        public AtInitElement(double number, LiteralInitElement element)
        {
            Number = number;
            Element = element;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(at {Number} {Element})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Element.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a PDDL numeric function assign initial element.
    /// </summary>
    public class EqualsNumericFunctionInitElement : InitElement
    {
        /// <summary>
        /// Function term.
        /// </summary>
        public BasicNumericFunctionTerm Function { get; set; }

        /// <summary>
        /// Argument number.
        /// </summary>
        public double Number { get; set; }

        /// <summary>
        /// Constructs an init element.
        /// </summary>
        /// <param name="function">Function term.</param>
        /// <param name="number">Argument number.</param>
        public EqualsNumericFunctionInitElement(BasicNumericFunctionTerm function, double number)
        {
            Function = function;
            Number = number;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(= {Function} {Number})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Function.Accept(visitor);
        }
    }

    /// <summary>
    /// Input data structure for a PDDL object function assign initial element.
    /// </summary>
    public class EqualsObjectFunctionInitElement : InitElement
    {
        /// <summary>
        /// Function term.
        /// </summary>
        public BasicObjectFunctionTerm Function { get; set; }

        /// <summary>
        /// Argument term.
        /// </summary>
        public ConstantTerm Term { get; set; }

        /// <summary>
        /// Constructs an init element.
        /// </summary>
        /// <param name="function">Function term.</param>
        /// <param name="term">Argument term.</param>
        public EqualsObjectFunctionInitElement(BasicObjectFunctionTerm function, ConstantTerm term)
        {
            Function = function;
            Term = term;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(= {Function} {Term})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Function.Accept(visitor);
            Term.Accept(visitor);
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a grounded numeric function term.
    /// </summary>
    public class BasicNumericFunctionTerm : IVisitable
    {
        /// <summary>
        /// Function name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Argument terms.
        /// </summary>
        public ConstantTerms Terms { get; set; } = new ConstantTerms();

        /// <summary>
        /// Constructs a basic numeric function term.
        /// </summary>
        /// <param name="name">Function name.</param>
        public BasicNumericFunctionTerm(string name)
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
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Terms.Accept(visitor);
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for a grounded object function term.
    /// </summary>
    public class BasicObjectFunctionTerm : IVisitable
    {
        /// <summary>
        /// Function name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Argument terms.
        /// </summary>
        public ConstantTerms Terms { get; set; } = new ConstantTerms();

        /// <summary>
        /// Constructs a basic object function term.
        /// </summary>
        /// <param name="name">Function name.</param>
        public BasicObjectFunctionTerm(string name)
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
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Terms.Accept(visitor);
            visitor.PostVisit(this);
        }
    }
}

