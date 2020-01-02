
namespace PAD.InputData.SAS
{
    /// <summary>
    /// Interface for a common SAS+ input data structure visitable by a generic visitor.
    /// </summary>
    public interface IVisitable
    {
        /// <summary>
        /// Accept method for the input data visitor.
        /// </summary>
        /// <param name="visitor">Visitor.</param>
        void Accept(IVisitor visitor);
    }

    /// <summary>
    /// Interface for a common SAS+ input data structure visitor.
    /// </summary>
    public interface IVisitor
    {
        // root structures

        void Visit(Problem data);
        void Visit(Version data);
        void Visit(Metric data);
        void Visit(Variable data);
        void Visit(MutexGroup data);
        void Visit(InitialState data);
        void Visit(GoalConditions data);
        void Visit(Operator data);
        void Visit(AxiomRule data);

        // component structures

        void Visit(Conditions data);
        void Visit(Effect data);
        void Visit(Assignment data);
    }

    /// <summary>
    /// A base visitor for input SAS+ data with null behaviour, implementing interface IVisitor.
    /// Derived visitors from this class can implement handles for a subset of input structures.
    /// </summary>
    public class BaseVisitor : IVisitor
    {
        // root structures

        public virtual void Visit(Problem data) { }
        public virtual void Visit(Version data) { }
        public virtual void Visit(Metric data) { }
        public virtual void Visit(Variable data) { }
        public virtual void Visit(MutexGroup data) { }
        public virtual void Visit(InitialState data) { }
        public virtual void Visit(GoalConditions data) { }
        public virtual void Visit(Operator data) { }
        public virtual void Visit(AxiomRule data) { }

        // component structures

        public virtual void Visit(Conditions data) { }
        public virtual void Visit(Effect data) { }
        public virtual void Visit(Assignment data) { }
    }
}
