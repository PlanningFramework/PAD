﻿using System;
using Irony.Parsing;
using PAD.InputData.PDDL.Loader.Grammar;
using PAD.InputData.PDDL.Loader.Ast;
using PAD.InputData.PDDL.Loader;
using PAD.InputData;

namespace PAD.Tests.PDDL
{
    /// <summary>
    /// Utility class for PDDL tests.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Parse the input file and converts it into the AST.
        /// </summary>
        /// <param name="grammarType">Grammar rule to be tested.</param>
        /// <param name="inputFile">Input file to be tested.</param>
        /// <returns>AST node of the loaded input file.</returns>
        public static BaseAstNode LoadAst(Type grammarType, string inputFile)
        {
            var grammar = new MasterTestGrammar();
            grammar.Root = (NonTerminal)Activator.CreateInstance(grammarType, grammar);

            return MasterParser.ParseAndCreateAst<BaseAstNode>(inputFile, grammar);
        }

        /// <summary>
        /// Exports the given input data into a string PDDL representation and checks if it's a valid input by loading it again.
        /// </summary>
        /// <param name="data">Input data to be checked.</param>
        /// <returns>True if the string representation of the data is a valid PDDL input. False otherwise.</returns>
        public static bool CheckToStringExport(PDDLInputData data)
        {
            string domainString = data.Domain?.ToString();
            string problemString = data.Problem?.ToString();

            string domainTempFile = CreateAndWriteToTemporaryFile(domainString);
            string problemTempFile = CreateAndWriteToTemporaryFile(problemString);

            PDDLInputData exportedData = new PDDLInputData(domainTempFile, problemTempFile);

            DeleteTemporaryFile(domainTempFile);
            DeleteTemporaryFile(problemTempFile);

            if (data.Domain != null && !data.Domain.ToString().Equals(exportedData.Domain.ToString()))
            {
                return false;
            }

            if (data.Problem != null && !data.Problem.ToString().Equals(exportedData.Problem.ToString()))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a temporary file and writes a given content into it. The created file needs to be deleted by Utilities.DeleteTemporaryFile method.
        /// </summary>
        /// <param name="fileContent">File content to be written.</param>
        /// <returns>File name of the created temporary file.</returns>
        private static string CreateAndWriteToTemporaryFile(string fileContent)
        {
            if (string.IsNullOrEmpty(fileContent))
            {
                return null;
            }

            string fileName = System.IO.Path.GetTempFileName();

            System.IO.StreamWriter streamWriter = System.IO.File.AppendText(fileName);
            streamWriter.Write(fileContent);
            streamWriter.Flush();
            streamWriter.Close();

            return fileName;
        }

        /// <summary>
        /// Deletes the given temporary file, if it exists.
        /// </summary>
        /// <param name="fileName">File name of the temporary file to be deleted.</param>
        private static void DeleteTemporaryFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }
        }
    }

    /// <summary>
    /// Specific master grammar used specifically by PDDL tests.
    /// </summary>
    public class MasterTestGrammar : MasterGrammar
    {
    }

    /// <summary>
    /// Auxiliary class for the encapsulation of file paths to the domain and problem files.
    /// </summary>
    public class FilePaths
    {
        /// <summary>
        /// ID of the problem.
        /// </summary>
        public string ProblemId { set; get; }

        /// <summary>
        /// Filepath to the PDDL domain.
        /// </summary>
        public string Domain { set; get; }

        /// <summary>
        /// Filepath to the PDDL problem.
        /// </summary>
        public string Problem { set; get; }

        /// <summary>
        /// Constructs the file paths structure.
        /// </summary>
        /// <param name="problemId">ID of the specified problem.</param>
        /// <param name="domain">Domain file path.</param>
        /// <param name="problem">Problem file path.</param>
        public FilePaths(string problemId, string domain, string problem)
        {
            ProblemId = problemId;
            Domain = domain;
            Problem = problem;
        }
    }
}
