
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Numeric expression - numeric function.
    /// </summary>
    public class NumericFunction : INumericExpression
    {
        /// <summary>
        /// Function atom.
        /// </summary>
        public IAtom FunctionAtom { set; get; } = null;

        /// <summary>
        /// ID manager of the corresponding planning problem.
        /// </summary>
        private IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Constructs the numeric function.
        /// </summary>
        /// <param name="functionAtom">Function atom.</param>
        /// <param name="idManager">ID manager.</param>
        public NumericFunction(IAtom functionAtom, IDManager idManager)
        {
            FunctionAtom = functionAtom;
            IDManager = idManager;
        }

        /// <summary>
        /// Undefined numeric value fro the numeric function.
        /// </summary>
        public const double UNDEFINED_VALUE = double.NaN;

        /// <summary>
        /// Default numeric function value, if not stated in the initial state.
        /// </summary>
        public const double DEFAULT_VALUE = UNDEFINED_VALUE;

        /// <summary>
        /// Checks whether the assigned value for the numeric function is undefined.
        /// </summary>
        /// <param name="value">Values to be checked.</param>
        /// <returns>True if the value is undefined, false otherwise.</returns>
        public static bool IsValueUndefined(double value) => double.IsNaN(value);

        /// <summary>
        /// Creates a deep copy of the numeric expression.
        /// </summary>
        /// <returns>Numeric expression clone.</returns>
        public INumericExpression Clone()
        {
            return new NumericFunction(FunctionAtom.Clone(), IDManager);
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return FunctionAtom.ToString(IDManager.Functions);
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return FunctionAtom.GetHashCode();
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            NumericFunction other = obj as NumericFunction;
            if (other == null)
            {
                return false;
            }
            return FunctionAtom.Equals(other.FunctionAtom);
        }

        /// <summary>
        /// Accepts a visitor evaluating the numeric expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>Result value of the numeric expression.</returns>
        public double Accept(INumericExpressionEvaluationVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a generic numeric expression visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public void Accept(INumericExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a visitor transforming numeric expressions.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        public INumericExpression Accept(INumericExpressionTransformVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
