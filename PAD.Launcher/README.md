
## Project PAD.Launcher

### 1. License

Project PAD.Launcher (a part of PAD Framework) 
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

* This project is a part of PAD Framework and provides components for automated parallel execution of planning tasks.
* The tasks for the execution can be created and launched programmatically or via program input parameters or configuration file.

### 3. Examples

Prepare planning tasks and execute them in parallel:

    var task = new PlanningTask
    {
      Type = Type.SAS,
      Heuristic = Heuristic.FFHeuristic,
      ProblemFile = "elevators.sas"
    };
    
    var task2 = new PlanningTaskExplicit
    {
      HeuristicSearch = GetMyHeuristicSearch()
    };
    
    TasksLauncher.Execute(task, task2);

Alternatively, compile the project as start-up and launch it with config file as a command line argument:

    Launcher.exe planningTasks.config

The config file for the planning tasks can be something like this (see the valid format description):

    MaxNumberOfParallelTasks=8

    \# Task 1
    Type=PDDL
    Algorithm=AStarSearch
    Heuristic=FFHeuristic
    Heap=BinomialHeap
    Domain=domain.pddl
    Problems=problem.pddl
    OutputType=ToFile
    OutputFile=results.txt
    TimeLimit=60
    MemoryLimit=50000

    \# Task 2
    Type=SAS+
    Algorithm=HillClimbingSearch
    Problems=problem1.sas;problem2.sas

    \# Task 3
    Type=SAS+
    Problems=Problems/SAS/

### 4. Valid format of configuration files

    [MaxNumberOfParallelTasks=UNSIGNED_INTEGER]

    \# <taskName1>
    [Type=PDDL|SAS+]
    [Algorithm=AStarSearch|BeamSearch|HillClimbingSearch]
    [Heuristic=BlindHeuristic|StripsHeuristic|PerfectRelaxationHeuristic|AdditiveRelaxationHeuristic|MaxRelaxationHeuristic|FFHeuristic|PDBHeuristic]
    [Heap=BinomialHeap|FibonacciHeap|FibonacciHeap2|LeftistHeap|RedBlackTreeHeap|RegularBinaryHeap|RegularTernaryHeap]
    [Domain=STRING]
    [Problems=STRING]
    [OutputType=ToFile|ToConsole]
    [OutputFile=STRING]
    [TimeLimit=UNSIGNED_INTEGER]
    [MemoryLimit=UNSIGNED_INTEGER]

    \# <taskName2>
    ...
