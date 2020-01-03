
## Project PAD.Planner

### 1. License

Project PAD.Planner (a part of PAD Framework) 
Copyright (C) 2020 Tomas Hurt
  
This program is free software: you can redistribute it and/or modify  
it under the terms of the GNU General Public License as published by  
the Free Software Foundation, either version 3 of the License, or  
(at your option) any later version.  
  
This program is distributed in the hope that it will be useful,  
but WITHOUT ANY WARRANTY; without even the implied warranty of  
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the  
GNU General Public License for more details.  
  
You should have received a copy of the GNU General Public License  
along with this program.  If not, see <http://www.gnu.org/licenses/>.

### 2. Description

* This project is a part of PAD Framework and provides a planner with the heuristic search engine.
* Available planning problems implementations:
    * PDDL
    * SAS+
* Available search algorithms:
    * A* Search
    * Beam Search
    * Hill-Climbing Search
    * Iterative Deepening A*
    * Multi-Heuristic A*
* Available heuristics:
    * Blind
    * STRIPS
    * AdditiveRelaxation
    * MaxRelaxation
    * PerfectRelaxation
    * FF
    * PDB
    * Weighted
    * Max, Min, Sum, WeightedSum
* Available heaps for open nodes:
    * Binomial
    * Fibonacci
    * Leftist
    * RedBlackTree
    * RegularBinary
    * RegularTernary
    * Radix
    * SingleBucket

### 3. Examples

Prepare all planning components and start the search:

    var problem = new Planner.SAS.Problem("problem.sas");
    var heuristic = new Planner.Heuristics.FFHeuristic(problem);
    var heap = new Planner.Heaps.BinomialHeap();

    var search = new Planner.Search.AStarSearch(problem, heuristic, heap);
    if (search.Start() == Planner.Search.ResultStatus.SolutionFound)
    {
      Console.WriteLine(search.GetSolutionPlan().ToString());
    }
