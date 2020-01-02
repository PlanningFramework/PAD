using System.Collections.Generic;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL definition terms (specific typed arguments for predicates and functions, e.g. "?paramA ?paramB - typeA").
    /// </summary>
    public class DefinitionTerms : List<DefinitionTerm>, IVisitable
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
            ForEach(term => term.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL definition term.
    /// </summary>
    public class DefinitionTerm : IVisitable
    {
        /// <summary>
        /// Name of the term.
        /// </summary>
        public string TermName { set; get; } = "";

        /// <summary>
        /// Name of the term type(s).
        /// </summary>
        public List<string> TypeNames { set; get; } = new List<string>();

        /// <summary>
        /// Specification of a default term type.
        /// </summary>
        public const string DEFAULT_TYPE = "object";

        /// <summary>
        /// Constructs a term.
        /// </summary>
        /// <param name="termName">Term name.</param>
        /// <param name="typeNames">Term type(s). Default type used if not specified.</param>
        public DefinitionTerm(string termName, string[] typeNames)
        {
            TermName = termName;
            TypeNames.AddRange(typeNames.GetTypesOrDefault(DEFAULT_TYPE));
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return TermName + TypeNames.ToTypeSpecString();
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    /// <summary>
    /// Input data structure for PDDL terms (concrete object arguments for predicates and functions in expressions, e.g. "?varA constB (objFuncC ?varA)").
    /// </summary>
    public class Terms : List<Term>, IVisitable
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
            ForEach(term => term.Accept(visitor));
        }
    }

    /// <summary>
    /// Input data structure for a single PDDL term.
    /// </summary>
    public abstract class Term : IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Input data structure for a constant term.
    /// </summary>
    public class ConstantTerm : Term
    {
        /// <summary>
        /// Constant name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Constructs the term.
        /// </summary>
        /// <param name="name">Constant name.</param>
        public ConstantTerm(string name)
        {
            Name = name;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Name;
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
    /// Input data structure for a variable term.
    /// </summary>
    public class VariableTerm : Term
    {
        /// <summary>
        /// Variable name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Constructs the term.
        /// </summary>
        /// <param name="name">Variable name.</param>
        public VariableTerm(string name)
        {
            Name = name;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Name;
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
    /// Input data structure for an object function.
    /// </summary>
    public class ObjectFunctionTerm : Term
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
        /// Constructs the term.
        /// </summary>
        /// <param name="name">Function name.</param>
        public ObjectFunctionTerm(string name)
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
    /// Input data structure for a list of constant terms.
    /// </summary>
    public class ConstantTerms : List<ConstantTerm>, IVisitable
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
            ForEach(term => term.Accept(visitor));
        }
    }
}
