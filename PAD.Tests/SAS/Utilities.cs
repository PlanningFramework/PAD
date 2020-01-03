using PAD.InputData;

namespace PAD.Tests.SAS
{
    /// <summary>
    /// Utility class for PDDL tests.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Exports the given input data into a string SAS+ representation and checks if it's a valid input by loading it again.
        /// </summary>
        /// <param name="data">Input data to be checked.</param>
        /// <returns>True if the string representation of the data is a valid SAS+ input. False otherwise.</returns>
        public static bool CheckToStringExport(SASInputData data)
        {
            string problemString = data.Problem?.ToString();
            string problemTempFile = CreateAndWriteToTemporaryFile(problemString);

            SASInputData exportedData = new SASInputData(problemTempFile);

            DeleteTemporaryFile(problemTempFile);

            if (data.Problem != null && !data.Problem.ToString().Equals(exportedData.Problem.ToString()))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a temporary file and writes a given content into it. The created file needs to be deleted by Utilities.DeleteTemporaryFile method.
        /// </summary>
        /// <param name="fileContent">File content to be written.</param>
        /// <returns>File name of the created temporary file.</returns>
        private static string CreateAndWriteToTemporaryFile(string fileContent)
        {
            if (string.IsNullOrEmpty(fileContent))
            {
                return null;
            }

            string fileName = System.IO.Path.GetTempFileName();

            System.IO.StreamWriter streamWriter = System.IO.File.AppendText(fileName);
            streamWriter.Write(fileContent);
            streamWriter.Flush();
            streamWriter.Close();

            return fileName;
        }

        /// <summary>
        /// Deletes the given temporary file, if it exists.
        /// </summary>
        /// <param name="fileName">File name of the temporary file to be deleted.</param>
        private static void DeleteTemporaryFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
        }
    }
}
