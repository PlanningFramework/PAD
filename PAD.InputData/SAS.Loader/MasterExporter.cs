using System.Collections.Generic;
using System.IO;
using System;

namespace PAD.InputData.SAS.Loader
{
    /// <summary>
    /// Master class for parsing SAS+ input files and exporting them into SAS+ input data structures.
    /// </summary>
    public class MasterExporter
    {
        /// <summary>
        /// SAS+ input problem currently being loaded.
        /// </summary>
        private Problem Problem { set; get; } = null;

        /// <summary>
        /// Current input file reader.
        /// </summary>
        private FileReader Reader { set; get; } = null;

        /// <summary>
        /// Parses and loads the specified SAS+ input problem file into the input data structure.
        /// </summary>
        /// <param name="problemFilePath">SAS+ input problem file.</param>
        /// <returns>Loaded SAS+ problem input data structure.</returns>
        public Problem LoadProblem(string problemFilePath)
        {
            Problem = new Problem();
            Problem.Name = Path.GetFileNameWithoutExtension(problemFilePath);
            Problem.FilePath = problemFilePath;

            Reader = new FileReader(problemFilePath);

            LoadVersion();
            LoadMetric();
            LoadVariables();
            LoadMutexGroups();
            LoadInitialState();
            LoadGoalConditions();
            LoadOperators();
            LoadAxiomRules();

            Reader.Close();

            return Problem;
        }

        /// <summary>
        /// Loads a SAS+ version section. Does not have to be present in SAS+ version 1 or 2.
        /// </summary>
        private void LoadVersion()
        {
            if (!CheckExpected(PeekNextLine(), "begin_version", false))
            {
                return;
            }

            CheckExpected(GetNextLine(), "begin_version");

            Problem.Version.Number = ParseAndCheckNumber(GetNextLine());

            CheckExpected(GetNextLine(), "end_version");
        }

        /// <summary>
        /// Loads a SAS+ metric section. Does not have to be present in SAS+ version 1.
        /// </summary>
        private void LoadMetric()
        {
            if (!CheckExpected(PeekNextLine(), "begin_metric", false))
            {
                // both version and metric sections are missing only in version 1; otherwise metric needs to be present
                if (Problem.Version.Number == 0)
                {
                    Problem.Version.Number = 1;
                    return;
                }
            }
            else
            {
                // version section is missing while metric section is present only in version 2
                if (Problem.Version.Number == 0)
                {
                    Problem.Version.Number = 2;
                }
            }

            CheckExpected(GetNextLine(), "begin_metric");

            int metric = ParseAndCheckNumber(GetNextLine());
            if (metric != 0 && metric != 1)
            {
                throw GetException($"Invalid metric specifier '{metric}' - parameter value can be only 0 or 1.");
            }
            Problem.Metric.IsUsed = (metric == 1);

            CheckExpected(GetNextLine(), "end_metric");
        }

        /// <summary>
        /// Loads a SAS+ variables section.
        /// </summary>
        private void LoadVariables()
        {
            int variablesCount = ParseAndCheckNumber(GetNextLine());
            for (int i = 0; i < variablesCount; ++i)
            {
                CheckExpected(GetNextLine(), "begin_variable");

                string variableName = GetNextLine();
                int variableAxiomLayer = ParseAndCheckNumber(GetNextLine(), InputNumberType.NonNegativeOrMinusOne);
                int variableDomainRange = ParseAndCheckNumber(GetNextLine());

                List<string> values = new List<string>();
                for (int j = 0; j < variableDomainRange; ++j)
                {
                    values.Add(GetNextLine());
                }

                Problem.Variables.Add(new Variable(variableName, variableAxiomLayer, values));

                CheckExpected(GetNextLine(), "end_variable");
            }
        }

        /// <summary>
        /// Loads a SAS+ mutexes section.
        /// </summary>
        private void LoadMutexGroups()
        {
            int mutexGroupsCount = ParseAndCheckNumber(GetNextLine());
            for (int i = 0; i < mutexGroupsCount; ++i)
            {
                CheckExpected(GetNextLine(), "begin_mutex_group");

                MutexGroup mutexGroup = new MutexGroup();

                int mutexGroupSize = ParseAndCheckNumber(GetNextLine());
                for (int j = 0; j < mutexGroupSize; ++j)
                {
                    var numberPair = ParseAndCheckNumberPair(GetNextLine());
                    mutexGroup.Add(new Assignment(numberPair.Item1, numberPair.Item2));
                }

                if (mutexGroup.Count > 0)
                {
                    Problem.MutexGroups.Add(mutexGroup);
                }

                CheckExpected(GetNextLine(), "end_mutex_group");
            }
        }

        /// <summary>
        /// Loads a SAS+ initial state section.
        /// </summary>
        private void LoadInitialState()
        {
            CheckExpected(GetNextLine(), "begin_state");

            for (int i = 0; i < Problem.Variables.Count; ++i)
            {
                int value = ParseAndCheckNumber(GetNextLine());
                Problem.InitialState.Add(value);
            }

            CheckExpected(GetNextLine(), "end_state");
        }

        /// <summary>
        /// Loads a SAS+ goal state section.
        /// </summary>
        private void LoadGoalConditions()
        {
            CheckExpected(GetNextLine(), "begin_goal");

            int conditionsCount = ParseAndCheckNumber(GetNextLine());
            for (int i = 0; i < conditionsCount; ++i)
            {
                var numberPair = ParseAndCheckNumberPair(GetNextLine());
                Problem.GoalConditions.Add(new Assignment(numberPair.Item1, numberPair.Item2));
            }

            CheckExpected(GetNextLine(), "end_goal");
        }

        /// <summary>
        /// Loads a SAS+ operators section.
        /// </summary>
        private void LoadOperators()
        {
            int operatorsCount = ParseAndCheckNumber(GetNextLine());
            for (int i = 0; i < operatorsCount; ++i)
            {
                CheckExpected(GetNextLine(), "begin_operator");

                Operator newOperator = new Operator();
                newOperator.Name = GetNextLine();

                int prevailConditionsCount = ParseAndCheckNumber(GetNextLine());
                for (int j = 0; j < prevailConditionsCount; ++j)
                {
                    var numberPair = ParseAndCheckNumberPair(GetNextLine());
                    newOperator.Conditions.Add(new Assignment(numberPair.Item1, numberPair.Item2));
                }

                int effectsCount = ParseAndCheckNumber(GetNextLine());
                for (int j = 0; j < effectsCount; ++j)
                {
                    Effect effect = new Effect();

                    var effectParamList = ParseAndCheckNumberList(GetNextLine());

                    int conditionsCount = effectParamList[0];
                    int parameterIdx = 1;

                    if (effectParamList.Count != 1 + conditionsCount*2 + 3)
                    {
                        throw GetException($"List of effect parameters '{string.Join(" ", effectParamList)}' is not in a correct form.");
                    }

                    for (int condition = 0; condition < conditionsCount; ++condition)
                    {
                        int conditionVariable = effectParamList[parameterIdx++];
                        int conditionEffect = effectParamList[parameterIdx++];
                        effect.Conditions.Add(new Assignment(conditionVariable, conditionEffect));
                    }

                    int effectVariable = effectParamList[parameterIdx++];
                    int effectPreconditionValue = effectParamList[parameterIdx++];
                    int effectNewValue = effectParamList[parameterIdx++];

                    if (effectPreconditionValue != -1)
                    {
                        // convert the effect precondition value into operator conditions
                        newOperator.Conditions.Add(new Assignment(effectVariable, effectPreconditionValue));
                    }

                    effect.PrimitiveEffect = new Assignment(effectVariable, effectNewValue);

                    newOperator.Effects.Add(effect);
                }

                newOperator.Cost = ParseAndCheckNumber(GetNextLine());

                Problem.Operators.Add(newOperator);

                CheckExpected(GetNextLine(), "end_operator");
            }
        }

        /// <summary>
        /// Loads a SAS+ axioms section.
        /// </summary>
        private void LoadAxiomRules()
        {
            int axiomRulesCount = ParseAndCheckNumber(GetNextLine());
            for (int i = 0; i < axiomRulesCount; ++i)
            {
                CheckExpected(GetNextLine(), "begin_rule");

                AxiomRule axiomRule = new AxiomRule();

                int conditionsCount = ParseAndCheckNumber(GetNextLine());
                for (int j = 0; j < conditionsCount; ++j)
                {
                    var numberPair = ParseAndCheckNumberPair(GetNextLine());
                    axiomRule.Conditions.Add(new Assignment(numberPair.Item1, numberPair.Item2));
                }

                var ruleHeadList = ParseAndCheckNumberList(GetNextLine());
                if (ruleHeadList.Count != 3)
                {
                    throw GetException($"Head of axiom rule '{string.Join(" ", ruleHeadList)}' is not in a correct form.");
                }

                int ruleVariable = ruleHeadList[0];
                int rulePreconditionValue = ruleHeadList[1];
                int ruleNewValue = ruleHeadList[2];

                if (rulePreconditionValue != -1)
                {
                    // convert the rule precondition value into the rule conditions
                    axiomRule.Conditions.Add(new Assignment(ruleVariable, rulePreconditionValue));
                }

                axiomRule.PrimitiveEffect = new Assignment(ruleVariable, ruleNewValue);

                Problem.AxiomRules.Add(axiomRule);

                CheckExpected(GetNextLine(), "end_rule");
            }
        }

        /// <summary>
        /// Peeks the next line from the file reader, without consuming it.
        /// </summary>
        /// <returns>Next line of the input file.</returns>
        private string PeekNextLine()
        {
            return Reader.PeekNextLine();
        }

        /// <summary>
        /// Fetches the next line from the file reader.
        /// </summary>
        /// <returns>Next line of the input file.</returns>
        private string GetNextLine()
        {
            return Reader.GetNextLine();
        }

        /// <summary>
        /// Checks the expected content of the string. Throws LoadingException if the string doesn't match to the expected content.
        /// </summary>
        /// <param name="text">String to be checked.</param>
        /// <param name="expectedContent">Expected content of the string.</param>
        /// <param name="throwIfMismatch">Should the method throw an exception when the string doesn't have expected content?</param>
        /// <returns>False if the string doesn't have expected content (and exception throwing is not enabled).</returns>
        private bool CheckExpected(string text, string expectedContent, bool throwIfMismatch = true)
        {
            if (!string.Equals(text, expectedContent))
            {
                if (throwIfMismatch)
                {
                    throw GetException($"'{expectedContent}' expected, but '{text}' value found.");
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Input mode of the number parsing function.
        /// </summary>
        private enum InputNumberType
        {
            /// <summary>
            /// The parsed number has to be non-negative.
            /// </summary>
            NonNegative,

            /// <summary>
            /// The parsed number has to be non-negative or -1.
            /// </summary>
            NonNegativeOrMinusOne
        }

        /// <summary>
        /// Parses and checks a number from the string. Throws LoadingException if the string is not a correct number.
        /// </summary>
        /// <param name="text">String to be parsed.</param>
        /// <param name="numberType">Input number type.</param>
        /// <returns>Parsed number from the given string.</returns>
        private int ParseAndCheckNumber(string text, InputNumberType numberType = InputNumberType.NonNegative)
        {
            int number;
            bool valid = int.TryParse(text, out number);

            if (!valid)
            {
                throw GetException($"Integer number expected, but '{text}' value found.");
            }

            if (number < 0)
            {
                if (numberType == InputNumberType.NonNegativeOrMinusOne)
                {
                    if (number != -1)
                    {
                        throw GetException($"Non-negative integer or -1 number expected, but '{number}' found.");
                    }
                }
                else
                {
                    throw GetException($"Non-negative integer number expected, but '{number}' found.");
                }
            }

            return number;
        }

        /// <summary>
        /// Parses and checks a pair of numbers from the string. Throws LoadingException if the string is not a correct pair of numbers.
        /// </summary>
        /// <param name="text">String to be parsed.</param>
        /// <returns>Parsed pair of numbers from the given string.</returns>
        private Tuple<int,int> ParseAndCheckNumberPair(string text)
        {
            string[] numbersList = SplitByWhiteSpaces(text);

            if (numbersList.Length != 2)
            {
                throw GetException($"Variable-value pair of numbers expected, but '{text}' found.");
            }

            int firstNumber = ParseAndCheckNumber(numbersList[0]);
            int secondNumber = ParseAndCheckNumber(numbersList[1]);

            return Tuple.Create(firstNumber, secondNumber);
        }

        /// <summary>
        /// Parses and checks a list of numbers from the string. Throws LoadingException if the string is not a correct list of numbers.
        /// </summary>
        /// <param name="text">String to be parsed.</param>
        /// <returns>Parsed list of numbers from the given string.</returns>
        private List<int> ParseAndCheckNumberList(string text)
        {
            List<int> retList = new List<int>();

            string[] numbersList = SplitByWhiteSpaces(text);

            if (numbersList.Length <= 1)
            {
                throw GetException($"List of numbers expected, but '{text}' found.");
            }

            for (int i = 0; i < numbersList.Length; ++i)
            {
                InputNumberType inputNumberType = (i == numbersList.Length - 2) ? InputNumberType.NonNegativeOrMinusOne
                                                                                : InputNumberType.NonNegative;
                retList.Add(ParseAndCheckNumber(numbersList[i], inputNumberType));
            }

            return retList;
        }

        /// <summary>
        /// Splits the given string by whitespaces into a list of string tokens.
        /// </summary>
        /// <param name="text">String to be parsed.</param>
        /// <returns>List of string tokens.</returns>
        private string[] SplitByWhiteSpaces(string text)
        {
            return text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Gets the loading exception to be thrown, with the location in the input data file.
        /// </summary>
        /// <param name="reason">Reason of a loading failure.</param>
        /// <returns>Loading exception to be thrown.</returns>
        private LoadingException GetException(string reason)
        {
            return new LoadingException($"Line {Reader.LineNumber}: {reason}");
        }
    }
}
