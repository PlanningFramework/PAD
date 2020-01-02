using System;

namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Preference for logical expression.
    /// </summary>
    public class PreferenceExpression : IExpression
    {
        /// <summary>
        /// Preference name ID.
        /// </summary>
        public int PreferenceNameID { set; get; } = IDManager.INVALID_ID;

        /// <summary>
        /// Child expression.
        /// </summary>
        public IExpression Child { set; get; } = null;

        /// <summary>
        /// ID manager of the corresponding planning problem.
        /// </summary>
        private IDManager IDManager { set; get; } = null;

        /// <summary>
        /// Constructs the preference expression.
        /// </summary>
        /// <param name="preferenceID">Preference name ID.</param>
        /// <param name="child">An argument of the expression.</param>
        /// <param name="idManager">ID manager.</param>
        public PreferenceExpression(int preferenceNameID, IExpression child, IDManager idManager)
        {
            PreferenceNameID = preferenceNameID;
            Child = child;
            IDManager = idManager;
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"(preference {IDManager.Preferences.GetNameFromID(PreferenceNameID)} {Child})";
        }

        /// <summary>
        /// Constructs a hash code used in the collections.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return HashHelper.GetHashCode("preference", PreferenceNameID, Child);
        }

        /// <summary>
        /// Checks the equality of objects.
        /// </summary>
        /// <param name="obj">Object to be checked.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            PreferenceExpression other = obj as PreferenceExpression;
            if (other == null)
            {
                return false;
            }
            return (PreferenceNameID == other.PreferenceNameID) && Child.Equals(other.Child);
        }

        /// <summary>
        /// Creates a deep copy of the expression.
        /// </summary>
        /// <returns>Expression clone.</returns>
        public IExpression Clone()
        {
            return new PreferenceExpression(PreferenceNameID, Child.Clone(), IDManager);
        }

        /// <summary>
        /// Accepts a visitor evaluating the logical expression.
        /// </summary>
        /// <param name="visitor">Evaluation visitor.</param>
        /// <returns>True if the expression is logically true. False otherwise.</returns>
        public bool Accept(IExpressionEvalVisitor visitor)
        {
            return Child.Accept(visitor);
        }

        /// <summary>
        /// Accepts a visitor counting some specific property of the logical expression.
        /// </summary>
        /// <param name="visitor">Property counting visitor.</param>
        /// <returns>Number of expression nodes fulfilling and non-fulfilling specified condition.</returns>
        public Tuple<int, int> Accept(IExpressionPropCountVisitor visitor)
        {
            return Child.Accept(visitor);
        }

        /// <summary>
        /// Accepts a visitor evaluating some specific property of the logical expression.
        /// </summary>
        /// <param name="visitor">Property evaluation visitor.</param>
        /// <returns>Result value of expression evaluation (fulfilling and non-fulfilling specified condition).</returns>
        public Tuple<double, double> Accept(IExpressionPropEvalVisitor visitor)
        {
            return Child.Accept(visitor);
        }

        /// <summary>
        /// Accepts an expression transformation visitor.
        /// </summary>
        /// <param name="visitor">Expression visitor.</param>
        public IExpression Accept(IExpressionTransformVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Accepts a generic expression visitor.
        /// </summary>
        /// <param name="visitor">Expression visitor.</param>
        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
