using System.Collections.Generic;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL action effects.
    /// </summary>
    public class Effects : List<Effect>, IVisitable
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
    /// Input data structure for a single PDDL action effect.
    /// </summary>
    public abstract class Effect : IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public abstract void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Input data structure for a single PDDL (conditional) action effect of type "forall".
    /// </summary>
    public class ForallEffect : Effect
    {
        /// <summary>
        /// List of expression parameters.
        /// </summary>
        public Parameters Parameters { get; set; } = new Parameters();

        /// <summary>
        /// List of effects for the expression to be applied.
        /// </summary>
        public Effects Effects { get; set; } = new Effects();

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="parameters">List of parameters.</param>
        /// <param name="effects">List of effects.</param>
        public ForallEffect(Parameters parameters, Effects effects)
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
    public class WhenEffect : Effect
    {
        /// <summary>
        /// Argument expression.
        /// </summary>
        public Expression Expression { get; set; } = null;

        /// <summary>
        /// List of effects for the expression to be applied.
        /// </summary>
        public PrimitiveEffects Effects { get; set; } = new PrimitiveEffects();

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="expression">Argument expression.</param>
        /// <param name="effects">List of effects.</param>
        public WhenEffect(Expression expression, PrimitiveEffects effects)
        {
            Expression = expression;
            Effects = effects;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(when {Expression} {Effects})";
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Expression.Accept(visitor);
            Effects.Accept(visitor);
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for PDDL action primitive effects (i.e. without conditional effects).
    /// </summary>
    public class PrimitiveEffects : List<PrimitiveEffect>, IVisitable
    {
        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.ToBlockString(null, false, true);
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
    /// Input data structure for a single PDDL action primitive effect (i.e. without conditional effects).
    /// </summary>
    public abstract class PrimitiveEffect : Effect
    {
    }

    /// <summary>
    /// Input data structure for a single PDDL action primitive effect of atomic formula type (i.e. predicate or equals effect).
    /// </summary>
    public abstract class AtomicFormulaEffect : PrimitiveEffect
    {
    }

    /// <summary>
    /// Input data structure for a predicate effect.
    /// </summary>
    public class PredicateEffect : AtomicFormulaEffect
    {
        /// <summary>
        /// Predicate name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Argument terms.
        /// </summary>
        public Terms Terms { get; set; } = new Terms();

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="name">Predicate name.</param>
        public PredicateEffect(string name)
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
    /// Input data structure for a equals (=) effect.
    /// </summary>
    public class EqualsEffect : AtomicFormulaEffect
    {
        /// <summary>
        /// First argument term.
        /// </summary>
        public Term Term1 { get; set; } = null;

        /// <summary>
        /// Second argument term.
        /// </summary>
        public Term Term2 { get; set; } = null;

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="term1">First argument term.</param>
        /// <param name="term2">Second argument term.</param>
        public EqualsEffect(Term term1, Term term2)
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
    /// Input data structure for a "not" effect.
    /// </summary>
    public class NotEffect : PrimitiveEffect
    {
        /// <summary>
        /// Argument effect.
        /// </summary>
        public AtomicFormulaEffect Argument { get; set; } = null;

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="argument">Argument effect.</param>
        public NotEffect(AtomicFormulaEffect effect)
        {
            Argument = effect;
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
    /// Input data structure for a numeric assignment effect.
    /// </summary>
    public class NumericAssignEffect : PrimitiveEffect
    {
        /// <summary>
        /// Assignment operator specifier.
        /// </summary>
        public AssignOperator AssignOperator { get; set; } = AssignOperator.ASSIGN;

        /// <summary>
        /// Numeric function of which value will be assigned.
        /// </summary>
        public NumericFunction Function { get; set; } = null;

        /// <summary>
        /// Numeric value to be assigned.
        /// </summary>
        public NumericExpression Value { get; set; } = null;

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="assignOperator">Assignment operator.</param>
        /// <param name="function">Numeric function.</param>
        /// <param name="value">Numeric value to be assigned.</param>
        public NumericAssignEffect(AssignOperator assignOperator, NumericFunction function, NumericExpression value)
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
            visitor.PostVisit(this);
        }
    }

    /// <summary>
    /// Input data structure for an object assignment effect.
    /// </summary>
    public class ObjectAssignEffect : PrimitiveEffect
    {
        /// <summary>
        /// Object function of which value will be assigned.
        /// </summary>
        public ObjectFunctionTerm Function { get; set; } = null;

        /// <summary>
        /// Object value to be assigned.
        /// </summary>
        public Term Value { get; set; } = null;

        /// <summary>
        /// Constructs the effect.
        /// </summary>
        /// <param name="function">Object function.</param>
        /// <param name="value">Object value to be assigned.</param>
        public ObjectAssignEffect(ObjectFunctionTerm function, Term value)
        {
            Function = function;
            Value = value;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(assign {Function} {Value})";
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
            visitor.PostVisit(this);
        }
    }
}
