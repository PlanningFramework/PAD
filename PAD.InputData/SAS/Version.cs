using System.Diagnostics;

namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for a SAS+ version.
    /// </summary>
    [DebuggerDisplay("Version = {VersionNumber}")]
    public class Version : IVisitable
    {
        /// <summary>
        /// Version number of the SAS+ input data. The latest version is 3.
        /// </summary>
        public int Number { set; get; } = 0;

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines("begin_version", Number, "end_version");
        }

        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
