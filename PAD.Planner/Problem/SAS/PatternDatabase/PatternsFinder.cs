using System.Collections.Generic;
using System;

namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of a patterns finder, automatically generating additive patterns for a pattern database. These patterns are
    /// derived from the graph of variables affected by operator effects: two variables are connected in this graph when there is
    /// an operator changing them both at once. The constructed patterns are the (connection) components in this graph, i.e. the
    /// variable groups corresponding to these components. Additionaly, the components not affecting the goals are omitted.
    /// </summary>
    public class PatternsFinder
    {
        /// <summary>
        /// Corresponding planning problem.
        /// </summary>
        private Problem Problem { set; get; } = null;

        /// <summary>
        /// Constructs the finder.
        /// </summary>
        /// <param name="problem">Planning problem.</param>
        public PatternsFinder(Problem problem)
        {
            Problem = problem;
        }

        /// <summary>
        /// Finds the list of additive patterns for the pattern database.
        /// </summary>
        /// <returns>Lsit of additive patterns.</returns>
        public List<HashSet<int>> FindAdditivePatterns()
        {
            var components = FindComponents(BuildAffectionGraph());
            DeleteNonGoalComponents(components);
            return components;
        }

        /// <summary>
        /// Builds the 'affection' graph where variables are vertices, that connected iff there is an operator in the planning
        /// problem changing them both at once. The result is a graph in the form of [vertex, [vertexNeighbours]].
        /// </summary>
        /// <returns>Affection graph base on the planning problem.</returns>
        private Dictionary<int, HashSet<int>> BuildAffectionGraph()
        {
            Dictionary<int, HashSet<int>> edges = new Dictionary<int, HashSet<int>>();
            for (int i = 0; i < Problem.Variables.Count; ++i)
            {
                edges.Add(i, new HashSet<int>());
            }

            foreach (Operator op in Problem.Operators)
            {
                for (int i = 0; i < op.Effects.Count; ++i)
                {
                    for (int j = i + 1; j < op.Effects.Count; ++j)
                    {
                        int firstVariable = op.Effects[i].GetAssignment().GetVariable();
                        int secondVariable = op.Effects[j].GetAssignment().GetVariable();

                        edges[firstVariable].Add(secondVariable);
                        edges[secondVariable].Add(firstVariable);
                    }
                }
            }

            return edges;
        }

        /// <summary>
        /// Finds the (connection) components in the affection graph.
        /// </summary>
        /// <param name="graph">Affection graph.</param>
        /// <returns>List of components (as a set of contained variables).</returns>
        private List<HashSet<int>> FindComponents(Dictionary<int, HashSet<int>> graph)
        {
            bool[] visited = new bool[Problem.Variables.Count];
            List<HashSet<int>> components = new List<HashSet<int>>();

            Action<int, int> AddReachable = null;
            AddReachable = (int variable, int componentNumber) =>
            {
                if (!visited[variable])
                {
                    visited[variable] = true;
                    components[componentNumber].Add(variable);
                    foreach (var item in graph[variable])
                    {
                        AddReachable(item, componentNumber);
                    }
                }
            };

            for (int variable = 0; variable < Problem.Variables.Count; ++variable)
            {
                if (!visited[variable])
                {
                    components.Add(new HashSet<int>());
                    AddReachable(variable, components.Count - 1);
                }
            }

            return components;
        }

        /// <summary>
        /// Deletes the components that are not affecting goals of the planning problem, i.e. are not interesting for us.
        /// </summary>
        /// <param name="components">Components.</param>
        private void DeleteNonGoalComponents(List<HashSet<int>> components)
        {
            components.RemoveAll(component =>
            {
                foreach (var goalCondition in Problem.GoalConditions.GetSimpleConditions())
                {
                    var condition = (Conditions)goalCondition;
                    foreach (var assignment in condition)
                    {
                        if (component.Contains(assignment.GetVariable()))
                        {
                            return false;
                        }
                    }
                }
                return true;
            });
        }
    }
}
