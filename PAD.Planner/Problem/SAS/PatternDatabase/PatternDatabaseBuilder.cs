using PAD.Planner.Heaps;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of the SAS+ pattern database builder, calculating the individual patterns. The patterns are supposed to be additive and can be
    /// explicitly chosen, or automatically determined by the builder by certain criteria. Then distances to the goals in the corresponding planning
    /// problem are calculated and the resulting pattern databases are returned to the caller.
    /// </summary>
    public class PatternDatabaseBuilder
    {
        /// <summary>
        /// Corresponding planning problem.
        /// </summary>
        private Problem Problem { set; get; } = null;

        /// <summary>
        /// Constructs the builder.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        public PatternDatabaseBuilder(Problem problem)
        {
            Problem = problem;
        }

        /// <summary>
        /// Builds the collection of calculated patterns. The patterns can be automatically found, or explicitly chosen by the user.
        /// </summary>
        /// <param name="findAdditivePatterns">Should the additive patterns be automatically found by the builder?</param>
        /// <param name="requestedPatterns">Explicitly requested patterns for the database (only if findAdditivePatterns is false).</param>
        /// <returns></returns>
        public List<Pattern> BuildPatterns(bool findAdditivePatterns = true, List<HashSet<int>> requestedPatterns = null)
        {
            List<Pattern> resultPatterns = new List<Pattern>();

            List<HashSet<int>> chosenPatterns = (findAdditivePatterns) ? FindAdditivePatterns() : requestedPatterns;

            foreach (var chosenPattern in chosenPatterns)
            {
                int[] pattern = chosenPattern.ToArray();
                Array.Sort(pattern);

                resultPatterns.Add(new Pattern(pattern, ComputePatternDistances(pattern)));
            }

            return resultPatterns;
        }

        /// <summary>
        /// Computes the distances to the goals for the specified pattern.
        /// </summary>
        /// <param name="pattern">Pattern (i.e. variables of the pattern) to process.</param>
        private PatternValuesDistances ComputePatternDistances(int[] pattern)
        {
            IHeap<double, ISimpleConditions> fringe = new LeftistHeap<ISimpleConditions>();
            InsertPatternConditions(fringe, (Conditions)Problem.GoalConditions, 0, pattern);

            PatternValuesDistances patternValuesDistances = new PatternValuesDistances();

            while (fringe.GetSize() > 0)
            {
                double distance = fringe.GetMinKey();
                ISimpleConditions conditions = fringe.RemoveMin();

                int[] patternValues = conditions.GetAssignedValues();

                Debug.Assert(pattern.Length == conditions.GetSize());
                Debug.Assert(pattern.Length == patternValues.Length);

                if (patternValuesDistances.ContainsKey(patternValues))
                {
                    // already processed with a lower cost
                    continue;
                }

                patternValuesDistances.Add(patternValues, distance);

                foreach (var predecessor in Problem.GetPredecessors(conditions))
                {
                    IConditions predecessorConditions = (IConditions)predecessor.GetPredecessorConditions();
                    IOperator predecessorOperator = (IOperator)predecessor.GetAppliedOperator();

                    foreach (var predecessorSimpleConditions in predecessorConditions.GetSimpleConditions())
                    {
                        double cost = distance + predecessorOperator.GetCost();
                        InsertPatternConditions(fringe, predecessorSimpleConditions, cost, pattern);
                    }
                }
            }

            return patternValuesDistances;
        }

        /// <summary>
        /// Inserts all conditions of the specified pattern satisfying the given goals to fringe (with the given cost).
        /// </summary>
        /// <param name="fringe">Fringe.</param>
        /// <param name="goalConditions">Current goal conditions.</param>
        /// <param name="cost">New conditions cost.</param>
        /// <param name="pattern">Requested pattern.</param>
        private void InsertPatternConditions(IHeap<double, ISimpleConditions> fringe, ISimpleConditions goalConditions, double cost, int[] pattern)
        {
            InsertPatternConditions(fringe, goalConditions, cost, pattern, 0, new List<int>());
        }

        /// <summary>
        /// Inserts all goal conditions of the corresponding pattern to fringe. I.e. fixes the goal values and generates all combinations
        /// for the rest of variables by the method divide-and-conquer.
        /// </summary>
        /// <param name="fringe">Fringe.</param>
        /// <param name="pattern">Requested pattern.</param>
        /// <param name="currentVariableIndex">Current variable index.</param>
        /// <param name="values">Generated values.</param>
        private void InsertPatternConditions(IHeap<double, ISimpleConditions> fringe, ISimpleConditions goalConditions, double cost, int[] pattern, int currentVariableIndex, List<int> values)
        {
            if (values.Count == pattern.Length)
            {
                Conditions conditions = new Conditions();
                int i = 0;

                foreach (var item in pattern)
                {
                    conditions.Add(new Assignment(item, values[i]));
                    i++;
                }

                fringe.Add(cost, conditions);
                return;
            }

            int currentVariable = pattern[currentVariableIndex];

            int constrainedGoalValue = -1;
            if (goalConditions.IsVariableConstrained(currentVariable, out constrainedGoalValue))
            {
                values.Add(constrainedGoalValue);
                InsertPatternConditions(fringe, goalConditions, cost, pattern, currentVariableIndex + 1, values);
                values.RemoveAt(values.Count - 1);
            }
            else
            {
                for (int i = 0; i < Problem.Variables[currentVariable].GetDomainRange(); ++i)
                {
                    values.Add(i);
                    InsertPatternConditions(fringe, goalConditions, cost, pattern, currentVariableIndex + 1, values);
                    values.RemoveAt(values.Count - 1);
                }
            }
        }

        /// <summary>
        /// Automatically finds additive patterns of the current planning problem.
        /// </summary>
        /// <returns>List of additive patterns.</returns>
        private List<HashSet<int>> FindAdditivePatterns()
        {
            PatternsFinder finder = new PatternsFinder(Problem);
            return finder.FindAdditivePatterns();
        }
    }
}
