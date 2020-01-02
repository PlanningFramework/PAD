using PAD.InputData.SAS;
using PAD.InputData.SAS.Validator;

namespace PAD.InputData
{
    /// <summary>
    /// Master class for a validation of SAS+ input data structure.
    /// </summary>
    public static class SASInputDataValidator
    {
        /// <summary>
        /// Validates the given SAS+ input data. Throws ValidationException in case of a validation failure.
        /// </summary>
        /// <param name="inputData">SAS+ input data.</param>
        public static void ValidateInputData(SASInputData inputData)
        {
            ValidateProblem(inputData.Problem);
        }

        /// <summary>
        /// Validates the given SAS+ problem data. Throws ValidationException in case of a validation failure.
        /// </summary>
        /// <param name="problem">SAS+ problem data.</param>
        public static void ValidateProblem(Problem problem)
        {
            new ValidateVisitor().CheckProblem(problem);
        }
    }
}
