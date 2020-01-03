using PAD.Launcher.Tasks.DefinitionTypes;
using System.Collections.Generic;
// ReSharper disable StringLiteralTypo

namespace PAD.Launcher.Tasks
{
    /// <summary>
    /// Loader/generator of the planning tasks from the input arguments or config file.
    /// </summary>
    public static class TasksLoader
    {
        /// <summary>
        /// Loads the planning tasks and the execution parameters from the program input arguments.
        /// If a single argument is specified, then we suppose it to be a config file.
        /// </summary>
        /// <param name="args">Program input arguments.</param>
        /// <returns>Planning tasks with the execution parameters.</returns>
        public static PlanningTasksWithExecutionParameters FromProgramArguments(string[] args)
        {
            ExecutionParameters executionParameters = new ExecutionParameters();
            List<PlanningTask> tasks = new List<PlanningTask>();

            PlanningTask currentTask = null;

            // task(s) can be specified either via config or via list of input arguments

            if (args.Length == 1)
            {
                // in the config, the task needs to be started with #<task name>
                args = ProcessConfigFile(args[0]);
            }
            else
            {
                // without the config, we implicitly define only a single task
                currentTask = new PlanningTask();
            }

            // go through the arguments

            foreach (var arg in args)
            {
                if (arg.StartsWith("#")) // start of the new task definition
                {
                    if (currentTask != null)
                    {
                        tasks.Add(currentTask);
                    }
                    currentTask = new PlanningTask();
                    continue;
                }

                var tokens = arg.Split(new[] {'='}, System.StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length != 2)
                {
                    throw new TasksLoaderException($"Invalid parameter '{arg}'!");
                }

                string paramName = tokens[0].Trim();
                string paramValue = tokens[1].Trim();

                switch (paramName.ToUpper())
                {
                    case "TYPE":
                    {
                        if (currentTask != null)
                        {
                            currentTask.Type = TypeConverter.Convert(paramValue);
                        }
                        break;
                    }
                    case "ALGORITHM":
                    {
                        if (currentTask != null)
                        {
                            currentTask.Algorithm = AlgorithmConverter.Convert(paramValue);
                        }
                        break;
                    }
                    case "HEURISTIC":
                    {
                        if (currentTask != null)
                        {
                            currentTask.Heuristic = HeuristicConverter.Convert(paramValue);
                        }
                        break;
                    }
                    case "HEAP":
                    {
                        if (currentTask != null)
                        {
                            currentTask.Heap = HeapConverter.Convert(paramValue);
                        }
                        break;
                    }
                    case "DOMAIN":
                    {
                        if (currentTask != null)
                        {
                            currentTask.DomainFile = paramValue;
                        }
                        break;
                    }
                    case "PROBLEMS":
                    {
                        if (currentTask != null)
                        {
                            currentTask.ProblemFile = paramValue;
                        }
                        break;
                    }
                    case "OUTPUTTYPE":
                    {
                        if (currentTask != null)
                        {
                            currentTask.OutputType = OutputTypeConverter.Convert(paramValue);
                        }
                        break;
                    }
                    case "OUTPUTFILE":
                    {
                        if (currentTask != null)
                        {
                            currentTask.OutputFile = paramValue;
                        }
                        break;
                    }
                    case "TIMELIMIT":
                    {
                        if (currentTask != null)
                        {
                            int timeLimit;
                            if (!int.TryParse(paramValue, out timeLimit))
                            {
                                throw new TasksLoaderException($"Invalid value of TimeLimit parameter '{paramValue}'!");
                            }
                            currentTask.TimeLimit = timeLimit;
                        }
                        break;
                    }
                    case "MEMORYLIMIT":
                    {
                        if (currentTask != null)
                        {
                            long memoryLimit;
                            if (!long.TryParse(paramValue, out memoryLimit))
                            {
                                throw new TasksLoaderException($"Invalid value of MemoryLimit parameter '{paramValue}'!");
                            }
                            currentTask.MemoryLimit = memoryLimit;
                        }
                        break;
                    }
                    case "MAXNUMBEROFPARALLELTASKS":
                    {
                        int maxNumberOfParallelTasks;
                        if (!int.TryParse(paramValue, out maxNumberOfParallelTasks))
                        {
                            throw new TasksLoaderException($"Invalid value of MaxNumberOfParallelTasks parameter '{paramValue}'!");
                        }
                        executionParameters.MaxNumberOfParallelTasks = maxNumberOfParallelTasks;
                        break;
                    }
                    default:
                    {
                        throw new TasksLoaderException($"Unknown parameter name '{paramName}'!");
                    }
                }
            }
            tasks.Add(currentTask);

            // now we validate each defined task

            List<IPlanningTask> finalTasks = new List<IPlanningTask>();

            foreach (var task in tasks)
            {
                // checks the domain and problem file paths

                if (task.Type == Type.PDDL)
                {
                    if (string.IsNullOrWhiteSpace(task.DomainFile))
                    {
                        throw new TasksLoaderException("Some of the tasks are missing domain file definition!");
                    }

                    if (!System.IO.File.Exists(task.DomainFile))
                    {
                        throw new TasksLoaderException($"Specified domain file '{task.DomainFile}' does not exist!");
                    }
                }

                if (string.IsNullOrWhiteSpace(task.ProblemFile))
                {
                    throw new TasksLoaderException("Not all tasks have input problem file(s) defined!");
                }

                // problem input files can be expressed as a single file, list of files, or a whole directory

                string problemFilePath = task.ProblemFile;

                List<string> problemPaths = new List<string>();
                if (problemFilePath.Contains(";"))
                {
                    var tokens = problemFilePath.Split(new[] {';'}, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (var path in tokens)
                    {
                        if (!System.IO.File.Exists(path))
                        {
                            throw new TasksLoaderException($"Specified problem file '{path}' does not exist!");
                        }
                    }
                    problemPaths.AddRange(tokens);
                }
                else if (System.IO.Directory.Exists(problemFilePath))
                {
                    problemPaths.AddRange(System.IO.Directory.GetFiles(problemFilePath));
                }
                else if (System.IO.File.Exists(problemFilePath))
                {
                    problemPaths.Add(problemFilePath);
                }
                else
                {
                    throw new TasksLoaderException($"Specified problem file '{problemFilePath}' does not exist!");
                }

                // process each path entry as a new task

                foreach (var path in problemPaths)
                {
                    PlanningTask clonedTask = task.Clone();
                    clonedTask.ProblemFile = path;

                    // if output file path not set (and should be set), then derive it from the problem file path
                    if (clonedTask.OutputType == OutputType.ToFile && string.IsNullOrWhiteSpace(clonedTask.OutputFile))
                    {
                        clonedTask.OutputFile = $"{clonedTask.ProblemFile}_results";
                    }

                    finalTasks.Add(clonedTask);
                }
            }

            return new PlanningTasksWithExecutionParameters(finalTasks.ToArray(), executionParameters);
        }

        /// <summary>
        /// Reads the config file and returns the array of line strings (without empty lines).
        /// </summary>
        /// <param name="configFile">Input config file.</param>
        /// <returns>Lines of the config file.</returns>
        private static string[] ProcessConfigFile(string configFile)
        {
            if (!System.IO.File.Exists(configFile))
            {
                throw new TasksLoaderException($"Config file '{configFile}' does not exist!");
            }

            List<string> lines = new List<string>();
            foreach (var line in System.IO.File.ReadLines(configFile))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                lines.Add(line);
            }

            return lines.ToArray();
        }
    }
}
