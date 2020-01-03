using PAD.InputData.PDDL;
// ReSharper disable UnusedMember.Global

namespace PAD.InputData
{
    /// <summary>
    /// Master input data structure specific for PDDL language.
    /// </summary>
    public class PDDLInputData : InputData
    {
        /// <summary>
        /// Input data structure for PDDL domain.
        /// </summary>
        public Domain Domain { set; get; } = new Domain();

        /// <summary>
        /// Input data structure for PDDL problem.
        /// </summary>
        public Problem Problem { set; get; } = new Problem();

        /// <summary>
        /// Constructs an empty PDDL input data.
        /// </summary>
        public PDDLInputData()
        {
        }

        /// <summary>
        /// Constructs a PDDL input data from the specified input files. The data are loaded and validated.
        /// </summary>
        /// <param name="domainFilePath">Input PDDL domain file.</param>
        /// <param name="problemFilePath">Input PDDL problem file.</param>
        /// <param name="validateData">Should the loaded data be validated?</param>
        public PDDLInputData(string domainFilePath, string problemFilePath, bool validateData = true)
        {
            LoadDomain(domainFilePath);

            if (validateData)
            {
                ValidateDomain();
            }

            LoadProblem(problemFilePath);

            if (validateData)
            {
                ValidateProblem();
            }
        }

        /// <summary>
        /// (Re)loads PDDL domain input data.
        /// </summary>
        /// <param name="domainFilePath">Input PDDL domain file.</param>
        public void LoadDomain(string domainFilePath)
        {
            Domain = PDDLInputDataLoader.LoadDomain(domainFilePath);
        }

        /// <summary>
        /// (Re)loads PDDL problem input data. Corresponding domain data needs to be present for the correct loading.
        /// </summary>
        /// <param name="problemFilePath">Input PDDL problem file.</param>
        public void LoadProblem(string problemFilePath)
        {
            Problem = PDDLInputDataLoader.LoadProblem(Domain, problemFilePath);
        }

        /// <summary>
        /// (Re)validates PDDL domain input data. Throws ValidationException in case of a validation failure.
        /// </summary>
        public void ValidateDomain()
        {
            PDDLInputDataValidator.ValidateDomain(Domain);
        }

        /// <summary>
        /// (Re)validates PDDL problem input data. Corresponding domain data needs to be present for the correct validation.
        /// Throws ValidationException in case of a validation failure.
        /// </summary>
        public void ValidateProblem()
        {
            PDDLInputDataValidator.ValidateProblem(Domain, Problem);
        }

        /// <summary>
        /// Exports the current domain data to the domain input file format.
        /// </summary>
        /// <returns>Domain data in the explicit PDDL input formalism.</returns>
        public string ExportDomainToFileFormat()
        {
            return Domain.ToString();
        }

        /// <summary>
        /// Exports the current problem data to the problem input file format.
        /// </summary>
        /// <returns>Problem data in the explicit PDDL input formalism.</returns>
        public string ExportProblemToFileFormat()
        {
            return Problem.ToString();
        }
    }
}
