﻿
namespace PAD.Planner.PDDL
{
    /// <summary>
    /// Implementation of an object function term.
    /// </summary>
    public class ObjectFunctionTerm : ITerm
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
        /// Undefined value for the object function.
        /// </summary>
        public const int UNDEFINED_VALUE = IDManager.UNDEFINED_ID;

        /// <summary>
        /// Default object function value, if not stated in the initial state.
        /// </summary>
        public const int DEFAULT_VALUE = UNDEFINED_VALUE;

        /// <summary>
        /// Constructs the term.
        /// </summary>
        /// <param name="functionAtom">Function atom.</param>
        /// <param name="idManager">ID manager.</param>
        public ObjectFunctionTerm(IAtom functionAtom, IDManager idManager)
        {
            FunctionAtom = functionAtom;
            IDManager = idManager;
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
            if (obj == this)
            {
                return true;
            }

            ObjectFunctionTerm other = obj as ObjectFunctionTerm;
            if (other == null)
            {
                return false;
            }

            return FunctionAtom.Equals(other.FunctionAtom);
        }

        /// <summary>
        /// Creates a deep copy of the term.
        /// </summary>
        /// <returns>A copy of the term.</returns>
        public ITerm Clone()
        {
            return new ObjectFunctionTerm(FunctionAtom, IDManager);
        }

        /// <summary>
        /// Accepts a term visitor.
        /// </summary>
        /// <param name="visitor">Term visitor.</param>
        public ITerm Accept(ITermTransformVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
