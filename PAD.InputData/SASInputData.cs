using PAD.InputData.SAS;
// ReSharper disable UnusedMember.Global

namespace PAD.InputData
{
    /// <summary>
    /// Input data structure specific for SAS+ language.
    /// </summary>
    public class SASInputData : InputData
    {
        /// <summary>
        /// Input data structure for SAS+ problem.
        /// </summary>
        public Problem Problem { set; get; } = new Problem();

        /// <summary>
        /// Constructs an empty SAS+ input data.
        /// </summary>
        public SASInputData()
        {
        }

        /// <summary>
        /// Constructs a SAS+ input data from the specified input file. The data are loaded and validated.
        /// </summary>
        /// <param name="problemFilePath">Input SAS+ problem file.</param>
        /// <param name="validateData">Should the loaded data be validated?</param>
        public SASInputData(string problemFilePath, bool validateData = true)
        {
            LoadProblem(problemFilePath);

            if (validateData)
            {
                ValidateProblem();
            }
        }

        /// <summary>
        /// (Re)loads SAS+ problem input data.
        /// </summary>
        /// <param name="problemFilePath">Input SAS+ problem file.</param>
        public void LoadProblem(string problemFilePath)
        {
            Problem = SASInputDataLoader.LoadProblem(problemFilePath);
        }

        /// <summary>
        /// (Re)validates SAS+ input data. Throws ValidationException in case of a validation failure.
        /// </summary>
        public void ValidateProblem()
        {
            SASInputDataValidator.ValidateProblem(Problem);
        }

        /// <summary>
        /// Exports the current problem data to the problem input file format.
        /// </summary>
        /// <returns>Problem data in the explicit SAS+ input formalism.</returns>
        public string ExportProblemToFileFormat()
        {
            return Problem.ToString();
        }
    }
}
