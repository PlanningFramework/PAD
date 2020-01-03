
namespace PAD.InputData.SAS
{
    /// <summary>
    /// Input data structure for a SAS+ metric.
    /// </summary>
    public class Metric : IVisitable
    {
        /// <summary>
        /// Is metric (action costs) used in the SAS+ problem?
        /// </summary>
        public bool IsUsed { set; get; } = false;

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Utilities.JoinLines("begin_metric", (IsUsed) ? 1 : 0, "end_metric");
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
