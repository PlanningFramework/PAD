using System.Diagnostics;
using Irony;

namespace PAD.InputData.PDDL.Loader.Grammar
{
    /// <summary>
    /// Type of identifier for specifying the identifier terminal.
    /// </summary>
    public enum IdentifierType
    {
        /// <summary>
        /// Constant form, e.g. "constA"
        /// </summary>
        CONSTANT,
        /// <summary>
        /// Variable form, e.g. "?varA"
        /// </summary>
        VARIABLE,
        /// <summary>
        /// Either constant of variable form, e.g. "constA", "?varA"
        /// </summary>
        VARIABLE_OR_CONSTANT,
        /// <summary>
        /// Requirement form, e.g. ":strips"
        /// </summary>
        REQUIREMENT
    }

    /// <summary>
    /// Identifier terminal (modification of the original Irony identifier terminal) - our identifiers cannot start with underscore,
    /// have to start with ? in case of variable, or with : in case of requirement, and have to allow special characters _ and -
    /// inside the identifier.
    /// </summary>
    public class IdentifierTerminal : Irony.Parsing.IdentifierTerminal
    {
        public IdentifierTerminal(string name, IdentifierType identifierType) : base(name)
        {
            AllChars = Strings.AllLatinLetters + Strings.DecimalDigits + "-_";
            AllFirstChars = Strings.AllLatinLetters;

            switch (identifierType)
            {
                case IdentifierType.CONSTANT:
                    break;
                case IdentifierType.VARIABLE:
                    AllFirstChars = "?";
                    break;
                case IdentifierType.VARIABLE_OR_CONSTANT:
                    AllFirstChars += "?";
                    break;
                case IdentifierType.REQUIREMENT:
                    AllFirstChars = ":";
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }
    }
}
