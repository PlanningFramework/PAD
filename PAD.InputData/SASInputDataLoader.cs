using PAD.InputData.SAS.Loader;

namespace PAD.InputData
{
    /// <summary>
    /// Master class for loading SAS+ data from an input file.
    /// </summary>
    public static class SASInputDataLoader
    {
        /// <summary>
        /// Loads a SAS+ input data from the given input file. Does not include an input data validation.
        /// </summary>
        /// <param name="problemFilePath">SAS+ problem input file.</param>
        /// <returns>Instance of master SAS+ input data structure.</returns>
        public static SASInputData LoadInputData(string problemFilePath)
        {
            SASInputData inputData = new SASInputData();
            inputData.Problem = LoadProblem(problemFilePath);
            return inputData;
        }

        /// <summary>
        /// Loads a SAS+ problem from the given input file. Does not include an input data validation.
        /// </summary>
        /// <param name="problemFilePath">SAS+ problem input file.</param>
        /// <returns>SAS+ problem input data.</returns>
        public static SAS.Problem LoadProblem(string problemFilePath)
        {
            return new MasterExporter().LoadProblem(problemFilePath);
        }
    }
}
