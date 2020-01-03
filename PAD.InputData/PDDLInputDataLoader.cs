using PAD.InputData.PDDL.Loader;
using PAD.InputData.PDDL.Loader.Grammar;
using PAD.InputData.PDDL.Loader.Ast;
// ReSharper disable UnusedMember.Global
// ReSharper disable UseObjectOrCollectionInitializer

namespace PAD.InputData
{
    /// <summary>
    /// Master class for loading PDDL data from input files.
    /// </summary>
    public static class PDDLInputDataLoader
    {
        /// <summary>
        /// Loads a PDDL input data from the given input files. Does not include an input data validation.
        /// </summary>
        /// <param name="domainFilePath">PDDL domain input file.</param>
        /// <param name="problemFilePath">PDDL problem input file.</param>
        /// <returns>Instance of master PDDL input data structure.</returns>
        public static PDDLInputData LoadInputData(string domainFilePath, string problemFilePath)
        {
            PDDLInputData inputData = new PDDLInputData();
            inputData.Domain = LoadDomain(domainFilePath);
            inputData.Problem = LoadProblem(inputData.Domain, problemFilePath);
            return inputData;
        }

        /// <summary>
        /// Loads a PDDL domain from the given input file. Does not include an input data validation.
        /// </summary>
        /// <param name="domainFilePath">PDDL domain input file.</param>
        /// <returns>PDDL domain input data.</returns>
        public static PDDL.Domain LoadDomain(string domainFilePath)
        {
            PDDL.Domain domain = MasterExporter.ToDomain(MasterParser.ParseAndCreateAst<DomainAstNode>(domainFilePath, new MasterGrammarDomain()));
            domain.FilePath = domainFilePath;
            return domain;
        }

        /// <summary>
        /// Loads a PDDL problem from the given input file. Does not include an input data validation.
        /// </summary>
        /// <param name="domainContext">Context of the corresponding domain.</param>
        /// <param name="problemFilePath">PDDL problem input file.</param>
        /// <returns>PDDL problem input data.</returns>
        public static PDDL.Problem LoadProblem(PDDL.Domain domainContext, string problemFilePath)
        {
            PDDL.Problem problem = MasterExporter.ToProblem(domainContext, MasterParser.ParseAndCreateAst<ProblemAstNode>(problemFilePath, new MasterGrammarProblem()));
            problem.FilePath = problemFilePath;
            return problem;
        }
    }
}
