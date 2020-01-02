using System.IO;

namespace PAD.InputData.SAS.Loader
{
    /// <summary>
    /// Enhanced version of StreamReader with the ability to peek lines, i.e. getting the next line in the stream without actually consuming it.
    /// </summary>
    public class FileReader : StreamReader
    {
        /// <summary>
        /// Current line number of the input file.
        /// </summary>
        public int LineNumber { set; get; } = 0;

        /// <summary>
        /// Buffered line from the input file. Used for the line peeking.
        /// </summary>
        private string BufferedLine { set; get; } = null;

        /// <summary>
        /// Constructs the file reader.
        /// </summary>
        /// <param name="filePath">File path.</param>
        public FileReader(string filePath) : base(filePath)
        {
        }

        /// <summary>
        /// Gets the next line from the input file without consuming it.
        /// </summary>
        /// <returns>Next line from the input file.</returns>
        public string PeekNextLine()
        {
            if (BufferedLine == null)
            {
                BufferedLine = ReadLine();
            }            

            return BufferedLine;
        }

        /// <summary>
        /// Gets the next line from the input file.
        /// </summary>
        /// <returns>Next text line from the input file.</returns>
        public string GetNextLine()
        {
            ++LineNumber;

            string nextLine;
            if (BufferedLine != null)
            {
                nextLine = BufferedLine;
                BufferedLine = null;
            }
            else
            {
                nextLine = ReadLine();
            }

            return nextLine;
        }
    }
}
