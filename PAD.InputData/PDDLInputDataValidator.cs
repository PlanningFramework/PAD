using PAD.InputData.PDDL;
using PAD.InputData.PDDL.Validator;
// ReSharper disable UnusedMember.Global

namespace PAD.InputData
{
    /// <summary>
    /// Master class for a validation of PDDL input data structure.
    /// </summary>
    public static class PDDLInputDataValidator
    {
        /// <summary>
        /// Validates the given PDDL input data. Throws ValidationException in case of a validation failure.
        /// </summary>
        /// <param name="inputData">PDDL input data.</param>
        public static void ValidateInputData(PDDLInputData inputData)
        {
            ValidateVisitor validator = new ValidateVisitor();
            validator.CheckDomain(inputData.Domain);
            validator.CheckProblem(inputData.Domain, inputData.Problem);
        }

        /// <summary>
        /// Validates the given PDDL domain data. Throws ValidationException in case of a validation failure.
        /// </summary>
        /// <param name="domain">PDDL domain data.</param>
        public static void ValidateDomain(Domain domain)
        {
            new ValidateVisitor().CheckDomain(domain);
        }

        /// <summary>
        /// Validates the given PDDL problem data. Throws ValidationException in case of a validation failure.
        /// </summary>
        /// <param name="correspondingDomain">Corresponding PDDL domain data.</param>
        /// <param name="problem">PDDL problem data.</param>
        public static void ValidateProblem(Domain correspondingDomain, Problem problem)
        {
            new ValidateVisitor().CheckProblem(correspondingDomain, problem);
        }
    }
}
