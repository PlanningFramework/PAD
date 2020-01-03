using PAD.Planner.Heuristics;
using System.Collections.Generic;

namespace PAD.Planner.Search
{
    /// <summary>
    /// Collection of transition candidates in Beam search.
    /// </summary>
    public class BeamSearchTransitionCandidates : LinkedList<BeamSearchTransitionCandidate>
    {
        /// <summary>
        /// Planning problem.
        /// </summary>
        protected ISearchableProblem Problem { set; get; }

        /// <summary>
        /// Heuristic.
        /// </summary>
        protected ISearchableHeuristic Heuristic { set; get; }

        /// <summary>
        /// Maximal size of the candidates collection.
        /// </summary>
        protected int MaxSize { set; get; }

        /// <summary>
        /// Constructs the transition candidates collection.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        /// <param name="heuristic">Heuristic.</param>
        /// <param name="maxSize">Maximal size of the collection.</param>
        public BeamSearchTransitionCandidates(ISearchableProblem problem, ISearchableHeuristic heuristic, int maxSize)
        {
            Problem = problem;
            Heuristic = heuristic;
            MaxSize = maxSize;
        }

        /// <summary>
        /// Selects the best transition candidates of the specified node and adds them to the collection.
        /// </summary>
        /// <param name="node">Search node to be evaluated.</param>
        public void SelectBestTransitionCandidates(ISearchNode node)
        {
            Clear();

            foreach (var transition in Problem.GetTransitions(node))
            {
                ISearchNode newNode = transition.GetTransitionResult();
                double appliedOperatorCost = transition.GetAppliedOperator().GetCost();

                double worstSuccessorCandidateCost = WorstTransitionCandidateCost();
                if (Count >= MaxSize && worstSuccessorCandidateCost < appliedOperatorCost)
                {
                    continue;
                }

                double hValueNewState = Heuristic.GetValue(newNode);
                if (Count < MaxSize || worstSuccessorCandidateCost > appliedOperatorCost + hValueNewState)
                {
                    Add(new BeamSearchTransitionCandidate(newNode, hValueNewState, transition.GetAppliedOperator()));
                }
            }
        }

        /// <summary>
        /// Adds the specified successor candidates to the collection.
        /// </summary>
        /// <param name="newCandidate">New successor candidate.</param>
        protected void Add(BeamSearchTransitionCandidate newCandidate)
        {
            if (Count == 0)
            {
                AddFirst(newCandidate);
                return;
            }

            if (WorstTransitionCandidateCost() < newCandidate.GetCost())
            {
                AddLast(newCandidate);
                return;
            }

            var iterator = Last;
            while (iterator.Previous != null && iterator.Previous.Value.GetCost() > newCandidate.GetCost())
            {
                iterator = iterator.Previous;
            }

            AddBefore(iterator, newCandidate);
            if (Count > MaxSize)
            {
                RemoveLast();
            }
        }

        /// <summary>
        /// Gets the worst cost of the successor candidates.
        /// </summary>
        /// <returns>Worst cost of the successors candidates.</returns>
        protected double WorstTransitionCandidateCost()
        {
            return (Count != 0) ? Last.Value.GetCost() : int.MaxValue;
        }
    }
}
