
## PAD Framework

### 1. Contact

- Author: Tomáš Hurt (hurthys@seznam.cz)

### 2. Description

**PAD Framework** is an open source planning framework under **GPL** license.

The purpose of PAD Framework is to provide an efficient tool for the development and testing of new algorithms in the research area of automated planning. It supports the two most popular input formalisms for the description of planning problems (**PDDL** and **SAS**<sup>**+**</sup>), while allowing to use an internal planner using heuristic search approach. The framework is implemented in **C#** and **.NET** environment.

**PAD Framework** consists of 4 primary **projects**:

  * **PAD.InputData** -- provides loaders and validators of PDDL 3.1 and SAS<sup>+</sup> input data into encapsulated data structures; can be used independently from the rest of the framework,
  * **PAD.Planner** -- provides planner with the heuristic search engine, implementing *A* Search*, *Beam Search* and *Hill-Climbing Search*, including implementations of many heuristics and heap data structures,
  * **PAD.Launcher** -- provides components for automated parallel execution of planning tasks batches, allowing parametrized definition of planning tasks via configuration file,
  * **PAD.Tests** -- provides validation of all critical components of the framework via a big suite of unit tests and black-box batch tests; containing many test-cases from the IPC (*International Planning Competition*)

### 3. How to use the PAD Framework?

* You need at least *Visual Studio 2015 Community* and *.NET Framework 4.5.2*.
* Just use the solution file *.\PAD.sln*, or import subset of the projects to your own solution.
* Alternatively, use *Mono* project to compile C# sources under Linux environment.

#### Used NuGet packages

* Irony 0.9.1
* Irony.Interpreter 0.9.1
* SoftUni.Wintellect.PowerCollections

### 4. Changelog

**PAD Framework 1.0.0** - January 2, 2020
- initialization of the repository
