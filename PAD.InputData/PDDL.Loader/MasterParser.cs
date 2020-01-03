using System.IO;
using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Ast;

namespace PAD.InputData.PDDL.Loader
{
    /// <summary>
    /// Master class for parsing input PDDL files.
    /// </summary>
    public static class MasterParser
    {
        /// <summary>
        /// Parses the PDDL input file based on the given grammar. The parse tree is then converted into the AST.
        /// </summary>
        /// <typeparam name="TargetAst">Target AST root node type.</typeparam>
        /// <param name="inputFilePath">Input PDDL file.</param>
        /// <param name="grammar">Specific grammar to match the input data.</param>
        /// <returns>Root node of the created AST.</returns>
        public static TargetAst ParseAndCreateAst<TargetAst>(string inputFilePath, Grammar.MasterGrammar grammar) where TargetAst : BaseAstNode
        {
            string loadedString = LoadFileIntoString(inputFilePath);

            if (string.IsNullOrEmpty(loadedString))
            {
                throw new LoadingException($"Input file {Path.GetFileName(inputFilePath)} is empty or corrupted.");
            }

            Parser parser = new Parser(grammar);
            ParseTree tree = parser.Parse(loadedString);

            if (tree.HasErrors())
            {
                var errorItem = tree.ParserMessages.Find(x => x.Level == Irony.ErrorLevel.Error);
                string message = $"{errorItem.Message} (file: {Path.GetFileName(inputFilePath)}, line: {errorItem.Location.Line + 1}, column: {errorItem.Location.Column + 1})";

                throw new LoadingException(message);
            }

            var treeRoot = tree.Root.AstNode as TargetAst;
            if (treeRoot == null)
            {
                throw new LoadingException($"AST tree creation failed during loading {Path.GetFileName(inputFilePath)}.");
            }

            return treeRoot;
        }

        /// <summary>
        /// Loads the specified file into a string.
        /// </summary>
        /// <param name="filePath">Input file.</param>
        /// <returns>Loaded string.</returns>
        private static string LoadFileIntoString(string filePath)
        {
            string loadedString;
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    loadedString = reader.ReadToEnd();
                }
            }
            catch (IOException ex)
            {
                throw new LoadingException($"I/O exception during loading {Path.GetFileName(filePath)}: {ex.Message}");
            }

            return loadedString;
        }
    }
}
