using System.Collections.Generic;
using PAD.InputData.PDDL.Traits;

namespace PAD.InputData.PDDL
{
    /// <summary>
    /// Input data structure for PDDL requirements.
    /// </summary>
    public class Requirements : HashSet<Requirement>, IVisitable
    {
        /// <summary>
        /// Adds a new requirement. If the given requirement implies some other requirements, these are added too.
        /// </summary>
        /// <param name="requirement">Requirement to be added.</param>
        public new void Add(Requirement requirement)
        {
            base.Add(requirement);

            switch (requirement)
            {
                case Requirement.ADL:
                {
                    base.Add(Requirement.STRIPS);
                    base.Add(Requirement.TYPING);
                    base.Add(Requirement.NEGATIVE_PRECONDITIONS);
                    base.Add(Requirement.DISJUNCTIVE_PRECONDITIONS);
                    base.Add(Requirement.EQUALITY);
                    base.Add(Requirement.EXISTENTIAL_PRECONDITIONS);
                    base.Add(Requirement.UNIVERSAL_PRECONDITIONS);
                    base.Add(Requirement.CONDITIONAL_EFFECTS);
                    break;
                }
                case Requirement.QUANTIFIED_PRECONDITIONS:
                {
                    base.Add(Requirement.EXISTENTIAL_PRECONDITIONS);
                    base.Add(Requirement.UNIVERSAL_PRECONDITIONS);
                    break;
                }
                case Requirement.FLUENTS:
                {
                    base.Add(Requirement.NUMERIC_FLUENTS);
                    base.Add(Requirement.OBJECT_FLUENTS);
                    break;
                }
                case Requirement.TIMED_INITIAL_LITERALS:
                {
                    base.Add(Requirement.DURATIVE_ACTIONS);
                    break;
                }
            }
        }

        /// <summary>
        /// String representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            List<string> stringList = new List<string>();
            foreach (var requirement in this)
            {
                stringList.Add(requirement.EnumToString());
            }
            return stringList.ToBlockString(":requirements");
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
