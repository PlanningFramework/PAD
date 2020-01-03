using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Effects builder converting input data effects into PDDL effects.
    /// </summary>
    public class EffectsBuilder : InputData.PDDL.BaseVisitor
    {
        /// <summary>
        /// Stack of effect parts.
        /// </summary>
        private Stack<IEffect> EffectsStack { get; } = new Stack<IEffect>();

        /// <summary>
        /// Terms builder.
        /// </summary>
        private Lazy<TermsBuilder> TermsBuilder { get; }

        /// <summary>
        /// ID manager converting predicate, function, constant and type names to their corresponding IDs.
        /// </summary>
        private IdManager IdManager { get; }

        /// <summary>
        /// Constructs the effects builder.
        /// </summary>
        /// <param name="idManager">ID manager.</param>
        public EffectsBuilder(IdManager idManager)
        {
            IdManager = idManager;
            TermsBuilder = new Lazy<TermsBuilder>(() => new TermsBuilder(IdManager));
        }

        /// <summary>
        /// Builds PDDL effect from the input data.
        /// </summary>
        /// <param name="effect">Input data effect.</param>
        /// <returns>Built effect.</returns>
        public IEffect Build(InputData.PDDL.Effect effect)
        {
            Debug.Assert(EffectsStack.Count == 0);
            EffectsStack.Clear();

            effect.Accept(this);

            Debug.Assert(EffectsStack.Count == 1);
            return EffectsStack.Pop();
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void Visit(InputData.PDDL.ForallEffect data)
        {
            IdManager.Variables.RegisterLocalParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.ForallEffect data)
        {
            List<IEffect> argumentEffects = new List<IEffect>();
            for (int i = 0; i < data.Effects.Count; ++i)
            {
                argumentEffects.Add(EffectsStack.Pop());
            }
            argumentEffects.Reverse();

            EffectsStack.Push(new ForallEffect(new Parameters(data.Parameters, IdManager), argumentEffects));
            IdManager.Variables.UnregisterLocalParameters(data.Parameters);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.WhenEffect data)
        {
            List<PrimitiveEffect> argumentEffects = new List<PrimitiveEffect>();
            for (int i = 0; i < data.Effects.Count; ++i)
            {
                argumentEffects.Add((PrimitiveEffect)EffectsStack.Pop());
            }
            argumentEffects.Reverse();

            ExpressionsBuilder expressionsBuilder = new ExpressionsBuilder(IdManager);
            IExpression argumentExpression = expressionsBuilder.Build(data.Expression);

            EffectsStack.Push(new WhenEffect(argumentExpression, argumentEffects));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.PredicateEffect data)
        {
            int predicateNameId = IdManager.Predicates.GetId(data.Name, data.Terms.Count);
            List<ITerm> terms = new List<ITerm>();

            data.Terms.ForEach(term => terms.Add(TermsBuilder.Value.Build(term)));

            EffectsStack.Push(new PredicateEffect(new Atom(predicateNameId, terms)));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.EqualsEffect data)
        {
            ITerm firstArgument = TermsBuilder.Value.Build(data.Term1);
            ITerm secondArgument = TermsBuilder.Value.Build(data.Term2);

            EffectsStack.Push(new EqualsEffect(firstArgument, secondArgument));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.NotEffect data)
        {
            EffectsStack.Push(new NotEffect((AtomicFormulaEffect)EffectsStack.Pop()));
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.NumericAssignEffect data)
        {
            int functionNameId = IdManager.Functions.GetId(data.Function.Name, data.Function.Terms.Count);
            List<ITerm> terms = new List<ITerm>();

            data.Function.Terms.ForEach(term => terms.Add(TermsBuilder.Value.Build(term)));

            NumericExpressionsBuilder numericExpressionsBuilder = new NumericExpressionsBuilder(IdManager);
            INumericExpression valueExpression = numericExpressionsBuilder.Build(data.Value);

            IAtom functionAtom = new Atom(functionNameId, terms);

            IEffect newEffect = null;
            switch (data.AssignOperator)
            {
                case InputData.PDDL.Traits.AssignOperator.ASSIGN:
                {
                    newEffect = new NumericAssignEffect(functionAtom, valueExpression, IdManager);
                    break;
                }
                case InputData.PDDL.Traits.AssignOperator.INCREASE:
                {
                    newEffect = new NumericIncreaseEffect(functionAtom, valueExpression, IdManager);
                    break;
                }
                case InputData.PDDL.Traits.AssignOperator.DECREASE:
                {
                    newEffect = new NumericDecreaseEffect(functionAtom, valueExpression, IdManager);
                    break;
                }
                case InputData.PDDL.Traits.AssignOperator.SCALE_UP:
                {
                    newEffect = new NumericScaleUpEffect(functionAtom, valueExpression, IdManager);
                    break;
                }
                case InputData.PDDL.Traits.AssignOperator.SCALE_DOWN:
                {
                    newEffect = new NumericScaleDownEffect(functionAtom, valueExpression, IdManager);
                    break;
                }
                default:
                {
                    Debug.Assert(false);
                    break;
                }
            }

            EffectsStack.Push(newEffect);
        }

        /// <summary>
        /// Visits the given input data node.
        /// </summary>
        /// <param name="data">Input data node.</param>
        public override void PostVisit(InputData.PDDL.ObjectAssignEffect data)
        {
            int functionNameId = IdManager.Functions.GetId(data.Function.Name, data.Function.Terms.Count);
            List<ITerm> terms = new List<ITerm>();

            data.Function.Terms.ForEach(term => terms.Add(TermsBuilder.Value.Build(term)));

            ITerm valueTerm = TermsBuilder.Value.Build(data.Value);

            EffectsStack.Push(new ObjectAssignEffect(new Atom(functionNameId, terms), valueTerm));
        }
    }
}
